
namespace Bookify.Web.Validators
{
    public class AuthorFormValidator : AbstractValidator<AuthorFormViewModel>
    {

        public AuthorFormValidator()
        {

            RuleFor(a => a.Name).MaximumLength(100).WithMessage(Errors.OherMaxLength)
                .Matches(RegexPatterns.CharactersOnly_Eng).WithMessage(Errors.OnlyEnglishLetters);

        }
    }
}
