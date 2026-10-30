using System.ComponentModel.DataAnnotations;

namespace RetailStoreBilling.Models;

public class Product
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    [Required, StringLength(40)]
    public string Sku { get; set; } = "";

    [Range(typeof(decimal), "0.01", "9999999")]
    public decimal Price { get; set; }

    [Range(0, 1000000)]
    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;
}
