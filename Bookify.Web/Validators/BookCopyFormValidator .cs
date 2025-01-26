
namespace Bookify.Web.Validators
{
    public class BookCopyFormValidator : AbstractValidator<BookCopyFormViewModel>
    {

        public BookCopyFormValidator()
        {
            RuleFor(x => x.EditionNumber).InclusiveBetween(1, 1000).WithMessage(Errors.InvalidRange);


        }
    }
}
