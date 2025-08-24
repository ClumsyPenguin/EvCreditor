namespace EvCreditor.Abstractions.Models;

public class OrderLine
{
    public decimal Quantity { get; set; }
    public decimal UnitPriceExcl { get; set; }
    public string? Description { get; set; }
    public decimal VATPercentage { get; set; }
}
