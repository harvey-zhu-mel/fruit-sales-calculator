namespace FruitSalesCalculator.Domain;

public sealed class Fruit
{
    public string Name { get; }
    public decimal BasePrice { get; }
    public PricingMethod PricingMethod { get; }

    public Fruit(string name, decimal basePrice, PricingMethod pricingMethod)
    {
        Name = name.Trim();
        BasePrice = basePrice;
        PricingMethod = pricingMethod;
    }
}