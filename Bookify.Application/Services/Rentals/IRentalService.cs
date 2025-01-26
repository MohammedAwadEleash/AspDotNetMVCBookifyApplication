namespace Bookify.Application.Services
{
    public interface IRentalService
    {
        IQueryable<Rental?> GetQueryableDetails();
        IEnumerable<BookCopy> GetRentalCopies(IEnumerable<int> copies);
        BookCopy? GetCopyBySerialNumber(string serialNumber);
        int GetNumberOfCopies(int rentalId);
        bool IsCopyInRental(int copyId);
        bool ISAllowExtend(Subscriber subscriber, Rental rental);
        void Return(Rental rental, IEnumerable<ReturnCopyDto> copies, bool penaltyPaid, string updatedById);
        string ValidateExtendedCopies(Subscriber subscriber, Rental rental);
        Rental? RentalDetails(int id);
        (string errorMessage, int? maxAllowedCopies) ValidateSubscriber(int susbscriberId, int? rentalId = null);
        (string errorMessage, ICollection<RentalCopy> copies) ValidateCopies(IList<int> selectedCopies, int susbscriberId, int? rentalId = null);
        Rental Create(int subscriberId, ICollection<RentalCopy> copies, string createdById);
        Rental Update(int rentalId, ICollection<RentalCopy> copies, string updatedById);
        Rental? MarkAsDeleted(int rentalId, string updatedById);


        // Dashboard: 
        IEnumerable<KeyValuePairDto> GetRentalsPerDay(DateTime? startDate, DateTime? endDate);
        IEnumerable<KeyValuePairDto> GetDelayedBooks(int year);
        IEnumerable<KeyValuePairDto> GetUnDelayedBooks(int year);
        IEnumerable<KeyValuePairDto> GetExtendedBooks(int year);
    }
}
