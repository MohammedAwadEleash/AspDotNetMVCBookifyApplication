


namespace Bookify.Application.Services
{
    internal class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Category> GetAll()
        {
            return _unitOfWork.Categories.GetAll();
        }
        public IEnumerable<Category> GetActiveCategories() => _unitOfWork.Categories.FindAll(c => !c.IsDeleted, c => c.Name);


        public Category? GetById(int id)
        {

            return _unitOfWork.Categories.GetById(id);


        }
        public bool AllowCategory(int id, string name)
        {
            Category? category = _unitOfWork.Categories.Find(c => c.Name == name);

            var isAllowed = category is null || category.Id.Equals(id);

            return isAllowed;
        }

        public Category Create(string name, string createdById)
        {
            var category = new Category
            {
                Name = name,
                CreatedById = createdById
            };
            _unitOfWork.Categories.Add(category);
            _unitOfWork.Complete();
            return category;

        }


        public Category? Update(int id, string name, string updatedById)
        {

            var category = GetById(id);

            if (category is null)
                return null;

            category.Name = name;
            category.LastUpdatedById = updatedById;
            category.LastUpdatedOn = DateTime.Now;

            _unitOfWork.Complete();
            return category;

        }

        public Category? ToggleStatus(int id, string updatedById)
        {

            var Category = _unitOfWork.Categories.GetById(id); //this part is  Data access logic

            // from here : the begining of  Business Logic:
            if (Category is null)
                return null;

            Category.IsDeleted = !Category.IsDeleted;
            Category.LastUpdatedById = updatedById;
            Category.LastUpdatedOn = DateTime.Now;
            /// the end of Business Logic


            _unitOfWork.Complete(); //this part is  Data access logic

            return Category;


        }

    }
}
