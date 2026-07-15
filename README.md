# Fruit Sales Calculator

A small C#/.NET 8 console application that calculates the total price of mixed fruit orders.

It supports:
- pricing per kilogram;
- pricing per item;
- an optional threshold based percentage discount;
- JSON configured fruit and pricing combinations;
- repeated order line for the same fruit consolidation;
- xUnit tests for core pricing, order behaviour and inital a fruit Catelog.

## Requirements

- C# / .NET 8 SDK or higher
- macOS, Windows, or Linux
- VS Code, Visual Studio or the .NET CLI


## Build, run, and test

From the repository root:

```bash
dotnet restore
dotnet build
```

Run the application:

```bash
dotnet run --project src/FruitSalesCalculator/FruitSalesCalculator.csproj
```

Run the tests:

```bash
dotnet test
```

## Default catalogue

Fruit definitions are loaded from:

```text
src/FruitSalesCalculator/fruits.json
```

| Fruit | Base price | Pricing method | Discount |
|---|---:|---|---|
| Apple | $2.00 | Per kilogram | None |
| Banana | $0.30 | Per item | None |
| Cherry | $5.00 | Per kilogram | 10% when total quantity is greater than 2kg |


## Example

Enter:

```text
Apple
1.5
Banana
4
Cherry
3
done
```

Expected calculation:

```text
Apple:  1.5kg × $2.00 = $3.00
Banana: 4 items × $0.30 = $1.20
Cherry: 3kg × $5.00 = $15.00
        10% discount   = -$1.50

Order total: $17.70
```

Output:
```text
Order receipt
-------------
Apple: 1.5 kg → $3.00
Banana: 4 item(s) → $1.20
Cherry: 3 kg → $13.50
  Discount: -$1.50
-------------
Order total: $17.70
```

The console displays the catalogue, accepts fruit names and amounts until `done`, applies the configured pricing method, and prints a receipt.

Fruit lookup by name is case-insensitive and ignores surrounding whitespace.


## Configuration

Default configuration:

```json
{
  "fruits": [
    {
      "name": "Apple",
      "basePrice": 2.00,
      "pricingMethod": "PerKilogram"
    },
    {
      "name": "Banana",
      "basePrice": 0.30,
      "pricingMethod": "PerItem"
    },
    {
      "name": "Cherry",
      "basePrice": 5.00,
      "pricingMethod": "PerKilogram",
      "thresholdDiscount": {
        "threshold": 2.00,
        "percentage": 10.00
      }
    }
  ]
}
```

Supported base pricing methods:

```text
PerKilogram
PerItem
```

`thresholdDiscount` is optional. Its threshold uses the same unit as the base pricing method:

```text
PerKilogram → kilograms
PerItem     → item count
```

For per-item pricing, quantities and thresholds must be whole numbers.


### Changes that require JSON configuration only (Fruit Catalog)

Existing behaviours can be recombined without changing C# code:

- change a base price;
- switch a fruit between per-kilogram and per-item pricing;
- add a fruit using an existing pricing method;
- add or remove the existing threshold discount;
- change the threshold or percentage;
- move/add the threshold discount to another fruit.

A genuinely new pricing algorithm requires a new strategy implementation, one loader mapping, and focused tests.


## Design

```text
fruits.json
    ↓
FruitCatalogLoader
    ↓
FruitCatalog
    ↓
Console order entry
    ↓
Order / OrderLine
    ↓
Fruit delegates to IPricingStrategy
    ↓
PriceCalculation
    ↓
Receipt and order total
```


### Core responsibilities

| Component | Responsibility |
|---|---|
| `Fruit` | Holds name, base price, pricing method, and delegates calculation |
| `PricingMethod` | Business/configuration value: per kilogram or per item |
| `IPricingStrategy` | Common pricing contract |
| `PerKilogramPricingStrategy` | Weight-based calculation |
| `PerItemPricingStrategy` | Item-based calculation and whole-number validation |
| `ThresholdDiscountPricingStrategy` | Applies a percentage above a configured threshold |
| `PriceCalculation` | Returns subtotal, discount, total, and applied rule |
| `OrderLine` | Holds one fruit, amount, and calculated price |
| `Order` | Consolidates repeated fruit entries and sums line totals |
| `FruitCatalogLoader` | Reads, validates, and composes configured strategies |
| `FruitCatalog` | Performs normalized fruit-name lookup |
| `Program` | Handles console input and output |


## Design patterns

### Strategy

`IPricingStrategy` defines interchangeable pricing behaviour.

Base implementations:

```text
PerKilogramPricingStrategy
PerItemPricingStrategy
```

`Fruit` delegates calculation to its configured strategy, so the order and console do not contain fruit-specific pricing conditions.

### Decorator

`ThresholdDiscountPricingStrategy` implements `IPricingStrategy` and wraps another strategy.

Cherry is composed as:

```text
ThresholdDiscountPricingStrategy
    └── PerKilogramPricingStrategy
```

The wrapped strategy calculates the base price first. The decorator applies the percentage only when:

```text
amount > threshold
```

This keeps base pricing separate from discount behaviour and lets the same discount wrap either base strategy.


## Key design decisions

### Configuration is separate from implementation

JSON contains the business value:

```json
"pricingMethod": "PerKilogram"
```

It does not contain C# class names.

`FruitCatalogLoader` maps the `PricingMethod` enum to the runtime strategy.

### No duplicate product code in JSON

The catalogue derives an internal lookup key using:

```text
Name.Trim().ToUpperInvariant()
```

The original trimmed name is retained for display.

For this assessment, normalized fruit names must be unique. A larger production catalogue would normally use a stable unique code.

### Repeated order entries are consolidated

Repeated entries for the same fruit are combined and repriced.

For example:

```text
Cherry: 2kg
Cherry: 1kg
```

becomes:

```text
Cherry: 3kg
```

The 10% discount then applies to the customer's total Cherry quantity.

### One optional threshold discount

The submitted scope supports one optional threshold discount per fruit.

Multiple tiers are excluded because the business would first need to define whether tiers stack, the highest tier wins, priority wins, or the cheapest result wins.


## Validation and assumptions

- Fruit names must not be blank.
- Configured base prices must be greater than zero.
- Order amounts must be greater than zero.
- Per-item quantities and thresholds must be whole numbers.
- Discount thresholds must be greater than zero.
- Discount percentages must be greater than zero and no more than 100%.
- Exactly the threshold receives no discount.
- The threshold uses the base pricing method's unit.
- Duplicate normalized fruit names are rejected.
- Monetary values use `decimal`.
- Discounts and final order totals are rounded to two decimal places using `MidpointRounding.AwayFromZero`.
- One currency is assumed.

## Tests

Tests use xUnit with explicit Arrange, Act, and Assert sections.

The suite covers:

- per-kilogram pricing;
- per-item pricing;
- zero base-price and zero-amount rejection;
- fractional item-quantity rejection;
- exact threshold behaviour;
- weight and item threshold discounts;
- mixed-order and empty-order totals;
- repeated-line consolidation and repricing;
- JSON and file-based configuration loading;
- normalized fruit lookup;
- duplicate-name rejection; and
- configuration-only recombination of existing strategies.


## Extending the solution

### New fruit

Add an entry to `fruits.json` using an existing pricing method.

### New base pricing model

For example, pricing per box:

1. Add a `PricingMethod` value.
2. Implement `IPricingStrategy`.
3. Add one mapping in `FruitCatalogLoader`.
4. Add xUnit tests.
5. Add a JSON example.

The order workflow does not change.

### Seasonal discount

Add a decorator that wraps an existing strategy and evaluates an effective date.

The effective date should be supplied through a small pricing context rather than read directly from the system clock, keeping tests deterministic.

### Loyalty pricing

Use a line-level decorator when loyalty applies to one fruit.

Use an order-level policy when loyalty applies to the complete basket.


## Project structure

```text
fruit-sales-calculator/
├── FruitSalesCalculator.sln
├── README.md
├── .gitignore
├── src/
│   └── FruitSalesCalculator/
│       ├── Configuration/
│       ├── Domain/
│       ├── Pricing/
│       ├── FruitSalesCalculator.csproj
│       ├── Program.cs
│       └── fruits.json
└── tests/
    └── FruitSalesCalculator.Tests/
        ├── Configuration/
        ├── Domain/
        ├── Pricing/
        └── FruitSalesCalculator.Tests.csproj
```


## Design ownership

The solution architecture and design decisions were defined and owned by Harvey Zhu, including:

- keeping the solution proportional to the task;
- separating `PricingMethod` configuration from runtime strategies;
- using JSON to recombine existing behaviour without C# changes;
- deriving the lookup key from the normalized fruit name;
- using Strategy and Decorator design patterns;
- supporting one optional threshold discount to a fruit flexible;
- consolidating repeated order entries before applying pricing and discount; 
- avoiding infrastructure not required by the assessment.

The implementation, validation, final decisions, and ability to explain the solution remain the author's responsibility.