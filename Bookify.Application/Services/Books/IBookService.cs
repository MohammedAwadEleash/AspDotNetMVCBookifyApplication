namespace Bookify.Application.Services
{
    public interface IBookService
    {
        (IEnumerable<Book> books, int count) GetFilteredBooks(DataTableFilterationDto dto);

        (IQueryable<Book> books, int count) GetFilteredQuerable(DataTableFilterationDto dto);

        public Book? GetDetails(int id);

        IQueryable<Book> GetDetailsQueryable();
        IQueryable<Book> Search(string value);
        Book Create(Book book, IList<int> selectedCategories, string createdById);
        Book? GetById(int id);
        Book? GetWithCategories(int id);

        Book Update(Book book, IList<int> selectedCategories, string updatedById, bool bookIsAvailableForRental);
        Book? ToggleStatus(int id, string updatedById);
        bool AllowBook(int id, string title, int AuthorId);
        // Dashboard:
        int GetNumberOfActiveBooks();
        IEnumerable<BookDto> GetLastAddedBooks(int numberOfBooks);
        IEnumerable<BookDto> GetTheTopBooks(int numberOfBooks);



    }
}
