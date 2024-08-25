using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

[Table("product")]
public record Product
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("uuid")]
    public Guid Uuid { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty!;

    [Column("price")]
    public decimal Price { get; set; } = default!;

    public List<Sale> Sales { get; set; } = default!;
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    [Column("created_by")]
    public Guid CreatedBy { get; set; }
}