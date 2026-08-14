using System.Net;
using System.Net.Http.Json;
using BookTracker.Blazor.Models.Auth;
using BookTracker.Blazor.Models.Books;

namespace BookTracker.Blazor.Api;

public sealed class BookTrackerClient(HttpClient httpClient)
{
    public async Task<LoginResponse?> Login(LoginRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("/Auth/Login", request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null; 
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LoginResponse>()
            ?? throw new InvalidOperationException("Login response was empty.");
    }

    public async Task<GetBookSummariesResponse> GetBooks(
        string? search,
        int page,
        int pageSize)
    {
        var url = $"/books?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(search))
        {
            url += $"&search={Uri.EscapeDataString(search)}";
        }

        return await httpClient.GetFromJsonAsync<GetBookSummariesResponse>(url)
            ?? throw new InvalidOperationException("Book list response was empty.");
    }

    public async Task<BookDetailsResponse?> GetBookDetails(int id)
    {
        using var response = await httpClient.GetAsync($"/books/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<BookDetailsResponse>()
            ?? throw new InvalidOperationException("Book details response was empty.");
    }
}