using FluentValidation;
using iVectorOne.SDK.V2;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest.Post
{
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.SearchRequest.ArrivalDate).NotNull().WithMessage(WarningMessages.ArrivalDateNotSpecified);
            RuleFor(x => x.SearchRequest.ArrivalDate).GreaterThan(DateTime.Now.Date).WithMessage(WarningMessages.ArrivalDateInThePast);
            RuleFor(x => x.SearchRequest.ArrivalDate).LessThan(DateTime.Now.AddYears(3).Date).WithMessage(WarningMessages.ArrivalDateToFarInTheFuture);
            RuleFor(x => x.SearchRequest.Duration).GreaterThanOrEqualTo(0).WithMessage(WarningMessages.DurationNotSpecified);
            RuleFor(x => x.SearchRequest.Duration).LessThan(63).WithMessage(WarningMessages.DurationInvalid);
            RuleFor(x => x.SearchRequest.Properties).NotNull().NotEmpty().WithMessage(WarningMessages.PropertyNotSpecified);
            RuleFor(x => x.SearchRequest.Properties.Count).LessThanOrEqualTo(500).WithMessage(WarningMessages.PropertiesOverLimit);
            RuleFor(x => x.SearchRequest.RoomRequests).NotEmpty().WithMessage(WarningMessages.RoomsNotSpecified);
            //RuleFor(x => x.SearchRequest.DedupeMethod).NotEmpty().WithMessage(WarningMessages.InvalidDedupeMethod);
            RuleForEach(x => x.SearchRequest.RoomRequests)
                .Must(x => x.Adults > 0).WithMessage(WarningMessages.AdultsNotSpecifiedInAllRooms)
                .Must(x => x.Adults <= 15).WithMessage(WarningMessages.Only15AdultsAllowed)
                .Must(x => x.Children <= 8).WithMessage(WarningMessages.Only8ChildrenAllowed)
                .Must(x => x.Infants <= 7).WithMessage(WarningMessages.Only7InfantsAllowed)
                .Must(x => x.Children == x.ChildAges.Count).WithMessage(WarningMessages.ChildAgesDontMatchChildren)
                .Must(x => x.ChildAges.All(a => a >= 2 && a < 18)).WithMessage(WarningMessages.InvalidChildAge);
        }
    }
}