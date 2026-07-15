using FruitSalesCalculator.Pricing;

namespace FruitSalesCalculator.Tests.Pricing;

public sealed class PerKilogramPricingStrategyTests
{
    [Fact]
    public void Calculate_returns_weight_based_price()
    {
        // Arrange
        var strategy = new PerKilogramPricingStrategy();

        // Act
        var result = strategy.Calculate(
            basePrice: 2.00m,
            amount: 1.50m);

        // Assert
        Assert.Equal(3.00m, result.Subtotal);
        Assert.Equal(0m, result.Discount);
        Assert.Equal(3.00m, result.Total);
    }

    [Fact]
    public void Calculate_rejects_zero_weight()
    {
        // Arrange
        var strategy = new PerKilogramPricingStrategy();

        // Act
        var action = () => strategy.Calculate(
            basePrice: 2.00m,
            amount: 0m);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            action);
    }
}