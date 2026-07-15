using FruitSalesCalculator.Domain;

namespace FruitSalesCalculator.Configuration;

public sealed class FruitCatalog
{
    private readonly Dictionary<string, Fruit> _fruits;
    public IEnumerable<Fruit> Fruits => _fruits.Values;

    public FruitCatalog(IEnumerable<Fruit> fruits)
    {
        if (fruits == null)
        {
            throw new ArgumentNullException(nameof(fruits), "Fruits collection cannot be null.");
        }

        _fruits = new Dictionary<string, Fruit>(StringComparer.Ordinal);

        foreach (var fruit in fruits)
        {
            if (fruit == null)
            {
                throw new ArgumentException("Fruit in the collection cannot be null.", nameof(fruits));
            }

            var normalizedName = NormalizeName(fruit.Name);

            if (!_fruits.TryAdd(normalizedName, fruit))
            {
                throw new InvalidOperationException(
                    $"Duplicate fruit name after normalization: '{fruit.Name}'.");
            }
        }
    }

    public bool TryGetFruit(string? name, out Fruit? fruit)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            fruit = null;
            return false;
        }

        return _fruits.TryGetValue(
            NormalizeName(name),
            out fruit);
    }

    internal static string NormalizeName(string name)
    {
        return name.Trim().ToUpperInvariant();
    }

}