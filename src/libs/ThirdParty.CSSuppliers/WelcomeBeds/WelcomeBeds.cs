namespace ThirdParty.CSSuppliers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.CSSuppliers.Models.WelcomeBeds;
    using Microsoft.Extensions.Logging;

    public class WelcomeBeds : IThirdParty
    {
        #region "Properties"

        public readonly IWelcomeBedsSettings _settings;
        public readonly ITPSupport _tpSupport;
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

        public bool TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails)
        {
            return false;
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        #endregion

        #region "Constructors"

        public WelcomeBeds(
            IWelcomeBedsSettings settings,
            ITPSupport tpSupport,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<WelcomeBeds> logger)
        {
            _settings = settings;
            _tpSupport = tpSupport;
            _serializer = serializer;
            _httpClient = httpClient;
            _logger = logger;
        }

        #endregion

        #region PreBook

        public bool PreBook(PropertyDetails propertyDetails)
        {
            string sReservationRequest = string.Empty;
            string sReservationResponse = string.Empty;

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
                        new Rate {
                            Total = new RateTotal
                            {
                                AmountAfterTax = ro.LocalCost.ToString(),  //decimal to string                           
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

                string[] sResort = _tpSupport.TPResortCodeByGeographyIdLookup(Source, propertyDetails.GeographyLevel3ID).Split('|');

                var reservationTpaExtensions = new TpaExtensions
                {
                    Providers = new List<Provider> {
                    new Provider {
                        Name = Constant.ProviderName,
                        Credentials = WelcomeBedsSearch.BuildCredentials(propertyDetails, _settings),
                        ProviderAreas = new List<Area>
                        {
                            new Area { TypeCode = "Country", AreaCode=sResort[0] },
                            new Area { TypeCode = "Province", AreaCode=sResort[1] },
                            new Area { TypeCode = "Town", AreaCode=sResort[2] },
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
                sReservationRequest = xmlReservationRequest.ToString();

                var oReservationRequest = new Request
                {
                    EndPoint = _settings.URL(propertyDetails),
                    SoapAction = "HotelRes",
                    Method = eRequestMethod.POST,
                    ContentType = "text/xml",
                    LogFileName = "Prebook",
                    CreateLog = true,
                    Source = Source,
                };

                oReservationRequest.SetRequest(xmlReservationRequest);
                oReservationRequest.Send(_httpClient, _logger).RunSynchronously();

                var soapEnvelopeXml = oReservationRequest.ResponseXML;
                var oResponse = Envelope<OtaHotelResRs>.DeSerialize(soapEnvelopeXml, _serializer);

                // Check to see if there were any errors
                if (oResponse.Errors.Any()) return false;

                decimal totalCost = SafeTypeExtensions.ToSafeDecimal(oResponse.HotelReservations.First().RoomStays.First().Total.AmountAfterTax);

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
                string reference = oResponse.HotelReservations?.FirstOrDefault().ResGlobalInfo?.HotelReservationIds.FirstOrDefault()?.ResIdValue ?? "";
                if (!string.IsNullOrEmpty(reference))
                {
                    propertyDetails.TPRef1 = reference;
                }


                var cancellations = new Cancellations();
                // Set up the cancellation charges
                cancellations.AddRange(oResponse.HotelReservations.First().RoomStays.First().RoomRates.First().Rates.First().CancelPolicies
                    .Select(cp =>
                    {
                        var startDt = SafeTypeExtensions.ToSafeDate(cp.Start);
                        var start = new DateTime(startDt.Year, startDt.Month, startDt.Day);
                        var endDt = SafeTypeExtensions.ToSafeDate(cp.End);
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
                propertyDetails.Errata.AddRange(oResponse.HotelReservations.First().RoomStays.First()
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
                if (!string.IsNullOrEmpty(sReservationRequest))
                {
                    propertyDetails.Logs.AddNew(Source, "WelcomeBeds PreBook Request", sReservationRequest);
                }

                if (!string.IsNullOrEmpty(sReservationResponse))
                {
                    propertyDetails.Logs.AddNew(Source, "WelcomeBeds PreBook Response", sReservationResponse);
                }
            }

            return true;
        }

        #endregion

        #region "Booking

        public string Book(PropertyDetails propertyDetails)
        {
            int guestCount = 0;
            var resGuests = new List<ResGuest>();
            string sReference = string.Empty;
            string sReservationRequest = string.Empty;
            string sReservationResponse = string.Empty;

            try
            {

                foreach (var r in propertyDetails.Rooms)
                {
                    foreach (var p in r.Passengers)
                    {
                        guestCount++;

                        string guestAge = Constant.AdultConstAge;
                        switch (p.PassengerType)
                        {
                            case PassengerType.Child:
                                guestAge = p.Age.ToString();
                                break;
                            case PassengerType.Infant:
                                guestAge = Constant.InfantConstAge;
                                break;
                            case PassengerType.Adult:
                            default:
                                guestAge = Constant.AdultConstAge;
                                break;
                        }

                        var resGuest = new ResGuest
                        {
                            Age = guestAge,
                            Profiles = new List<ProfileInfo>
                        {
                            new ProfileInfo { Profile = new Profile { Customer = new Customer
                            {
                                PersonName = new PersonName
                                {
                                    GivenName = string.Equals(p.FirstName, Constant.TBA)? $"{Constant.TBA}{guestCount}" : p.FirstName,
                                    Surname = string.Equals(p.LastName, Constant.TBA)? $"{Constant.TBA}{guestCount}" : p.LastName
                                },
                                TpaExtensions = new TpaExtensions
                                {
                                    ProviderTokens = new List<Token>
                                    {
                                        new Token
                                        {
                                            TokenCode = Constant.PaxType,
                                            TokenName = (p.PassengerType == PassengerType.Adult ||
                                                (p.PassengerType == PassengerType.Child && p.Age >= 12))
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
                        Comments = propertyDetails.BookingComments.Select(bc => new Comment { Text = bc.Text }).ToList()
                    },
                    TpaExtensions = new TpaExtensions
                    {
                        Providers = new List<Provider> {
                        new Provider
                        {
                            Name = Constant.ProviderName,
                            Credentials = WelcomeBedsSearch.BuildCredentials(propertyDetails, _settings)
                        }
                    },
                        ProviderTokens = new List<Token>
                    {
                        new Token { TokenCode = propertyDetails.BookingReference, TokenName = "RecordId" },
                        new Token { TokenCode = _settings.AgencyName(propertyDetails), TokenName = "TravelAgentName" }
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
                sReservationRequest = xmlReservationRequest.ToString();

                var oReservationRequest = new Request
                {
                    EndPoint = _settings.URL(propertyDetails),
                    SoapAction = "HotelRes",
                    Method = eRequestMethod.POST,
                    ContentType = "text/xml",
                    CreateLog = true,
                    LogFileName = "Book",
                    Source = Source,
                };

                oReservationRequest.SetRequest(xmlReservationRequest);

                // Send the request
                oReservationRequest.Send(_httpClient, _logger).RunSynchronously();

                sReservationResponse = oReservationRequest.ResponseString;

                var oResponse = Envelope<OtaHotelResRs>.DeSerialize(oReservationRequest.ResponseXML, _serializer);

                // Check for a successful response
                if (oResponse.Errors.Any())
                {
                    throw new Exception("The request was not returned successfully");
                }

                // Check the status
                string sResStatusCode = oResponse.HotelReservations.FirstOrDefault()?.TpaExtensions
                    .ProviderTokens.FirstOrDefault(t => t.TokenName == "ResStatusCode")?.TokenCode;

                if (!string.Equals(sResStatusCode, "CA") && !string.Equals(sResStatusCode, "SC"))
                {
                    throw new Exception("The booking was not confirmed");
                }

                // Get the booking reference
                sReference = oResponse.HotelReservations.FirstOrDefault()?.ResGlobalInfo?.HotelReservationIds.FirstOrDefault()?.ResIdValue ?? "failed";

            }
            catch (Exception e)
            {
                propertyDetails.Warnings.AddNew("Book Exception", e.Message);
                sReference = "failed";
            }
            finally
            {
                if (!string.IsNullOrEmpty(sReservationRequest))
                {
                    propertyDetails.Logs.AddNew(Source, "WelcomeBeds Book Request", sReservationRequest);
                }

                if (!string.IsNullOrEmpty(sReservationResponse))
                {
                    propertyDetails.Logs.AddNew(Source, "WelcomeBeds Book Response", sReservationResponse);
                }
            }

            return sReference;
        }


        #endregion

        #region "Cancelation"

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails)
        {
            ThirdPartyCancellationResponse oTPCancellationResponse = new();
            string sCancellationRequest = string.Empty;
            string sCancellationResponse = string.Empty;

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
                        Providers = new List<Provider> {
                            new Provider{
                                Name = Constant.ProviderName,
                                Credentials = WelcomeBedsSearch.BuildCredentials(propertyDetails, _settings)
                            }
                        },
                        ProviderID = new ProviderID { Provider = Constant.ProviderName }
                    }
                };

                var sCancelationRequest = Envelope<OtaCancelRq>.Serialize(cancelationRequest, _serializer);

                var oCancellationRequest = new Request
                {
                    EndPoint = _settings.URL(propertyDetails),
                    SoapAction = "HotelCancel",
                    Method = eRequestMethod.POST,
                    ContentType = "text/xml",
                    LogFileName = "Cancel",
                    CreateLog = true,
                };

                oCancellationRequest.SetRequest(sCancelationRequest);
                oCancellationRequest.Send(_httpClient, _logger).RunSynchronously();

                sCancellationResponse = oCancellationRequest.ResponseString;
                var cancelResponse = Envelope<OtaCancelRs>.DeSerialize(oCancellationRequest.ResponseXML, _serializer);

                if (!cancelResponse.IsSuccess)
                {
                    throw new Exception("The request was not returned successfully");
                }

                if (!string.IsNullOrEmpty(cancelResponse.CancelInfoRs.UniqueId.Id))
                {
                    oTPCancellationResponse.Success = true;
                    oTPCancellationResponse.TPCancellationReference = cancelResponse.CancelInfoRs.UniqueId.Id;
                }
                else
                {
                    throw new Exception("No cancellation reference");
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.Message);
                oTPCancellationResponse.Success = false;
                oTPCancellationResponse.TPCancellationReference = string.Empty;
            }
            finally
            {
                if (!string.IsNullOrEmpty(sCancellationRequest))
                {
                    propertyDetails.Logs.AddNew(Source, "WelcomeBeds Cancellation Request", sCancellationRequest);
                }

                if (!string.IsNullOrEmpty(sCancellationResponse))
                {
                    propertyDetails.Logs.AddNew(Source, "WelcomeBeds Cancellation Response", sCancellationResponse);
                }
            }
            return oTPCancellationResponse;
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

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails)
        {
            return new ThirdPartyCancellationFeeResult();
        }
    }
}
