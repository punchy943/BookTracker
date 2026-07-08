using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.BookList;

public class GetBookListQuery(IBookRepository bookRepository)
{
    public async Task<IReadOnlyList<BookInfo>> Execute()
    {
        var books = await bookRepository.GetAllAsync();
        var summary = books.Select(b => new BookInfo
        {
            Id = b.Id,
            Author = b.Author.Value,
            Title = b.Title.Value
        });
        return [.. summary];
        // plaats hier de code die nu nog in BookService.GetAllBooks staat
    }
}