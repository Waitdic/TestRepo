namespace iVectorOne.Factories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Repositories;
    using iVectorOne.Models.Tokens.Transfer;
    using iVectorOne.SDK.V2;
    using iVectorOne.Services;
    using iVectorOne.Utility;
    using Prebook = SDK.V2.TransferPrebook;
    using Book = SDK.V2.TransferBook;
    using Precancel = SDK.V2.TransferPrecancel;
    using Cancel = SDK.V2.TransferCancel;
    using Intuitive;
    using iVectorOne.Suppliers.Models.WelcomeBeds;
    using iVectorOne.SDK.V2.Book;

    /// <summary>
    /// Factory that builds up transfer details from api requests, used to pass into the third party code
    /// </summary>
    /// <seealso cref="ITransferDetailsFactory" />
    public class TransferDetailsFactory : ITransferDetailsFactory
    {
        /// <summary>
        /// The token service
        /// </summary>
        private readonly ITokenService _tokenService;

        private readonly ITPSupport _support;

        /// <summary>Initializes a new instance of the <see cref="TransferDetailsFactory" /> class.</summary>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens.</param>
        public TransferDetailsFactory(
            ITokenService tokenService,
            ITPSupport support)
        {
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        /// <inheritdoc />
        public async Task<TransferDetails> CreateAsync(Prebook.Request request)
        {
            var transferDetails = new TransferDetails();

            var transferToken = await _tokenService.DecodeTransferTokenAsync(request.BookingToken, request.Account);

            if (transferToken is not null)
            {
                transferDetails = new TransferDetails()
                {
                    AccountID = request.Account.AccountID,
                    DepartureDate = transferToken.DepartureDate,
                    DepartureTime = transferToken.DepartureTime,
                    OneWay = transferToken.OneWay,
                    Source = transferToken.Source,
                    SupplierID = transferToken.SupplierID,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(transferToken.ISOCurrencyID),
                    ThirdPartyConfigurations = request.Account.Configurations,
                    Adults = transferToken.Adults,
                    Children = transferToken.Children,
                    Infants = transferToken.Infants,
                    //LocalCost = 123M, 
                    SupplierReference = request.SupplierReference,
                    ThirdPartySettings = request.ThirdPartySettings
                };

                if (!transferDetails.OneWay)
                {
                    transferDetails.ReturnDate = transferDetails.DepartureDate.AddDays(transferToken.Duration);
                    transferDetails.ReturnTime = transferToken.ReturnTime;
                }
            }

            Validate(transferToken!, transferDetails);

            return transferDetails;
        }

        /// <summary>Validates wether the transfer details matches the expected values from the token</summary>
        /// <param name="transferToken">The transfer token.</param>
        /// <param name="transferDetails">The transfer details.</param>
        private void Validate(TransferToken transferToken, TransferDetails transferDetails)
        {
            if (transferToken == null
                || transferToken.DepartureDate == DateTime.MinValue)
            {
                transferDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidBookingToken);
            }
        }

        /// <summary>
        /// Loops through the passengers numbers on the room token and turns them into dummy passengers
        /// </summary>
        /// <param name="roomToken">The room token.</param>
        /// <returns>A passengers collection</returns>
        //private Passengers SetupPrebookPassengers(RoomToken roomToken)
        //{
        //    var passengers = new Passengers();

        //    for (int i = 0; i < roomToken.Adults; i++)
        //    {
        //        var passenger = new Passenger()
        //        {
        //            PassengerType = PassengerType.Adult,
        //            Age = 50
        //        };
        //        passengers.Add(passenger);
        //    }

        //    for (int i = 0; i < roomToken.Children; i++)
        //    {
        //        var passenger = new Passenger()
        //        {
        //            PassengerType = PassengerType.Child,
        //            Age = roomToken.ChildAges[i]
        //        };
        //        passengers.Add(passenger);
        //    }

        //    for (int i = 0; i < roomToken.Infants; i++)
        //    {
        //        var passenger = new Passenger()
        //        {
        //            PassengerType = PassengerType.Infant,
        //            Age = 1
        //        };
        //        passengers.Add(passenger);
        //    }

        //    return passengers;
        //}

        /// <inheritdoc />
        public async Task<TransferDetails> CreateAsync(Book.Request request)
        {
            var transferDetails = new TransferDetails();

            var leadCustomer = request.LeadCustomer;
            var transferToken = await _tokenService.DecodeTransferTokenAsync(request.BookingToken, request.Account);

            if (transferToken is not null)
            {

                transferDetails = new TransferDetails()
                {
                    AccountID = request.Account.AccountID,
                    DepartureDate = transferToken.DepartureDate,
                    DepartureTime = transferToken.DepartureTime,
                    OneWay = transferToken.OneWay,
                    Source = transferToken.Source,
                    SupplierID = transferToken.SupplierID,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(transferToken.ISOCurrencyID),
                    ThirdPartyConfigurations = request.Account.Configurations,
                    Adults = transferToken.Adults,
                    Children = transferToken.Children,
                    Infants = transferToken.Infants,
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

                if (!transferDetails.OneWay)
                {
                    transferDetails.ReturnDate = transferDetails.DepartureDate.AddDays(transferToken.Duration);
                    transferDetails.ReturnTime = transferToken.ReturnTime;
                }

                SetupGuests(request, transferToken, transferDetails);
                SetupJourneyDetails(request, transferDetails);
            }

            this.Validate(transferToken!, transferDetails);

            return transferDetails;
        }

        /// <inheritdoc />
        public async Task<TransferDetails> CreateAsync(Precancel.Request request)
        {
            var transferDetails = new TransferDetails();

            var transferToken = await _tokenService.PopulateTransferTokenAsync(request.SupplierBookingReference);

            if (transferToken is not null && !string.IsNullOrEmpty(transferToken.Source))
            {
                transferDetails = new TransferDetails()
                {
                    AccountID = request.Account.AccountID,
                    ConfirmationReference = request.SupplierBookingReference,
                    SupplierReference = request.SupplierReference,
                    Source = transferToken.Source,
                    SupplierID = transferToken.SupplierID,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(transferToken.ISOCurrencyID),
                    ThirdPartyConfigurations = request.Account.Configurations,
                    TransferBookingID = transferToken.TransferBookingID,
                    ThirdPartySettings = request.ThirdPartySettings
                };

                request.BookingID = transferToken.TransferBookingID;
            }
            else
            {
                transferDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidSupplierBookingReference);
            }

            return transferDetails;
        }

        /// <inheritdoc />
        public async Task<TransferDetails> CreateAsync(Cancel.Request request)
        {
            var transferDetails = new TransferDetails();

            var transferToken = await _tokenService.PopulateTransferTokenAsync(request.SupplierBookingReference);

            if (transferToken is not null && !string.IsNullOrEmpty(transferToken.Source))
            {
                transferDetails = new TransferDetails()
                {
                    AccountID = request.Account.AccountID,
                    ConfirmationReference = request.SupplierBookingReference,
                    SupplierReference = request.SupplierReference,
                    Source = transferToken.Source,
                    SupplierID = transferToken.SupplierID,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(transferToken.ISOCurrencyID),
                    ThirdPartyConfigurations = request.Account.Configurations,
                    TransferBookingID = transferToken.TransferBookingID,
                    ThirdPartySettings = request.ThirdPartySettings
                };

                request.BookingID = transferToken.TransferBookingID;
            }
            else
            {
                transferDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidSupplierBookingReference);
            }

            return transferDetails;
        }

        private void SetupGuests(Book.Request request, TransferToken transferToken, TransferDetails transferDetails)
        {
            foreach (var guestDetail in request.GuestDetails)
            {
                var passenger = new Passenger()
                {
                    Title = guestDetail.Title,
                    FirstName = guestDetail.FirstName,
                    LastName = guestDetail.LastName,
                    DateOfBirth = guestDetail.DateOfBirth,
                    Age = guestDetail.DateOfBirth.GetAgeAtTargetDate(transferToken.DepartureDate)
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
                transferDetails.Passengers.Add(passenger);
            }

            //foreach (var (child, childage) in room.Passengers.Where(x => x.PassengerType == PassengerType.Child).Zip(roomToken.ChildAges, (r1, r2) => (r1, r2)))
            //{
            //    if (child.Age == 0)
            //    {
            //        child.Age = childage;
            //    }
            //}

            if (transferDetails.Passengers.TotalAdults != transferToken.Adults ||
                transferDetails.Passengers.TotalChildren != transferToken.Children ||
                transferDetails.Passengers.TotalInfants != transferToken.Infants)
            {
                transferDetails.Warnings.AddNew("Validate failure", $"The guest details do not match what was searched for");
            }
        }

        private void SetupJourneyDetails(Book.Request request, TransferDetails transferDetails)
        {
            transferDetails.OutboundPickUpDetails = SetupJourneyDetails(request.OutboundPickUpDetails);
            transferDetails.OutboundDropoffDetails = SetupJourneyDetails(request.OutboundDropOffDetails);
            transferDetails.ReturnPickUpDetails = SetupJourneyDetails(request.ReturnPickUpDetails);
            transferDetails.ReturnDropOffDetails = SetupJourneyDetails(request.ReturnDropOffDetails);
        }

        private JourneyDetails SetupJourneyDetails(Book.JourneyDetails journeyDetails)
        {
            if (journeyDetails is null)
            {
                return new JourneyDetails();
            }

            return new JourneyDetails
            {
                FlightCode = journeyDetails.FlightCode,
                AccommodationName = journeyDetails.AccommodationName,
                TrainDetails = journeyDetails.TrainDetails,
                VesselName = journeyDetails.VesselName
            };
        }
    }
}