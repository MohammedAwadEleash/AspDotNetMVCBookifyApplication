namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]

    public class BookCopiesController : Controller
    {


        private readonly IMapper _mapper;
        private readonly IValidator<BookCopyFormViewModel> _validator;

        private readonly IBookCopyService _bookCopyService;
        private readonly IBookService _bookService;



        public BookCopiesController(IMapper mapper, IValidator<BookCopyFormViewModel> validator, IBookCopyService bookCopyService, IBookService bookService)
        {
            _mapper = mapper;
            _validator = validator;
            _bookCopyService = bookCopyService;
            _bookService = bookService;
        }

        [AjaxOnly]
        public IActionResult Create(int BookId)
        {

            var book = _bookService.GetById(BookId);
            if (book is null)
                return NotFound();
            var viewModel = new BookCopyFormViewModel
            {

                BookId = BookId,
                ShowRentalInput = book.IsAvailableForRental


            };


            return PartialView("Form", viewModel);

        }

        [HttpPost]
        public IActionResult Create(BookCopyFormViewModel model)
        {

            var validationResult = _validator.Validate(model);
            if (!validationResult.IsValid)
                return BadRequest();

            var bookCopy = _bookCopyService.Create(model.BookId, model.EditionNumber, model.IsAvailableForRental, User.GetUserId());

            if (bookCopy is null)
                return NotFound();

            var viewModel = _mapper.Map<BookCopyViewModel>(bookCopy);

            return PartialView("_BookCopyRow", viewModel);
        }

        [AjaxOnly]
        public IActionResult Edit(int id)

        {

            var copy = _bookCopyService.GetDetails(id);    // GetDetails is get BookCopies and Book

            if (copy is null)
                return NotFound();

            var viewModel = _mapper.Map<BookCopyFormViewModel>(copy);
            viewModel.ShowRentalInput = copy.Book!.IsAvailableForRental;

            return PartialView("Form", viewModel);
        }
        [HttpPost]

        public IActionResult Edit(BookCopyFormViewModel model)

        {

            var validationResult = _validator.Validate(model);
            if (!validationResult.IsValid)
                return BadRequest();

            var copy = _bookCopyService.Update(model.Id, model.EditionNumber, model.IsAvailableForRental, User.GetUserId());
            if (copy is null)
                return NotFound();




            var viewModel = _mapper.Map<BookCopyViewModel>(copy);


            return PartialView("_BookCopyRow", viewModel);
        }
        [HttpPost]

        public IActionResult ToggleStatus(int id)
        {

            var bookCopy = _bookCopyService.ToggleStatus(id, User.GetUserId());
            if (bookCopy is null)
                return NotFound();


            return Ok(bookCopy.LastUpdatedOn.ToString());
        }



        public IActionResult RentalHistory(int id)

        {
            var copies = _bookCopyService.GetRentalHistoryForThisCopy(id);

            var viewModel = _mapper.Map<IEnumerable<CopyHistoryViewModel>>(copies);

            return View(viewModel);

        }
    }


}
