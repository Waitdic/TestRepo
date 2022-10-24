using FluentValidation;

namespace iVectorOne_Admin_Api.Features.V1.Properties.Search
{
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Query).NotEmpty();
            RuleFor(x => x.Query).MinimumLength(4);
        }
    }
}
