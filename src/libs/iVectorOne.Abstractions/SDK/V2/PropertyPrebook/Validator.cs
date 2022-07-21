namespace iVectorOne.SDK.V2.PropertyPrebook
{
    using System;
    using System.Linq;
    using FluentValidation;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.BookingToken).NotEmpty().WithMessage(WarningMessages.InvalidBookingToken);
            RuleFor(x => x.RoomBookings).NotEmpty().WithMessage(WarningMessages.InvalidRoomBookings);
            RuleForEach(x => x.RoomBookings)
                .Must(x => !string.IsNullOrWhiteSpace(x.RoomBookingToken)).WithMessage(WarningMessages.InvalidRoomBookingToken)
                .Must(x => !string.IsNullOrWhiteSpace(x.SupplierReference1)).WithMessage(WarningMessages.InvalidRoomSupplierBooking1Reference);
        }
    }
}