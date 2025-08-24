namespace EvCreditor.Abstractions.Models;

public class Customer
{
    public string? Name { get; set; }
    public string? VATNumber { get; set; }
    public string PartyType { get; set; } = "Customer";
    public List<Address> Addresses { get; set; } = new();
}
