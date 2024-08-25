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
    public async Task<IActionResult> GetProduct()
    {
        var products = await context.Product
            .Select(p => (ProductResponse)p)
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
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
    public async Task<IActionResult> PutProduct(Guid id, ProductPersistRequest product)
    {
        Expression<Func<Models.Product, bool>> predicate = p => p.Uuid == id;

        if (!product)
        {
            return BadRequest(product.Erros);
        }

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
    public async Task<IActionResult> PostProduct(ProductPersistRequest product)
    {
        if (!product)
        {
            return BadRequest(product.Erros);
        }
        var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
        product.CreatedBy = userId;
        context.Product.Add(product);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Uuid }, product);
    }

    [HttpDelete("{id:guid}")]
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