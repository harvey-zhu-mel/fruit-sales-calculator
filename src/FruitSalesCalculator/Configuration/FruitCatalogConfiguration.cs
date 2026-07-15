using FruitSalesCalculator.Domain;

namespace FruitSalesCalculator.Configuration;

public sealed class FruitCatalogConfiguration
{
    public List<FruitConfiguration?>? Fruits { get; init; }
}

public sealed class FruitConfiguration
{
    public string? Name { get; init; }

    public decimal BasePrice { get; init; }

    public PricingMethod? PricingMethod { get; init; }

    public ThresholdDiscountConfiguration? ThresholdDiscount { get; init; }
}

public sealed class ThresholdDiscountConfiguration
{
    public decimal Threshold { get; init; }

    public decimal Percentage { get; init; }
}