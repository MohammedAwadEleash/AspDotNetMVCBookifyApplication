namespace Bookify.Domain.Entities
{
    public class BookCategory
    {

        public int BookId { set; get; }
        public Book? Book { set; get; }

        public int CategoryId { set; get; }
        public Category? Category { set; get; }
    }
}
