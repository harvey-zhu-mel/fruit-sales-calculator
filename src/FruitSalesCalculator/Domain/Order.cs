namespace FruitSalesCalculator.Domain;

public sealed class Order
{
    private readonly List<OrderLine> _lines = [];
    public IReadOnlyList<OrderLine> Lines => _lines;

    public OrderLine AddLine(Fruit fruit, decimal amount)
    {
        var line = new OrderLine(fruit, amount);
        _lines.Add(line);
        return line;
    }

    public decimal CalculateTotal()
    {
        return _lines.Sum(line => line.PriceCalculation.Total);
    }
   
}