namespace iVectorOne.SDK.V2.ExtraPrebook
{
    using System;
    using System.Linq;
    using FluentValidation;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.BookingToken).NotEmpty().WithMessage(WarningMessages.InvalidBookingToken);
            RuleFor(x => x.SupplierReference).NotEmpty().WithMessage(WarningMessages.InvalidSupplierBookingReference);
        }
    }
}