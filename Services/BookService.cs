using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using AutoMapper;
using Contract.DTOs.Books;
using Contract.Exceptions.Books;
using Contract.Models;
using Domain.Entities;
using Domain.RepositoriyInterfaces;
using Microsoft.Extensions.Options;
using Services.Abstractions;

public class BookService : IBookService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly AppSettings _appSettings;
    private const string CacheKey = "BooksCache";

    public BookService(IRepositoryManager repositoryManager, IMapper mapper, IOptions<AppSettings> appSettings, IMemoryCache memoryCache)
    {
        _repositoryManager = repositoryManager;
        _mapper = mapper;
        _memoryCache = memoryCache;
        _appSettings = appSettings.Value;
    }

    public async Task<FilteredResponse<BookModel>> GetAllAsync(ListFilter filter)
    {
        // Check if cache contains the list of books
        if (!_memoryCache.TryGetValue(CacheKey, out IEnumerable<Book> books))
        {
            books = await _repositoryManager.BookRepository.GetAllAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            _memoryCache.Set(CacheKey, books, cacheEntryOptions);
        }

        int totalCount = books.Count();
        var paginatedBooks = await _repositoryManager.BookRepository.GetByFilter(filter);
        paginatedBooks = paginatedBooks.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);

        var bookModels = _mapper.Map<IEnumerable<Book>, IEnumerable<BookModel>>(paginatedBooks);

        return new FilteredResponse<BookModel>()
        {
            Page = filter.Page,
            PageSize = filter.PageSize,
            Data = bookModels,
            TotalCount = totalCount
        };
    }

    public async Task<BookModel> GetByIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue(CacheKey, out IEnumerable<Book> books))
        {
            var bookFromCache = books.FirstOrDefault(b => b.Id == bookId);
            if (bookFromCache != null)
            {
                return _mapper.Map<Book, BookModel>(bookFromCache);
            }
        }

        var bookEntity = await _repositoryManager.BookRepository.GetByIdAsync(bookId, cancellationToken);
        if (bookEntity == null)
        {
            throw new BookNotFoundException(bookId);
        }

        return _mapper.Map<Book, BookModel>(bookEntity);
    }

    public async Task<IEnumerable<BookModel>> CreateAsync(IList<BookCreateRequest> bookCreateRequest, CancellationToken cancellationToken = default)
    {
        var books = _mapper.Map<IList<BookCreateRequest>, IList<Book>>(bookCreateRequest);
        _repositoryManager.BookRepository.Insert(books);
        await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        await RefreshCache();

        return _mapper.Map<IEnumerable<Book>, IEnumerable<BookModel>>(books);
    }

    public async Task UpdateAsync(Guid bookId, BookUpdateRequest bookUpdateRequest, CancellationToken cancellationToken = default)
    {
        var book = await _repositoryManager.BookRepository.GetByIdAsync(bookId);
        if (book == null)
        {
            throw new BookNotFoundException(bookId);
        }

        _mapper.Map(bookUpdateRequest, book);
        await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        await RefreshCache();
    }

    public async Task DeleteAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        var book = await _repositoryManager.BookRepository.GetByIdAsync(bookId, cancellationToken);
        if (book == null)
        {
            throw new BookNotFoundException(bookId);
        }

        _repositoryManager.BookRepository.Remove(book);
        await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        await RefreshCache();
    }

    public async Task RefreshCache()
    {
        var books = await _repositoryManager.BookRepository.GetAllAsync();

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));

        _memoryCache.Set(CacheKey, books, cacheEntryOptions);
    }
}
