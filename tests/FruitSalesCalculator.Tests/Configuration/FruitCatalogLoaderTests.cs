using FruitSalesCalculator.Configuration;
using FruitSalesCalculator.Domain;

namespace FruitSalesCalculator.Tests.Configuration;

public sealed class FruitCatalogLoaderTests
{
    [Fact]
    public void LoadFromJson_loads_fruit_and_normalizes_lookup_name()
    {
        // Arrange
        const string json = """
        {
          "fruits": [
            {
              "name": "Apple",
              "basePrice": 2.00,
              "pricingMethod": "PerKilogram"
            }
          ]
        }
        """;

        // Act
        var catalog =
            FruitCatalogLoader.LoadFromJson(json);

        var found =
            catalog.TryGetFruit(
                "  apple  ",
                out var apple);

        // Assert
        Assert.True(found);
        Assert.NotNull(apple);
        Assert.Equal("Apple", apple!.Name);
        Assert.Equal(2.00m, apple.BasePrice);
        Assert.Equal(
            PricingMethod.PerKilogram,
            apple.PricingMethod);
    }

    [Fact]
    public void LoadFromJson_applies_cherry_discount_above_weight_threshold()
    {
        // Arrange
        const string json = """
        {
          "fruits": [
            {
              "name": "Cherry",
              "basePrice": 5.00,
              "pricingMethod": "PerKilogram",
              "thresholdDiscount": {
                "threshold": 2.00,
                "percentage": 10.00
              }
            }
          ]
        }
        """;

        // Act
        var catalog = FruitCatalogLoader.LoadFromJson(json);

        var found = catalog.TryGetFruit(
            "Cherry",
            out var cherry);

        // Assert
        Assert.True(found);
        Assert.NotNull(cherry);
        Assert.Equal(
            PricingMethod.PerKilogram,
            cherry!.PricingMethod);

        var thresholdPrice = cherry.CalculatePrice(2m);
        var discountedPrice = cherry.CalculatePrice(3m);

        Assert.Equal(10.00m, thresholdPrice.Subtotal);
        Assert.Equal(0.00m, thresholdPrice.Discount);
        Assert.Equal(10.00m, thresholdPrice.Total);

        Assert.Equal(15.00m, discountedPrice.Subtotal);
        Assert.Equal(1.50m, discountedPrice.Discount);
        Assert.Equal(13.50m, discountedPrice.Total);
    }

    [Fact]
    public void LoadFromJson_recombines_existing_strategies_without_code_changes()
    {
        // Arrange
        const string json = """
        {
          "fruits": [
            {
              "name": "Apple",
              "basePrice": 1.00,
              "pricingMethod": "PerItem"
            },
            {
              "name": "Banana",
              "basePrice": 0.30,
              "pricingMethod": "PerKilogram",
              "thresholdDiscount": {
                "threshold": 2.00,
                "percentage": 10.00
              }
            },
            {
              "name": "Cherry",
              "basePrice": 5.00,
              "pricingMethod": "PerKilogram"
            }
          ]
        }
        """;

        // Act
        var catalog =
            FruitCatalogLoader.LoadFromJson(json);

        catalog.TryGetFruit("Apple", out var apple);
        catalog.TryGetFruit("Banana", out var banana);
        catalog.TryGetFruit("Cherry", out var cherry);

        var applePrice =
            apple!.CalculatePrice(3m);

        var bananaPrice =
            banana!.CalculatePrice(3m);

        var cherryPrice =
            cherry!.CalculatePrice(3m);

        // Assert
        Assert.Equal(3.00m, applePrice.Total);
        Assert.Equal(0.81m, bananaPrice.Total);
        Assert.Equal(15.00m, cherryPrice.Total);
    }

    [Fact]
    public void LoadFromJson_rejects_duplicate_normalized_names()
    {
        // Arrange
        const string json = """
        {
          "fruits": [
            {
              "name": "Apple",
              "basePrice": 2.00,
              "pricingMethod": "PerKilogram"
            },
            {
              "name": " apple ",
              "basePrice": 1.00,
              "pricingMethod": "PerItem"
            }
          ]
        }
        """;

        // Act
        var action = () =>
            FruitCatalogLoader.LoadFromJson(json);

        // Assert
        Assert.Throws<InvalidOperationException>(
            action);
    }

    [Fact]
    public void LoadFromFile_reads_configuration_file()
    {
        // Arrange
        const string json = """
        {
          "fruits": [
            {
              "name": "Pear",
              "basePrice": 1.50,
              "pricingMethod": "PerItem"
            }
          ]
        }
        """;

        var path = Path.Combine(
            Path.GetTempPath(),
            $"fruits-{Guid.NewGuid():N}.json");

        File.WriteAllText(path, json);

        try
        {
            // Act
            var catalog =
                FruitCatalogLoader.LoadFromFile(path);

            var found =
                catalog.TryGetFruit(
                    "pear",
                    out var pear);

            // Assert
            Assert.True(found);
            Assert.NotNull(pear);
            Assert.Equal(1.50m, pear!.BasePrice);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void LoadFromJson_applies_banana_discount_above_item_threshold()
    {
        // Arrange
        const string json = """
        {
          "fruits": [
            {
              "name": "Banana",
              "basePrice": 0.30,
              "pricingMethod": "PerItem",
              "thresholdDiscount": {
                "threshold": 2.00,
                "percentage": 10.00
              }
            }
          ]
        }
        """;

        // Act
        var catalog = FruitCatalogLoader.LoadFromJson(json);

        var found = catalog.TryGetFruit(
            "Banana",
            out var banana);

        // Assert
        Assert.True(found);
        Assert.NotNull(banana);
        Assert.Equal(
            PricingMethod.PerItem,
            banana!.PricingMethod);

        var thresholdPrice = banana.CalculatePrice(2m);
        var discountedPrice = banana.CalculatePrice(3m);

        Assert.Equal(0.60m, thresholdPrice.Subtotal);
        Assert.Equal(0.00m, thresholdPrice.Discount);
        Assert.Equal(0.60m, thresholdPrice.Total);

        Assert.Equal(0.90m, discountedPrice.Subtotal);
        Assert.Equal(0.09m, discountedPrice.Discount);
        Assert.Equal(0.81m, discountedPrice.Total);
    }
}