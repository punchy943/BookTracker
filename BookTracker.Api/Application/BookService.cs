using BookTracker.Api.Application.BookList;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.GetBookById;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Domain;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application;

public class BookService(IBookRepository bookRepository)
{
    public async Task<IReadOnlyList<BookInfo>> GetAllBooks()
    {
        var books = await bookRepository.GetAllAsync();
        var summary = books.Select(b => new BookInfo
        {
            Id = b.Id,
            Author = b.Author,
            Title = b.Title
        });
        return [.. summary];
    }

    public async Task<CreateBookResponse> CreateBook(CreateBookRequest request)
    {
        var book =
            new Book
            {
                Author = request.Author,
                Title = request.Title,
                Year = request.Year
            };
        var savedBook = await bookRepository.AddAsync(book);
        return
            new CreateBookResponse
            {
                Id = savedBook.Id,
                Author = savedBook.Author,
                Title = savedBook.Title,
                Year = savedBook.Year
            };
    }

    public async Task<bool> DeleteBook(int id)
    {
        return await bookRepository.DeleteAsync(id); // ... roep hier de juiste methode van IBookRepository aan
    }

    public async Task<bool> UpdateBook(int id, UpdateBookRequest request)
    {
        var book =
            new Book
            {
                Id = id,
                Title = request.Title,
                Author = request.Author,
                Year = request.Year
            };

        return await bookRepository.UpdateAsync(book);
    }

    public async Task<BookDetails?> GetBookById(int id)
    {
        var book = await bookRepository.GetByIdAsync(id);

        if (book is null)
        {
            return null;
        }

        return
            new BookDetails
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Year = book.Year
            };
    }
}
