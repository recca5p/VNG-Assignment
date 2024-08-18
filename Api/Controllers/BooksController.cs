using Contract.DTOs.Books;
using Contract.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public BooksController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] ListFilter filter,CancellationToken cancellationToken)
    {
        var books = await _serviceManager.BookService.GetAllAsync(filter);

        return Ok(new ApiResponse<FilteredResponse<BookModel>>()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "success",
            Data = books
        });
    }

    [HttpGet("{bookId:guid}")]
    public async Task<IActionResult> GetBookById(Guid bookId, CancellationToken cancellationToken)
    {
        var book = await _serviceManager.BookService.GetByIdAsync(bookId, cancellationToken);

        return Ok(new ApiResponse<BookModel>()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "success",
            Data = book
        });    
    }

    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] IList<BookCreateRequest> request, CancellationToken cancellationToken)
    {
        try
        {
            var token = HttpContext.Request.Headers["xAuth"].FirstOrDefault()?.Split(" ").Last();

            var response = await _serviceManager.BookService.CreateAsync(request, cancellationToken);

            return Ok(new ApiResponse<IEnumerable<BookModel>>()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Create success",
                Data = response
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpPost("{bookId:guid}")]
    public async Task<IActionResult> UpdateBook(Guid guid, [FromBody] BookUpdateRequest request, CancellationToken cancellationToken)
    {
        await _serviceManager.BookService.UpdateAsync(guid, request, cancellationToken);

        return Ok(new ApiResponse<string>()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "update success",
            Data = string.Empty
        });        
    }

    [HttpDelete("{bookId:guid}")]
    public async Task<IActionResult> DeleteAccount(Guid bookId, CancellationToken cancellationToken)
    {
        await _serviceManager.BookService.DeleteAsync(bookId, cancellationToken);

        return Ok(new ApiResponse<string>()
        {
            Success = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "delete success",
            Data = string.Empty
        });     
    }
}