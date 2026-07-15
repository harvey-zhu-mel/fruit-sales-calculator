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
        var total = _lines.Sum(line => line.Price.Total);
        return decimal.Round(
            total,
            decimals: 2,
            mode: MidpointRounding.AwayFromZero);
    }
   

}