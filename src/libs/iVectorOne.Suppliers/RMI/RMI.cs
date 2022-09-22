namespace iVectorOne.Suppliers.RMI
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
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.RMI.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;

    public class RMI : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IRMISettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<RMI> _logger;

        public string Source => ThirdParties.RMI;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return true;
        }

        public bool SupportsRemarks => false;

        #endregion

        #region Constructors

        public RMI(IRMISettings settings, ITPSupport support, ISerializer serializer, HttpClient httpClient, ILogger<RMI> logger)
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
            //'No prebook so repeat search for each room to confirm room availability
            var success = false;
            var request = new Request();
            var cancellations = new Cancellations();

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                try
                {
                    //'Check for change in prices and get cancellation policies
                    var requestXml = BuildPrebookXML(propertyDetails, roomDetails.MealBasisCode);
                    request = await SendRequestAsync(requestXml, propertyDetails, "PreBook");
                    var searchResponse = _serializer.DeSerialize<SearchResponse>(request.ResponseXML);

                    if (searchResponse.ReturnStatus.Success.ToLower() == "true")
                    {
                        var room = searchResponse.PropertyResults.Where(propertyResult => propertyResult.PropertyId == propertyDetails.TPKey)
                            .SelectMany(propertyResults => propertyResults.RoomTypes
                            .Where(roomType => roomType.RoomId == roomDetails.RoomTypeCode
                                            && roomType.MealBasisId == roomDetails.MealBasisCode
                                            && roomType.RoomsAppliesTo.RoomRequest == Math.Abs(roomDetails.PropertyRoomBookingID))).FirstOrDefault();

                        if (room != null)
                        {
                            var cost = room.Total;
                            if (cost != 0)
                            {
                                roomDetails.LocalCost = cost;
                                roomDetails.GrossCost = cost;
                                success = true;
                            }
                            else
                            {
                                throw new Exception("Room price not found");
                            }

                            foreach (var oCancellationPolicy in room.CancellationPolicies)
                            {
                                var cancelBy = oCancellationPolicy.CancelBy.ToSafeDate();
                                var penalty = oCancellationPolicy.Penalty.ToSafeDecimal();
                                var now = DateTime.Now;

                                if (cancelBy > now)
                                {
                                    cancellations.AddNew(now, cancelBy, penalty);
                                }
                                else
                                {
                                    cancellations.AddNew(cancelBy, now, penalty);
                                }
                            }
                        }

                        foreach (var errata in searchResponse.PropertyResults
                                     .Where(propertyResult => propertyResult.PropertyId == propertyDetails.TPKey)
                                     .SelectMany(propertyResult => propertyResult.Errata))
                        {
                            var text = errata.Description;

                            if (!string.IsNullOrEmpty(errata.EndDate))
                            {
                                text = $"End Date: {errata.EndDate}, {text}";
                            }

                            if (!string.IsNullOrEmpty(errata.StartDate))
                            {
                                text = $"Start Date: {errata.StartDate}, {text}";
                            }

                            text += errata.EndDate;
                            propertyDetails.Errata.AddNew("Important Information", text);
                        }
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    propertyDetails.Warnings.AddNew("Prebook Exception", ex.Message);
                }
                finally
                {
                    propertyDetails.AddLog("Prebook", request);
                }
            }
            propertyDetails.Cancellations = Cancellations.MergeMultipleCancellationPolicies(cancellations);

            return success;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var webRequest = new Request();
            string bookingReference = "";
            try
            {
                var requestXml = await BuildBookXMLAsync(propertyDetails);
                webRequest = await SendRequestAsync(requestXml, propertyDetails, "Book");

                var bookResponse = _serializer.DeSerialize<BookResponse>(webRequest.ResponseXML);

                bookingReference = string.Equals(bookResponse.ReturnStatus.Success.ToLower(), "true")
                    ? bookResponse.BookingDetails.BookingReference
                    : "failed";
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.Message);
            }
            finally
            {
                propertyDetails.AddLog("Book", webRequest);
            }

            return bookingReference;
        }

        #endregion

        #region Cancellation

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdpartyCancellationResponse = new ThirdPartyCancellationResponse();

            var request = new Request();

            try
            {
                var requestXml = BuildCancelXML(propertyDetails);
                request = await SendRequestAsync(requestXml, propertyDetails, "Cancel");
                var cancelResponse = _serializer.DeSerialize<CancelResponse>(request.ResponseXML);

                if (string.Equals(cancelResponse.ReturnStatus.Success.ToLower(), "true"))
                {
                    thirdpartyCancellationResponse.Success = true;
                    thirdpartyCancellationResponse.TPCancellationReference = propertyDetails.SourceReference;
                    thirdpartyCancellationResponse.Amount = SafeTypeExtensions.ToSafeDecimal(cancelResponse.BookingDetails.Amount);
                }
                else
                {
                    thirdpartyCancellationResponse.Success = false;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.Message);
            }
            finally
            {
                propertyDetails.AddLog("Cancellation", request);
            }

            return thirdpartyCancellationResponse;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        #endregion

        #region Booking search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return "";
        }

        #endregion

        #region Booking Status Update

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }

        #endregion

        #region End Session

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        #endregion

        #region XML Builders

        public string BuildPrebookXML(PropertyDetails propertyDetails, string mealBasisCode)
        {
            var preBookRequest = new SearchRequest
            {
                LoginDetails = BuildLoginDetails(propertyDetails),
                SearchDetails =
                {
                    ArrivalDate = propertyDetails.ArrivalDate.ToString(Constant.DateFormat),
                    Duration = propertyDetails.Duration,
                    PropertyID = propertyDetails.TPKey,
                    MealBasisId = mealBasisCode,
                    MinStarRating = 0,
                    MinimumPrice = 0,
                    MaximumPrice = 0,
                    RoomRequests = propertyDetails.Rooms.Select(oRoom =>
                    {
                        return  new RoomRequest
                        {
                            Adults = oRoom.Adults,
                            Children = oRoom.Children,
                            Infants = oRoom.Infants,
                            ChildAges = oRoom.ChildAges.Select(iAge => new ChildAge{ Age = iAge}).ToList()
                        };
                    }).ToList()
                }
            };

            return _serializer.Serialize(preBookRequest).OuterXml;
        }

        public async Task<string> BuildBookXMLAsync(PropertyDetails propertyDetails)
        {
            var loginDetails = BuildLoginDetails(propertyDetails);
            var roomBookings = new List<RoomBooking>();
            foreach(var room in propertyDetails.Rooms)
            {
                //'Skips first passenger due to occasions where multiple lead guests or no lead guests
                var roomPassengers = (room.PropertyRoomBookingID == 1)
                    ? room.Passengers.OrderBy(p => !p.IsLeadGuest).Skip(1).ToList()
                    : room.Passengers.ToList();

                var roomBooking = new RoomBooking
                {
                    RoomID = room.ThirdPartyReference,
                    MealBasisID = room.MealBasisCode,
                    Adults = room.Adults,
                    Children = room.Children,
                    Infants = room.Infants,
                };

                foreach(var passenger in roomPassengers.Where(x => x.PassengerType != PassengerType.Infant))
                {
                    var guest = new Guest
                    {
                        GuestType = passenger.PassengerType.ToString(),
                        FirstName = passenger.FirstName,
                        LastName = passenger.LastName,
                        Title = (passenger.PassengerType == PassengerType.Adult) ? passenger.Title : string.Empty,
                        Age = (passenger.PassengerType == PassengerType.Child) ? passenger.Age : 0,
                        Nationality = await GetNationalityAsync(passenger.NationalityCode)
                    };

                    roomBooking.Guests.Add(guest);
                }

                roomBookings.Add(roomBooking);
            }

            var leadGuest = new LeadGuest
            {
                FirstName = propertyDetails.LeadGuestFirstName,
                LastName = propertyDetails.LeadGuestLastName,
                Title = propertyDetails.LeadGuestTitle
            };

            var bookRequest = new BookRequest
            {
                LoginDetails = loginDetails,
                BookDetails =
                {
                    ArrivalDate = propertyDetails.ArrivalDate.ToString(Constant.DateFormat),
                    Duration = propertyDetails.Duration,
                    LeadGuest =leadGuest,
                    TradeReference = propertyDetails.BookingReference,
                    RoomBookings = roomBookings,
                    Request = propertyDetails.Rooms.Any(x => !string.IsNullOrEmpty(x.SpecialRequest)) ?
                        string.Join(Environment.NewLine, propertyDetails.Rooms.Select(x => x.SpecialRequest)) :
                        string.Empty
                }
            };
            return _serializer.Serialize(bookRequest).OuterXml;
        }

        private async Task<string> GetNationalityAsync(string nationalityCode)
        {
            var nationalityIsoCode = "";
            try
            {
                nationalityIsoCode = await _support.TPNationalityLookupAsync(Source, nationalityCode);
            }
            catch (Exception)
            {
            }
            return nationalityIsoCode;
        }

        public string BuildCancelXML(PropertyDetails propertyDetails)
        {
            var cancelRequest = new CancelRequest
            {
                LoginDetails = BuildLoginDetails(propertyDetails),
                BookingReference = propertyDetails.SourceReference,
                Reason = _settings.DefaultCancellationReason(propertyDetails) //'A cancellation reason must be specified
            };

            return _serializer.Serialize(cancelRequest).OuterXml;
        }

        public LoginDetails BuildLoginDetails(IThirdPartyAttributeSearch SearchDetails)
        {
            return new LoginDetails
            {
                Login = _settings.Login(SearchDetails),
                Password = _settings.Password(SearchDetails),
                Version = _settings.Version(SearchDetails)
            };
        }

        #endregion

        #region Request Sender

        public async Task<Request> SendRequestAsync(string requestXml, PropertyDetails propertyDetails, string logFilename)
        {
            var webRequest = new Request
            {
                EndPoint = _settings.URL(propertyDetails),
                Method = RequestMethod.POST,
                Source = Source,
                AuthenticationMode = AuthenticationMode.Basic,
                UserName = _settings.Login(propertyDetails),
                Password = _settings.Password(propertyDetails),
                UseGZip = true,
                CreateLog = true,
                LogFileName = logFilename
            };
            webRequest.SetRequest(requestXml);
            await webRequest.Send(_httpClient, _logger);

            return webRequest;
        }

        #endregion
    }
}