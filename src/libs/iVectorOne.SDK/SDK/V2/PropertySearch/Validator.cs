namespace iVectorOne.SDK.V2.PropertySearch
{
    using System;
    using System.Linq;
    using FluentValidation;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ArrivalDate).NotNull().WithMessage(WarningMessages.ArrivalDateNotSpecified);
            RuleFor(x => x.ArrivalDate).GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage(WarningMessages.ArrivalDateInThePast);
            RuleFor(x => x.ArrivalDate).LessThan(DateTime.Now.AddYears(3).Date).WithMessage(WarningMessages.ArrivalDateToFarInTheFuture);
            RuleFor(x => x.Duration).GreaterThanOrEqualTo(0).WithMessage(WarningMessages.DurationNotSpecified);
            RuleFor(x => x.Duration).LessThan(63).WithMessage(WarningMessages.DurationInvalid);
            RuleFor(x => x.Properties).NotNull().NotEmpty().WithMessage(WarningMessages.PropertyNotSpecified);
            RuleFor(x => x.Properties.Count).LessThanOrEqualTo(500).WithMessage(WarningMessages.PropertiesOverLimit);
            RuleFor(x => x.RoomRequests).NotEmpty().WithMessage(WarningMessages.RoomsNotSpecified);
            RuleFor(x => x.DedupeMethod).NotEmpty().WithMessage(WarningMessages.InvalidDedupeMethod);
            RuleForEach(x => x.RoomRequests)
                .Must(x => x.Adults > 0).WithMessage(WarningMessages.AdultsNotSpecifiedInAllRooms)
                .Must(x => x.Adults <= 15).WithMessage(WarningMessages.Only15AdultsAllowed)
                .Must(x => x.Children <= 8).WithMessage(WarningMessages.Only8ChildrenAllowed)
                .Must(x => x.Infants <= 7).WithMessage(WarningMessages.Only7InfantsAllowed)
                .Must(x => x.Children == x.ChildAges.Count).WithMessage(WarningMessages.ChildAgesDontMatchChildren)
                .Must(x => x.ChildAges.All(a => a >= 2 && a < 18)).WithMessage(WarningMessages.InvalidChildAge);
        }
    }
}