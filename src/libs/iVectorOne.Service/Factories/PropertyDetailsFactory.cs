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
    using iVectorOne.Utility;
    using Prebook = SDK.V2.PropertyPrebook;
    using Book = SDK.V2.PropertyBook;
    using Precancel = SDK.V2.PropertyPrecancel;
    using Cancel = SDK.V2.PropertyCancel;

    /// <summary>
    /// Factory that builds up property details from api requests, used to pass into the third party code
    /// </summary>
    /// <seealso cref="IPropertyDetailsFactory" />
    public class PropertyDetailsFactory : IPropertyDetailsFactory
    {
        /// <summary>
        /// The token service
        /// </summary>
        private readonly ITokenService _tokenService;

        /// <summary>Repository for retrieving third party meal basis</summary>
        private readonly IMealBasisLookupRepository _mealbasisRepository;

        private readonly ITPSupport _support;

        /// <summary>Initializes a new instance of the <see cref="PropertyDetailsFactory" /> class.</summary>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens.</param>
        /// <param name="currencyRepository">Repository for looking up Currency information.</param>
        ///  /// <param name="mealBasisLookup">Repository for looking up mealbasis information.</param>
        public PropertyDetailsFactory(
            ITokenService tokenService,
            IMealBasisLookupRepository mealBasisLookup,
            ITPSupport support)
        {
            _tokenService = tokenService;
            _mealbasisRepository = mealBasisLookup;
            _support = support;
        }

        /// <inheritdoc />
        public async Task<PropertyDetails> CreateAsync(Prebook.Request request)
        {
            var propertyDetails = new PropertyDetails();

            var propertyToken = await _tokenService.DecodePropertyTokenAsync(request.BookingToken, request.User);

            if (propertyToken is not null)
            {
                propertyDetails = new PropertyDetails()
                {
                    SubscriptionID = request.User.SubscriptionID,
                    ArrivalDate = propertyToken.ArrivalDate,
                    DepartureDate = propertyToken.ArrivalDate.AddDays(propertyToken.Duration),
                    Source = propertyToken.Source,
                    CentralPropertyID = propertyToken.CentralPropertyID,
                    PropertyID = propertyToken.PropertyID,
                    TPKey = propertyToken.TPKey,
                    ISONationalityCode = request.NationalityID,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(propertyToken.CurrencyID),
                    OpaqueRates = request.OpaqueRates,
                    SellingCountry = request.SellingCountry,
                    ThirdPartyConfigurations = request.User.Configurations,
                    ResortCode = propertyToken.GeographyCode,
                };

                foreach (var room in request.RoomBookings)
                {
                    var roomToken = _tokenService.DecodeRoomToken(room.RoomBookingToken);

                    if (roomToken is not null && roomToken.Adults != 0)
                    {
                        var passengers = SetupPrebookPassengers(roomToken);
                        var mealbasisId = PropertyFactoryHelper.GetNumFromList(roomToken.MealBasisID, 2);
                        var localCost = PropertyFactoryHelper.GetNumFromList(roomToken.LocalCost, 7) / 100m;

                        var roomDetails = new RoomDetails()
                        {
                            ThirdPartyReference = room.SupplierReference1,
                            RoomTypeCode = room.SupplierReference2,
                            Passengers = passengers,
                            LocalCost = localCost,
                            PropertyRoomBookingID = roomToken.PropertyRoomBookingID,
                            MealBasisCode = await _mealbasisRepository.GetMealBasisCodefromTPMealbasisIDAsync(
                                propertyToken.Source,
                                mealbasisId,
                                propertyDetails.SubscriptionID)
                        };
                        propertyDetails.LocalCost += localCost;
                        propertyDetails.Rooms.Add(roomDetails);
                    }
                    else
                    {
                        propertyDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidRoomBookingToken);
                    }
                }
            }

            Validate(propertyToken!, propertyDetails);

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
            var propertyToken = await _tokenService.DecodePropertyTokenAsync(request.BookingToken, request.User);

            if (propertyToken is not null)
            {
                propertyDetails = new PropertyDetails()
                {
                    SubscriptionID = request.User.SubscriptionID,
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
                    Source = propertyToken.Source,
                    BookingReference = request.BookingReference,
                    PropertyID = propertyToken.PropertyID,
                    PropertyName = propertyToken.PropertyName,
                    TPKey = propertyToken.TPKey,
                    ArrivalDate = propertyToken.ArrivalDate,
                    DepartureDate = propertyToken.ArrivalDate.AddDays(propertyToken.Duration),
                    ISONationalityCode = request.NationalityID,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(propertyToken.CurrencyID),
                    OpaqueRates = request.OpaqueRates,
                    SellingCountry = request.SellingCountry,
                    ThirdPartyConfigurations = request.User.Configurations,
                };

                int roomNumber = 0;
                foreach (var roomBooking in request.RoomBookings)
                {
                    roomNumber += 1;
                    await BuildBookRoomAsync(propertyToken, propertyDetails, roomNumber, roomBooking);
                }
            }

            this.Validate(propertyToken!, propertyDetails);

            return propertyDetails;
        }

        /// <summary>Create a Room on the property details using the room request</summary>
        /// <param name="propertyToken">The property token.</param>
        /// <param name="propertyDetails">The property details.</param>
        /// <param name="roomNumber">The room number.</param>
        /// <param name="roomBooking">The room booking.</param>
        private async Task BuildBookRoomAsync(PropertyToken propertyToken, PropertyDetails propertyDetails, int roomNumber, Book.RoomBooking roomBooking)
        {
            var roomToken = _tokenService.DecodeRoomToken(roomBooking.RoomBookingToken);

            if (roomToken is not null && roomToken.Adults != 0)
            {
                var mealbasisId = PropertyFactoryHelper.GetNumFromList(roomToken.MealBasisID, 2);
                decimal localCost = PropertyFactoryHelper.GetNumFromList(roomToken.LocalCost, 7) / 100m;

                var room = new RoomDetails()
                {
                    ThirdPartyReference = roomBooking.SupplierReference,
                    LocalCost = localCost,
                    GrossCost = localCost,
                    PropertyRoomBookingID = roomToken.PropertyRoomBookingID,
                    SpecialRequest = roomBooking.SpecialRequest,
                    MealBasisCode = await _mealbasisRepository.GetMealBasisCodefromTPMealbasisIDAsync(
                        propertyToken.Source,
                        mealbasisId,
                        propertyDetails.SubscriptionID)
                };

                foreach (var guestDetail in roomBooking.GuestDetails)
                {
                    var passenger = new Passenger()
                    {
                        Title = guestDetail.Title,
                        FirstName = guestDetail.FirstName,
                        LastName = guestDetail.LastName,
                        DateOfBirth = guestDetail.DateOfBirth,
                        Age = guestDetail.DateOfBirth.GetAgeAtTargetDate(propertyToken.ArrivalDate)
                    };

                    switch (guestDetail.Type)
                    {
                        case Book.GuestType.Unset:
                        case Book.GuestType.Adult:
                            passenger.PassengerType = PassengerType.Adult;
                            break;
                        case Book.GuestType.Child:
                            passenger.PassengerType = PassengerType.Child;
                            break;
                        case Book.GuestType.Infant:
                            passenger.PassengerType = PassengerType.Infant;
                            break;
                    }

                    room.Passengers.Add(passenger);
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

            var token = await _tokenService.DecodeBookTokenAsync(request.BookingToken, request.User);
            var propertyToken = await _tokenService.DecodePropertyTokenAsync(request.BookingToken, request.User);

            if (token is not null && propertyToken is not null && token.PropertyID != 0 && !string.IsNullOrEmpty(token.Source))
            {
                propertyDetails = new PropertyDetails()
                {
                    SubscriptionID = request.User.SubscriptionID,
                    SourceReference = request.SupplierBookingReference,
                    Source = token.Source,
                    TPKey = propertyToken.TPKey,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(propertyToken.CurrencyID),
                    ThirdPartyConfigurations = request.User.Configurations,
                };

                SetSupplierReference1(propertyDetails, request.SupplierReference1);
                SetSupplierReference2(propertyDetails, request.SupplierReference2);
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

            var token = await _tokenService.DecodeBookTokenAsync(request.BookingToken, request.User);
            var propertyToken = await _tokenService.DecodePropertyTokenAsync(request.BookingToken, request.User);

            if (token is not null && propertyToken is not null && token.PropertyID != 0 && !string.IsNullOrEmpty(token.Source))
            {
                propertyDetails = new PropertyDetails()
                {
                    SubscriptionID = request.User.SubscriptionID,
                    SourceReference = request.SupplierBookingReference,
                    Source = token.Source,
                    TPKey = propertyToken.TPKey,
                    ISOCurrencyCode = await _support.ISOCurrencyCodeLookupAsync(propertyToken.CurrencyID),
                    ThirdPartyConfigurations = request.User.Configurations,
                };

                SetSupplierReference1(propertyDetails, request.SupplierReference1);
                SetSupplierReference2(propertyDetails, request.SupplierReference2);
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