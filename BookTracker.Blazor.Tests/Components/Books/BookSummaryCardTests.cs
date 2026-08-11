using Bunit;
using BookTracker.Blazor.Components.Books;
using BookTracker.Blazor.Models.Books;
using AngleSharp.Dom;

namespace BookTracker.Blazor.Tests.Components.Books;

public class BookSummaryCardTests : BunitContext
{
    [Fact]
    public void ShowsTitleAndAuthor()
    {
        var book = new BookSummary
        {
            Id = 42,
            Title = "Dune",
            Author = "Frank Herbert"
        };

        var cut = Render<BookSummaryCard>(parameters => parameters
            .Add(component => component.Book, book));

        Assert.Equal("Dune", cut.Find("h2").TextContent);
        Assert.Equal("Frank Herbert", cut.Find("p").TextContent);
    }

    [Fact]
    public void ReturnsBookIdWhenSelected()
    {
        var book = new BookSummary
        {
            Id = 42,
            Title = "Dune",
            Author = "Frank Herbert"
        };
        int? selectedBookId = null;

        var cut = Render<BookSummaryCard>(parameters => parameters
            .Add(component => component.Book, book)
            .Add(component => component.OnSelected, id => selectedBookId = id));

        cut.Find("button").Click();

        Assert.Equal(42, selectedBookId);
    }
}