using System.Net.Http.Json;
using Api.ViewObjects;

namespace Api.IntergrationTests;

public class ProductIntegrationTests(ApplicationFactory factory) : IClassFixture<ApplicationFactory>
{
    private const string ROOT_PATH = "api/Products";

    [Fact]
    public async Task Should_table_product_is_empty_result_ok()
    {
        using var client = factory.CreateClient();
        var response = await client.GetAsync(ROOT_PATH);

        var result = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        Assert.Empty(result!);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Should_result_BadRequest_when_product_invalid()
    {
        using var client = factory.CreateClient();
        var productRequest = new ProductPersistRequest
        {
            Name = "abc",
            Price = 0,
        };

        var response = await client.PostAsJsonAsync(ROOT_PATH, productRequest);

        var result = await response.Content.ReadAsStringAsync();

        Assert.Contains("Name", result);
        Assert.Contains("Price", result);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Should_result_Created_when_product_valid_and_response_get_by_location()
    {
        using var client = factory.CreateClient();
        var productRequest = new ProductPersistRequest
        {
            Name = Guid.NewGuid().ToString(),
            Price = 1,
        };

        var response = await client.PostAsJsonAsync(ROOT_PATH, productRequest);

        var result = await client.GetFromJsonAsync<ProductResponse>(response.Headers.Location);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(productRequest.Name, result!.Name);
    }

    [Fact]
    public async Task Should_result_NoContet_when_product_delete()
    {
        using var client = factory.CreateClient();

        var productRequest = new ProductPersistRequest
        {
            Name = Guid.NewGuid().ToString(),
            Price = 1,
        };

        var response = await client.PostAsJsonAsync(ROOT_PATH, productRequest);
        var result = await client.GetFromJsonAsync<ProductResponse>(response.Headers.Location);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(productRequest.Name, result!.Name);

        var responseDelete = await client.DeleteAsync($"{ROOT_PATH}/{result.Uuid}");
        Assert.Equal(System.Net.HttpStatusCode.NoContent, responseDelete.StatusCode);

        var responseAfterDelete = await client.GetAsync(response.Headers.Location);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, responseAfterDelete.StatusCode);

    }


    [Fact]
    public async Task Should_result_NoContet_when_product_update()
    {
        using var client = factory.CreateClient();

        var productRequest = new ProductPersistRequest
        {
            Name = Guid.NewGuid().ToString(),
            Price = 1,
        };

        var response = await client.PostAsJsonAsync(ROOT_PATH, productRequest);
        var result = await client.GetFromJsonAsync<ProductResponse>(response.Headers.Location);

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(productRequest.Name, result!.Name);

        var productPutRequest = new ProductPersistRequest
        {
            Name = Guid.NewGuid().ToString(),
            Price = 1,
        };
        var responseUpdate = await client.PutAsJsonAsync($"{ROOT_PATH}/{result.Uuid}", productPutRequest);
        Assert.Equal(System.Net.HttpStatusCode.NoContent, responseUpdate.StatusCode);

        var responseAfterUpdate = await client.GetAsync(response.Headers.Location);
        var resultAfterUpdate = await responseAfterUpdate.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.Equal(System.Net.HttpStatusCode.OK, responseAfterUpdate.StatusCode);
        Assert.Equal(productPutRequest.Name, resultAfterUpdate!.Name);

    }

}