﻿namespace iVectorOne.SDK.V2.TransferSearch
{
    using System;
    using System.Linq;
    using FluentValidation;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.DepartureDate).NotNull().WithMessage(WarningMessages.DepartureDateNotSpecified);
            RuleFor(x => x.DepartureDate).GreaterThan(DateTime.Now.Date).WithMessage(WarningMessages.DepartureDateInThePast);
            RuleFor(x => x.DepartureDate).LessThan(DateTime.Now.AddYears(3).Date).WithMessage(WarningMessages.DepartureDateToFarInTheFuture);
            RuleFor(x => x.DepartureLocationID).GreaterThanOrEqualTo(1).WithMessage(WarningMessages.InvalidDepartureLocationID);
            RuleFor(x => x.ArrivalLocationID).GreaterThanOrEqualTo(1).WithMessage(WarningMessages.InvalidArrivalLocationID);
            RuleFor(x => x.Supplier).NotNull().NotEmpty().WithMessage(WarningMessages.InvalidSupplier);
            RuleFor(x => x.Adults + x.Children).GreaterThanOrEqualTo(1).WithMessage(WarningMessages.NoAdultsOrChildrenSpecified);
        }
    }
}