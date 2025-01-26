
namespace Bookify.Web.Core.ViewModels
{
    public class AuthorFormViewModel
    {
        public int Id { get; set; }


        // index=0 -> Name="Category"
        // index =1 -> 100
        [/*MaxLength(100, ErrorMessage = Errors.MaxLength),*/ Display(Name = "Author"),
        /*RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)*/]

        [Remote("AllowItem", null!, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]

        public string Name { get; set; } = null!;
    }
}
