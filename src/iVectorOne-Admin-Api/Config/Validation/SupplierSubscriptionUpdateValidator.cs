using FluentValidation;
using iVectorOne_Admin_Api.Config.Requests;

namespace iVectorOne_Admin_Api.Config.Validation
{
    public class SupplierSubscriptionUpdateValidator : AbstractValidator<SupplierSubscriptionUpdateRequest>
    {
        public SupplierSubscriptionUpdateValidator()
        {
            RuleFor(x => x.SubscriptionId).GreaterThan(0).WithMessage(Warnings.ConfigWarnings.SubscriptionIDWarning);
            RuleFor(x => x.SupplierId).GreaterThan(0).WithMessage(Warnings.ConfigWarnings.SupplierIDWarning);
            RuleFor(x => x.TenantId).GreaterThan(0).WithMessage(Warnings.ConfigWarnings.TenantIDWarning);
        }
    }
}