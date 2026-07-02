using BookTracker.Api.Application.BookList;
using BookTracker.Api.Domain;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application;

public class BookService(IBookRepository bookRepository)
{
    public async Task<IReadOnlyList<BookInfo>> GetAllBooks()
    {
        var books = await bookRepository.GetAllAsync();
        var summary = books.Select(b => new BookInfo {
            Id = b.Id, 
            Author = b.Author, 
            Title = b.Title
            });
        return [.. summary];
    }
}
