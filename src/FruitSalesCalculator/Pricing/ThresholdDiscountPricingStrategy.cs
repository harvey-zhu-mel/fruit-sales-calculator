using FruitSalesCalculator.Domain;

namespace FruitSalesCalculator.Pricing;

public sealed class ThresholdDiscountPricingStrategy : IPricingStrategy
{
    private readonly IPricingStrategy _innerStrategy;
    private readonly decimal _threshold;
    private readonly decimal _percentage;

     public string Description => $"{_percentage:0.##}% discount applied above {_threshold:0.##}";

    public ThresholdDiscountPricingStrategy(
        IPricingStrategy innerStrategy,
        decimal threshold,
        decimal percentage)
    {
        ArgumentNullException.ThrowIfNull(innerStrategy);

        if (threshold <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(threshold),
                "Discount threshold must be greater than zero.");
        }

        if (percentage <= 0 || percentage > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(percentage),
                "Discount percentage must be greater than zero and no more than 100.");
        }

        _innerStrategy = innerStrategy;
        _threshold = threshold;
        _percentage = percentage;
    }

    public PriceCalculation Calculate(decimal basePrice, decimal amount)
    {
        var baseCalculation = _innerStrategy.Calculate(basePrice, amount);

        if (amount <= _threshold)
        {
            return baseCalculation;
        }

        var additionalDiscount = Round(baseCalculation.Total * (_percentage / 100m));
        var totalDiscount = Round(baseCalculation.Discount + additionalDiscount);
        var finalTotal = Round(baseCalculation.Total - additionalDiscount);

        return new PriceCalculation(Subtotal: baseCalculation.Subtotal, Discount: totalDiscount, Total: finalTotal, AppliedRule: Description);
    }

    private static decimal Round(decimal value)
    {
        return decimal.Round(
            value,
            decimals: 2,
            mode: MidpointRounding.AwayFromZero);
    }

}