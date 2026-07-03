using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.BookList;
using Microsoft.AspNetCore.Mvc.Testing;
using QuickFuzzr;

namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task GetBooksReturnsBooks()
    {
        var client = factory.CreateClient();
        
        var response = await client.GetAsync("/books");
        var result = await response.Content.ReadFromJsonAsync<List<BookInfo>>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(result);
        Assert.Empty(result); 
    }
}