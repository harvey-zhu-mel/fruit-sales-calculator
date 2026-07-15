using FruitSalesCalculator.Domain;
using FruitSalesCalculator.Pricing;

namespace FruitSalesCalculator.Tests.Domain;

public sealed class OrderTests
{
    [Fact]
    public void CalculateTotal_returns_total_for_mixed_order()
    {
        // Arrange
        var apple = new Fruit(
            name: "Apple",
            basePrice: 2.00m,
            pricingMethod: PricingMethod.PerKilogram,
            pricingStrategy:
                new PerKilogramPricingStrategy());

        var banana = new Fruit(
            name: "Banana",
            basePrice: 0.30m,
            pricingMethod: PricingMethod.PerItem,
            pricingStrategy:
                new PerItemPricingStrategy());

        var cherry = new Fruit(
            name: "Cherry",
            basePrice: 5.00m,
            pricingMethod: PricingMethod.PerKilogram,
            pricingStrategy:
                new ThresholdDiscountPricingStrategy(
                    new PerKilogramPricingStrategy(),
                    threshold: 2m,
                    percentage: 10m));

        var order = new Order();

        order.AddLine(apple, 1.50m);
        order.AddLine(banana, 4m);
        order.AddLine(cherry, 3m);

        // Act
        var total = order.CalculateTotal();

        // Assert
        Assert.Equal(17.70m, total);
    }

    [Fact]
    public void CalculateTotal_returns_zero_for_empty_order()
    {
        // Arrange
        var order = new Order();

        // Act
        var total = order.CalculateTotal();

        // Assert
        Assert.Equal(0m, total);
    }

    [Fact]
    public void AddLine_consolidates_the_same_fruit_and_recalculates_discount()
    {
        // Arrange
        var cherry = new Fruit(
            name: "Cherry",
            basePrice: 5.00m,
            pricingMethod: PricingMethod.PerKilogram,
            pricingStrategy:
                new ThresholdDiscountPricingStrategy(
                    new PerKilogramPricingStrategy(),
                    threshold: 2m,
                    percentage: 10m));

        var order = new Order();

        // Act
        order.AddLine(cherry, 2m);
        order.AddLine(cherry, 1m);

        // Assert
        var line = Assert.Single(order.Lines);

        Assert.Equal(3m, line.Amount);
        Assert.Equal(15.00m, line.Price.Subtotal);
        Assert.Equal(1.50m, line.Price.Discount);
        Assert.Equal(13.50m, line.Price.Total);
    }

    [Fact]
    public void AddLine_consolidates_the_same_fruit_and_no_discount()
    {
        // Arrange
        var cherry = new Fruit(
            name: "Cherry",
            basePrice: 5.00m,
            pricingMethod: PricingMethod.PerKilogram,
            pricingStrategy:
                new ThresholdDiscountPricingStrategy(
                    new PerKilogramPricingStrategy(),
                    threshold: 2m,
                    percentage: 10m));

        var order = new Order();

        // Act
        order.AddLine(cherry, 1m);
        order.AddLine(cherry, 1m);

        // Assert
        var line = Assert.Single(order.Lines);

        Assert.Equal(2m, line.Amount);
        Assert.Equal(10.00m, line.Price.Subtotal);
        Assert.Equal(0m, line.Price.Discount);
        Assert.Equal(10.00m, line.Price.Total);
    }
}