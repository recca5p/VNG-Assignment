using Contract.DTOs.Books;
using Contract.Models;

namespace Services.Abstractions;

public interface IBookService
{
    Task<FilteredResponse<BookModel>> GetAllAsync(ListFilter filter);
    Task<BookModel> GetByIdAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookModel>> CreateAsync(IList<BookCreateRequest> book, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid bookId, BookUpdateRequest book, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task RefreshCache();
}