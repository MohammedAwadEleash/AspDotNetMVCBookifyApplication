using HashidsNet;

namespace Bookify.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHashids _hashids;
        private readonly IBookService _bookService;

        public SearchController(IApplicationDbContext context, IMapper mapper, IHashids hashids, IBookService bookService)
        {
            _context = context;
            _mapper = mapper;
            _hashids = hashids;
            _bookService = bookService;
        }

        public IActionResult Index()
        {


            return View();



        }
        public IActionResult Find(string query)
        {


            var qyerybooks = _bookService.Search(query);

            var searchResult = _mapper.ProjectTo<BookSearchResultViewModel>(qyerybooks).ToList();

            foreach (var item in searchResult)
                item.Key = _hashids.EncodeHex(item.Id.ToString());

            return Ok(searchResult);



        }
        public IActionResult BookDetails(string bookkey)
        {

            var bookId = int.Parse(_hashids.DecodeHex(bookkey));

            var book = _bookService.GetDetailsQueryable().SingleOrDefault(b => b.Id == bookId && !b.IsDeleted);



            if (book is null)
                return NotFound();


            var viewModel = _mapper.Map<BookViewModel>(book);




            return View(viewModel);


        }




    }
}
