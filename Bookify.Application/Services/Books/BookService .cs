
using System.Linq.Dynamic.Core;

namespace Bookify.Application.Services
{
    public class BookService : IBookService
    {

        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public (IEnumerable<Book> books, int count) GetFilteredBooks(DataTableFilterationDto dto)
        {
            IQueryable<Book> books = _unitOfWork.Books.GetDetailsQuerable();


            if (!string.IsNullOrEmpty(dto.searchValue))
            {
                books = books.Where(b => b.Title.Contains(dto.searchValue!) || b.Author!.Name.Contains(dto.searchValue!));

            }
            // Using System.Linq.Dynamic.Core Package:

            books = books.OrderBy($"{dto.sortColumn}   {dto.sortColumnDirection}");

            var data = books.Skip(dto.skip).Take(dto.pageSize);
            var recordsTotal = _unitOfWork.Books.Count();

            return (data, recordsTotal);
        }

        public (IQueryable<Book> books, int count) GetFilteredQuerable(DataTableFilterationDto dto)
        {
            IQueryable<Book> books = _unitOfWork.Books.GetDetailsQuerable();


            if (!string.IsNullOrEmpty(dto.searchValue))
            {
                books = books.Where(b => b.Title.Contains(dto.searchValue!) || b.Author!.Name.Contains(dto.searchValue!));

            }
            // Using System.Linq.Dynamic.Core Package:

            books = books.OrderBy($"{dto.sortColumn}   {dto.sortColumnDirection}");

            var data = books.Skip(dto.skip).Take(dto.pageSize);
            var recordsTotal = _unitOfWork.Books.Count();

            return (data, recordsTotal);
        }
        public Book Create(Book book, IList<int> selectedCategories, string createdById)

        {
            foreach (var category in selectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = category });


            book.CreatedById = createdById;
            _unitOfWork.Books.Add(book);

            _unitOfWork.Complete();

            return book;
        }
        public Book? GetById(int id)
        {
            return _unitOfWork.Books.GetById(id);
        }

        public Book? GetWithCategories(int id)
        {
            return _unitOfWork.Books.Find(predicate: b => b.Id == id, include: b => b.Include(x => x.Categories));
        }

        public Book Update(Book book, IList<int> selectedCategories, string updatedById, bool bookIsAvailableForRental)
        {


            book.LastUpdatedById = updatedById;
            book.LastUpdatedOn = DateTime.Now;
            foreach (var category in selectedCategories)
            {
                book.Categories.Add(new BookCategory { CategoryId = category });
            }
            if (!bookIsAvailableForRental)
            {
                var copies = _unitOfWork.BookCopies.FindAll(c => c.BookId == book.Id);

                foreach (var copy in book.Copies)
                {
                    copy.IsAvailableForRental = false;

                }
            }
            _unitOfWork.Complete();


            return book;
        }


        public Book? ToggleStatus(int id, string updatedById)
        {
            var book = GetById(id);
            if (book is null)
                return null;
            book.IsDeleted = !book.IsDeleted;
            book.LastUpdatedById = updatedById;
            book.LastUpdatedOn = DateTime.Now;
            _unitOfWork.Complete();

            return book;

        }

        public bool AllowBook(int id, string title, int AuthorId)

        {
            var book = _unitOfWork.Books.Find(b => b.Title == title && b.AuthorId == AuthorId);

            var isValid = book is null || book.Id.Equals(id);

            return isValid;

        }

        public IQueryable<Book> GetDetailsQueryable()
        {
            return _unitOfWork.Books.GetDetailsQuerable();
        }
        public Book? GetDetails(int id)
        {
            return _unitOfWork.Books.GetDetailsQuerable().SingleOrDefault(b => b.Id == id);
        }
        public IQueryable<Book> Search(string value)
        {
            var qyery = _unitOfWork.Books.GetQueryable().Include(b => b.Author)
               .Where(b => !b.IsDeleted && (b.Title.Contains(value) || b.Author!.Name.Contains(value)));

            return qyery;
        }
        // Dashboard:
        public int GetNumberOfActiveBooks()
        {
            return _unitOfWork.Books.Count(b => !b.IsDeleted);
        }

        public IEnumerable<BookDto> GetLastAddedBooks(int numberOfBooks)
        {
            var lastAddedBooks = _unitOfWork.Books.GetQueryable().Include(b => b.Author)
        .Where(b => !b.IsDeleted).OrderByDescending(b => b.Id).Take(numberOfBooks)
        .Select(b => new BookDto(
          b.Id,
          b.Title,
          b.ImageThumbnailUrl,
          b.Author!.Name

        )

        )

        .ToList();



            return lastAddedBooks;

        }

        public IEnumerable<BookDto> GetTheTopBooks(int numberOfBooks)
        {
            var topBooks = _unitOfWork.RentalCopies.GetQueryable()
                .Include(rc => rc.BookCopy)
                .ThenInclude(c => c.Book!)
                .ThenInclude(b => b.Author!)
                .GroupBy(rc => new
                {
                    BookId = rc.BookCopy!.BookId,
                    BookTitle = rc.BookCopy.Book!.Title,
                    ImageThumbnailUrl = rc.BookCopy.Book!.ImageThumbnailUrl,
                    AuthorName = rc.BookCopy!.Book!.Author!.Name



                }).Select(b => new

                {
                    b.Key.BookId,
                    b.Key.BookTitle,
                    b.Key.ImageThumbnailUrl,
                    b.Key.AuthorName,
                    Count = b.Count()




                }).OrderByDescending(b => b.Count)
                .Take(numberOfBooks)
                .Select(b => new BookDto
                (
                    b.BookId,
                    b.BookTitle,
                    b.ImageThumbnailUrl,
                    b.AuthorName

                    )


                ).ToList();
            return topBooks;
        }


    }
}
