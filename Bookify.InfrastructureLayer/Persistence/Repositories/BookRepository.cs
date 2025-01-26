
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Persistence.Repositories
{
    internal class BookRepository : BaseRepository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Book> GetDetailsQuerable()
        {
            return base._context.Books
         .Include(b => b.Author)
       .Include(b => b.Copies)
        .Include(b => b.Categories)
         .ThenInclude(bc => bc.Category);
            //  Note : here after this linq code The data is still in the database  and has not been perform Query on it yet.

        }

        // public Book ? GetDetails(int id)
        // {
        //     return base._context.Books
        //  .Include(b => b.Author)
        //.Include(b => b.Copies).ThenInclude(c => c.Rentals)
        // .Include(b => b.Categories)
        //  .ThenInclude(bc => bc.Category).SingleOrDefault(b => b.Id == id);
        //     //  Note : here after this linq code The data is still in the database  and has not been perform Query on it yet.

        // }
    }
}
