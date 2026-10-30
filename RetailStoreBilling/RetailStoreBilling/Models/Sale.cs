using System.ComponentModel.DataAnnotations;

namespace RetailStoreBilling.Models;

public class Sale
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(120)]
    public string? CustomerName { get; set; }

    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }

    [StringLength(30)]
    public string PaymentMethod { get; set; } = "Cash";

    public List<SaleItem> Items { get; set; } = [];
}
