namespace FruitSalesCalculator.Domain;

public sealed record PriceCalculation(decimal Subtotal, decimal Discount, decimal Total, string AppliedRule);
