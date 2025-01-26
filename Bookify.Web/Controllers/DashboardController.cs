namespace Bookify.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {

        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IBookService _bookService;
        private readonly ISubscriberService _subscriberService;
        private readonly IRentalService _rentalService;

        public DashboardController(IApplicationDbContext context, IMapper mapper, IBookService bookService, ISubscriberService subscriberService, IRentalService rentalService)
        {
            _context = context;
            _mapper = mapper;
            _bookService = bookService;
            _subscriberService = subscriberService;
            _rentalService = rentalService;
        }

        public IActionResult Index()
        {


            var numberOfBooks = _bookService.GetNumberOfActiveBooks();
            var numberOfSubscribers = _subscriberService.GetNumberOfActiveSubscribers();
            var lastAddedBooks = _bookService.GetLastAddedBooks(8);
            var topBooks = _bookService.GetTheTopBooks(8);



            numberOfBooks = numberOfBooks <= 10 ? numberOfBooks : (numberOfBooks / 10) * 10;

            var viewModel = new DashboardViewModel
            {

                NumberOfCopies = numberOfBooks,
                NumberOfSubscriber = numberOfSubscribers,
                LastAddedBooks = _mapper.Map<IEnumerable<BookViewModel>>(lastAddedBooks),
                TopBooks = _mapper.Map<IEnumerable<BookViewModel>>(topBooks)
            };
            return View(viewModel);



        }
        [AjaxOnly]
        public IActionResult GetRentalsPerDay(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.Today.AddDays(-29);
            endDate ??= DateTime.Today;

            var data = _rentalService.GetRentalsPerDay(startDate, endDate);
            var chartItems = _mapper.Map<IEnumerable<ChartItemViewModel>>(data);
            return Ok(chartItems);


        }

        [AjaxOnly]
        public IActionResult GetSubscribersPerCity()
        {

            var data = _subscriberService.GetSubscribersPerCity();
            var chartItems = _mapper.Map<IEnumerable<ChartItemViewModel>>(data);


            return Ok(chartItems);
        }

        [AjaxOnly]

        public IActionResult GetStatusRentedBooksperMonth(int year)
        {

            var months = Enumerable.Range(1, 12).Select(n => new DateTime(year, n, 1).ToString("MMMM")).ToList();


            var delayedBooksDto = _rentalService.GetDelayedBooks(year);

            var unDelayedBooksDto = _rentalService.GetUnDelayedBooks(year);
            var extendedBooksDto = _rentalService.GetExtendedBooks(year);

            var delayedBooks = _mapper.Map<IEnumerable<ChartItemViewModel>>(delayedBooksDto);
            var unDelayedBooks = _mapper.Map<IEnumerable<ChartItemViewModel>>(unDelayedBooksDto);
            var extendedBooks = _mapper.Map<IEnumerable<ChartItemViewModel>>(extendedBooksDto);


            var figures = new List<ChartItemViewModel>();

            string label;
            for (int i = 0; i < months.Count; i++)
            {
                label = months[i];
                var delayedBooksMonth = delayedBooks.SingleOrDefault(c => c.Label == label);
                var unDelayedBooksMonth = unDelayedBooks.SingleOrDefault(c => c.Label == label);
                var extendedBooksMonth = extendedBooks.SingleOrDefault(c => c.Label == label);
                ChartItemViewModel item = new()
                {
                    Label = label,

                    NumOfDelayedBooks = delayedBooksMonth is null ? "0" : delayedBooksMonth.Value,
                    NumOfUnDelayedBooks = unDelayedBooksMonth is null ? "0" : unDelayedBooksMonth.Value,
                    NumOfExtendedBooks = extendedBooksMonth is null ? "0" : extendedBooksMonth.Value,
                };
                figures.Add(item);

            }


            return Ok(figures);
        }


    }
}
