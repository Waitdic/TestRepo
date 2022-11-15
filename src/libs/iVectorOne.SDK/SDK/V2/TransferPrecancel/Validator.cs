namespace iVectorOne.SDK.V2.TransferPrecancel
{
    using System;
    using System.Linq;
    using FluentValidation;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.SupplierBookingReference).NotEmpty().WithMessage(WarningMessages.InvalidSupplierBookingReference);
            RuleFor(x => x.BookingToken).NotEmpty().WithMessage(WarningMessages.InvalidBookingToken);
        }
    }
}