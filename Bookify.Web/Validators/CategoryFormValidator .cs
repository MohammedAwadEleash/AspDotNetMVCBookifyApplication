
namespace Bookify.Web.Validators
{

    public class CategoryFormValidator : AbstractValidator<CategoryFormViewModel>
    {

        public CategoryFormValidator()
        {

            RuleFor(c => c.Name).MaximumLength(100).WithMessage(Errors.OherMaxLength)
            .Matches(RegexPatterns.CharactersOnly_Eng).WithMessage(Errors.OnlyEnglishLetters);

        }
    }
}