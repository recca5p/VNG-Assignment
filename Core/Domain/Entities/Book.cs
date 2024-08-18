using Domain.Entities.Base;
using Domain.Entities.Base.Inteface;

namespace Domain.Entities;

public class Book : AuditEntity<Guid>
{
    public string Author { get; set; }
    public string Title { get; set; }
    public DateTime PublishYear { get; set; }
    public User User { get; set; }
}