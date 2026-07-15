using FruitSalesCalculator.Domain;

namespace FruitSalesCalculator.Pricing;

public interface IPricingStrategy
{
    string Description { get; }

    PriceCalculation Calculate(decimal basePrice, decimal amount);
}