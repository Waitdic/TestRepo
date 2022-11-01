using FluentValidation;
using iVectorOne.SDK.V2;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewer
{
    public class Validator : AbstractValidator<Request>
    {
        public enum EnvironmentEnum { All, Live, Test };

        public enum TypeEnum { All, Book, PreBook };

        public enum StatusEnum { All, Successful, Unsuccessful };

        public Validator()
        {
            RuleFor(x => x.StartDate).NotEmpty().WithMessage("Start Date is required.");
            RuleFor(x => x.EndDate).NotEmpty().WithMessage("End Date is required.");
            RuleFor(x => x).Must(x => x.EndDate > x.StartDate)
                    .WithMessage("EndTime must greater than StartTime");
            RuleFor(x => x.Environment).IsEnumName(typeof(EnvironmentEnum), caseSensitive: false);
            RuleFor(x => x.Type).IsEnumName(typeof(TypeEnum), caseSensitive: false);
            RuleFor(x => x.Status).IsEnumName(typeof(StatusEnum), caseSensitive: false);
        }
    }
}
