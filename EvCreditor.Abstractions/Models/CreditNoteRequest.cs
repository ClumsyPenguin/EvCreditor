namespace EvCreditor.Abstractions.Models;

public class CreditNoteRequest
{
    public string OrderType { get; set; } = "CreditNote";
    public string OrderDirection { get; set; } = "Income";
    public string? OrderNumber { get; set; }
    public string? OrderDate { get; set; }
    public string? ExpiryDate { get; set; }
    public string? AboutInvoiceNumber { get; set; }
    public Customer? Customer { get; set; }
    public List<OrderLine> OrderLines { get; set; } = new();
}
