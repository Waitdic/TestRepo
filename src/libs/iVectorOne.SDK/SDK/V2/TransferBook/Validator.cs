namespace iVectorOne.SDK.V2.TransferBook
{
    using FluentValidation;
    using iVectorOne.SDK.V2.Book;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.BookingToken).NotEmpty().WithMessage(WarningMessages.InvalidBookingToken);
            RuleFor(x => x.BookingReference).NotEmpty().WithMessage(WarningMessages.InvalidBookingReference);
            RuleFor(x => x.SupplierReference).NotEmpty().WithMessage(WarningMessages.InvalidSupplierBookingReference);
            RuleFor(x => x.LeadCustomer).NotEmpty().WithMessage(WarningMessages.InvalidLeadCustomer);

            //TODO centralise this with property
            RuleFor(x => x.LeadCustomer.CustomerTitle).NotEmpty().WithMessage(WarningMessages.InvalidLeadGuestTitle);
            RuleFor(x => x.LeadCustomer.CustomerFirstName).NotEmpty().WithMessage(WarningMessages.InvalidLeadGuestFirstName);
            RuleFor(x => x.LeadCustomer.CustomerLastName).NotEmpty().WithMessage(WarningMessages.InvalidLeadGuestLastName);
            RuleFor(x => x.LeadCustomer.DateOfBirth).NotEmpty().WithMessage(WarningMessages.InvalidLeadGuestDateOfBirth);
            RuleFor(x => x.LeadCustomer.CustomerAddress1).NotEmpty().WithMessage(WarningMessages.InvalidCustomerAddress1);
            RuleFor(x => x.LeadCustomer.CustomerTownCity).NotEmpty().WithMessage(WarningMessages.InvalidCustomerTownCity);
            RuleFor(x => x.LeadCustomer.CustomerCounty).NotEmpty().WithMessage(WarningMessages.InvalidCustomerCounty);
            RuleFor(x => x.LeadCustomer.CustomerPostcode).NotEmpty().WithMessage(WarningMessages.InvalidCustomerPostcode);
            RuleFor(x => x.LeadCustomer.CustomerBookingCountryCode).NotEmpty().WithMessage(WarningMessages.InvalidCustomerBookingCountryCode);
            RuleFor(x => x.LeadCustomer.CustomerPhone).NotEmpty().WithMessage(WarningMessages.InvalidCustomerPhone);
            RuleFor(x => x.LeadCustomer.CustomerPhone).Matches(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$").WithMessage(WarningMessages.InvalidCustomerPhone);
            RuleFor(x => x.LeadCustomer.CustomerMobile).NotEmpty().WithMessage(WarningMessages.InvalidCustomerMobile);
            RuleFor(x => x.LeadCustomer.CustomerMobile).Matches(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$").WithMessage(WarningMessages.InvalidCustomerMobile);
            RuleFor(x => x.LeadCustomer.CustomerEmail).NotEmpty().WithMessage(WarningMessages.InvalidCustomerEmail);
            RuleFor(x => x.LeadCustomer.CustomerEmail).Length(0, 320).WithMessage(WarningMessages.InvalidCustomerEmail);
            RuleFor(x => x.LeadCustomer.CustomerEmail).Matches(@"^[a-zA-Z0-9]+(?!.*([.! # $ % & ' * + - \/ = ? ^ _ ` { |])\1)([\w! # $ % & ' * + .\- \/ = ? ^ _ ` { |])+[a-zA-Z0-9]+@([a-zA-Z0-9]{2,253})([ a-zA-Z0-9 \- . ])+\.[a-zA-Z0-9]{2,63}$").WithMessage(WarningMessages.InvalidCustomerEmail);

            RuleForEach(x => x.GuestDetails)
                .Must(x => x.Type != GuestType.Unset).WithMessage(WarningMessages.InvalidPaxType)
                .Must(x => !string.IsNullOrWhiteSpace(x.Title)).WithMessage(WarningMessages.InvalidGuestTitle)
                .Must(x => !string.IsNullOrWhiteSpace(x.FirstName)).WithMessage(WarningMessages.InvalidGuestFirstName)
                .Must(x => !string.IsNullOrWhiteSpace(x.LastName)).WithMessage(WarningMessages.InvalidGuestLastName)
                .Must(x => x.DateOfBirth >= System.DateTime.MinValue).WithMessage(WarningMessages.InvalidDateOfBirth)
                .OverridePropertyName("GuestDetails");
        }
    }
}