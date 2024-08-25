using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

[Table("customer")]
public record Customer
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("uuid")]
    public Guid Uuid { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty!;

    public List<Sale> Sales { get; set; } = default!;
}