using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.BookList;
using BookTracker.Api.Domain;
using QuickPulse.Show;

namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests : IntegrationTest
{

    [Fact]
    public async Task GetBooksReturnsBooks()
    {
        Writer.Seed(db => db.Books.Add(
            new Book
            {
                Title = "Cannery Row",
                Author = "John Steinbeck",
                Year = 1945
            }
        ));

        var response = await Client.GetAsync("/books");
        var result = await response.Content.ReadFromJsonAsync<List<BookInfo>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(result);

        var bookInfo = Assert.Single(result);
        Assert.Equal("Cannery Row", bookInfo.Title);
        Assert.Equal("John Steinbeck", bookInfo.Author);
    }
}