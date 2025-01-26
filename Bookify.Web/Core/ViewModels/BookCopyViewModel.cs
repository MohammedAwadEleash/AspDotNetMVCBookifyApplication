namespace Bookify.Web.Core.ViewModels
{
    public class BookCopyViewModel
    {
        public int Id { get; set; }
        public string? BookTitle { get; set; }
        public int BookId { get; set; }
        public BookViewModel? Book { get; set; }
        public string? BookThumbnailUrl { get; set; }
        public bool IsAvailableForRental { get; set; }
        public int EditionNumber { get; set; }
        public int SerialNumber { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { set; get; } = DateTime.Now;
        public ICollection<RentalCopyViewModel> Rentals { get; set; } = new List<RentalCopyViewModel>();

    }
}
