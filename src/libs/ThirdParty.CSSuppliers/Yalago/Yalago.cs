namespace ThirdParty.CSSuppliers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.Models.Yalago;
    using ThirdParty.Interfaces;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Search.Models;

    public class Yalago : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IYalagoSettings _settings;
        private readonly ITPSupport _support;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Yalago> _logger;

        public string Source => ThirdParties.YALAGO;

        public bool SupportsRemarks => true;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            throw new NotImplementedException();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return 0;
        }

        #endregion

        #region Constructors

        public Yalago(IYalagoSettings settings, ITPSupport support, HttpClient httpClient, ILogger<Yalago> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        public bool SupportsBookingSearch => throw new NotImplementedException();

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var cancellationResponse = new ThirdPartyCancellationResponse();

            ThirdPartyCancellationFeeResult thirdPartyCancellationFeeResult = new ThirdPartyCancellationFeeResult();

            thirdPartyCancellationFeeResult = GetCancellationCost(propertyDetails);

            YalagoCancellationRequest.ExpectedCharge expectedCharge = new YalagoCancellationRequest.ExpectedCharge();
            YalagoCancellationRequest.Charge charge = new YalagoCancellationRequest.Charge()
            {
                Amount = thirdPartyCancellationFeeResult.Amount,
                Currency = thirdPartyCancellationFeeResult.CurrencyCode
            };

            expectedCharge.charge = charge;

            YalagoCancellationRequest cancelRequest = new YalagoCancellationRequest()
            {
                BookingRef = propertyDetails.SourceReference,
                expectedCharge = expectedCharge
            };

            string cancelRequestString = JsonConvert.SerializeObject(cancelRequest);

            try
            {
                IThirdPartyAttributeSearch searchDetails = new SearchDetails
                {
                    ThirdPartyConfigurations = propertyDetails.ThirdPartyConfigurations,
                };

                request = BuildRequest("Cancel", cancelRequestString, searchDetails, propertyDetails, _settings.CancelURL(searchDetails));

                request.Send(_httpClient, _logger).RunSynchronously();

                var response = JsonConvert.DeserializeObject<YalagoCancellationResponse>(request.ResponseString);
                if (thirdPartyCancellationFeeResult.Success && response.Status == "1")
                {
                    cancellationResponse.Success = response.Status == "1";
                    cancellationResponse.CurrencyCode = thirdPartyCancellationFeeResult.CurrencyCode;
                    cancellationResponse.Amount = thirdPartyCancellationFeeResult.Amount;
                    cancellationResponse.TPCancellationReference = response.BookingRef;
                }
                else
                {
                    cancellationResponse.Success = false;
                    cancellationResponse.TPCancellationReference = "failed";
                }
            }

            catch (Exception exception)
            {
                cancellationResponse.Success = false;

                cancellationResponse.TPCancellationReference = "failed";

                propertyDetails.Warnings.AddNew("Cancellation Exception", exception.ToString());
            }

            finally
            {
                if (request.EndPoint != "")
                {
                    propertyDetails.Logs.AddNew(ThirdParties.YALAGO, "Yalago Cancellation Request", request.RequestString);
                }

                if (request.ResponseString != "")
                {
                    propertyDetails.Logs.AddNew(ThirdParties.YALAGO, "Yalago Cancellation Response", request.ResponseString);
                }
            }

            return cancellationResponse;
        }

        public string CreateReconciliationReference(string inputReference)
        {
            throw new NotImplementedException();
        }

        public void EndSession(PropertyDetails propertyDetails)
        {

        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails)
        {
            var request = new Request();
            YalagoPreCancelRequest preCancelRequest = new YalagoPreCancelRequest()
            {
                BookingRef = propertyDetails.SourceReference,
                GetTaxBreakdown = false
            };
            string preCancelRequestString = JsonConvert.SerializeObject(preCancelRequest);
            var cancellationCostResponse = new ThirdPartyCancellationFeeResult();
            var response = new YalagoPreCancelResponse();
            try
            {
                IThirdPartyAttributeSearch searchDetails = new SearchDetails
                {
                    ThirdPartyConfigurations = propertyDetails.ThirdPartyConfigurations,
                };
                request = BuildRequest("PreCancel", preCancelRequestString, searchDetails, propertyDetails, _settings.PreCancelURL(searchDetails));
                request.Send(_httpClient, _logger).RunSynchronously();

                response = JsonConvert.DeserializeObject<YalagoPreCancelResponse>(request.ResponseString);

                if (response.IsCancellable)
                {
                    cancellationCostResponse.Amount = response.charge.charge.Amount;
                    cancellationCostResponse.CurrencyCode = response.cancellationPolicyStatic[0].cancellationCharges[0].charge.Currency;
                    cancellationCostResponse.Success = response.IsCancellable;
                }
                else
                {
                    cancellationCostResponse.Success = response.IsCancellable;
                }
            }
            catch (Exception exception)
            {
                cancellationCostResponse.Success = false;

                propertyDetails.Warnings.AddNew("GetCancellationCost Exception", exception.ToString());
            }
            finally
            {
                if (request.EndPoint != "")
                {
                    propertyDetails.Logs.AddNew(ThirdParties.YALAGO, "Yalago GetCancellationCost Request", request.RequestString);
                }

                if (request.ResponseString != "")
                {
                    propertyDetails.Logs.AddNew(ThirdParties.YALAGO, "Yalago GetCancellationCost Response", request.ResponseString);
                }
            }

            return cancellationCostResponse;
        }

        private Request BuildRequest(string requestType, string requestString, IThirdPartyAttributeSearch searchDetails, PropertyDetails propertyDetails, string url)
        {
            var request = new Request
            {
                Source = ThirdParties.YALAGO,
                Method = eRequestMethod.POST,
                EndPoint = url,
                ContentType = "application/json",
                UseGZip = _settings.UseGZip(searchDetails),
                CreateLog = propertyDetails.CreateLogs,
                LogFileName = requestType,
                Accept = "application/gzip",
                TimeoutInSeconds = 100,
                KeepAlive = true
            };

            request.SetRequest(requestString);

            request.Headers.Add("X-Api-Key", _settings.API_Key(searchDetails));

            return request;
        }

        public bool PreBook(PropertyDetails propertyDetails)
        {
            bool prebookSuccess = true;
            var response = new YalagoPreBookResponse();
            var request = new Request();
            try
            {
                IThirdPartyAttributeSearch searchDetails = new SearchDetails
                {
                    ThirdPartyConfigurations = propertyDetails.ThirdPartyConfigurations,
                };

                string sourceMarket = _support.TPCountryCodeLookup(ThirdParties.YALAGO, propertyDetails.SellingCountry);
                if (string.IsNullOrEmpty(sourceMarket))
                {
                    sourceMarket = _settings.CountryCode(searchDetails);
                }

                var opaqueSearch = SafeTypeExtensions.ToSafeBoolean(propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[4]);
                var getPackagePrice = opaqueSearch && _settings.ReturnOpaqueRates(propertyDetails) && propertyDetails.OpaqueRates;

                YalagoPreBookRequest preBookRequest = new YalagoPreBookRequest()
                {
                    Culture = _settings.Language(searchDetails),
                    CheckInDate = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                    CheckOutDate = propertyDetails.DepartureDate.ToString("yyyy-MM-dd"),
                    LocationId = SafeTypeExtensions.ToSafeInt(propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[2]),
                    EstablishmentId = SafeTypeExtensions.ToSafeInt(propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[0]),
                    SourceMarket = sourceMarket,
                    GetLocalCharges = true,
                    GetPackagePrice = getPackagePrice
                };

                List<YalagoPreBookRequest.Room> rooms = new List<YalagoPreBookRequest.Room>();

                foreach (RoomDetails roomDetails in propertyDetails.Rooms)
                {
                    YalagoPreBookRequest.Room room = new YalagoPreBookRequest.Room()
                    {
                        BoardCode = roomDetails.ThirdPartyReference.Split('|')[1],
                        RoomCode = roomDetails.ThirdPartyReference.Split('|')[3],
                        Adults = roomDetails.Adults,
                        ChildAges = roomDetails.ChildAndInfantAges.ToArray()
                    };

                    rooms.Add(room);
                }

                preBookRequest.Rooms = rooms.ToArray();

                string requestString = JsonConvert.SerializeObject(preBookRequest);

                request = BuildRequest("PreBook", requestString, searchDetails, propertyDetails, _settings.PreBookURL(searchDetails));

                request.Send(_httpClient, _logger).RunSynchronously();

                response = JsonConvert.DeserializeObject<YalagoPreBookResponse>(request.ResponseString);

                var processedRooms = new List<int>();

                if (response?.establishment?.Rooms is null || !response.establishment.Rooms.Any())
                {
                    propertyDetails.Warnings.AddNew("Prebook Response Error", "No room details found on prebook response");
                    return false;
                }

                foreach (YalagoPreBookResponse.Room room in response.establishment.Rooms.Where(r => r?.Boards is object))
                {
                    foreach (YalagoPreBookResponse.Board board in room.Boards.Where(b => b is object))
                    {

                        foreach (RoomDetails roomDetails in propertyDetails.Rooms.Where(o =>
                                                                                     !processedRooms.Contains(o.PropertyRoomBookingID) &&
                                                                                     propertyDetails.Rooms.IndexOf(o) == SafeTypeExtensions.ToSafeInt(board.RequestedRoomIndex) &&
                                                                                     SafeTypeExtensions.ToSafeString(o.RoomType).ToLower() == SafeTypeExtensions.ToSafeString(room.Description).ToLower() &&
                                                                                     o.ThirdPartyReference.Split('|')[3].ToLower() == room.Code.ToLower() &&
                                                                                     o.ThirdPartyReference.Split('|')[5] == SafeTypeExtensions.ToSafeString(board.Type)))

                        {
                            foreach (YalagoLocalCharge localCharge in board.LocalCharges)
                            {
                                propertyDetails.Errata.Add(new Erratum("Local Charge", localCharge.Amount.Currency.ToString() + localCharge.Amount.Amount.ToString()));

                            }

                            roomDetails.LocalCost = SafeTypeExtensions.ToSafeDecimal(board.netCost.Amount);
                            roomDetails.GrossCost = SafeTypeExtensions.ToSafeDecimal(board.netCost.Amount);


                            if (response.InfoItems != null)
                            {
                                var importantInfo = new HashSet<string>(
                                    response.InfoItems
                                        .Where(i => !string.IsNullOrWhiteSpace(i?.Description))
                                        .Select(i => i.Description));

                                propertyDetails.Errata.AddRange(importantInfo.Select(description =>
                                    new Erratum("ImportantInformation", description)));
                            }

                            var orderedCancellations = board.cancellationPolicy.CancellationCharges.OrderByDescending(o => o.ExpiryDate.Date).ToList();
                            for (var i = 0; i < orderedCancellations.Count; i++)
                            {
                                var cancellationCharge = orderedCancellations[i];
                                var startDate = (i + 1) < orderedCancellations.Count ?
                                    orderedCancellations[i + 1].ExpiryDate.Date.AddDays(1) : DateTime.Now.Date;
                                propertyDetails.Cancellations.AddNew(startDate, cancellationCharge.ExpiryDate.Date, SafeTypeExtensions.ToSafeDecimal(cancellationCharge.charge.Amount));
                            }

                            processedRooms.Add(roomDetails.PropertyRoomBookingID);

                        }
                    }
                }

                propertyDetails.Cancellations.Solidify(SolidifyType.Sum);

                prebookSuccess = response.establishment != null;
            }

            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                prebookSuccess = false;
            }

            finally
            {
                if (request.RequestString != "")
                {
                    propertyDetails.Logs.AddNew(ThirdParties.YALAGO, "Yalago PreBook Request", request.RequestString);
                }

                if (request.ResponseString != "")
                {
                    propertyDetails.Logs.AddNew(ThirdParties.YALAGO, "Yalago PreBook Response", request.ResponseString);
                }


            }


            return prebookSuccess;
        }

        public string Book(PropertyDetails propertyDetails)
        {
            string reference = string.Empty;
            bool opaqueSearch = false;
            int i = 1;
            var bookingRequest = new Request();

            try
            {
                var totalRooms = propertyDetails.Rooms.Count;
                List<YalagoCreateBookingRequest.Room> roomsList = new List<YalagoCreateBookingRequest.Room>();
                foreach (RoomDetails room in propertyDetails.Rooms)
                {
                    List<YalagoCreateBookingRequest.Guest> guestList = new List<YalagoCreateBookingRequest.Guest>();

                    foreach (Passenger passenger in room.Passengers)
                    {
                        if (passenger.Age == 0 && passenger.PassengerType != PassengerType.Infant)
                        {
                            passenger.Age = 25;
                        }

                        YalagoCreateBookingRequest.Guest guest = new YalagoCreateBookingRequest.Guest()
                        {
                            Age = passenger.Age,
                            FirstName = passenger.FirstName,
                            LastName = passenger.LastName,
                            Title = passenger.Title
                        };
                        guestList.Add(guest);
                    }
                    YalagoCreateBookingRequest.ExpectedNetCost expectedNetCost = new YalagoCreateBookingRequest.ExpectedNetCost
                    {
                        Amount = Math.Round(SafeTypeExtensions.ToSafeDecimal(room.GrossCost), 2),
                        Currency = propertyDetails.CurrencyCode
                    };

                    YalagoCreateBookingRequest.Room requestRoom = new YalagoCreateBookingRequest.Room()
                    {
                        Guests = guestList.ToArray(),
                        RoomCode = room.ThirdPartyReference.Split('|')[3],
                        BoardCode = room.ThirdPartyReference.Split('|')[1],
                        AffiliateRoomRef = SafeTypeExtensions.ToSafeString("room" + i++),
                        SpecialRequests = SafeTypeExtensions.ToSafeString(propertyDetails.BookingComments),
                        ExpectedNetCost = expectedNetCost
                    };

                    roomsList.Add(requestRoom);

                    opaqueSearch = SafeTypeExtensions.ToSafeBoolean(room.ThirdPartyReference.Split('|')[4]);

                }

                var getPackagePrice = opaqueSearch && _settings.ReturnOpaqueRates(propertyDetails) && propertyDetails.OpaqueRates; ;

                YalagoCreateBookingRequest.ContactDetails contactDetails = new YalagoCreateBookingRequest.ContactDetails()
                {
                    Title = propertyDetails.LeadGuestTitle,
                    Address1 = propertyDetails.LeadGuestAddress1,
                    Address2 = propertyDetails.LeadGuestAddress2,
                    FirstName = propertyDetails.LeadGuestFirstName,
                    LastName = propertyDetails.LeadGuestLastName,
                    PostCode = propertyDetails.LeadGuestPostcode
                };

                string sourceMarket;
                if (string.IsNullOrEmpty(_support.TPCountryCodeLookup(ThirdParties.YALAGO, propertyDetails.SellingCountry)))
                {
                    sourceMarket = _settings.CountryCode(propertyDetails);
                }
                else
                {
                    sourceMarket = _support.TPCountryCodeLookup(ThirdParties.YALAGO, propertyDetails.SellingCountry);
                }
                var request = new YalagoCreateBookingRequest()
                {
                    AffiliateRef = propertyDetails.BookingReference,
                    CheckInDate = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                    CheckOutDate = propertyDetails.DepartureDate.ToString("yyyy-MM-dd"),
                    EstablishmentId = SafeTypeExtensions.ToSafeInt(propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[0]),
                    Culture = _settings.Language(propertyDetails),
                    GetPackagePrice = getPackagePrice,
                    GetTaxBreakdown = true,
                    GetLocalCharges = true,
                    Rooms = roomsList.ToArray(),
                    contactDetails = contactDetails,
                    SourceMarket = sourceMarket
                };

                string requestString = JsonConvert.SerializeObject(request);

                IThirdPartyAttributeSearch searchDetails = new SearchDetails
                {
                    ThirdPartyConfigurations = propertyDetails.ThirdPartyConfigurations,
                };

                bookingRequest = BuildRequest("Book", requestString, searchDetails, propertyDetails, _settings.BookingURL(searchDetails));

                bookingRequest.Send(_httpClient, _logger).RunSynchronously();

                YalagoCreateBookingResponse response = new YalagoCreateBookingResponse();

                response = JsonConvert.DeserializeObject<YalagoCreateBookingResponse>(bookingRequest.ResponseString);

                if (response != null && response.Bookings != null && !string.IsNullOrEmpty(response.BookingRef))
                {
                    reference = response.BookingRef;
                }
                else
                {
                    reference = "failed";
                }

            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("Book Exception", exception.ToString());

                reference = "failed";
            }
            finally
            {
                if (bookingRequest.RequestString != "")
                {
                    propertyDetails.Logs.AddNew(ThirdParties.YALAGO, "Yalago Book Request", bookingRequest.RequestString);
                }

                if (bookingRequest.ResponseString != "")
                {
                    propertyDetails.Logs.AddNew(ThirdParties.YALAGO, "Yalago Book Response", bookingRequest.ResponseString);
                }
            }

            return reference;
        }
    }
}