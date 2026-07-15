using FruitSalesCalculator.Domain;

namespace FruitSalesCalculator.Pricing;

public sealed class PerItemPricingStrategy : IPricingStrategy
{
    public string Description => "Per Item Pricing";

    public PriceCalculation Calculate(decimal basePrice, decimal amount)
    {
        if (basePrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(basePrice), "Base price must be greater than zero.");
        }

        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        if (amount != decimal.Truncate(amount))
        {
            throw new ArgumentException(
                "Item quantity must be a whole number.",
                nameof(amount));
        }

        var subtotal = basePrice * amount;
        var discount = 0m; // No discount for per item pricing
        var total = subtotal - discount;

        return new PriceCalculation(Subtotal: subtotal, Discount: discount, Total: total, AppliedRule: Description);
    }
}