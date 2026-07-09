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

        var result = await response.ReadJsonAs<CreateBookResponse>(HttpStatusCode.Created);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("The Heart Is a Lonely Hunter", result.Title);

        var book = Reader.Query(context => context.Find<Book>(result.Id));

        Assert.NotNull(book);
        Assert.Equal("The Heart Is a Lonely Hunter", book.Title.Value);
        Assert.Equal("Carson McCullers", book.Author.Value);
        Assert.Equal(1940, book.Year);
    }

    [Fact]
    public async Task PostBookReturnsBadRequestWhenTitleIsWhitespace()
    {
        var request =
            new CreateBookRequest
            {
                Title = "   ",
                Author = "Carson McCullers",
                Year = 1940
            };

        var response = await Client.PostAsJsonAsync("/books", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }
}