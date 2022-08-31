namespace iVectorOne.SDK.V2.PropertyContent
{
    using FluentValidation;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.PropertyIDs).NotEmpty().WithMessage(WarningMessages.InvalidPropertyID);
            RuleFor(x => x.PropertyIDs.Count).LessThanOrEqualTo(500).WithMessage(WarningMessages.TooManyPropertyIDsSpecified);
        }
    }
}