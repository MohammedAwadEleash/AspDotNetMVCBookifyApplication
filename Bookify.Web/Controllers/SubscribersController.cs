using Hangfire;
using Microsoft.AspNetCore.DataProtection;

namespace Bookify.Web.Controllers
{

    [Authorize(Roles = AppRoles.Reception)]
    public class SubscribersController : Controller
    {


        private readonly IDataProtector _dataProtector;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IMapper _mapper;
        private readonly IWhatsAppService _whatsAppService;
        private readonly IImageService _imageService;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly IEmailSender _emailSender;
        private readonly ISubscriberService _subscriberService;

        public SubscribersController(IMapper mapper, IImageService imageService, IDataProtectionProvider dataProtector, IWhatsAppService whatsAppService, IWebHostEnvironment webHostEnvironment, IEmailBodyBuilder emailBodyBuilder, IEmailSender emailSender, ISubscriberService subscriberService)
        {
            _mapper = mapper;
            _imageService = imageService;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
            _whatsAppService = whatsAppService;
            _webHostEnvironment = webHostEnvironment;
            _emailBodyBuilder = emailBodyBuilder;
            _emailSender = emailSender;
            _subscriberService = subscriberService;
        }
        public IActionResult Index()
        {



            return View();
        }

        [HttpPost]
        public IActionResult Search(SearchFormViewModel model)

        {
            if (!ModelState.IsValid)
                return BadRequest();

            var query = _subscriberService.GetQueryable();






            var viewModel = _mapper.ProjectTo<SubscriberSearchResultViewModel>(query).SingleOrDefault(s => s.Email == model.Value
            || s.NationalId == model.Value || s.MobileNumber == model.Value);


            if (viewModel is not null)
            {
                viewModel.Key = _dataProtector.Protect(viewModel.Id.ToString());
                // here : Id has just been protected 

            }

            return PartialView("_Result", viewModel);
        }
        public IActionResult Details(string id)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(id));  // id is decrypted

            var subscriber = _subscriberService.GetQueryableDetails().SingleOrDefault(s => s.Id == subscriberId);

            if (subscriber is null)
                return NotFound();

            var viewModel = _mapper.Map<SubscriberViewModel>(subscriber);


            viewModel.Key = id;


            return View(viewModel);




        }
        [HttpGet]
        public IActionResult Create()
        {



            //  PopulateViewModel : Create new instance of SubscriberFormViewModel

            var viewModel = PopulateViewModel(null);
            return View("Form", viewModel);
        }



        [HttpPost]
        public async Task<IActionResult?> Create(SubscriberFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model)); // to fill Dropdown Lists Again

            var subscriber = _mapper.Map<Subscriber>(model);




            var extentionImage = Path.GetExtension(model.Image!.FileName);
            var imageName = $"{Guid.NewGuid()}.{extentionImage}";
            var imagePath = "/images/subscribers";


            var result = await _imageService.UploadAsync(model.Image, imageName, imagePath, hashThumbnail: true);
            // subscriber  has  Thumbnail Image

            if (!result.isUploaded)
            {
                ModelState.AddModelError("Image", result.errorMessage!);
                return View("Form", PopulateViewModel(model));

            }


            subscriber = _subscriberService.Create(subscriber, imagePath, imageName, User.GetUserId());
            //Send Welcome message using emaail 

            var placeholders = new Dictionary<string, string>()
                {
                    { "[imageUrl]", "https://res.cloudinary.com/eleash/image/upload/v1733777850/icon-positive-vote-1_nx5xzt.svg" },
                    { "[header]", $"Welcome {model.FirstName}" },
                    { "[body]", "thanks for joining Bookify \U0001f929" },
            };

            var body = _emailBodyBuilder.GetEmailBody(EmailTemplates.Notification, placeholders);
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(model.Email, "Welcome to Bookify", body));




            //Send welcome message using WhatsApp


            if (model.HasWhatsApp)
            {


                var mobileNumber = _webHostEnvironment.IsDevelopment() ? "201001779462" : $"2{model.MobileNumber}";



                BackgroundJob.Enqueue(() => _whatsAppService.SendWhatsAppMessage(mobileNumber, $"Welcome {model.FirstName}" +
                    $",thanks for joining Bookify \U0001f929"));


            }

            var subscriberId = _dataProtector.Protect(subscriber.Id.ToString());


            return RedirectToAction(nameof(Details), new { id = subscriberId });

        }



        [HttpGet]
        public IActionResult Edit(string id)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(id));

            var subscriber = _subscriberService.GetById(subscriberId);


            if (subscriber is null)
                return NotFound();

            var viewModel = _mapper.Map<SubscriberFormViewModel>(subscriber);
            viewModel = PopulateViewModel(viewModel);
            viewModel.Key = id;

            return View("Form", viewModel);



        }
        [HttpPost]
        public async Task<IActionResult?> Edit(SubscriberFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model)); // to fill Dropdown Lists Again

            var subscriberId = int.Parse(_dataProtector.Unprotect(model.Key!));



            var subscriber = _subscriberService.GetById(subscriberId);

            if (subscriber is null)
                return NotFound();

            if (model.Image is not null)

            {
                if (!string.IsNullOrEmpty(subscriber.ImageUrl))
                    _imageService.DeleteImage(subscriber.ImageUrl, subscriber.ImageThumbnailUrl);


                var extentionImage = Path.GetExtension(model.Image.FileName);
                var imageName = $"{Guid.NewGuid()}.{extentionImage}";
                var imagePath = "/images/subscribers";



                var result = await _imageService.UploadAsync(model.Image, imageName, imagePath, hashThumbnail: true);
                // subscriber  has  Thumbnail Image

                if (!result.isUploaded)
                {
                    ModelState.AddModelError("Image", result.errorMessage!);
                    return View("Form", PopulateViewModel(model));

                }


                model.ImageUrl = $"{imagePath}/{imageName}";
                model.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";

            }

            else if (!string.IsNullOrEmpty(subscriber.ImageUrl))
            {
                model.ImageUrl = subscriber.ImageUrl;
                model.ImageThumbnailUrl = subscriber.ImageThumbnailUrl;

            }

            subscriber = _mapper.Map(model, subscriber);
            _subscriberService.Update(subscriber, User.GetUserId());

            return RedirectToAction(nameof(Details), new { id = model.Key });

        }

        [AjaxOnly]
        public IActionResult GetAreas(int gevernorateId)
        {
            var areas = _subscriberService.GetActiveAreasByGovernorateId(gevernorateId);

            var areaList = _mapper.Map<IEnumerable<SelectListItem>>(areas);

            return Ok(areaList);
        }


        [HttpPost]

        public IActionResult RenewalSubscription(string sKey)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(sKey));
            var subscriber = _subscriberService.GetSubscriberWithSubscriptions(subscriberId);


            if (subscriber is null)
                return NotFound();

            if (subscriber.Isblacklisted)  // if (Isblacklisted = true) the Add button shouldn't be visible in Details view  
                return BadRequest();

            var newsubscription = _subscriberService.RenewSubscription(subscriber, User.GetUserId());



            //  Send  email message

            var placeholders = new Dictionary<string, string>()
                {
                    { "[imageUrl]", "https://res.cloudinary.com/eleash/image/upload/v1733777850/icon-positive-vote-1_nx5xzt.svg" },
                    { "[header]", $"Welcome {subscriber.FirstName}" },
                    { "[body]", $"your subscription has been renewed through {newsubscription.EndDate.ToString("d, MMM, yyyy")}🎉🎉 \U0001f973 ✅" },
            };

            var body = _emailBodyBuilder.GetEmailBody(EmailTemplates.Notification, placeholders);

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(subscriber.Email, "Bookify Subscription Renewal", body));

            //  BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(subscriber.Email, "Bookify Subscription Renewal", body), TimeSpan.FromMinutes(1));

            //  Send  WhatsApp message


            if (subscriber.HasWhatsApp)
            {


                var mobileNumber = _webHostEnvironment.IsDevelopment() ? "201001779462" : $"2{subscriber.MobileNumber}";

                BackgroundJob.Enqueue(() => _whatsAppService.SendWhatsAppMessage(mobileNumber, $"Welcome {subscriber.FirstName}, your subscription has been renewed through {newsubscription.EndDate.ToString("d, MMM, yyyy")}🎉🎉 \U0001f973 ✅"));

            }
            var viewModel = _mapper.Map<SubscriptionViewModel>(newsubscription);

            return PartialView("_SubscriptionRow", viewModel);

        }

        public IActionResult AllowEmail(SubscriberFormViewModel model)
        {
            int subscriberId = 0;
            if (!string.IsNullOrEmpty(model.Key))
                subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));



            var isValid = _subscriberService.AllowEmail(subscriberId, model.Email);
            return Json(isValid);



        }

        public IActionResult AllowNationalId(SubscriberFormViewModel model)
        {

            int subscriberId = 0;
            if (!string.IsNullOrEmpty(model.Key))

                subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));


            var isValid = _subscriberService.AllowNationalId(subscriberId, model.NationalId);
            return Json(isValid);

        }



        public IActionResult AllowMobileNumber(SubscriberFormViewModel model)

        {

            int subscriberId = 0;
            if (!string.IsNullOrEmpty(model.Key))
                subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));


            var isValid = _subscriberService.AllowMobileNumber(subscriberId, model.MobileNumber);
            return Json(isValid);


        }



        private SubscriberFormViewModel PopulateViewModel(SubscriberFormViewModel? model)
        {





            SubscriberFormViewModel viewModel = model is null ? new SubscriberFormViewModel() : model;


            var governorates = _subscriberService.GetActiveGovernorates();
            viewModel.Governorates = _mapper.Map<IEnumerable<SelectListItem>>(governorates);


            if (model?.GovernorateId > 0)
            {
                var areas = _subscriberService.GetActiveAreasByGovernorateId(model.GovernorateId);


                viewModel.Areas = _mapper.Map<IEnumerable<SelectListItem>>(areas);

            }




            return viewModel;




        }



    }



}
