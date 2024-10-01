using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Api.Models;
using FluentValidation;

namespace Api.ViewObjects;

public class ProductPersistRequest : IValidatableObject
{
    public string Name { get; set; } = string.Empty!;

    public decimal Price { get; set; } = default!;

    [JsonIgnore]
    public Guid Uuid { get; set; } = Guid.NewGuid();

    [JsonIgnore]
    public Guid CreatedBy { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return new ProductCreateRequestValidator()
            .Validate(this).Errors
            .Select(v => new ValidationResult(v.ErrorMessage, [v.PropertyName]));
    }

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

    private sealed class ProductCreateRequestValidator : AbstractValidator<ProductPersistRequest>
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