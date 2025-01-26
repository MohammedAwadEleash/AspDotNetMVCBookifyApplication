namespace Bookify.Web.Core.ViewModels
{
    public class BookRowViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;

        // Foreign Key from Authors
        public string Author { get; set; } = null!;
        public string Publisher { get; set; } = null!;
        public DateTime PublishingDate { get; set; }
        public string? ImageThumbnailUrl { get; set; }



        public string Hall { get; set; } = null!;
        public bool IsAvailableForRental { get; set; }



        public IEnumerable<string> Categories { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public DateTime? LastUpdatedOn { get; set; }
    }
}
