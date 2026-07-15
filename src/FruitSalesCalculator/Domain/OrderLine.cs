namespace FruitSalesCalculator.Domain;

public sealed class OrderLine
{
    public Fruit Fruit { get; }
    public decimal Amount { get; }

    public PriceCalculation PriceCalculation { get; }
    
    public OrderLine(Fruit fruit, decimal amount)
    {
        if (fruit == null)
        {
            throw new ArgumentNullException(nameof(fruit), "Fruit cannot be null.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        Fruit = fruit;
        Amount = amount;
    }
}