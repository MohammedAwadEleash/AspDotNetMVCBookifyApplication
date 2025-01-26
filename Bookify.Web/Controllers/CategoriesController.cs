namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]

    public class CategoriesController : Controller
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<CategoryFormViewModel> _validator;
        private readonly ICategoryService _categoryService;


        public CategoriesController(IApplicationDbContext context, IMapper mapper, IValidator<CategoryFormViewModel> validator, ICategoryService categoryService)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var Categories = _categoryService.GetAll();
            var viewModel = _mapper.Map<IEnumerable<CategoryViewModel>>(Categories);
            return View(viewModel);
        }

        [HttpGet]
        [AjaxOnly]  // its job Return or dispaly  Form 
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        // its job recieve  input Data  passed From Form (view) to store it in Database
        [HttpPost]
        public IActionResult Create(CategoryFormViewModel model)
        {
            var validationResult = _validator.Validate(model);


            if (!validationResult.IsValid)
                return BadRequest();


            var category = _categoryService.Create(model.Name, User.GetUserId());



            var viewModel = _mapper.Map<CategoryViewModel>(category);
            return PartialView("_CategoryRow", viewModel);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var category = _categoryService.GetById(id);
            if (category is null)

                return NotFound();

            var viewModel = _mapper.Map<CategoryFormViewModel>(category);


            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        public IActionResult Edit(CategoryFormViewModel model)
        {
            var validationResult = _validator.Validate(model);

            if (!validationResult.IsValid)
                return BadRequest();

            var category = _categoryService.Update(model.Id, model.Name, User.GetUserId());

            if (category is null)
                return NotFound();

            var viewModel = _mapper.Map<CategoryViewModel>(category);

            return PartialView("_CategoryRow", viewModel);

        }
        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {

            var category = _categoryService.ToggleStatus(id, User.GetUserId());

            if (category is null)
                return NotFound();


            return Ok(category.LastUpdatedOn.ToString());

        }
        public IActionResult AllowItem(CategoryFormViewModel model)
        {


            var isValid = _categoryService.AllowCategory(model.Id, model.Name);


            return Json(isValid);
        }

    }
}