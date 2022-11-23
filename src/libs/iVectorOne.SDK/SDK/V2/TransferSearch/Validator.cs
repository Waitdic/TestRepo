namespace iVectorOne.SDK.V2.TransferSearch
{
    using System;
    using System.Linq;
    using FluentValidation;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.DepartureLocationID).GreaterThanOrEqualTo(1).WithMessage(WarningMessages.InvalidDepartureLocationID);
            RuleFor(x => x.ArrivalLocationID).GreaterThanOrEqualTo(1).WithMessage(WarningMessages.InvalidArrivalLocationID);
            RuleFor(x => x.DepartureDate).NotEmpty().WithMessage(WarningMessages.DepartureDateNotSpecified);
            RuleFor(x => x.DepartureDate).GreaterThan(DateTime.Now.Date).WithMessage(WarningMessages.DepartureDateInThePast);
            RuleFor(x => x.DepartureDate).LessThan(DateTime.Now.AddYears(3).Date).WithMessage(WarningMessages.DepartureDateToFarInTheFuture);
            RuleFor(x => x.ReturnDate).NotEmpty().When(x => x.OneWay == false).WithMessage(WarningMessages.ReturnDateNotSpecified);
            RuleFor(x => x.Supplier).NotNull().NotEmpty().WithMessage(WarningMessages.InvalidSupplier);
            RuleFor(x => x.Adults + x.Children).GreaterThanOrEqualTo(1).WithMessage(WarningMessages.NoAdultsOrChildrenSpecified);
            RuleFor(x => x.Adults).LessThanOrEqualTo(15).WithMessage(WarningMessages.Only15AdultsAllowedTransfer);
            RuleFor(x => x.Children).LessThanOrEqualTo(8).WithMessage(WarningMessages.Only8ChildrenAllowedTransfer);
            RuleFor(x => x.Infants).LessThanOrEqualTo(7).WithMessage(WarningMessages.Only7InfantsAllowedTransfer);
            RuleFor(x => x.ReturnTime).NotNull().NotEmpty().When(x => x.OneWay == false).WithMessage(WarningMessages.ReturnTimeNotSpecified);
            RuleFor(x => x.ReturnTime).Matches("^(?:[01]\\d|2[0-3]):[0-5]\\d$").When(x => x.OneWay == false).WithMessage(WarningMessages.ReturnTimeInvalid);
            RuleFor(x => x.DepartureTime).NotEmpty().NotNull().WithMessage(WarningMessages.DepartureTimeRequired);
            RuleFor(x => x.DepartureTime).Matches("^(?:[01]\\d|2[0-3]):[0-5]\\d$").WithMessage(WarningMessages.DepartureTimeInvalid);

        }
    }
}