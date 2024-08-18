namespace Contract.Exceptions.Books;

public class BookNotFoundException : NotFoundException
{
    public BookNotFoundException(Guid bookId)
        : base($"The book with the identifier {bookId} was not found.")
    {
    }
}