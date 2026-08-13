using System.Net;

namespace BookTracker.Blazor.Tests.TestHelpers;

public class FakeHttpMessageHandler(Task<HttpResponseMessage> response) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return response;
    }
    public FakeHttpMessageHandler(HttpResponseMessage response)
        : this(Task.FromResult(response))
    {
    }
}