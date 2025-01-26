namespace Bookify.Application.Services
{
    internal class BookCopyService : IBookCopyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookCopyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public BookCopy? GetById(int id)
        {
            return _unitOfWork.BookCopies.GetById(id);
        }
        public BookCopy? GetDetails(int id)
        {

            return _unitOfWork.BookCopies.Find(c => c.Id == id, c => c.Include(c => c.Book!));
        }
        public BookCopy? Create(int bookId, int editionNumber, bool isAvailableForRentalm, string createdById)
        {

            var book = _unitOfWork.Books.GetById(bookId);
            if (book is null)
                return null;


            BookCopy copy = new BookCopy
            {
                EditionNumber = editionNumber,
                IsAvailableForRental = book.IsAvailableForRental ? isAvailableForRentalm : false,
                CreatedById = createdById

            };

            book.Copies.Add(copy);
            _unitOfWork.Complete();
            return copy;


        }
        public BookCopy? Update(int id, int editionNumber, bool isAvailableForRental, string updatedById)
        {

            var copy = GetDetails(id);
            if (copy is null || copy.Book is null)
                return null;


            copy.EditionNumber = editionNumber;
            copy.IsAvailableForRental = copy.Book!.IsAvailableForRental ? isAvailableForRental : false;

            copy.LastUpdatedById = updatedById;
            copy.LastUpdatedOn = DateTime.Now;

            _unitOfWork.Complete();

            return copy;

        }


        public BookCopy? ToggleStatus(int id, string updatedById)
        {
            var copy = GetById(id);
            if (copy is null)
                return null;
            copy.IsDeleted = !copy.IsDeleted;
            copy.LastUpdatedById = updatedById;
            copy.LastUpdatedOn = DateTime.Now;
            _unitOfWork.Complete();

            return copy;

        }



        public IEnumerable<RentalCopy> GetRentalHistoryForThisCopy(int id)
        {

            var copies = _unitOfWork.RentalCopies.FindAll(predicate: rc => rc.BookCopyId == id,
            include: rc => rc.Include(rc => rc.Rental!).ThenInclude(r => r.subscriber!),
            orderBy: rc => rc.RentalDate, orderByDirection: OrderBy.Descending);
            return copies;
        }

    }
}
