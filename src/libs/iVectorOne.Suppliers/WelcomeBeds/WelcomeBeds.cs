namespace iVectorOne.Suppliers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.Models.WelcomeBeds;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;

    public class WelcomeBeds : IThirdParty, ISingleSource
    {
        #region Properties

        public readonly IWelcomeBedsSettings _settings;
        public readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<WelcomeBeds> _logger;

        public string Source => ThirdParties.WELCOMEBEDS;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        #endregion

        #region Constructors

        public WelcomeBeds(
            IWelcomeBedsSettings settings,
            ITPSupport support,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<WelcomeBeds> logger)
        {
            _settings = settings;
            _support = support;
            _serializer = serializer;
            _httpClient = httpClient;
            _logger = logger;
        }

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            string reservationRequest = string.Empty;
            string reservationResponse = string.Empty;
            var webRequest = new Request();

            try
            {
                var roomOptions = propertyDetails.Rooms.Select(rd =>
                {
                    string[] splits = rd.ThirdPartyReference.Split('|');
                    return new
                    {
                        Token = splits[0],
                        RoomTypeCode = splits[1],
                        RatePlanCode = splits[2],
                        PromotionCode = splits[3],
                        CurrencyCode = splits[4],
                        LocalCost = rd.LocalCost,
                        Adults = Enumerable.Range(0, rd.Adults).Select(_ => new GuestCount { Count = 1, Age = 30 }),
                        Children = rd.ChildAges.Select(age => new GuestCount { Count = 1, Age = age }),
                        Infants = Enumerable.Range(0, rd.Infants).Select(_ => new GuestCount { Count = 1, Age = 1 })
                    };
                });

                var roomStay = new RoomStay
                {
                    RoomTypes = roomOptions.Select(rd => rd.RoomTypeCode).Distinct().Select(rtc => new RoomType { RoomTypeCode = rtc }).ToList(),
                    RatePlans = roomOptions.Select(rd => rd.RatePlanCode).Distinct().Select(rpc => new RatePlan { RatePlanCode = rpc }).ToList(),
                    RoomRates = roomOptions.Select(ro => new RoomRate
                    {
                        RoomTypeCode = ro.RoomTypeCode,
                        RatePlanCode = ro.RatePlanCode,
                        InvBlockCode = "1",
                        PromotionCode = ro.PromotionCode,
                        AvailabilityStatus = Constant.AvailableForSale,
                        Rates = new List<Rate>
                        {
                            new Rate
                            {
                                Total = new RateTotal
                                {
                                    AmountAfterTax = ro.LocalCost.ToString(),
                                    CurrencyCode = ro.CurrencyCode
                                },
                                TpaExtensions = new TpaExtensions
                                {
                                    RoomToken = new RoomToken { Token = ro.Token }
                                }
                            }
                        },
                        GuestCounts = ro.Adults.Concat(ro.Children).Concat(ro.Infants).ToList()
                    }).ToList(),
                    TimeSpan = new StayTimeSpan
                    {
                        Start = propertyDetails.ArrivalDate.ToString(Constant.DateFormat),
                        End = propertyDetails.DepartureDate.ToString(Constant.DateFormat)
                    },
                    BasicPropertyInfo = new BasicPropertyInfo
                    {
                        HotelCode = propertyDetails.TPKey
                    }
                };

                string[] resort = propertyDetails.ResortCode.Split('|');

                var reservationTpaExtensions = new TpaExtensions
                {
                    Providers = new List<Provider>
                    {
                        new Provider
                        {
                            Name = Constant.ProviderName,
                            Credentials = WelcomeBedsSearch.BuildCredentials(propertyDetails, _settings),
                            ProviderAreas = new List<Area>
                            {
                                new Area { TypeCode = "Country", AreaCode=resort[0] },
                                new Area { TypeCode = "Province", AreaCode=resort[1] },
                                new Area { TypeCode = "Town", AreaCode=resort[2] },
                            }
                        }
                    },
                    ProviderID = new ProviderID { Provider = Constant.ProviderName }
                };

                var preBookRequest = new OtaHotelResRq
                {
                    ResStatus = Constant.PreBookResStatus,
                    Version = _settings.Version(propertyDetails),
                    HotelReservations = new List<HotelReservation>
                    {
                        new HotelReservation
                        {
                            RoomStays = new List<RoomStay> { roomStay },
                            TpaExtensions = reservationTpaExtensions
                        }
                    }
                };

                var xmlReservationRequest = Envelope<OtaHotelResRq>.Serialize(preBookRequest, _serializer);
                reservationRequest = xmlReservationRequest.ToString();

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    SoapAction = "HotelRes",
                    Method = RequestMethod.POST,
                    ContentType = "text/xml",
                    LogFileName = "Prebook",
                    CreateLog = true,
                    Source = Source,
                };

                webRequest.SetRequest(xmlReservationRequest);
                await webRequest.Send(_httpClient, _logger);

                var soapEnvelopeXml = webRequest.ResponseXML;
                var response = Envelope<OtaHotelResRs>.DeSerialize(soapEnvelopeXml, _serializer);

                // Check to see if there were any errors
                if (response.Errors.Any()) return false;

                decimal totalCost = response.HotelReservations.First().RoomStays.First().Total.AmountAfterTax.ToSafeDecimal();

                // Check for price changes
                if (propertyDetails.LocalCost != totalCost)
                {
                    // Calculate the cost per room based on the total price
                    decimal roomPrice = totalCost / propertyDetails.Rooms.Count;
                    foreach (var roomDetail in propertyDetails.Rooms)
                    {
                        roomDetail.LocalCost = roomPrice;
                        roomDetail.GrossCost = roomPrice;
                    }
                }

                // Get the booking reference out
                string reference = response.HotelReservations?.FirstOrDefault().ResGlobalInfo?.HotelReservationIds.FirstOrDefault()?.ResIdValue ?? "";
                if (!string.IsNullOrEmpty(reference))
                {
                    propertyDetails.TPRef1 = reference;
                }

                var cancellations = new Cancellations();
                // Set up the cancellation charges
                cancellations.AddRange(response.HotelReservations.First().RoomStays.First().RoomRates.First().Rates.First().CancelPolicies
                    .Select(cp =>
                    {
                        var startDt = cp.Start.ToSafeDate();
                        var start = new DateTime(startDt.Year, startDt.Month, startDt.Day);
                        var endDt = cp.End.ToSafeDate();
                        var end = new DateTime(endDt.Year, endDt.Month, endDt.Day + 1);
                        decimal amount = SafeTypeExtensions.ToSafeDecimal(cp.AmountPercent.Amount);

                        return new Cancellation
                        {
                            Amount = amount,
                            EndDate = end,
                            StartDate = start
                        };
                    }));

                // Merge the cancellations together so the prices add up nicely
                cancellations.Solidify(SolidifyType.Sum);

                // Put the cancellations on the booking
                propertyDetails.Cancellations = cancellations;

                // Picking up the errata            
                propertyDetails.Errata.AddRange(response.HotelReservations.First().RoomStays.First()
                    .RoomRates.First().Descriptions.Select(d => new Erratum
                    {
                        Title = "Room Rate Description",
                        Text = d.Text
                    }));

            }
            catch (Exception e)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", e.Message);
                return false;
            }
            finally
            {
                propertyDetails.AddLog("PreBook", webRequest);
            }

            return true;
        }

        #endregion

        #region Booking

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            int guestCount = 0;
            var resGuests = new List<ResGuest>();
            string reference = string.Empty;
            string reservationRequest = string.Empty;
            string reservationResponse = string.Empty;
            var webRequest = new Request();

            try
            {
                foreach (var room in propertyDetails.Rooms)
                {
                    foreach (var passenger in room.Passengers)
                    {
                        guestCount++;

                        string guestAge = Constant.AdultConstAge;
                        guestAge = passenger.PassengerType switch
                        {
                            PassengerType.Child => passenger.Age.ToString(),
                            PassengerType.Infant => Constant.InfantConstAge,
                            _ => Constant.AdultConstAge,
                        };
                        var resGuest = new ResGuest
                        {
                            Age = guestAge,
                            Profiles = new List<ProfileInfo>
                            {
                                new ProfileInfo { Profile = new Profile { Customer = new Customer
                                {
                                    PersonName = new PersonName
                                    {
                                        GivenName = string.Equals(passenger.FirstName, Constant.TBA)? $"{Constant.TBA}{guestCount}" : passenger.FirstName,
                                        Surname = string.Equals(passenger.LastName, Constant.TBA)? $"{Constant.TBA}{guestCount}" : passenger.LastName
                                    },
                                    TpaExtensions = new TpaExtensions
                                    {
                                        ProviderTokens = new List<Token>
                                        {
                                            new Token
                                            {
                                                TokenName = Constant.PaxType,
                                                TokenCode = (passenger.PassengerType == PassengerType.Adult ||
                                                    (passenger.PassengerType == PassengerType.Child && passenger.Age >= 12))
                                                    ? Constant.ADULT
                                                    : Constant.CHILD
                                            }
                                        }
                                    }
                                }}}
                            }
                        };
                        resGuests.Add(resGuest);
                    }
                }

                var reservation = new HotelReservation
                {
                    ResGuests = resGuests,
                    ResGlobalInfo = new ResGlobalInfo
                    {
                        HotelReservationIds = new List<HotelReservationId>
                        {
                            new() { ResIdValue = propertyDetails.TPRef1 }
                        },
                        Comments = propertyDetails.BookingComments.Count > 0 
                            ? propertyDetails.BookingComments.Select(x => new Comment { Text = x.Text }).ToList() 
                            : new List<Comment>()
                    },
                    TpaExtensions = new TpaExtensions
                    {
                        Providers = new List<Provider>
                        {
                            new Provider
                            {
                                Name = Constant.ProviderName,
                                Credentials = WelcomeBedsSearch.BuildCredentials(propertyDetails, _settings)
                            }
                        },
                        ProviderTokens = new List<Token>
                        {
                            new Token { TokenCode = propertyDetails.BookingReference, TokenName = "RecordId" },
                            new Token { TokenCode = _settings.Agency(propertyDetails), TokenName = "TravelAgentName" }
                        },
                        ProviderID = new ProviderID { Provider = Constant.ProviderName }
                    }
                };

                var bookRequest = new OtaHotelResRq
                {
                    ResStatus = Constant.BookResStatus,
                    Version = _settings.Version(propertyDetails),
                    HotelReservations = new List<HotelReservation> { reservation }
                };

                var xmlReservationRequest = Envelope<OtaHotelResRq>.Serialize(bookRequest, _serializer);
                reservationRequest = xmlReservationRequest.ToString();

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    SoapAction = "HotelRes",
                    Method = RequestMethod.POST,
                    ContentType = "text/xml",
                    CreateLog = true,
                    LogFileName = "Book",
                    Source = Source,
                };

                webRequest.SetRequest(xmlReservationRequest);

                // Send the request
                await webRequest.Send(_httpClient, _logger);

                reservationResponse = webRequest.ResponseString;

                var response = Envelope<OtaHotelResRs>.DeSerialize(webRequest.ResponseXML, _serializer);

                // Check for a successful response
                if (response.Errors.Any())
                {
                    throw new Exception("The request was not returned successfully");
                }

                // Check the status
                string resStatusCode = response.HotelReservations.FirstOrDefault()?.TpaExtensions
                    .ProviderTokens.FirstOrDefault(t => t.TokenName == "ResStatusCode")?.TokenCode ?? string.Empty;

                if (!string.Equals(resStatusCode, "CA") && !string.Equals(resStatusCode, "SC"))
                {
                    throw new Exception("The booking was not confirmed");
                }

                // Get the booking reference
                reference = response.HotelReservations.FirstOrDefault()?.ResGlobalInfo?.HotelReservationIds.FirstOrDefault()?.ResIdValue ?? "failed";

            }
            catch (Exception e)
            {
                propertyDetails.Warnings.AddNew("Book Exception", e.Message);
                reference = "failed";
            }
            finally
            {
                propertyDetails.AddLog("Book", webRequest);
            }

            return reference;
        }


        #endregion

        #region Cancelation

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            ThirdPartyCancellationResponse tpCancellationResponse = new();
            var webRequest = new Request();

            try
            {
                var cancelationRequest = new OtaCancelRq
                {
                    CancelType = Constant.CancelType,
                    Version = _settings.Version(propertyDetails),
                    Pos = new Pos
                    {
                        Source = new PosSource
                        {
                            BookingChannel = new BookingChannel { ChannelType = Constant.BookingChannelTVP }
                        }
                    },
                    UniqueId = new UniqueId { Id = propertyDetails.SourceReference },
                    TpaExtensions = new TpaExtensions
                    {
                        Providers = new List<Provider>
                        {
                            new Provider
                            {
                                Name = Constant.ProviderName,
                                Credentials = WelcomeBedsSearch.BuildCredentials(propertyDetails, _settings)
                            }
                        },
                        ProviderID = new ProviderID { Provider = Constant.ProviderName }
                    }
                };

                var xmlCancelationRequest = Envelope<OtaCancelRq>.Serialize(cancelationRequest, _serializer);

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    SoapAction = "HotelCancel",
                    Method = RequestMethod.POST,
                    ContentType = "text/xml",
                    LogFileName = "Cancel",
                    CreateLog = true,
                };

                webRequest.SetRequest(xmlCancelationRequest);
                await webRequest.Send(_httpClient, _logger);

                var cancelResponse = Envelope<OtaCancelRs>.DeSerialize(webRequest.ResponseXML, _serializer);

                if (!cancelResponse.IsSuccess)
                {
                    throw new Exception("The request was not returned successfully");
                }

                if (!string.IsNullOrEmpty(cancelResponse.CancelInfoRs.UniqueId.Id))
                {
                    tpCancellationResponse.Success = true;
                    tpCancellationResponse.TPCancellationReference = cancelResponse.CancelInfoRs.UniqueId.Id;
                }
                else
                {
                    throw new Exception("No cancellation reference");
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.Message);
                tpCancellationResponse.Success = false;
                tpCancellationResponse.TPCancellationReference = string.Empty;
            }
            finally
            {
                propertyDetails.AddLog("Cancellation", webRequest);
                }

            return tpCancellationResponse;
        }

        #endregion

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        public void EndSession(PropertyDetails propertyDetails) { }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }
    }
}
