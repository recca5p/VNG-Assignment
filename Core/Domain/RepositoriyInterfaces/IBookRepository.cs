using Contract.Models;
using Domain.Entities;

namespace Domain.RepositoriyInterfaces;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetByFilter(ListFilter filter);
    Task<Book> GetByIdAsync(Guid bookId, CancellationToken cancellationToken = default);
    void Insert(Book book);
    void Insert(IList<Book> books);
    void Remove(Book book);
}