using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModels
{
    public class SubscriberFormViewModel
    {

        public string? Key { set; get; }


        [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "First Name"),
            RegularExpression(RegexPatterns.DenySpecialCharacters, ErrorMessage = Errors.DenySpecialCharacters)]
        public string FirstName { get; set; } = null!;


        [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Last Name")
               , RegularExpression(RegexPatterns.DenySpecialCharacters, ErrorMessage = Errors.DenySpecialCharacters)]
        public string LastName { get; set; } = null!;

        [Display(Name = " Date of Birth "), AssertThat("DateOfBirth <= Today()", ErrorMessage = Errors.NotAllowFutureDates)]
        public DateTime DateOfBirth { get; set; }



        [MaxLength(14, ErrorMessage = Errors.MaxLength), Display(Name = " National ID"),
            RegularExpression(RegexPatterns.NationalID, ErrorMessage = Errors.InvalidNationalID)]
        [Remote("AllowNationalId", null!, AdditionalFields = "Key", ErrorMessage = Errors.Duplicated)]
        public string NationalId { get; set; } = null!;


        [MaxLength(11, ErrorMessage = Errors.MaxLength), Display(Name = " Mobile Number"),
            RegularExpression(RegexPatterns.MobileNumber, ErrorMessage = Errors.InvalidMobileNumber)]

        [Remote("AllowMobileNumber", null!, AdditionalFields = "Key", ErrorMessage = Errors.Duplicated)]
        public string MobileNumber { get; set; } = null!;


        [Display(Name = "Is he Has WhatsApp ?")]
        public bool HasWhatsApp { get; set; }

        [MaxLength(50, ErrorMessage = Errors.MaxLength)]

        [RegularExpression(RegexPatterns.emailPattern, ErrorMessage = Errors.InvalidEmail)]
        [Remote("AllowEmail", null!, AdditionalFields = "Key", ErrorMessage = Errors.Duplicated)]

        public string Email { get; set; } = null!;


        [RequiredIf("Key == '' ", ErrorMessage = Errors.EmptyImage)]
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }

        [Display(Name = "Area")]
        public int AreaId { get; set; }

        public IEnumerable<SelectListItem>? Areas { get; set; }


        [Display(Name = "Governorate ")]
        public int GovernorateId { get; set; }
        public IEnumerable<SelectListItem>? Governorates { get; set; }


        [MaxLength(500, ErrorMessage = Errors.MaxLength)]
        public string Address { get; set; } = null!;






    }
}
