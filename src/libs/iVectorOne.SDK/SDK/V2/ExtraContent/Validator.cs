namespace iVectorOne.SDK.V2.ExtraContent
{
    using FluentValidation;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
          RuleFor(x => x.Supplier).NotEmpty().WithMessage(WarningMessages.InvalidSupplier);
        }
    }
}