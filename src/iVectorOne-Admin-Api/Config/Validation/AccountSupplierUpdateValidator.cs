﻿namespace iVectorOne_Admin_Api.Config.Validation
{
    using FluentValidation;
    using iVectorOne_Admin_Api.Config.Requests;

    public class AccountSupplierUpdateValidator : AbstractValidator<AccountSupplierUpdateRequest>
    {
        public AccountSupplierUpdateValidator()
        {
            RuleFor(x => x.AccountId).GreaterThan(0).WithMessage(Warnings.ConfigWarnings.AccountIDWarning);
            RuleFor(x => x.SupplierId).GreaterThan(0).WithMessage(Warnings.ConfigWarnings.SupplierIDWarning);
            RuleFor(x => x.TenantId).GreaterThan(0).WithMessage(Warnings.ConfigWarnings.TenantIDWarning);
        }
    }
}