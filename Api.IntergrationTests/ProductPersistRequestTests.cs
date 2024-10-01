using System.Text.Json;
using Api.ViewObjects;
using Xunit.Abstractions;

namespace Api.IntergrationTests;

public class ProductPersistRequestTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void Test01()
    {
        var request = new ProductPersistRequest
        {
            Name = "a",
            Price = 0,
        };

        var results = request.Validate(null!);
        outputHelper.WriteLine(JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
        Assert.Empty(results);
    }
}