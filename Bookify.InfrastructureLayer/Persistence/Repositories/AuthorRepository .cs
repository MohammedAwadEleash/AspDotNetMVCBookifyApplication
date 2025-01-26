using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Persistence.Repositories;

public class AuthorRepository : IAuthorRepository
{

    private readonly IApplicationDbContext _context;

    public AuthorRepository(IApplicationDbContext context)
    {
        _context = context;
    }



    public IEnumerable<Author> GetAll()
    {
        return _context.Authors.AsNoTracking().ToList();

    }
    public Author Add(Author auhtor, string userId)
    {
        _context.Authors.Add(auhtor);
        auhtor.CreatedById = userId;
        _context.SaveChanges();
        return auhtor;

    }

    Author? IAuthorRepository.GetById(int id) => _context.Authors.Find(id);

}
