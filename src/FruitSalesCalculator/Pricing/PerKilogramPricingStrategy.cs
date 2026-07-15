using FruitSalesCalculator.Domain;

namespace FruitSalesCalculator.Pricing;

public sealed class PerKilogramPricingStrategy : IPricingStrategy
{
    public string Description => "Per Kilogram Pricing";

    public PriceCalculation Calculate(decimal basePrice, decimal amount)
    {
        if (basePrice <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(basePrice), "Base price must be greater than zero.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Weight must be greater than zero.");
        }

        var subtotal = basePrice * amount;
        var discount = 0m; // No discount for per kilogram pricing
        var total = subtotal - discount;
        return new PriceCalculation(Subtotal: subtotal, Discount: discount, Total: total, AppliedRule: Description);
    }
}