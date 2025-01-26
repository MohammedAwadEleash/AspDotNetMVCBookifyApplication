using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using System.Linq.Dynamic.Core;


namespace Bookify.Web.Controllers

{
    [Authorize(Roles = AppRoles.Archive)]

    public class BooksController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly IImageService _imageService;

        private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private int _maxAllowedSize = 2097152;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;

        public BooksController(IMapper mapper,
            IWebHostEnvironment webHostEnvironment, IOptions<CloudinarySettings> cloudinary,
            IImageService imageService, IBookService bookService, IAuthorService authorService, ICategoryService categoryService)
        {
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;

            Account account = new()
            {
                Cloud = cloudinary.Value.Cloud,
                ApiKey = cloudinary.Value.ApiKey,
                ApiSecret = cloudinary.Value.ApiSecret
            };

            _cloudinary = new Cloudinary(account);
            _imageService = imageService;
            _bookService = bookService;
            _authorService = authorService;
            _categoryService = categoryService;
        }



        public IActionResult Index()
        {
            return View();
        }
        [HttpPost, IgnoreAntiforgeryToken]
        public IActionResult GetBooks()
        {

            //var form = Request.Form;
            var filteredbooksDto = Request.Form.GetDataTableFilters();


            var (books, recordsTotal) = _bookService.GetFilteredQuerable(dto: filteredbooksDto);


            // Important Note : books are  still inside the database, as long as the query has not been excuted yet;
            var mappedData = _mapper.ProjectTo<BookRowViewModel>(books).ToList();


            /// SE=econd Way : Get All Data from DataBasde  (never dQuerable):
            //var (books, recordsTotal) = _bookService.GetFilteredBooks( dto: filteredbooksDto);
            //var mappedData = _mapper.Map<IEnumerable<BookRowViewModel>>(books);
            //var mappedData = _mapper.Map<IEnumerable<BookRowViewModel>>(books);

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);


        }

        public IActionResult Details(int id)
        {

            var book = _bookService.GetDetailsQueryable().SingleOrDefault(c => c.Id == id);




            if (book is null)
                return NotFound();
            var viewMode = _mapper.Map<BookViewModel>(book);






            return View(viewMode);

        }
        public IActionResult Create()
        {


            var viewModel = PopulateViewModel(null);

            return View("Form", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(BookFormViewModel model)
        {


            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));
            var book = _mapper.Map<Book>(model);


            if (model.Image is not null)

            {


                var extension = Path.GetExtension(model.Image.FileName);
                var imageName = $"{Guid.NewGuid()}{extension}";
                var imagePath = "/images/books";

                var result = await _imageService.UploadAsync(model.Image, imageName, imagePath, hashThumbnail: true);
                // result = Touple
                if (!result.isUploaded)
                {
                    ModelState.AddModelError(nameof(model.Image), result.errorMessage!);


                    return View("Form", PopulateViewModel(model));


                }


                book.ImageUrl = $"{imagePath}/{imageName}";
                book.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";

                // Cloudinary service : 

                //using var stream = model.Image.OpenReadStream();

                //var imageParams = new ImageUploadParams
                //{

                //    File = new FileDescription(imageName, stream),
                //  UseFilename=true
                //};
                //var result = await _cloudinary.UploadAsync(imageParams);

                //book.ImageUrl=result.SecureUrl.ToString();
                //book.ImageThumbnailUrl = GetThumbnailUrl(book.ImageUrl);
                //book.ImagePublicId = result.PublicId;
            }

            book = _bookService.Create(book, model.SelectedCategories, User.GetUserId());

            return RedirectToAction(nameof(Details), new { id = book.Id });


        }


        [HttpGet]

        public IActionResult Edit(int id)
        {

            var book = _bookService.GetWithCategories(id);
            //var book = _context.Books.Include(b=>b.Categories).SingleOrDefault(b=>b.Id==id);

            if (book is null)
                return NotFound();


            var model = _mapper.Map<BookFormViewModel>(book);
            var viewModel = PopulateViewModel(model);

            viewModel.SelectedCategories = book.Categories.Select(c => c.CategoryId).ToList();
            return View("Form", viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(BookFormViewModel model)
        {


            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            // var book = _context.Books.Include(b => b.Categories).Include(b=>b.Copies).SingleOrDefault(b => b.Id == model.Id);

            var book = _bookService.GetWithCategories(model.Id);

            if (book is null)
                return NotFound();


            if (model.Image is not null)
            {
                // if this book  had a Image 
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    _imageService.DeleteImage(book.ImageUrl, book.ImageThumbnailUrl);
                }


                var extension = Path.GetExtension(model.Image.FileName);

                var imageName = $"{Guid.NewGuid()}{extension}";
                var result = await _imageService.UploadAsync(model.Image, imageName, "/images/books", hashThumbnail: true);
                /// result = Touple
                if (!result.isUploaded)
                {
                    ModelState.AddModelError(nameof(model.Image), result.errorMessage!);

                    return View("Form", PopulateViewModel(model));

                }


                model.ImageUrl = $"/images/books/{imageName}";
                model.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";


                ////model.ImageUrl = imageName;
                ////using var stream = model.Image.OpenReadStream();
                ////var imageParams = new ImageUploadParams
                ////{

                ////    File = new FileDescription(imageName, stream),
                ////    UseFilename = true
                ////};
                ////var  result = await  _cloudinary.UploadAsync(imageParams);

                ////model.ImageUrl = result.SecureUrl.ToString();
                ////imagePublicId = result.PublicId;
                ////book.ImageThumbnailUrl = GetThumbnailUrl(model.ImageUrl);


            }

            else if (!string.IsNullOrEmpty(book.ImageUrl))   // here : image of model was null
            {
                model.ImageUrl = book.ImageUrl;
                model.ImageThumbnailUrl = book.ImageThumbnailUrl;
            }

            book = _mapper.Map(model, book);

            book = _bookService.Update(book, model.SelectedCategories, User.GetUserId(), model.IsAvailableForRental);


            return RedirectToAction(nameof(Details), new { id = book.Id });

            //book.LastUpdatedOn = DateTime.Now;
            //book.LastUpdatedById = User.GetUserId();

            ////  book.ImagePublicId = imagePublicId;





            //if (!model.IsAvailableForRental)
            //{

            //    foreach (var copy in book.Copies)
            //    {
            //        copy.IsAvailableForRental = false;

            //    }
            //}
        }
        public IActionResult AllowItem(BookFormViewModel model)
        {
            var bookIsValid = _bookService.AllowBook(model.Id, model.Title, model.AuthorId);
            return Json(bookIsValid);
        }



        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var book = _bookService.ToggleStatus(id, User.GetUserId());
            if (book is null)
                return NotFound();

            return Ok(book.LastUpdatedOn.ToString());


        }
        private BookFormViewModel PopulateViewModel(BookFormViewModel? model)
        {

            BookFormViewModel viewModel = model is null ? new BookFormViewModel() : model;
            var authors = _authorService.GetActiveAuthors();

            var Categories = _categoryService.GetActiveCategories();



            viewModel.Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors);
            viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(Categories);


            return viewModel;

        }


        // this function is specific of Cloudinary Service:
        private string GetThumbnailUrl(string url)
        {

            var separator = "image/upload/";
            var urlParts = url.Split(separator);

            var thumbnailUrl = $"{urlParts[0]}{separator}c_thumb,w_200,g_face/{urlParts[1]}";

            return thumbnailUrl;
        }
    }
}
