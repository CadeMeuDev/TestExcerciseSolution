using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

[Table("product_sale")]
public record ProductSale
{
    [Key]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    public Product Product { get; set; } = default!;

    [Column("sale_id")]
    public int SaleId { get; set; }

    public Sale Sale { get; set; } = default!;
}