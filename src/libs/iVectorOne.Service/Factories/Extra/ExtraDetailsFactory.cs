
namespace iVectorOne.Factories
{
    using Intuitive;
    using iVectorOne.Lookups;
    using iVectorOne.Models.Extra;
    using iVectorOne.Models.Tokens.Extra;
    using iVectorOne.SDK.V2;
    using iVectorOne.Services;
    using System;
    using System.Threading.Tasks;
    using Prebook = SDK.V2.ExtraPrebook;
    using Book = SDK.V2.ExtraBook;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Models;
    using iVectorOne.SDK.V2.Book;

    public class ExtraDetailsFactory : IExtraDetailsFactory
    {
        /// <summary>
        /// The token service
        /// </summary>
        private readonly ITokenService _tokenService;

        private readonly ITPSupport _support;

        /// <summary>Initializes a new instance of the <see cref="TransferDetailsFactory" /> class.</summary>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens.</param>
        public ExtraDetailsFactory(
            ITokenService tokenService,
            ITPSupport support)
        {
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
            _support = Ensure.IsNotNull(support, nameof(support));
        }
        public async Task<ExtraDetails> CreateAsync(Book.Request request)
        {
            var extraDetails = new ExtraDetails();

            var leadCustomer = request.LeadCustomer;
            var extraToken = _tokenService.DecodeExtraToken(request.BookingToken);

            if (extraToken is not null)
            {
                string source = await _support.SupplierNameLookupAsync(extraToken.SupplierID);

                extraDetails = new ExtraDetails()
                {
                    AccountID = request.Account.AccountID,
                    DepartureDate = extraToken.DepartureDate,
                    DepartureTime = extraToken.DepartureTime,
                    OneWay = extraToken.OneWay,
                    Source = source,
                    SupplierID = extraToken.SupplierID,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(extraToken.ISOCurrencyID),
                    ThirdPartyConfigurations = request.Account.Configurations,
                    Adults = extraToken.Adults,
                    Children = extraToken.Children,
                    Infants = extraToken.Infants,
                    SupplierReference = request.SupplierReference,
                    BookingReference = request.BookingReference,

                    LeadGuestTitle = leadCustomer.CustomerTitle,
                    LeadGuestFirstName = leadCustomer.CustomerFirstName,
                    LeadGuestLastName = leadCustomer.CustomerLastName,
                    LeadGuestDateOfBirth = leadCustomer.DateOfBirth,
                    LeadGuestAddress1 = leadCustomer.CustomerAddress1,
                    LeadGuestAddress2 = leadCustomer.CustomerAddress2,
                    LeadGuestTownCity = leadCustomer.CustomerTownCity,
                    LeadGuestCounty = leadCustomer.CustomerCounty,
                    LeadGuestPostcode = leadCustomer.CustomerPostcode,
                    LeadGuestCountryCode = request.LeadCustomer.CustomerBookingCountryCode,
                    LeadGuestPhone = leadCustomer.CustomerPhone,
                    LeadGuestMobile = leadCustomer.CustomerMobile,
                    LeadGuestEmail = leadCustomer.CustomerEmail,
                    LeadGuestPassportNumber = leadCustomer.PassportNumber,
                    ThirdPartySettings = request.ThirdPartySettings
                };

                if (!extraDetails.OneWay)
                {
                    extraDetails.ReturnDate = extraDetails.DepartureDate.AddDays(extraToken.Duration);
                    extraDetails.ReturnTime = extraToken.ReturnTime;
                }

                SetupGuests(request, extraToken, extraDetails);
                SetupJourneyDetails(request, extraDetails);
            }

            this.Validate(extraToken!, extraDetails);

            return extraDetails;
        }

        public async Task<ExtraDetails> CreateAsync(Prebook.Request request)
        {
            var extraDetails = new ExtraDetails();

            var extraToken = _tokenService.DecodeExtraToken(request.BookingToken);

            if (extraToken is not null)
            {
                string source = await _support.SupplierNameLookupAsync(extraToken.SupplierID);

                extraDetails = new ExtraDetails()
                {
                    AccountID = request.Account.AccountID,
                    DepartureDate = extraToken.DepartureDate,
                    DepartureTime = extraToken.DepartureTime,
                    OneWay = extraToken.OneWay,
                    Source = source,
                    SupplierID = extraToken.SupplierID,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(extraToken.ISOCurrencyID),
                    ThirdPartyConfigurations = request.Account.Configurations,
                    Adults = extraToken.Adults,
                    Children = extraToken.Children,
                    Infants = extraToken.Infants,
                    //LocalCost = 123M, 
                    SupplierReference = request.SupplierReference,
                    ThirdPartySettings = request.ThirdPartySettings
                };

                if (!extraDetails.OneWay)
                {
                    extraDetails.ReturnDate = extraDetails.DepartureDate.AddDays(extraToken.Duration);
                    extraDetails.ReturnTime = extraToken.ReturnTime;
                }
            }

            Validate(extraToken!, extraDetails);

            return extraDetails;
        }

        #region Private functions

        /// <summary>Validates wether the extra details matches the expected values from the token</summary>
        /// <param name="extraToken">The extra token.</param>
        /// <param name="transferDetails">The extra details.</param>
        private void Validate(ExtraToken extraToken, ExtraDetails extraDetails)
        {
            if (extraToken == null
                || extraToken.DepartureDate == DateTime.MinValue)
            {
                extraDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidBookingToken);
            }
        }

        private void SetupGuests(Book.Request request, ExtraToken extraToken, ExtraDetails extraDetails)
        {
            foreach (var guestDetail in request.GuestDetails)
            {
                var passenger = new Passenger()
                {
                    Title = guestDetail.Title,
                    FirstName = guestDetail.FirstName,
                    LastName = guestDetail.LastName,
                    DateOfBirth = guestDetail.DateOfBirth,
                    Age = guestDetail.DateOfBirth.GetAgeAtTargetDate(extraToken.DepartureDate)
                };

                switch (guestDetail.Type)
                {
                    case GuestType.Unset:
                    case GuestType.Adult:
                        passenger.PassengerType = PassengerType.Adult;
                        break;
                    case GuestType.Child:
                        passenger.PassengerType = PassengerType.Child;
                        break;
                    case GuestType.Infant:
                        passenger.PassengerType = PassengerType.Infant;
                        break;
                }
                extraDetails.Passengers.Add(passenger);
            }

            if (extraDetails.Passengers.TotalAdults != extraToken.Adults ||
                extraDetails.Passengers.TotalChildren != extraToken.Children ||
                extraDetails.Passengers.TotalInfants != extraToken.Infants)
            {
                extraDetails.Warnings.AddNew("Validate failure", $"The guest details do not match what was searched for");
            }
        }

        private void SetupJourneyDetails(Book.Request request, ExtraDetails extraDetails)
        {
            var outboundJourney = new Book.JourneyDetails();
            var returnJourney = new Book.JourneyDetails();

            if (request.OutboundDetails != null)
            {
                outboundJourney.FlightCode = request.OutboundDetails.FlightCode;
            }

            if (request.ReturnDetails != null)
            {
                returnJourney.FlightCode = request.ReturnDetails.FlightCode;
            }

            extraDetails.OutboundDetails = outboundJourney;
            extraDetails.ReturnDetails = returnJourney;
        }
        #endregion
    }
}
