

namespace Bookify.Application.Services
{
    public interface ICategoryService
    {

        IEnumerable<Category> GetAll();
        IEnumerable<Category> GetActiveCategories();
        Category? GetById(int id);


        bool AllowCategory(int id, string name);
        Category Create(string name, string createdById);
        Category? Update(int id, string name, string updatedById);
        Category? ToggleStatus(int id, string updatedById);


    }
}
