namespace FruitSalesCalculator.Domain;

public sealed class Order
{
    private readonly List<OrderLine> _lines = [];
    public IReadOnlyList<OrderLine> Lines => _lines;

    public OrderLine AddLine(Fruit fruit, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(fruit);

        var existingIndex = _lines.FindIndex(line =>
            string.Equals(
                line.Fruit.Name,
                fruit.Name,
                StringComparison.OrdinalIgnoreCase));

        if (existingIndex >= 0)
        {
            var consolidatedLine = new OrderLine(
                fruit,
                _lines[existingIndex].Amount + amount);

            _lines[existingIndex] = consolidatedLine;

            return consolidatedLine;
        }

        var newLine = new OrderLine(fruit, amount);
        _lines.Add(newLine);

        return newLine;
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