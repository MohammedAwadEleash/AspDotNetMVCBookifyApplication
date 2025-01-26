namespace Bookify.Application.Comman.Repositories
{
    public interface IAuthorRepository
    {
        IEnumerable<Author> GetAll();
        Author Add(Author auhtor, string userId);

        Author? GetById(int id);
    }
}
