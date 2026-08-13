using Bunit;
using BookTracker.Blazor.Pages;
using BookTracker.Blazor.Models.Books;
using System.Net;
using System.Net.Http.Json;
using BookTracker.Blazor.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using BookTracker.Blazor.Api;

namespace BookTracker.Blazor.Tests.Pages;

public class HomeTests : BunitContext
{
    private static HttpResponseMessage CreateMessage(GetBookSummariesResponse content)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(content)
        };
    }

    [Fact]
    public void HomeShowsAllBooksWithAuthors()
    {
        var apiResponse = new GetBookSummariesResponse
        {
            Items = [
                new BookSummary{Id = 1, Title = "Programming under water.", Author = "Lukas Motte"},
                new BookSummary{Id = 2, Title = "Why you should visit Pythong.org.", Author = "Peter-Paul"}
            ],
            Page = 1,
            PageSize = 10,
            TotalItems = 2,
            TotalPages = 1
        };

        var testHandler = new FakeHttpMessageHandler(CreateMessage(apiResponse));
        var httpClient = new HttpClient(testHandler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new BookTrackerClient(httpClient));

        var cut = Render<Home>();

        Assert.Contains("Programming under water.", cut.Markup);
        Assert.Contains("Lukas Motte", cut.Markup);
        Assert.Contains("Why you should visit Pythong.org.", cut.Markup);
        Assert.Contains("Peter-Paul", cut.Markup);
    }

    [Fact]
    public void HidesAuthorsWhenToggleIsClicked()
    {
        var apiResponse = new GetBookSummariesResponse
        {
            Items = [
                new BookSummary{Id = 1, Title = "Programming under water.", Author = "Lukas Motte"},
                new BookSummary{Id = 2, Title = "Why you should visit Pythong.org.", Author = "Peter-Paul"}
            ],
            Page = 1,
            PageSize = 10,
            TotalItems = 2,
            TotalPages = 1
        };

        var testHandler = new FakeHttpMessageHandler(CreateMessage(apiResponse));
        var httpClient = new HttpClient(testHandler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new BookTrackerClient(httpClient));

        var cut = Render<Home>();

        cut.Find("#ToggleAuthors").Click();

        Assert.Contains("Programming under water.", cut.Markup);
        Assert.DoesNotContain("Lukas Motte", cut.Markup);
        Assert.Contains("Why you should visit Pythong.org.", cut.Markup);
        Assert.DoesNotContain("Peter-Paul", cut.Markup);
    }

    [Fact]
    public void ShowsLoadingStateWhileRequestIsPending()
    {
        var apiResponse = new GetBookSummariesResponse
        {
            Items = [],
            Page = 1,
            PageSize = 10,
            TotalItems = 0,
            TotalPages = 1
        };

        var response = new TaskCompletionSource<HttpResponseMessage>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        var httpClient = new HttpClient(new FakeHttpMessageHandler(response.Task))
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new BookTrackerClient(httpClient));

        var cut = Render<Home>();

        Assert.Equal("Boeken laden...", cut.Find("#status").TextContent);

        response.SetResult(CreateMessage(apiResponse));
        cut.WaitForAssertion(() => Assert.Contains("Geen boeken gevonden.", cut.Markup));
    }
}