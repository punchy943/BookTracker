using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.CreateBook;

public class CreateBookTests : IntegrationTest
{
    [Fact]
    public async Task PostBookCreatesBook()
    {
        var request =
            new CreateBookRequest
            {
                Title = "The Heart Is a Lonely Hunter",
                Author = "Carson McCullers",
                Year = 1940
            };

        var response = await Client.PostAsJsonAsync("/books", request);
        var result = await response.Content.ReadFromJsonAsync<CreateBookResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("The Heart Is a Lonely Hunter", result.Title);

        var book = Reader.Query(context => context.Find<Book>(result.Id));

        Assert.NotNull(book);
        Assert.Equal("The Heart Is a Lonely Hunter", book.Title);
        Assert.Equal("Carson McCullers", book.Author);
        Assert.Equal(1940, book.Year);
    }
}