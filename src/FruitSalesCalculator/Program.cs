using System.Globalization;
using FruitSalesCalculator.Configuration;
using FruitSalesCalculator.Domain;

try
{
    var configurationPath = Path.Combine(
        AppContext.BaseDirectory,
        "fruits.json");

    var catalog = FruitCatalogLoader.LoadFromFile(configurationPath);
    var order = new Order();

    PrintCatalog(catalog);
    TakeOrder(catalog, order);
    PrintReceipt(order);
}
catch (Exception exception)
{
    Console.Error.WriteLine(
        $"Application error: {exception.Message}");

    Environment.ExitCode = 1;
}

static void TakeOrder(
    FruitCatalog catalog,
    Order order)
{
    while (true)
    {
        Console.Write(
            "\nEnter fruit name or 'done': ");

        var input = Console.ReadLine()?.Trim();

        if (input is null ||
            input.Equals(
                "done",
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (input.Length == 0)
        {
            continue;
        }

        if (!catalog.TryGetFruit(input, out var fruit) ||
            fruit is null)
        {
            Console.WriteLine(
                $"Fruit '{input}' was not found.");

            continue;
        }

        var amount = ReadAmount(fruit);

        if (amount is null)
        {
            Console.WriteLine("Item cancelled.");
            continue;
        }

        try
        {
            var line = order.AddLine(
                fruit,
                amount.Value);

            PrintAddedLine(line);
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(
                $"Could not add item: {exception.Message}");
        }
    }
}

static decimal? ReadAmount(Fruit fruit)
{
    var amountDescription = fruit.PricingMethod switch
    {
        PricingMethod.PerKilogram =>
            "weight in kilograms",

        PricingMethod.PerItem =>
            "item quantity",

        _ => "amount"
    };

    while (true)
    {
        Console.Write(
            $"Enter {amountDescription} for {fruit.Name} " +
            "(blank to cancel): ");

        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        if (TryParseAmount(input, out var amount))
        {
            return amount;
        }

        Console.WriteLine(
            "Enter a valid numeric amount.");
    }
}

static bool TryParseAmount(
    string input,
    out decimal amount)
{
    return decimal.TryParse(
               input,
               NumberStyles.Number,
               CultureInfo.CurrentCulture,
               out amount)
           ||
           decimal.TryParse(
               input,
               NumberStyles.Number,
               CultureInfo.InvariantCulture,
               out amount);
}

static void PrintCatalog(FruitCatalog catalog)
{
    Console.WriteLine("Fruit Sales Calculator");
    Console.WriteLine("\nAvailable fruit:");

    foreach (var fruit in catalog.Fruits.OrderBy(
                 fruit => fruit.Name,
                 StringComparer.OrdinalIgnoreCase))
    {
        Console.WriteLine(
            $"- {fruit.Name}: " +
            $"${fruit.BasePrice:0.00} " +
            fruit.PricingDescription);
    }
}

static void PrintAddedLine(OrderLine line)
{
    Console.WriteLine(
        $"Added {line.Fruit.Name}: " +
        $"{FormatAmount(line)} = " +
        $"${line.Price.Total:0.00}");

    if (line.Price.Discount <= 0m)
    {
        return;
    }

    Console.WriteLine(
        $"  Discount: -${line.Price.Discount:0.00}");

    Console.WriteLine(
        $"  Rule: {line.Price.AppliedRule}");
}

static void PrintReceipt(Order order)
{
    Console.WriteLine("\nOrder receipt");
    Console.WriteLine("-------------");

    if (order.Lines.Count == 0)
    {
        Console.WriteLine("No items were added.");
    }

    foreach (var line in order.Lines)
    {
        Console.WriteLine(
            $"{line.Fruit.Name}: " +
            $"{FormatAmount(line)} " +
            $"→ ${line.Price.Total:0.00}");

        if (line.Price.Discount > 0m)
        {
            Console.WriteLine(
                $"  Discount: -${line.Price.Discount:0.00}");
        }
    }

    Console.WriteLine("-------------");
    Console.WriteLine(
        $"Order total: ${order.CalculateTotal():0.00}");
}

static string FormatAmount(OrderLine line)
{
    return line.Fruit.PricingMethod switch
    {
        PricingMethod.PerKilogram =>
            $"{line.Amount:0.##} kg",

        PricingMethod.PerItem =>
            $"{line.Amount:0} item(s)",

        _ => line.Amount.ToString(
            CultureInfo.InvariantCulture)
    };
}