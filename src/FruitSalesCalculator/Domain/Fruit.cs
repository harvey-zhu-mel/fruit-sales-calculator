namespace FruitSalesCalculator.Domain;

public sealed class Fruit
{
    public string Name { get; }
    public decimal BasePrice { get; }
    public PricingMethod PricingMethod { get; }

    public Fruit(string name, decimal basePrice, PricingMethod pricingMethod)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Fruit name cannot be null or whitespace.", nameof(name));
        }

        if(basePrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(basePrice), "Base price cannot be negative or zero.");
        }

        Name = name.Trim();
        BasePrice = basePrice;
        PricingMethod = pricingMethod;
    }
}