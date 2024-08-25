using Api.Models;

namespace Api.ViewObjects;

public class ProductResponse
{
    public Guid Uuid { get; set; }

    public string Name { get; set; } = string.Empty!;

    public decimal Price { get; set; } = default!;

    public static implicit operator ProductResponse(Product product)
    {
        return new ProductResponse
        {
            Uuid = product.Uuid,
            Name = product.Name,
            Price = product.Price,
        };
    }
}