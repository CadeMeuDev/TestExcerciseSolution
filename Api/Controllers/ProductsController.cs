using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Api.Data;
using Api.ViewObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductsController(StoreContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductResponse>))]
    public async Task<IActionResult> GetProduct()
    {
        var products = await context.Product
            .Select(p => (ProductResponse)p)
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await context.Product.SingleOrDefaultAsync(p => p.Uuid == id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok((ProductResponse)product);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<ValidationResult>))]
    public async Task<IActionResult> PutProduct(Guid id, ProductPersistRequest product)
    {
        Expression<Func<Models.Product, bool>> predicate = p => p.Uuid == id;

        var exists = await context.Product.AnyAsync(predicate);
        if (!exists)
        {
            return NotFound();
        }

        await context.Product.Where(predicate)
            .ExecuteUpdateAsync(prop => prop
            .SetProperty(p => p.Name, product.Name)
            .SetProperty(p => p.Price, product.Price));

        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> PostProduct(ProductPersistRequest product)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
        product.CreatedBy = userId;
        context.Product.Add(product);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Uuid }, product);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        Expression<Func<Models.Product, bool>> predicate = p => p.Uuid == id;

        var exists = await context.Product.AnyAsync(predicate);
        if (exists)
        {
            await context.Product.Where(predicate).ExecuteDeleteAsync();

            return NoContent();
        }
        return NotFound();
    }
}