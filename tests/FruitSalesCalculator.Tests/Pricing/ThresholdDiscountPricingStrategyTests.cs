using FruitSalesCalculator.Pricing;

namespace FruitSalesCalculator.Tests.Pricing;

public sealed class ThresholdDiscountPricingStrategyTests
{
    [Fact]
    public void Calculate_does_not_discount_exact_threshold()
    {
        // Arrange
        var strategy =
            new ThresholdDiscountPricingStrategy(
                new PerKilogramPricingStrategy(),
                threshold: 2m,
                percentage: 10m);

        // Act
        var result = strategy.Calculate(
            basePrice: 5.00m,
            amount: 2.00m);

        // Assert
        Assert.Equal(10.00m, result.Subtotal);
        Assert.Equal(0m, result.Discount);
        Assert.Equal(10.00m, result.Total);
    }

    [Fact]
    public void Calculate_applies_discount_above_threshold_by_weight()
    {
        // Arrange
        var strategy =
            new ThresholdDiscountPricingStrategy(
                new PerKilogramPricingStrategy(),
                threshold: 2m,
                percentage: 10m);

        // Act
        var result = strategy.Calculate(
            basePrice: 5.00m,
            amount: 3.00m);

        // Assert
        Assert.Equal(15.00m, result.Subtotal);
        Assert.Equal(1.50m, result.Discount);
        Assert.Equal(13.50m, result.Total);
    }

    [Fact]
    public void Calculate_applies_discount_above_threshold_by_quantity()
    {
        // Arrange
        var strategy =
            new ThresholdDiscountPricingStrategy(
                new PerItemPricingStrategy(),
                threshold: 2m,
                percentage: 10m);

        // Act
        var result = strategy.Calculate(
            basePrice: 5.00m,
            amount: 3.00m);

        // Assert
        Assert.Equal(15.00m, result.Subtotal);
        Assert.Equal(1.50m, result.Discount);
        Assert.Equal(13.50m, result.Total);
    }
}