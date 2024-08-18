using Contract.Models;
using Domain.Entities;
using Domain.RepositoriyInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IRepositoryDbContext _dbContext;
    
    public BookRepository(IRepositoryDbContext dbContext) => _dbContext = dbContext;
    
    public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Books.ToListAsync(cancellationToken);

    public async Task<IEnumerable<Book>> GetByFilter(ListFilter filter)
    {
        IQueryable<Book> query = _dbContext.Books.Where(b => b.IsDeleted == false);

        if (!string.IsNullOrEmpty(filter.SearchName))
        {
            query = query.Where(s => s.Author.ToLower().Contains(filter.SearchName.ToLower())
                                     || s.Title.ToLower().Contains(filter.SearchName.ToLower()));
        }
        
        switch (filter.OrderType)
        {
            case OrderTypeEnum.Ascending:
                query = query.OrderBy(s => s.CreatedDateTime);
                break;
            case OrderTypeEnum.Descending:
                query = query.OrderByDescending(s => s.CreatedDateTime);
                break;
            default:
                query = query.OrderBy(s => s.CreatedDateTime);
                break;
        }
        
        return await query.ToListAsync();
    }
    
    
    public async Task<Book> GetByIdAsync(Guid bookId, CancellationToken cancellationToken = default) =>
        await _dbContext.Books.FirstOrDefaultAsync(x => x.Id == bookId, cancellationToken);
    
    public void Insert(Book book) => _dbContext.Books.Add(book);
    
    public void Insert(IList<Book> books) => _dbContext.Books.AddRange(books);
    
    public void Remove(Book book) => _dbContext.Books.Remove(book);
}