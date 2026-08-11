using Bunit;
using BookTracker.Blazor.Pages;

namespace BookTracker.Blazor.Tests.Pages;

public class BookSummaryCardTests : BunitContext
{
    [Fact]
    public void HomeShowsAllBooksWithAuthors()
    {
        var cut = Render<Home>();

        Assert.Contains("Dune", cut.Markup);
        Assert.Contains("The Big Sleep", cut.Markup);
        Assert.Contains("Frank Herbert", cut.Markup);
        Assert.Contains("Raymond Chandler", cut.Markup);
    }
    [Fact]
    public void HidesAuthorsWhenToggleIsClicked()
    {
        var cut = Render<Home>();

        cut.Find("button").Click();

        Assert.DoesNotContain("Frank Herbert", cut.Markup);
        Assert.DoesNotContain("Raymond Chandler", cut.Markup);
    }
}