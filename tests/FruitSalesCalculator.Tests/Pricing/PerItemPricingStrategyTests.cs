using FruitSalesCalculator.Pricing;

namespace FruitSalesCalculator.Tests.Pricing;

public sealed class PerItemPricingStrategyTests
{
    [Fact]
    public void Calculate_returns_item_based_price()
    {
        // Arrange
        var strategy = new PerItemPricingStrategy();

        // Act
        var result = strategy.Calculate(
            basePrice: 0.30m,
            amount: 4m);

        // Assert
        Assert.Equal(1.20m, result.Subtotal);
        Assert.Equal(0m, result.Discount);
        Assert.Equal(1.20m, result.Total);
    }

    [Fact]
    public void Calculate_rejects_fractional_item_quantity()
    {
        // Arrange
        var strategy = new PerItemPricingStrategy();

        // Act
        var action = () => strategy.Calculate(
            basePrice: 0.30m,
            amount: 2.50m);

        // Assert
        Assert.Throws<ArgumentException>(
            action);
    }

    [Fact]
    public void Calculate_rejects_zero_item_quantity()
    {
        // Arrange
        var strategy = new PerItemPricingStrategy();

        // Act
        var action = () => strategy.Calculate(
            basePrice: 0.30m,
            amount: 0m);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            action);
    }

    [Fact]
    public void Calculate_rejects_zero_base_price()
    {
        // Arrange
        var strategy = new PerItemPricingStrategy();

        // Act
        var action = () => strategy.Calculate(
            basePrice: 0m,
            amount: 2m);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            action);
    }
}