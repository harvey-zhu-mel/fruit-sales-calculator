using System.Text.Json;
using System.Text.Json.Serialization;
using FruitSalesCalculator.Domain;
using FruitSalesCalculator.Pricing;

namespace FruitSalesCalculator.Configuration;

public static class FruitCatalogLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters =
        {
            new JsonStringEnumConverter(
                namingPolicy: null,
                allowIntegerValues: false)
        }
    };

    public static FruitCatalog LoadFromFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException(
                "Configuration path is required.",
                nameof(path));
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Fruit configuration file was not found.",
                path);
        }

        var json = File.ReadAllText(path);

        return LoadFromJson(json);
    }

    public static FruitCatalog LoadFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException(
                "Fruit configuration JSON is required.",
                nameof(json));
        }

        FruitCatalogConfiguration configuration;

        try
        {
            configuration =
                JsonSerializer.Deserialize<FruitCatalogConfiguration>(
                    json,
                    JsonOptions)
                ?? throw new InvalidOperationException(
                    "Fruit configuration could not be loaded.");
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "Fruit configuration JSON is invalid.",
                exception);
        }

        if (configuration.Fruits is not { Count: > 0 })
        {
            throw new InvalidOperationException(
                "At least one fruit must be configured.");
        }

        var fruits = configuration.Fruits
            .Select(CreateFruit)
            .ToArray();

        return new FruitCatalog(fruits);
    }

    private static Fruit CreateFruit(
        FruitConfiguration? configuration)
    {
        if (configuration is null)
        {
            throw new InvalidOperationException(
                "Fruit configuration cannot be null.");
        }

        var name = configuration.Name?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException(
                "Every fruit must have a name.");
        }

        if (configuration.BasePrice <= 0)
        {
            throw new InvalidOperationException(
                $"Fruit '{name}' must have a base price greater than zero.");
        }

        if (configuration.PricingMethod is null)
        {
            throw new InvalidOperationException(
                $"Fruit '{name}' must have a pricing method.");
        }

        var pricingMethod = configuration.PricingMethod.Value;

        IPricingStrategy pricingStrategy = pricingMethod switch
        {
            PricingMethod.PerKilogram =>
                new PerKilogramPricingStrategy(),

            PricingMethod.PerItem =>
                new PerItemPricingStrategy(),

            _ => throw new InvalidOperationException(
                $"Fruit '{name}' has an unsupported pricing method.")
        };

        var discount = configuration.ThresholdDiscount;

        if (discount is not null)
        {
            if (discount.Threshold <= 0)
            {
                throw new InvalidOperationException(
                    $"Fruit '{name}' must have a discount threshold greater than zero.");
            }

            if (discount.Percentage <= 0 ||
                discount.Percentage > 100)
            {
                throw new InvalidOperationException(
                    $"Fruit '{name}' has an invalid discount percentage.");
            }

            if (pricingMethod == PricingMethod.PerItem &&
                discount.Threshold != decimal.Truncate(discount.Threshold))
            {
                throw new InvalidOperationException(
                    $"Fruit '{name}' uses per-item pricing, so its threshold must be a whole number.");
            }

            pricingStrategy =
                new ThresholdDiscountPricingStrategy(
                    pricingStrategy,
                    discount.Threshold,
                    discount.Percentage);
        }

        return new Fruit(
            name,
            configuration.BasePrice,
            pricingMethod,
            pricingStrategy);
    }
}