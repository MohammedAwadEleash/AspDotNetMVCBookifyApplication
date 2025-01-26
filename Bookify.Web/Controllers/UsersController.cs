using Bookify.Application.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace Bookify.Web.Controllers
{

    [Authorize(Roles = AppRoles.Admin)]
    public class UsersController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly IUserService _userService;


        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, IEmailBodyBuilder emailBodyBuilder, IUserService userService)
        {
            _userManager = userManager;
            this._roleManager = roleManager;
            _mapper = mapper;
            _emailSender = emailSender;
            this._webHostEnvironment = webHostEnvironment;
            _emailBodyBuilder = emailBodyBuilder;
            _userService = userService;
        }



        public async Task<IActionResult> Index()
        {


            var users = await _userService.GetUsersAsync();

            var viewModel = _mapper.Map<IEnumerable<UserViewModel>>(users);


            // _context.Users.ToList(); 

            return View(viewModel);
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Create()
        {


            var roles = await _userService.GetRolesAsync();


            var viewModel = new UserFormViewModel
            {
                Roles = roles.Select(r => new SelectListItem
                {

                    Text = r.Name,
                    Value = r.Name
                })
            };

            ///UserFormViewModel viewModel = await PopulateViewModel(null);
            return PartialView("_Form", viewModel);

        }
        [HttpPost]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {

            if (!ModelState.IsValid)
                return BadRequest();


            var dtoUser = _mapper.Map<CreateUserDto>(model);

            var result = await _userService.CreateUserAsync(dtoUser, User.GetUserId());

            if (result.IsSucceeded)
            {

                var user = result.User;
                var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(result.VerificationCode));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user!.Id, code },
                    protocol: Request.Scheme);

                var placeholders = new Dictionary<string, string>()
                {
                    { "[imageUrl]", "https://res.cloudinary.com/eleash/image/upload/v1733777850/icon-positive-vote-1_nx5xzt.svg" },
                    { "[header]", $"Welcome {user.FullName}, thanks for joinning us!" },
                    { "[body]", "please confirm your email" },
                    { "[url]", $"{HtmlEncoder.Default.Encode(callbackUrl!)}" },
                    { "[linkTitle]", "Active Account!l" }
                };

                var body = _emailBodyBuilder.GetEmailBody(EmailTemplates.Email, placeholders);

                await _emailSender.SendEmailAsync(user.Email!, "Confirm your email", body);


                var viewModel = _mapper.Map<UserViewModel>(user);
                return PartialView("_UserRow", viewModel);

            }

            return BadRequest(string.Join(',', result.Errors!));

        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(string id)
        {

            var user = await _userService.GetUsersByIdAsync(id);

            if (user is null)
                return NotFound();

            var roles = await _userService.GetRolesAsync();

            var viewModel = _mapper.Map<UserFormViewModel>(user);

            viewModel.SelectedRoles = await _userService.GetUsersRolesAsync(user);  // Get the selected roles of this user
            viewModel.Roles = roles.Select(r => new SelectListItem
            {

                Text = r.Name,
                Value = r.Name
            });



            return PartialView("_Form", viewModel);



        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserFormViewModel model)

        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userService.GetUsersByIdAsync(model.Id!);
            if (user is null)
                return NotFound();



            user = _mapper.Map(model, user);



            var result = await _userService.UpdateUserAsync(user, model.SelectedRoles, User.GetUserId());
            if (result.IsSucceeded)
            {
                var viewModel = _mapper.Map<UserViewModel>(result.User);
                return PartialView("_UserRow", viewModel);
            }


            return BadRequest(string.Join(',', result.Errors!));

        }


        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> ResetPassword(string id)
        {

            var user = await _userService.GetUsersByIdAsync(id);


            if (user is null)
                return NotFound();

            var viewModel = new ResetPasswordFormViewModel
            {
                Id = user.Id
            };

            return PartialView("_ResetPasswordForm", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordFormViewModel model)
        {


            if (!ModelState.IsValid)
                return BadRequest();




            var user = await _userService.GetUsersByIdAsync(model.Id);

            if (user is null)
                return NotFound();


            var result = await _userService.ResetPasswordAsync(user, model.Password, User.GetUserId());
            if (result.IsSucceeded)
            {
                var viewModel = _mapper.Map<UserViewModel>(result.User);

                return PartialView("_UserRow", viewModel);

            }

            return BadRequest(string.Join(',', result.Errors!));

        }





        public async Task<IActionResult> AllowUserName(UserFormViewModel model)
        {


            var isValid = await _userService.AllowUserNameAsync(model.Id, model.UserName);
            return Json(isValid);

        }
        public async Task<IActionResult> AllowEmail(UserFormViewModel model)
        {

            var isValid = await _userService.AllowEmailAsync(model.Id, model.Email);
            return Json(isValid);


        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {

            var user = await _userService.ToggleStatus(id, User.GetUserId());
            if (user is null)
                return NotFound();

            return Ok(user.LastUpdatedOn.ToString());
        }



        [HttpPost]
        public async Task<IActionResult> Unlock(string id)
        {

            var user = await _userService.UnlockUserAsync(id);
            if (user is null)
                return NotFound();

            return Ok();

        }

        //
        private async Task<UserFormViewModel> PopulateViewModel(UserFormViewModel? model)

        {


            var viewModel = model is null ? new UserFormViewModel() : model;
            viewModel.Roles = await _roleManager.Roles.Select(r => new SelectListItem
            {

                Text = r.Name,
                Value = r.Name
            }).ToListAsync();



            return viewModel;
        }




    }
}

