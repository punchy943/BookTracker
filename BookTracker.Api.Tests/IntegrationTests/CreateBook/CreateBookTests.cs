using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateBook;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookTracker.Api.Tests.IntegrationTests.CreateBook;

public class CreateBookTests
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task PostBookCreatesBook()
    {
        var client = factory.CreateClient();
        var request =
            new CreateBookRequest
            {
                Title = "The Heart Is a Lonely Hunter",
                Author = "Carson McCullers",
                Year = 1940
            };
        
        var response = await client.PostAsJsonAsync("/books", request);
        var result = await response.Content.ReadFromJsonAsync<CreateBookResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("The Heart Is a Lonely Hunter", result.Title);
    }
}