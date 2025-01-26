namespace Bookify.Application.Services
{
    public interface IBookCopyService
    {

        BookCopy? GetById(int id);
        BookCopy? Create(int bookId, int editionNumber, bool isAvailableForRental, string createdById);
        BookCopy? Update(int id, int editionNumber, bool isAvailableForRental, string updatedById);

        BookCopy? ToggleStatus(int id, string updatedById);
        BookCopy? GetDetails(int id);
        IEnumerable<RentalCopy> GetRentalHistoryForThisCopy(int id);


    }
}
