
namespace Bookify.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }


        // index=0 -> Name="Category"
        // index =1 -> 100


        [/*MaxLength(3, ErrorMessage = Errors.MaxLength),*/ Display(Name = "Category"),
             RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]

        [Remote("AllowItem", null!, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]

        public string Name { get; set; } = null!;
    }
}