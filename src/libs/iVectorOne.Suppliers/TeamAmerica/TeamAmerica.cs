namespace iVectorOne.CSSuppliers.TeamAmerica
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.CSSuppliers.TeamAmerica.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;

    public class TeamAmerica : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly ITeamAmericaSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TeamAmerica> _logger;

        public string Source => ThirdParties.TEAMAMERICA;

        public bool SupportsBookingSearch => true;
        public bool SupportsRemarks => true;

        #endregion

        #region Constructors

        public TeamAmerica(
            ITeamAmericaSettings settings,
            ITPSupport support,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<TeamAmerica> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            bool @return = true;
            var searchRequests = new List<Request>();

            //'check availability/cancelllation policy for each room
            var available = true;

            try
            {
                //'check availability/cancelllation policy for each room
                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    //'Build/send search
                    var request = BuildSearchXml(propertyDetails, roomDetails);
                    var searchRequest = await SendRequestAsync(Constant.SoapActionPreBook, request, propertyDetails, Constant.SoapActionPreBook);
                    var hotelOffer = Envelope<PriceSearchResponse>.DeSerialize(searchRequest.ResponseXML, _serializer).HotelSearchResponse.Offers.First();

                    // 'check availability
                    if (!IsEveryNightAvailable(hotelOffer))
                    {
                        available = false;
                    }
                    else
                    {
                        //'check the price

                        //'get the occupancy returned at search (Single, Double, Triple or Quad)
                        string occupancy = GetOccupancy(roomDetails, true);

                        decimal cost = hotelOffer.NightlyInfos.Select(night
                            => (night.Prices.FirstOrDefault(x => string.Equals(x.Occupancy, occupancy))?.AdultPrice ?? "0").ToSafeDecimal()).Sum();

                        //'Compare with the original price
                        if (cost > 0 && roomDetails.LocalCost.ToSafeDecimal() != cost)
                        {
                            roomDetails.LocalCost = cost.ToSafeDecimal();
                            roomDetails.GrossCost = cost.ToSafeDecimal();
                        }

                        string averageNightRate = hotelOffer.AverageRates.FirstOrDefault(rate => string.Equals(rate.Occupancy, occupancy))?.AverageNightlyRate ?? "";

                        roomDetails.ThirdPartyReference += $"|{averageNightRate}";
                    }

                    searchRequests.Add(searchRequest);

                    if (!available)
                    {
                        @return = false;
                    }
                    else
                    {
                        GetCancellationCosts(propertyDetails, hotelOffer);
                    }
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBookExcpetion", ex.Message);
                @return = false;
            }
            finally
            {
                //'Add the xml to the booking
                foreach (var searchRequest in searchRequests)
                {
                    propertyDetails.AddLog("Price Check", searchRequest);
                }
            }

            return @return;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string reference = "";
            var request = new Request();

            try
            {
                //'Build/send Confirmation
                string requestBody = await BuildBookXmlAsync(propertyDetails);
                request = await SendRequestAsync(Constant.SoapActionBook, requestBody, propertyDetails, "Book");
                var response = Envelope<NewMultiItemReservationResponse>.DeSerialize(request.ResponseXML, _serializer).ReservationResponse;
                string responseBody = request.ResponseString;

                //'Check for errors or pending bookings
                if (responseBody.ToLower().Contains("error") || string.Equals(response.ReservationInformation.ReservationStatus, "RQ"))
                {
                    reference = Constant.Failed;
                }
                else
                {
                    //'get the confirmation code
                    reference = response.ReservationInformation.ReservationNumber;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("BookException", ex.Message);
                reference = Constant.Failed;
            }
            finally
            {
                propertyDetails.AddLog("Book", request);
            }

            return reference;
        }

        #endregion

        #region CancelBooking

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var cancelResponse = new ThirdPartyCancellationResponse
            {
                Success = true
            };

            var request = new Request();

            try
            {
                var requestBody = BuildCancelXml(propertyDetails);
                request = await SendRequestAsync(Constant.SoapActionCancel, requestBody, propertyDetails, Constant.SoapActionCancel);

                var response = Envelope<CancelReservationResponse>.DeSerialize(request.ResponseXML, _serializer).CancelResponse;

                if (string.Equals(response.ReservationStatusCode, "CX"))
                {
                    cancelResponse.TPCancellationReference = propertyDetails.SourceReference;
                }
                else
                {
                    cancelResponse.TPCancellationReference = Constant.Failed;
                    cancelResponse.Success = false;
                }
            }
            catch (Exception)
            {
                cancelResponse.TPCancellationReference = Constant.Failed;
                cancelResponse.Success = false;
            }
            finally
            {
                propertyDetails.AddLog("Cancellation", request);
            }

            return cancelResponse;
        }

        #endregion

        #region Other methods

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return "";
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new();
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }
        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        #endregion

        #region Helpers

        public static bool IsEveryNightAvailable(HotelOffer hotelOffer)
        {
            return hotelOffer.NightlyInfos.All(night => string.Equals(night.Status, Constant.Available));
        }

        public async Task<Request> SendRequestAsync(string soapAction, string xml, IThirdPartyAttributeSearch searchDetails, string logFile)
        {
            var request = new Request
            {
                EndPoint = _settings.URL(searchDetails),
                SoapAction = $"{_settings.URL(searchDetails)}/{soapAction}",
                Method = RequestMethod.POST,
                Source = Source,
                ContentType = ContentTypes.Text_Xml_charset_utf_8,
                LogFileName = logFile,
                CreateLog = true
            };
            request.SetRequest(xml);
            await request.Send(_httpClient, _logger);

            return request;
        }

        public string BuildSearchXml(PropertyDetails propertyDetails, RoomDetails roomDetails)
        {
            var request = new PriceSearch
            {
                UserName = _settings.Username(propertyDetails),
                Password = _settings.Password(propertyDetails),
                CityCode = propertyDetails.ResortCode,
                ProductCode = roomDetails.ThirdPartyReference.Split('|')[0],
                RequestType = Constant.SearchTypeHotel,
                Occupancy = "",
                ArrivalDate = propertyDetails.ArrivalDate.ToString(Constant.DateTimeFormat),
                NumberOfNights = propertyDetails.Duration,
                NumberOfRooms = propertyDetails.Rooms.Count,
                DisplayClosedOut = Constant.TokenNo,
                DisplayOnRequest = Constant.TokenNo,
                VendorIds = { propertyDetails.TPKey }
            };

            return Envelope<PriceSearch>.Serialize(request, _serializer);
        }

        public void GetCancellationCosts(PropertyDetails propertyDetails, HotelOffer hotelOffer)
        {
            foreach (var policy in hotelOffer.CancellationPolicies)
            {
                var cancellation = new Cancellation
                {
                    StartDate = propertyDetails.ArrivalDate.AddDays(-policy.NumberDaysPrior),
                    EndDate = propertyDetails.ArrivalDate,
                };

                var penaltyAmount = policy.PenaltyAmount.ToSafeDecimal();

                switch (policy.PenaltyType)
                {
                    case "Nights":
                        cancellation.Amount = penaltyAmount * propertyDetails.LocalCost / propertyDetails.Duration;
                        break;
                    case "Percent":
                        cancellation.Amount = penaltyAmount * propertyDetails.LocalCost / 100;
                        break;
                    case "Dollars":
                        cancellation.Amount = penaltyAmount;
                        break;
                }

                propertyDetails.Cancellations.Add(cancellation);
            }

            propertyDetails.Cancellations.Solidify(SolidifyType.LatestStartDate);
        }

        public string GetOccupancy(RoomDetails roomDetails, bool adultOnly)
        {
            string occupancy = "";
            string familyPlan = roomDetails.ThirdPartyReference.Split('|')[1];
            int childAge = roomDetails.ThirdPartyReference.Split('|')[2].ToSafeInt();

            int adults;
            int children = 0;

            if (string.Equals(familyPlan, Constant.TokenYes))
            {
                adults = roomDetails.AdultsSetAgeOrOver(childAge + 1);
                children = adultOnly ? 0 : roomDetails.ChildrenUnderSetAge(childAge + 1) + roomDetails.Infants;
            }
            else
            {
                adults = roomDetails.Adults + roomDetails.Children + roomDetails.Infants;
            }

            switch (adults)
            {
                case 1:
                    switch (children)
                    {
                        case 0:
                            occupancy = "Single";
                            break;
                        case 1:
                            occupancy = "SGL+1CH";
                            break;
                        //'iAdults must be greater or equal to iChildren, else use DBL room
                        case 2:
                            occupancy = "DBL+1CH";
                            break;
                        case 3:
                            occupancy = "DBL+2CH";
                            break;
                    }
                    break;
                case 2:
                    switch (children)
                    {
                        case 0:
                            occupancy = "Double";
                            break;
                        case 1:
                            occupancy = "DBL+1CH";
                            break;
                        case 2:
                            occupancy = "DBL+2CH";
                            break;
                    }
                    break;
                case 3:
                    switch (children)
                    {
                        case 0:
                            occupancy = "Triple";
                            break;
                        case 1:
                            occupancy = "TPL+1CH";
                            break;
                    }
                    break;
                case 4:
                    occupancy = "Quad";
                    break;
            }

            return occupancy;
        }

        public async Task<string> BuildBookXmlAsync(PropertyDetails propertyDetails)
        {
            var bookRequest = new NewMultiItemReservation
            {
                UserName = _settings.Username(propertyDetails),
                Password = _settings.Password(propertyDetails),
                AgentName = _settings.CompanyName(propertyDetails),
                AgentEmail = _settings.CompanyAddressEmail(propertyDetails),
                ClientReference = propertyDetails.BookingReference,
            };

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                var roomItem = new RoomItem()
                {
                    ProductCode = roomDetails.ThirdPartyReference.Split('|')[0],
                    ProductDate = propertyDetails.ArrivalDate.ToString(Constant.DateTimeFormat),
                    Occupancy = GetOccupancy(roomDetails, false),
                    NumberOfNights = propertyDetails.Duration,
                    Language = Constant.ENG,
                    Quantity = 1,
                    ItemRemarks = propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any() ?
                                         string.Join("\n", propertyDetails.Rooms.Select(x => x.SpecialRequest)) :
                                         "",
                    RateExpected = roomDetails.ThirdPartyReference.Split('|')[3],
                };

                foreach (var passenger in roomDetails.Passengers)
                {
                    string title = string.Equals(passenger.Title, "Master")
                            ? "Mstr"
                            : ((passenger.Title.Length > 4)
                                ? passenger.Title.Substring(0, 4)
                                : passenger.Title);

                    string familyPlan = roomDetails.ThirdPartyReference.Split('|')[1];
                    int childAge = roomDetails.ThirdPartyReference.Split('|')[2].ToSafeInt();
                    string passengerType = passenger.PassengerType == PassengerType.Adult ||
                            string.Equals(familyPlan, Constant.TokenNo) ||
                            passenger.Age > childAge ?
                        Constant.AdultCode :
                        Constant.ChildCode;

                    int passengerAge = passenger.Age;
                    if (passengerAge == 0)
                    {
                        if (passenger.PassengerType == PassengerType.Adult) passengerAge = Constant.AgeAdult;
                        if (passenger.PassengerType == PassengerType.Child) passengerAge = Constant.AgeChild;
                        if (passenger.PassengerType == PassengerType.Infant) passengerAge = Constant.AgeInfant;
                    }

                    string nationalityCode = !string.IsNullOrWhiteSpace(passenger.NationalityCode) ?
                        (await _support.TPNationalityLookupAsync(Source, passenger.NationalityCode)) :
                        _settings.DefaultNationalityCode(propertyDetails);

                    roomItem.Passengers.Add(new NewPassenger
                    {
                        Salutation = title,
                        FamilyName = passenger.LastName,
                        FirstName = passenger.FirstName,
                        PassengerType = passengerType,
                        NationalityCode = nationalityCode,
                        PassengerAge = passengerAge
                    });
                }

                bookRequest.RoomItems.Add(roomItem);
            }

            return Envelope<NewMultiItemReservation>.Serialize(bookRequest, _serializer);
        }

    public string BuildCancelXml(PropertyDetails propertyDetails)
    {
        var cancelResponse = new CancelReservation
        {
            UserName = _settings.Username(propertyDetails),
            Password = _settings.Password(propertyDetails),
            ReservationNumber = propertyDetails.SourceReference
        };

        return Envelope<CancelReservation>.Serialize(cancelResponse, _serializer);
    }

    #endregion
}
}