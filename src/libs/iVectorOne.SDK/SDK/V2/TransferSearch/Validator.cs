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
            RuleFor(x => x.DepartureDate).NotNull().WithMessage(WarningMessages.DepartureDateNotSpecified);
            RuleFor(x => x.DepartureDate).GreaterThan(DateTime.Now.Date).WithMessage(WarningMessages.DepartureDateInThePast);
            RuleFor(x => x.DepartureDate).LessThan(DateTime.Now.AddYears(3).Date).WithMessage(WarningMessages.DepartureDateToFarInTheFuture);
            RuleFor(x => x.ReturnDate).NotNull().When(x => x.OneWay == false).WithMessage(WarningMessages.ReturnDateSpecified);
            RuleFor(x => x.ReturnDate).Null().When(x => x.OneWay == true).WithMessage(WarningMessages.ReturnDateNotSpecified);
            RuleFor(x => x.Supplier).NotNull().NotEmpty().WithMessage(WarningMessages.InvalidSupplier);
            RuleFor(x => x.Adults + x.Children).GreaterThanOrEqualTo(1).WithMessage(WarningMessages.NoAdultsOrChildrenSpecified);
            RuleFor(x => x.Adults).LessThanOrEqualTo(15).WithMessage(WarningMessages.Only15AdultsAllowedTransfer);
            RuleFor(x => x.Children).LessThanOrEqualTo(8).WithMessage(WarningMessages.Only8ChildrenAllowedTransfer);
            RuleFor(x => x.Infants).LessThanOrEqualTo(7).WithMessage(WarningMessages.Only7InfantsAllowedTransfer);
            RuleFor(x => x.ReturnTime).NotEmpty().When(x => x.OneWay == false).WithMessage(WarningMessages.ReturnTimeSpecified);
            RuleFor(x => x.ReturnTime).Empty().When(x => x.OneWay == true).WithMessage(WarningMessages.ReturnTimeNotSpecified);
            RuleFor(x => x.ReturnTime).Matches("^(?:[01]\\d|2[0-3]):[0-5]\\d$").When(x => x.OneWay == false).WithMessage(WarningMessages.ReturnTimeInvalid);
            RuleFor(x => x.DepartureTime).NotEmpty().WithMessage(WarningMessages.DepartureTimeRequired);
            RuleFor(x => x.DepartureTime).Matches("^(?:[01]\\d|2[0-3]):[0-5]\\d$").WithMessage(WarningMessages.DepartureTimeInvalid);
            RuleFor(x => ((x.ReturnDate.Value.Subtract(x.DepartureDate.Value)).Days)).LessThan(63).When(x=>x.ReturnDate!=null && x.DepartureDate!=null).WithMessage(WarningMessages.InvalidDuration);
            RuleFor(x=>x.DepartureDate).LessThanOrEqualTo(x=>x.ReturnDate).WithMessage(WarningMessages.ReturnDateNotBeforeDepartureDate);

        }
    }
}