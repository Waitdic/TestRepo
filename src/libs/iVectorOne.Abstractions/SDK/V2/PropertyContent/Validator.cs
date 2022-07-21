namespace ThirdParty.SDK.V2.PropertyContent
{
    using FluentValidation;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.PropertyIDs).NotEmpty().WithMessage("At least one Property ID must be provided");
        }
    }
}