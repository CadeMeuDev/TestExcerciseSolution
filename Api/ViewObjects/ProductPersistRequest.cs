using System.Text.Json.Serialization;
using Api.Models;
using FluentValidation;

namespace Api.ViewObjects;

public class ProductPersistRequest
{
    public string Name { get; set; } = string.Empty!;

    public decimal Price { get; set; } = default!;

    [JsonIgnore]
    public Guid Uuid { get; set; } = Guid.NewGuid();

    [JsonIgnore]
    public Guid CreatedBy { get; set; }

    [JsonIgnore]
    public string Erros => string.Join("\n", new ProductCreateRequestValidator().Validate(this).Errors.Select(x => x.ErrorMessage));

    public static implicit operator Product(ProductPersistRequest product)
    {
        return new Product
        {
            Name = product.Name,
            Price = product.Price,
            Uuid = product.Uuid,
            CreatedBy = product.CreatedBy,
        };
    }

    public static implicit operator bool(ProductPersistRequest product)
    {
        return new ProductCreateRequestValidator().Validate(product).IsValid;
    }

    public class ProductCreateRequestValidator : AbstractValidator<ProductPersistRequest>
    {
        public ProductCreateRequestValidator()
        {
            RuleFor(p => p.Uuid)
                .NotEmpty();

            RuleFor(p => p.Name)
                .Length(5, 50);

            RuleFor(p => p.Price)
                .GreaterThan(0);
        }
    }
}