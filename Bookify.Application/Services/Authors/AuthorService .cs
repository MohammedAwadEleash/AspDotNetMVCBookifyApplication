namespace Bookify.Application.Services
{
    internal class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Author> GetAll()
        {
            return _unitOfWork.Authors.GetAll(withNoTracking: false);
        }

        public IEnumerable<Author> GetActiveAuthors() => _unitOfWork.Authors.FindAll(a => !a.IsDeleted, a => a.Name).ToList();



        public bool AllowAuthor(int id, string name)
        {
            Author? author = _unitOfWork.Authors.Find(a => a.Name == name);

            var isAllowed = author is null || author.Id.Equals(id);

            return isAllowed;
        }


        public Author? GetById(int id)
        {

            return _unitOfWork.Authors.GetById(id);


        }

        public Author Create(string name, string createdById)
        {
            var author = new Author
            {
                Name = name,
                CreatedById = createdById
            };
            _unitOfWork.Authors.Add(author);
            _unitOfWork.Complete();
            return author;

        }


        public Author? Update(int id, string name, string updatedById)
        {

            var author = _unitOfWork.Authors.GetById(id);

            if (author is null)
                return null;
            author.Name = name;
            author.LastUpdatedById = updatedById;
            author.LastUpdatedOn = DateTime.Now;
            //_unitOfWork.Authors.Update(author);
            _unitOfWork.Complete();
            return author;

        }

        public Author? ToggleStatus(int id, string updatedById)
        {

            var author = _unitOfWork.Authors.GetById(id); //this part is  Data access logic

            // from here : the begining of  Business Logic:
            if (author is null)
                return null;

            author.IsDeleted = !author.IsDeleted;
            author.LastUpdatedById = updatedById;
            author.LastUpdatedOn = DateTime.Now;
            /// the end of Business Logic


            _unitOfWork.Complete(); //this part is  Data access logic

            return author;


        }





    }
}
