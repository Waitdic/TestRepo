using FluentValidation;
using iVectorOne.SDK.V2;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.BookingViewer
{
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Query).NotEmpty();
        }
    }
}
