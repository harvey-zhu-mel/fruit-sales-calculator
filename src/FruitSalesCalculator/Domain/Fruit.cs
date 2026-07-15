using FruitSalesCalculator.Pricing;

namespace FruitSalesCalculator.Domain;

public sealed class Fruit
{
    private readonly IPricingStrategy _pricingStrategy;

    public string Name { get; }
    public decimal BasePrice { get; }
    public PricingMethod PricingMethod { get; }
    public string PricingDescription => _pricingStrategy.Description;

    public Fruit(string name, decimal basePrice, PricingMethod pricingMethod, IPricingStrategy pricingStrategy)
    {
        
    
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Fruit name cannot be null or whitespace.", nameof(name));
        }

        if(basePrice <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(basePrice), "Base price cannot be negative or zero.");
        }

        ArgumentNullException.ThrowIfNull(pricingStrategy);

        Name = name.Trim();
        BasePrice = basePrice;
        PricingMethod = pricingMethod;
        _pricingStrategy = pricingStrategy;
    }

    public PriceCalculation CalculatePrice(decimal amount)
    {
        return _pricingStrategy.Calculate(BasePrice, amount);
    }
}