namespace ThirdParty.CSSuppliers.ATI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVector.Search.Property;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.ATI.Models;
    using ThirdParty.CSSuppliers.ATI.Models.Common;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using Erratum = ThirdParty.Models.Property.Booking.Erratum;
    using Intuitive.Helpers.Extensions;

    public class ATI : IThirdParty, ISingleSource
    {
        private readonly IATISettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ATI> _logger;

        public ATI(
            IATISettings settings,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<ATI> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public string Source => ThirdParties.ATI;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails, false);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails, false);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        // No Prebook method, so just check price with another search
        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            bool success = false;
            var webRequest = new Request();

            try
            {
                decimal cost = 0;
                var cancellations = new Cancellations();

                webRequest = await BuildAndSendSearchAsync(propertyDetails);
                var cleanedResponse = _serializer.DeSerialize<Envelope<AtiAvailabilitySearch>>(webRequest.ResponseXML);
                var roomRates = new List<RoomRates>();

                foreach (var roomBooking in propertyDetails.Rooms)
                {
                    var roomStay = cleanedResponse
                        .Body
                        .Content
                        .RoomStays
                        .First(x => x
                            .RoomTypes
                            .Any(roomType => roomType.RoomTypeCode == $"{propertyDetails.TPKey}-{roomBooking.ThirdPartyReference}"));

                    decimal roomCost = roomStay.Total.AmountAfterTax / 100;
                    cost += roomCost;

                    string absoluteDeadline = roomStay.CancelPenalties.First().Deadline.AbsoluteDeadline;
                    bool hasCancellationDate = DateTime.TryParse(absoluteDeadline, out var cancellationDeadline);

                    if (absoluteDeadline == "This reservation cannot be cancelled.")
                    {
                        hasCancellationDate = true;
                        cancellationDeadline = DateTime.Now;
                    }

                    if (hasCancellationDate)
                    {
                        cancellations.AddNew(cancellationDeadline, propertyDetails.ArrivalDate, cost);
                    }

                    var errata = GetErrata(roomStay);
                    propertyDetails.Errata.AddRange(errata);
                    roomRates.Add(new RoomRates { RoomRate = roomStay.RoomRates });

                    roomBooking.LocalCost = roomCost;
                    roomBooking.GrossCost = roomCost;
                }

                propertyDetails.TPRef1 = _serializer.Serialize(new RoomRatesCollection { RoomRates = roomRates.ToArray() }).InnerXml;

                if (cancellations.Count > 0)
                {
                    propertyDetails.Cancellations.Add(cancellations.OrderBy(canx => canx.EndDate).FirstOrDefault());
                    propertyDetails.Cancellations[0].Amount = cost;
                }

                //Check for a price change
                if (cost > 0 && (propertyDetails.LocalCost != cost))
                {
                    propertyDetails.LocalCost = cost;
                    propertyDetails.GrossCost = cost;
                }

                success = true;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PrebookException", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Pre-Book Availability", webRequest);
            }

            return success;
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var webRequest = new Request();
            string bookingReference;

            try
            {
                string apiVersion = _settings.APIVersion(propertyDetails, false);
                string userId = _settings.UserID(propertyDetails);
                string password = _settings.Password(propertyDetails);
                string url = _settings.URL(propertyDetails);

                var paxRphLookup = new Dictionary<Passenger, int>();
                int i = 1;
                foreach (var passenger in propertyDetails.Rooms.SelectMany(room => room.Passengers))
                {
                    paxRphLookup.Add(passenger, i);
                    i++;
                }

                var bookingRequests = new List<Comment>();
                if (propertyDetails.BookingComments.Any())
                {
                    bookingRequests.AddRange(propertyDetails.BookingComments.Select(c => new Comment { Text = c.Text }));
                }

                var roomRateLookup = _serializer.DeSerialize<RoomRatesCollection>(propertyDetails.TPRef1);

                var bookRequest = new Envelope<AtiBookRequest>();
                bookRequest.Body.Content = new AtiBookRequest
                {
                    Version = apiVersion,
                    TransactionIdentifier = propertyDetails.BookingReference.ToSafeInt(),
                    POS = new Pos { Source = new Source { UserId = userId, } },
                    HotelReservations = new[]
                    {
                        new HotelReservation
                        {
                            RoomStays = propertyDetails.Rooms.Select((room, j) => new RoomStay
                            {
                                RoomRates = roomRateLookup.RoomRates[j].RoomRate,
                                GuestCounts = room.Passengers.Select(pax => new GuestCount
                                {
                                    AgeQualifyingCode = pax.PassengerType == PassengerType.Adult ? "10" : "08",
                                    Age = (pax.PassengerType == PassengerType.Adult) && (pax.Age == 0) ? 30 : pax.Age,
                                    ResGuestRPH = paxRphLookup[pax],
                                }).ToArray(),
                                TimeSpan = new Models.Common.TimeSpan
                                {
                                    Duration =  "P0Y0M" + propertyDetails.Duration + "D",
                                    Start = propertyDetails.ArrivalDate,
                                },
                                BasicPropertyInfo = new BasicPropertyInfo
                                {
                                    HotelCode = propertyDetails.TPKey + "-" + room.ThirdPartyReference,
                                    Comments = bookingRequests.ToArray(),
                                }
                            }).ToArray(),
                            ResGuests = new []
                            {
                                new ResGuest
                                {
                                    Profiles = new Profiles
                                    {
                                        ProfileInfo = propertyDetails.Rooms
                                            .SelectMany(room => room.Passengers)
                                            .Select((pax, sel) => new Profile
                                        {
                                            RPH = sel + 1,
                                            Customer = new Customer
                                            {
                                                PersonName = new PersonName
                                                {
                                                    GivenName = pax.FirstName,
                                                    NamePrefix = pax.Title,
                                                    Surname = pax.LastName
                                                }
                                            }
                                        }).ToArray(),
                                    }
                                }
                            }
                        }
                    }
                };

                var xmlRequest = _serializer.Serialize(bookRequest);

                webRequest = new Request
                {
                    UserName = userId,
                    Password = password,
                    AuthenticationMode = AuthenticationMode.Basic,
                    EndPoint = url,
                    Method = RequestMethod.POST,
                    SoapAction = "",
                    SOAP = true,
                    Source = propertyDetails.Source,
                    ContentType = ContentTypes.Text_xml
                };
                webRequest.SetRequest(xmlRequest);
                await webRequest.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<Envelope<AtiBookResponse>>(webRequest.ResponseXML);
                var hotelReservations = response.Body.Content.HotelReservations;

                bookingReference = hotelReservations.Select(x => x.UniqueID.ID).First();

                bool failed = hotelReservations.Any(x => x.ResStatus != "CONFIRMED");

                if (string.IsNullOrEmpty(bookingReference) || failed)
                {
                    bookingReference = "failed";
                }

                if (response.Body.Content.OTA_PkgBookRS.PackageReservation.ItineraryItems
                    .Any(i => i.Accommodation.RoomProfiles
                        .Any(p => p.RoomAmenity == "Itinerary item is pending")))
                {
                    bookingReference = "failed";
                }
            }
            catch (Exception ex)
            {
                bookingReference = "failed";
                propertyDetails.Warnings.AddNew("BookException", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Book", webRequest);
            }

            return bookingReference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            //create xml for cancellation request
            var webRequest = new Request();
            var cancellationResponse = new ThirdPartyCancellationResponse();

            try
            {
                var requestXml = BuildCancellationRequest(propertyDetails);
                webRequest = new Request
                {
                    UserName = _settings.UserID(propertyDetails),
                    Password = _settings.Password(propertyDetails),
                    AuthenticationMode = AuthenticationMode.Basic,
                    EndPoint = _settings.URL(propertyDetails),
                    Method = RequestMethod.POST,
                    SoapAction = "",
                    SOAP = true,
                    Source = propertyDetails.Source,
                    LogFileName = "Cancellation",
                    ContentType = ContentTypes.Text_xml,
                    CreateLog = true
                };
                webRequest.SetRequest(requestXml);
                await webRequest.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<Envelope<AtiCancellationResponse>>(webRequest.ResponseXML);

                if (string.IsNullOrEmpty(response.Body.Content.Success))
                {
                    cancellationResponse.TPCancellationReference = response.Body.Content.UniqueID.ID;
                    cancellationResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("CancellationException", ex.ToString());
                cancellationResponse.Success = false;
            }
            finally
            {
                propertyDetails.AddLog("Cancellation", webRequest);
            }

            return cancellationResponse;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        public void EndSession(PropertyDetails propertyDetails) { }

        private async Task<Request> BuildAndSendSearchAsync(PropertyDetails propertyDetails)
        {
            string apiVersion = _settings.APIVersion(propertyDetails, false);
            string userID = _settings.UserID(propertyDetails);
            string password = _settings.Password(propertyDetails);
            string url = _settings.URL(propertyDetails);

            var criterionList = new List<Criterion>
            {
                new() { HotelRef = new HotelRef { HotelCode = propertyDetails.TPKey } }
            };

            var roomDetails = new iVector.Search.Property.RoomDetails();
            roomDetails.AddRange(propertyDetails.Rooms.Select(x => new RoomDetail
            {
                Adults = x.Adults,
                ChildAges = x.ChildAndInfantAges
            }));

            var requestXml = _serializer.Serialize(
                ATISearch.GetSearchRequestXml(
                    propertyDetails.ArrivalDate,
                    propertyDetails.Duration,
                    roomDetails,
                    criterionList,
                    apiVersion,
                    userID));

            var atiRequest = new Request
            {
                UserName = userID,
                Password = password,
                AuthenticationMode = AuthenticationMode.Basic,
                EndPoint = url,
                Method = RequestMethod.POST,
                LogFileName = "Hotel Availability",
                ContentType = ContentTypes.Text_xml,
                SOAP = true,
                UseGZip = true,
                Source = propertyDetails.Source,
                CreateLog = true
            };
            atiRequest.SetRequest(requestXml);
            await atiRequest.Send(_httpClient, _logger);

            return atiRequest;
        }

        private static List<Erratum> GetErrata(RoomStay roomStay)
        {
            return roomStay.RoomRates
                .SelectMany(room => room.Rates)
                .SelectMany(r => r.Fees)
                .Select(f =>
                {
                    f.Amount /= 100;
                    return f;
                })
                .GroupBy(fee => new
                {
                    fee.Code,
                    fee.CurrencyCode,
                    fee.Mandatory,
                    fee.Included
                })
                .Select(group => new
                {
                    group.Key.Code,
                    group.Key.CurrencyCode,
                    group.Key.Included,
                    group.Key.Mandatory,
                    Amount = group.Sum(fee => fee.Amount)
                })
                .Select(feeType =>
                {
                    var sb = new StringBuilder();
                    sb.Append(feeType.Mandatory ? "Mandatory " : "Optional ");
                    sb.Append(
                        $"{feeType.Code.ToLower()} fee of {feeType.CurrencyCode} {feeType.Amount}");
                    sb.Append(feeType.Included ? " included" : " not included");
                    return new Erratum("Fee", sb.ToString());
                })
                .ToList();
        }

        public XmlDocument BuildCancellationRequest(PropertyDetails propertyDetails)
        {
            return _serializer.Serialize(new Envelope<AtiCancellationRequest>
            {
                Body =
                {
                    Content = new AtiCancellationRequest
                    {
                        Version = _settings.APIVersion(propertyDetails, false),
                        UniqueID = new Models.Common.UniqueId { ID = propertyDetails.SourceReference }
                    }
                }
            });
        }
    }
}