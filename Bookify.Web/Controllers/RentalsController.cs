
using Microsoft.AspNetCore.DataProtection;

namespace Bookify.Web.Controllers
{

    [Authorize(Roles = AppRoles.Reception)]
    public class RentalsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IDataProtector _dataProtector;
        private readonly IRentalService _rentalService;
        private readonly ISubscriberService _subscriberService;



        public RentalsController(IMapper mapper, IDataProtectionProvider dataProtector, IRentalService rentalService, ISubscriberService subscriberService)
        {
            _mapper = mapper;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
            _rentalService = rentalService;
            _subscriberService = subscriberService;
        }


        public IActionResult Details(int id)
        {


            var rental = _rentalService.GetQueryableDetails().SingleOrDefault(r => r.Id == id);         // this function get  reference to  all rentals with  RentalCopies, BookCopy ,Book (but they still in database)

            var viewModel = _mapper.Map<RentalViewModel>(rental);
            // here SingleOrDefault returns one record  for  the specified rental by id after column filtering  Process by ProjecTo

            if (viewModel is null)
                return NotFound();



            return View("RentalDetails", viewModel);



        }


        public IActionResult Create(string sKey)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(sKey));



            var (errorMessage, maxAllowedCopies) = _rentalService.ValidateSubscriber(subscriberId);



            if (!string.IsNullOrEmpty(errorMessage))
                return View("NotAllowedRental", errorMessage);

            var viewModel = new RentalFormViewModel
            {
                SubscriberKey = sKey,
                MaxAllowedCopies = maxAllowedCopies
            };

            return View("Form", viewModel);
        }



        [HttpPost]

        public IActionResult Create(RentalFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var subscriberId = int.Parse(_dataProtector.Unprotect(model.SubscriberKey));

            var (errorMessage, maxAlloedCopies) = _rentalService.ValidateSubscriber(subscriberId);

            if (!string.IsNullOrEmpty(errorMessage))
                return View("NotAllowedRental", errorMessage);





            var (ErrorMessage, copies) = _rentalService.ValidateCopies(model.SelectedCopies, subscriberId);


            if (!string.IsNullOrEmpty(ErrorMessage))
                return View("NotAllowedRental", ErrorMessage);


            var rental = _rentalService.Create(subscriberId, copies, User.GetUserId());
            return RedirectToAction(nameof(Details), new { id = rental.Id });
        }


        public IActionResult Edit(int id)
        {

            var rental = _rentalService.RentalDetails(id);

            if (rental is null || rental.CreatedOn.Date != DateTime.Today)
                return NotFound();


            var (errorMessage, maxAllowedCopies) = _rentalService.ValidateSubscriber(rental.SubscriberId, rental.Id);

            if (!string.IsNullOrEmpty(errorMessage))
                return View("NotAllowedRental", errorMessage);


            var currentCopiesIds = rental.RentalCopies.Select(c => c.BookCopyId).ToList();


            var currentCopies = _rentalService.GetRentalCopies(currentCopiesIds);

            var viewModel = new RentalFormViewModel()
            {

                SubscriberKey = _dataProtector.Protect(rental.SubscriberId.ToString()),
                MaxAllowedCopies = maxAllowedCopies,
                CurrentCopies = _mapper.Map<IEnumerable<BookCopyViewModel>>(currentCopies)

            };

            return View("Form", viewModel);
        }


        [HttpPost]
        public IActionResult Edit(RentalFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);
            var id = model.Id ?? 0;
            var rental = _rentalService.RentalDetails(id);

            if (rental is null || rental.CreatedOn.Date != DateTime.Today)
                return NotFound();

            var subscriberId = int.Parse(_dataProtector.Unprotect(model.SubscriberKey));


            var (errorMessage, maxAllowedCopies) = _rentalService.ValidateSubscriber(subscriberId, model.Id);

            if (!string.IsNullOrEmpty(errorMessage))
                return View("NotAllowedRental", errorMessage);




            var (ErrorMessage, copies) = _rentalService.ValidateCopies(model.SelectedCopies, subscriberId, model.Id);
            if (!string.IsNullOrEmpty(ErrorMessage))
                return View("NotAllowedRental", ErrorMessage);

            var rentntal = _rentalService.Update(id, copies, User.GetUserId());
            return RedirectToAction(nameof(Details), new { id = rental.Id });
        }




        public IActionResult Return(int id)
        {

            var rental = _rentalService.RentalDetails(id);
            if (rental is null || rental.CreatedOn.Date == DateTime.Today)
                return NotFound();

            var subscriber = _subscriberService.GetSubscriberWithSubscriptions(rental.SubscriberId);


            var viewModel = new ReturnFormViewModel
            {
                Id = rental.Id,
                Copies = _mapper.Map<IList<RentalCopyViewModel>>(rental.RentalCopies.Where(rc => !rc.ReturnDate.HasValue)),
                SelectedCopies = rental.RentalCopies.Where(rc => !rc.ReturnDate.HasValue).Select(rc => new ReturnCopyViewModel { Id = rc.BookCopyId, IsReturned = rc.ExtendedOn.HasValue ? false : null }).ToList(),

                AllowExtend = _rentalService.ISAllowExtend(subscriber!, rental)

            };
            return View(viewModel);
        }


        [HttpPost]


        public IActionResult Return(ReturnFormViewModel model)
        {


            var rental = _rentalService.RentalDetails(model.Id);

            if (rental is null || rental.CreatedOn.Date == DateTime.Today)
                return NotFound();

            var copies = _mapper.Map<IList<RentalCopyViewModel>>(rental.RentalCopies.Where(rc => !rc.ReturnDate.HasValue));
            if (!ModelState.IsValid)
            {

                model.Copies = copies;
                return View(model);
            }
            var subscriber = _subscriberService.GetSubscriberWithSubscriptions(rental.SubscriberId);

            // This case is for extending the copy only:
            if (model.SelectedCopies.Any(re => re.IsReturned.HasValue && !re.IsReturned.Value))
            {


                // Note : ValidateExtendedCopies   this function :Verify whether the subscriber, subscription and rental meet the criteria for extending this Copy or not ;

                var errorMessage = _rentalService.ValidateExtendedCopies(subscriber, rental);




                if (!string.IsNullOrEmpty(errorMessage))
                {
                    model.Copies = copies;
                    ModelState.AddModelError("", errorMessage);
                    return View(model);
                }
            }


            var copiesDto = _mapper.Map<IEnumerable<ReturnCopyDto>>(model.SelectedCopies);



            _rentalService.Return(rental, copiesDto, model.PenaltyPaid, User.GetUserId());


            return RedirectToAction(nameof(Details), new { id = rental.Id });
        }
        [HttpPost]
        public IActionResult GetCopyDetails(SearchFormViewModel model)

        {

            if (!ModelState.IsValid)
                return BadRequest();

            var copy = _rentalService.GetCopyBySerialNumber(model.Value);

            if (copy is null)
                return NotFound(Errors.InvalidSerialNumber);

            if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
                return BadRequest(Errors.NotAvilableRental);

            // check if the copy is in  rental
            var copyIsInRental = _rentalService.IsCopyInRental(copy.Id);
            if (copyIsInRental)
                return BadRequest(Errors.CopyIsInRental);


            var viewModel = _mapper.Map<BookCopyViewModel>(copy);
            return PartialView("_CopyDetails", viewModel);


        }


        [HttpPost]

        public IActionResult MarkAsDeleted(int id)
        {

            var rental = _rentalService.MarkAsDeleted(id, User.GetUserId());

            if (rental is null)
                return NotFound();


            var numberOfCopies = _rentalService.GetNumberOfCopies(id);

            return Ok(numberOfCopies);

        }




    }
}