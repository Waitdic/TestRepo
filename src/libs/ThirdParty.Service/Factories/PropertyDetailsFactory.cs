namespace ThirdParty.Factories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Repositories;
    using ThirdParty.Models.Tokens;
    using ThirdParty.SDK.V2;
    using ThirdParty.Services;
    using ThirdParty.Utility;
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

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A property details object
        /// </returns>
        public async Task<PropertyDetails> CreateAsync(Prebook.Request request, User user)
        {
            PropertyDetails propertyDetails = new PropertyDetails();

            var propertyToken = await _tokenService.DecodePropertyTokenAsync(request.BookingToken);
            
            if (propertyToken != null)
            {
                propertyDetails = new PropertyDetails()
                {
                    ArrivalDate = propertyToken.ArrivalDate,
                    DepartureDate = propertyToken.ArrivalDate.AddDays(propertyToken.Duration),
                    Source = propertyToken.Source,
                    PropertyID = propertyToken.CentralPropertyID,
                    TPPropertyID = propertyToken.PropertyID,
                    TPKey = propertyToken.TPKey,
                    NationalityID = request.NationalityID,
                    CurrencyCode = await _support.TPCurrencyCodeLookupAsync(propertyToken.CurrencyID),
                    OpaqueRates = request.OpaqueRates,
                    SellingCountry = request.SellingCountry,
                    ThirdPartyConfigurations = user.Configurations,
                };

#if DEBUG
                propertyDetails.CreateLogs = true;
#endif

                foreach (var room in request.RoomBookings)
                {
                    var roomToken = _tokenService.DecodeRoomToken(room.RoomBookingToken);

                    if (roomToken != null && roomToken.Adults != 0 )
                    {
                        var passengers = SetupPrebookPassengers(roomToken);
                        var mealbasisID = PropertyFactoryHelper.GetNumFromList(roomToken.MealBasis, 2);
                        var localCost = PropertyFactoryHelper.GetNumFromList(roomToken.LocalCost, 7) / 100m;

                        var roomDetails = new RoomDetails()
                        {
                            ThirdPartyReference = room.SupplierReference1,
                            RoomTypeCode = room.SupplierReference2,
                            Passengers = passengers,
                            LocalCost = localCost,
                            PropertyRoomBookingID = roomToken.PropertyRoomBookingID,
                            MealBasisCode = await _mealbasisRepository.GetMealBasisCodefromTPMealbasisIDAsync(propertyToken.Source, mealbasisID)
                        };
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

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A property details object
        /// </returns>
        public async Task<PropertyDetails> CreateAsync(Book.Request request, User user)
        {
            var propertyDetails = new PropertyDetails();

            var leadCustomer = request.LeadCustomer;
            var propertyToken = await _tokenService.DecodePropertyTokenAsync(request.BookingToken);

            if (propertyToken != null)
            {
                propertyDetails = new PropertyDetails()
                {
                    LeadGuestTitle = leadCustomer.CustomerTitle,
                    LeadGuestFirstName = leadCustomer.CustomerFirstName,
                    LeadGuestLastName = leadCustomer.CustomerLastName,
                    DateOfBirth = leadCustomer.DateOfBirth,
                    LeadGuestAddress1 = leadCustomer.CustomerAddress1,
                    LeadGuestAddress2 = leadCustomer.CustomerAddress2,
                    LeadGuestTownCity = leadCustomer.CustomerTownCity,
                    LeadGuestCounty = leadCustomer.CustomerCounty,
                    LeadGuestPostcode = leadCustomer.CustomerPostcode,
                    LeadGuestBookingCountryID = _support.TPBookingCountryCodeLookup(propertyToken.Source, request.LeadCustomer.CustomerBookingCountryCode),
                    LeadGuestPhone = leadCustomer.CustomerPhone,
                    LeadGuestMobile = leadCustomer.CustomerMobile,
                    LeadGuestEmail = leadCustomer.CustomerEmail,
                    TPRef1 = request.SupplierReference1,
                    TPRef2 = request.SupplierReference2,
                    Source = propertyToken.Source,
                    BookingReference = request.BookingReference,
                    TPPropertyID = propertyToken.PropertyID,
                    CreateLogs = true,
                    TPKey = propertyToken.TPKey,
                    ArrivalDate = propertyToken.ArrivalDate,
                    DepartureDate = propertyToken.ArrivalDate.AddDays(propertyToken.Duration),
                    NationalityID = request.NationalityID,
                    CurrencyCode = await _support.TPCurrencyCodeLookupAsync(propertyToken.CurrencyID),
                    OpaqueRates = request.OpaqueRates,
                    SellingCountry = request.SellingCountry,
                    ThirdPartyConfigurations = user.Configurations,
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

            if(roomToken != null && roomToken.Adults != 0)
            {
                var mealbasisID = PropertyFactoryHelper.GetNumFromList(roomToken.MealBasis, 2);
                var localCost = (decimal) PropertyFactoryHelper.GetNumFromList(roomToken.LocalCost, 7) / 100m;

                var room = new RoomDetails()
                {
                    ThirdPartyReference = roomBooking.SupplierReference,
                    LocalCost = localCost,
                    GrossCost = localCost,
                    PropertyRoomBookingID = roomToken.PropertyRoomBookingID,
                    SpecialRequest = roomBooking.SpecialRequest,
                    MealBasisCode = await _mealbasisRepository.GetMealBasisCodefromTPMealbasisIDAsync(propertyToken.Source, mealbasisID)
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

                if (room.Passengers.Count(p => p.PassengerType == PassengerType.Adult) != roomToken.Adults
                    || room.Passengers.Count(p => p.PassengerType == PassengerType.Child) != roomToken.Children
                    || room.Passengers.Count(p => p.PassengerType == PassengerType.Infant) != roomToken.Infants)
                {
                    propertyDetails.Warnings.AddNew("Validate failure", $"The guest details for room {roomNumber} do not match what was searched for");
                }

                propertyDetails.Rooms.Add(room);
            }
            else
            {
                propertyDetails.Warnings.AddNew("Validate failure", WarningMessages.InvalidRoomBookingToken);
            }
        }

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A property details object
        /// </returns>
        public async Task<PropertyDetails> CreateAsync(Precancel.Request request, User user)
        {
            var propertyDetails = new PropertyDetails();

            var token = await _tokenService.DecodeBookTokenAsync(request.BookingToken);
            var propertyToken = await _tokenService.DecodePropertyTokenAsync(request.BookingToken);

            if (token != null && token.PropertyID != 0 && !string.IsNullOrEmpty(token.Source))
            {
                propertyDetails = new PropertyDetails()
                {
                    SourceReference = request.SupplierBookingReference,
                    Source = token.Source,
                    CreateLogs = true,
                    TPKey = propertyToken.TPKey,
                    CurrencyCode = await _support.TPCurrencyCodeLookupAsync(propertyToken.CurrencyID),
                    ThirdPartyConfigurations = user.Configurations,
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

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A property details object
        /// </returns>
        public async Task<PropertyDetails> CreateAsync(Cancel.Request request, User user)
        {
            var propertyDetails = new PropertyDetails();

            var token = await _tokenService.DecodeBookTokenAsync(request.BookingToken);
            var propertyToken = await _tokenService.DecodePropertyTokenAsync(request.BookingToken);

            if (token != null && token.PropertyID != 0 && !string.IsNullOrEmpty(token.Source))
            {
                propertyDetails = new PropertyDetails()
                {
                    SourceReference = request.SupplierBookingReference,
                    Source = token.Source,
                    CreateLogs = true,
                    TPKey = propertyToken.TPKey,
                    CurrencyCode = await _support.TPCurrencyCodeLookupAsync(propertyToken.CurrencyID),
                    ThirdPartyConfigurations = user.Configurations,
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
