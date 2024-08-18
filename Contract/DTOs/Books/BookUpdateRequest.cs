using System.ComponentModel.DataAnnotations;

namespace Contract.DTOs.Books;

public class BookUpdateRequest
{
    public string Author { get; set; }
    public DateTime PublishYear { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public Guid UpdateById { get; set; } 
}