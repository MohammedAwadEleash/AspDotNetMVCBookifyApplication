

namespace Bookify.Application.Comman.Repositories
{
    public interface IBookRepository : IBaseRepository<Book>
    {
        IQueryable<Book> GetDetailsQuerable();


    }
}
