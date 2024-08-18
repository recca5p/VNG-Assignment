using System.ComponentModel.DataAnnotations;

namespace Contract.DTOs.Books;

public class BookCreateRequest
{
    public string Author { get; set; }
    public DateTime PublishYear { get; set; }
    [Required]
    public string Title { get; set; }
    public Guid CreatedById { get; set; } 
}