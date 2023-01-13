namespace iVectorOne.Factories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Repositories;
    using iVectorOne.Models.Tokens;
    using iVectorOne.SDK.V2;
    using iVectorOne.Services;
    using Prebook = SDK.V2.PropertyPrebook;
    using Book = SDK.V2.PropertyBook;
    using Precancel = SDK.V2.PropertyPrecancel;
    using Cancel = SDK.V2.PropertyCancel;
    using Intuitive;
    using iVectorOne.SDK.V2.Book;

    /// <summary>
    /// Factory that builds up property details from api requests, used to pass into the third party code
    /// </summary>
    /// <seealso cref="IPropertyDetailsFactory" />
    public class PropertyDetailsFactory : IPropertyDetailsFactory
    {
        /// <summary>The token service</summary>
        private readonly ITokenService _tokenService;

        /// <summary>Repository for retrieving third party meal basis</summary>
        private readonly IMealBasisLookupRepository _mealbasisRepository;

        private readonly ITPSupport _support;

        private readonly IPropertyContentRepository _contentRepository;

        /// <summary>Initializes a new instance of the <see cref="PropertyDetailsFactory" /> class.</summary>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens.</param>
        /// <param name="currencyRepository">Repository for looking up Currency information.</param>
        ///  /// <param name="mealBasisLookup">Repository for looking up mealbasis information.</param>
        public PropertyDetailsFactory(
            ITokenService tokenService,
            IMealBasisLookupRepository mealBasisLookup,
            ITPSupport support,
            IPropertyContentRepository contentRepository)
        {
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
            _mealbasisRepository = Ensure.IsNotNull(mealBasisLookup, nameof(mealBasisLookup));
            _support = Ensure.IsNotNull(support, nameof(support));
            _contentRepository = Ensure.IsNotNull(contentRepository, nameof(contentRepository));
        }

        /// <inheritdoc />
        public async Task<PropertyDetails> CreateAsync(Prebook.Request request)
        {
            var propertyDetails = new PropertyDetails();

            var propertyToken = _tokenService.DecodePropertyToken(request.BookingToken);
            var firstRoomToken = _tokenService.DecodeRoomToken(request.RoomBookings[0].RoomBookingToken);

            if (propertyToken is not null)
            {
                int propertyId = firstRoomToken?.PropertyID > 0 ? firstRoomToken.PropertyID : propertyToken.PropertyID;
                var propertyContent = await _contentRepository.GetContentforPropertyAsync(propertyId, request.Account, string.Empty);

                propertyDetails = new PropertyDetails()
                {
                    AccountID = request.Account.AccountID,
                    ArrivalDate = propertyToken.ArrivalDate,
                    DepartureDate = propertyToken.ArrivalDate.AddDays(propertyToken.Duration),
                    CentralPropertyID = propertyContent.CentralPropertyID,
                    Source = propertyContent.Source,
                    SupplierID = propertyContent.SupplierID,
                    PropertyID = propertyId,
                    TPKey = propertyContent.TPKey,
                    ISONationalityCode = request.NationalityID,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(propertyToken.ISOCurrencyID),
                    OpaqueRates = request.OpaqueRates,
                    SellingCountry = request.SellingCountry,
                    ThirdPartyConfigurations = request.Account.Configurations,
                    ResortCode = propertyContent.GeographyCode,
                };

                foreach (var room in request.RoomBookings)
                {
                    var roomToken = _tokenService.DecodeRoomToken(room.RoomBookingToken);

                    if (roomToken is not null && roomToken.Adults != 0)
                    {
                        var roomContent = await _contentRepository.GetContentforPropertyAsync(roomToken.PropertyID, request.Account, string.Empty);

                        if (roomContent.Source == propertyContent.Source)
                        {
                            var passengers = SetupPrebookPassengers(roomToken);
                            var mealbasisId = roomToken.MealBasisID;
                            var localCost = roomToken.LocalCost;

                            var roomDetails = new RoomDetails()
                            {
                                ThirdPartyReference = room.SupplierReference1,
                                RoomTypeCode = room.SupplierReference2,
                                Passengers = passengers,
                                LocalCost = localCost,
                                PropertyRoomBookingID = roomToken.PropertyRoomBookingID,
                                MealBasisCode = await _mealbasisRepository.GetMealBasisCodefromTPMealbasisIDAsync(
                                    propertyContent.Source,
                                    mealbasisId,
                                    propertyDetails.AccountID)
                            };
                            propertyDetails.LocalCost += localCost;
                            propertyDetails.Rooms.Add(roomDetails);
                        }
                        else
                        {
                            propertyDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidRoomCombination);
                        }
                    }
                    else
                    {
                        propertyDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidRoomBookingToken);
                    }
                }
            }

            if (!propertyDetails.Warnings.Any())
            {
                Validate(propertyToken!, propertyDetails);
            }

            return propertyDetails;
        }

        /// <summary>Validates wether the property details matches the expected values from teh token</summary>
        /// <param name="propertyToken">The property token.</param>
        /// <param name="propertyDetails">The property details.</param>
        private void Validate(PropertyToken propertyToken, PropertyDetails propertyDetails)
        {
            if (propertyToken == null
                || propertyToken.PropertyID == 0
                || propertyToken.Rooms == 0
                || propertyToken.Duration == 0
                || propertyToken.ArrivalDate == DateTime.MinValue)
            {
                propertyDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidBookingToken);
            }
            else if (propertyDetails.Rooms.Count() != propertyToken.Rooms)
            {
                propertyDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidRooms);
            }
        }

        /// <summary>
        /// Loops through the passengers numbers on the room token and turns them into dummy passengers
        /// </summary>
        /// <param name="roomToken">The room token.</param>
        /// <returns>A passengers collection</returns>
        private Passengers SetupPrebookPassengers(RoomToken roomToken)
        {
            var passengers = new Passengers();

            for (int i = 0; i < roomToken.Adults; i++)
            {
                var passenger = new Passenger()
                {
                    PassengerType = PassengerType.Adult,
                    Age = 50
                };
                passengers.Add(passenger);
            }

            for (int i = 0; i < roomToken.Children; i++)
            {
                var passenger = new Passenger()
                {
                    PassengerType = PassengerType.Child,
                    Age = roomToken.ChildAges[i]
                };
                passengers.Add(passenger);
            }

            for (int i = 0; i < roomToken.Infants; i++)
            {
                var passenger = new Passenger()
                {
                    PassengerType = PassengerType.Infant,
                    Age = 1
                };
                passengers.Add(passenger);
            }

            return passengers;
        }

        /// <inheritdoc />
        public async Task<PropertyDetails> CreateAsync(Book.Request request)
        {
            var propertyDetails = new PropertyDetails();

            var leadCustomer = request.LeadCustomer;
            var propertyToken = _tokenService.DecodePropertyToken(request.BookingToken);

            if (propertyToken is not null)
            {
                var propertyContent = await _contentRepository.GetContentforPropertyAsync(propertyToken.PropertyID, request.Account, string.Empty);
                propertyDetails = new PropertyDetails()
                {
                    AccountID = request.Account.AccountID,
                    SupplierID = propertyContent.SupplierID,
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
                    TPRef1 = request.SupplierReference1,
                    TPRef2 = request.SupplierReference2,
                    Source = propertyContent.Source,
                    BookingReference = request.BookingReference,
                    ComponentNumber = request.ComponentNumber,
                    PropertyID = propertyToken.PropertyID,
                    PropertyName = propertyContent.PropertyName,
                    TPKey = propertyContent.TPKey,
                    ArrivalDate = propertyToken.ArrivalDate,
                    DepartureDate = propertyToken.ArrivalDate.AddDays(propertyToken.Duration),
                    ISONationalityCode = request.NationalityID,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(propertyToken.ISOCurrencyID),
                    OpaqueRates = request.OpaqueRates,
                    SellingCountry = request.SellingCountry,
                    ThirdPartyConfigurations = request.Account.Configurations,
                };

                int roomNumber = 0;
                foreach (var roomBooking in request.RoomBookings)
                {
                    roomNumber += 1;
                    await BuildBookRoomAsync(propertyDetails, roomNumber, roomBooking);
                }
            }

            this.Validate(propertyToken!, propertyDetails);

            return propertyDetails;
        }

        /// <summary>Create a Room on the property details using the room request</summary>
        /// <param name="propertyDetails">The property details.</param>
        /// <param name="roomNumber">The room number.</param>
        /// <param name="roomBooking">The room booking.</param>
        private async Task BuildBookRoomAsync(PropertyDetails propertyDetails, int roomNumber, Book.RoomBooking roomBooking)
        {
            var roomToken = _tokenService.DecodeRoomToken(roomBooking.RoomBookingToken);

            if (roomToken is not null && roomToken.Adults != 0)
            {
                var mealbasisId = roomToken.MealBasisID;
                decimal localCost = roomToken.LocalCost;

                var room = new RoomDetails()
                {
                    ThirdPartyReference = roomBooking.SupplierReference,
                    LocalCost = localCost,
                    GrossCost = localCost,
                    PropertyRoomBookingID = roomToken.PropertyRoomBookingID,
                    SpecialRequest = roomBooking.SpecialRequest,
                    MealBasisCode = await _mealbasisRepository.GetMealBasisCodefromTPMealbasisIDAsync(
                        propertyDetails.Source,
                        mealbasisId,
                        propertyDetails.AccountID)
                };

                foreach (var guestDetail in roomBooking.GuestDetails)
                {
                    var passenger = new Passenger()
                    {
                        Title = guestDetail.Title,
                        FirstName = guestDetail.FirstName,
                        LastName = guestDetail.LastName,
                        DateOfBirth = guestDetail.DateOfBirth,
                        Age = guestDetail.DateOfBirth.GetAgeAtTargetDate(propertyDetails.ArrivalDate)
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

                    room.Passengers.Add(passenger);
                }

                foreach (var (child, childage) in room.Passengers.Where(x => x.PassengerType == PassengerType.Child).Zip(roomToken.ChildAges, (r1, r2) => (r1, r2)))
                {
                    if (child.Age == 0)
                    {
                        child.Age = childage;
                    }
                }

                if (room.Passengers.TotalAdults != roomToken.Adults ||
                    room.Passengers.TotalChildren != roomToken.Children ||
                    room.Passengers.TotalInfants != roomToken.Infants)
                {
                    propertyDetails.Warnings.AddNew("Validate failure", $"The guest details for room {roomNumber} do not match what was searched for");
                }

                propertyDetails.LocalCost += localCost;
                propertyDetails.Rooms.Add(room);
            }
            else
            {
                propertyDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidRoomBookingToken);
            }
        }

        /// <inheritdoc />
        public async Task<PropertyDetails> CreateAsync(Precancel.Request request)
        {
            var propertyDetails = new PropertyDetails();

            var token = _tokenService.DecodeBookToken(request.BookingToken);
            var propertyToken = _tokenService.DecodePropertyToken(request.BookingToken); // todo - this almost certainly isn't needed

            if (token is not null && propertyToken is not null && token.PropertyID != 0)
            {
                var propertyContent = await _contentRepository.GetContentforPropertyAsync(token.PropertyID, request.Account, request.SupplierBookingReference);

                propertyDetails = new PropertyDetails()
                {
                    AccountID = request.Account.AccountID,
                    SourceReference = request.SupplierBookingReference,
                    Source = propertyContent.Source,
                    SupplierID = propertyContent.SupplierID,
                    TPKey = propertyContent.TPKey,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(propertyToken.ISOCurrencyID),
                    ThirdPartyConfigurations = request.Account.Configurations,
                    BookingID = propertyContent.BookingID,
                };

                SetSupplierReference1(propertyDetails, request.SupplierReference1);
                SetSupplierReference2(propertyDetails, request.SupplierReference2);

                // hack - editing param
                request.BookingID = propertyContent.BookingID;
            }
            else
            {
                propertyDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidBookingToken);
            }

            return propertyDetails;
        }

        /// <inheritdoc />
        public async Task<PropertyDetails> CreateAsync(Cancel.Request request)
        {
            var propertyDetails = new PropertyDetails();

            var token = _tokenService.DecodeBookToken(request.BookingToken);
            var propertyToken = _tokenService.DecodePropertyToken(request.BookingToken);

            if (token is not null && propertyToken is not null && token.PropertyID != 0)
            {
                var propertyContent = await _contentRepository.GetContentforPropertyAsync(token.PropertyID, request.Account, request.SupplierBookingReference);
                propertyDetails = new PropertyDetails()
                {
                    AccountID = request.Account.AccountID,
                    SourceReference = request.SupplierBookingReference,
                    Source = propertyContent.Source,
                    SupplierID = propertyContent.SupplierID,
                    TPKey = propertyContent.TPKey,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(propertyToken.ISOCurrencyID),
                    ThirdPartyConfigurations = request.Account.Configurations,
                    BookingID = propertyContent.BookingID,
                };

                SetSupplierReference1(propertyDetails, request.SupplierReference1);
                SetSupplierReference2(propertyDetails, request.SupplierReference2);

                // hack - editing param
                request.BookingID = propertyContent.BookingID;
            }
            else
            {
                propertyDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidBookingToken);
            }

            return propertyDetails;
        }

        private void SetSupplierReference1(PropertyDetails propertyDetails, string supplierReference1)
        {
            switch (propertyDetails.Source)
            {
                case ThirdParties.MTS:
                    propertyDetails.Rooms.Add(new RoomDetails());
                    propertyDetails.Rooms[0].ThirdPartyReference = supplierReference1;
                    break;
                default:
                    propertyDetails.SourceSecondaryReference = supplierReference1;
                    break;
            };
        }

        private void SetSupplierReference2(PropertyDetails propertyDetails, string supplierReference2)
        {
            propertyDetails.TPRef1 = supplierReference2;
        }
    }
}