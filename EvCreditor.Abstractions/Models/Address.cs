namespace EvCreditor.Abstractions.Models;

public class Address
{
    public string AddressType { get; set; } = "InvoiceAddress";
    public string? Name { get; set; }
    public string? Street { get; set; }
    public string? StreetNumber { get; set; }
    public string? City { get; set; }
    public string? Box { get; set; }
    public string? CountryCode { get; set; }
}
