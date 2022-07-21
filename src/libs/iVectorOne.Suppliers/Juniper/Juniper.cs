namespace iVectorOne.CSSuppliers.Juniper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using iVectorOne;
    using iVectorOne.CSSuppliers.Juniper.Model;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;

    public class Juniper : IThirdParty, IMultiSource
    {
        #region Properties

        private readonly IJuniperSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Juniper> _logger;

        public List<string> Sources => Constant.JuniperSources;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails, source);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails, source);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        #endregion

        #region Constructors

        public Juniper(IJuniperSettings settings, ISerializer serializer, HttpClient httpClient, ILogger<Juniper> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            bool success = false;
            string hotelBookRuleURL = JuniperHelper.ConstructUrl(
                _settings.BaseURL(propertyDetails, propertyDetails.Source),
                _settings.HotelBookingRuleURL(propertyDetails, propertyDetails.Source));
            string soapAction = _settings.HotelBookingRuleSOAPAction(propertyDetails, propertyDetails.Source);
            bool useGzip = _settings.UseGZip(propertyDetails, propertyDetails.Source);

            foreach (var room in propertyDetails.Rooms)
            {
                var request = new Request();

                try
                {
                    string preBookRequest = BuildPreBookRequest(propertyDetails, room.ThirdPartyReference);
                    request = JuniperHelper.BuildWebRequest(hotelBookRuleURL, soapAction, preBookRequest, Constant.PreBookLogFile, propertyDetails.Source, useGzip);
                    await request.Send(_httpClient, _logger);

                    var responseXml = _serializer.CleanXmlNamespaces(request.ResponseXML);
                    var responseBody = responseXml.SelectSingleNode("Envelope/Body").FirstChild.OuterXml;
                    var response = _serializer.DeSerialize<OTA_HotelBookingRuleServiceResponse>(responseBody);
                    var bookingRule = response.HotelBookingRuleRS.RuleMessage.BookingRules.First();

                    var cost = bookingRule.TpaExtensions.TotalPrice.Content.ToSafeDecimal();

                    if (cost == 0)
                    {
                        throw new Exception("Rate no longer available");
                    }
                    else if (!decimal.Equals(room.LocalCost, cost))
                    {
                        room.LocalCost = cost;
                        room.GrossCost = cost;
                    }

                    ProcessCancellationPolicies(propertyDetails, room, bookingRule);

                    var description = bookingRule.Description.Text;
                    description = Regex.Replace(description, @"[\]<][^>]*>", string.Empty);

                    if (!string.IsNullOrEmpty(description))
                    {
                        propertyDetails.Errata.AddNew("Important Information", description);
                    }

                    success = true;
                }
                catch (Exception ex)
                {
                    propertyDetails.Warnings.AddNew("Prebook Exception", ex.Message);
                    success = false;
                    break;
                }
                finally
                {
                    propertyDetails.AddLog("Prebook", request);
                }
            }

            // merge policies
            propertyDetails.Cancellations.Solidify(SolidifyType.Sum);

            return success;
        }

        public void ProcessCancellationPolicies(PropertyDetails propertyDetails, RoomDetails room, BookingRule bookingRule)
        {
            var refundable = true;
            foreach (var cancellationPolicy in bookingRule.CancelPenalties)
            {
                string policy = cancellationPolicy?.PenaltyDescription.Text ?? "";
                if (policy.ToLower().Contains("non") && policy.ToLower().Contains("refundable"))
                {
                    propertyDetails.Cancellations.AddNew(DateTime.Now, new DateTime(2099, 12, 25), room.LocalCost);
                    refundable = false;
                }
            }

            if (refundable)
            {
                foreach (var policyRule in bookingRule.TpaExtensions.CancellationPolicyRules)
                {
                    if (!string.IsNullOrEmpty(policyRule.DateFrom))
                    {
                        DateTime dateFrom = policyRule.DateFrom.ToSafeDate();
                        DateTime dateTo = string.IsNullOrEmpty(policyRule.DateTo)
                            ? new DateTime(2099, 12, 25)
                            : policyRule.DateTo.ToSafeDate().AddDays(-1);

                        decimal amount = 0;

                        if (policyRule.FixedPrice.ToSafeDecimal() > 0)
                        {
                            amount = policyRule.FixedPrice.ToSafeDecimal();
                        }
                        else if (policyRule.PercentPrice.ToSafeDecimal() > 0)
                        {
                            amount = room.LocalCost * policyRule.PercentPrice.ToSafeDecimal() / 100;
                        }
                        else if (policyRule.FirstNightPrice.ToSafeDecimal() > 0)
                        {
                            amount = policyRule.FirstNightPrice.ToSafeDecimal();
                        }
                        else if (policyRule.Nights.ToSafeInt() > 0)
                        {
                            amount = room.LocalCost / propertyDetails.Duration * policyRule.Nights.ToSafeInt();
                        }

                        propertyDetails.Cancellations.AddNew(dateFrom, dateTo, amount);
                    }
                }
            }
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var references = new List<string>();
            var bookRequest = "";
            var responseXml = new XmlDocument();

            foreach (var room in propertyDetails.Rooms)
            {
                var request = new Request();

                try
                {
                    bookRequest = BuildBookRequest(propertyDetails, room);

                    request = new Request
                    {
                        EndPoint = JuniperHelper.ConstructUrl(
                            _settings.BaseURL(propertyDetails, propertyDetails.Source),
                            _settings.HotelBookURL(propertyDetails, propertyDetails.Source)),
                        SOAP = true,
                        SoapAction = _settings.HotelBookSOAPAction(propertyDetails, propertyDetails.Source),
                        Method = RequestMethod.POST,
                        Source = propertyDetails.Source,
                        LogFileName = Constant.BookLogFile,
                        CreateLog = true,
                        UseGZip = _settings.UseGZip(propertyDetails, propertyDetails.Source),
                    };

                    request.SetRequest(bookRequest);
                    await request.Send(_httpClient, _logger);

                    responseXml = _serializer.CleanXmlNamespaces(request.ResponseXML);
                    var responseBody = responseXml.SelectSingleNode("Envelope/Body").FirstChild.OuterXml;
                    var response = _serializer.DeSerialize<OTA_HotelResV2ServiceResponse>(responseBody);

                    if (response.OTA_HotelResRS?.Success ?? false)
                    {
                        var hotelReservation = response.OTA_HotelResRS.HotelReservations.First();
                        var bookingStatus = hotelReservation.ResStatus;
                        var reference = hotelReservation.UniqueId.Id;

                        var bookingStatuses = new[] { "Pag", "Ok", "Con", "Con*" };

                        if (!string.IsNullOrEmpty(bookingStatus)
                            && bookingStatuses.Contains(bookingStatus)
                            && !string.IsNullOrEmpty(reference))
                        {
                            references.Add(reference);
                        }
                        else
                        {
                            references.Add(Constant.FailedToken);
                        }
                    }
                    else
                    {
                        var error = response.OTA_HotelResRS.Errors?.First()?.ShortText;
                        if (!string.IsNullOrEmpty(error))
                        {
                            propertyDetails.Warnings.AddNew("Third Party Error", error, WarningType.ThirdPartyError);
                        }
                        references.Add(Constant.FailedToken);
                    }
                }
                catch (Exception ex)
                {
                    references.Add(Constant.FailedToken);
                    propertyDetails.Warnings.AddNew("Book Exception", ex.Message);
                }
                finally
                {
                    propertyDetails.AddLog("Book", request);
                }
            }

            var bookingReference = string.Join("|", references);
            if (references.Contains(Constant.FailedToken))
            {
                propertyDetails.SourceSecondaryReference = bookingReference;
                return Constant.FailedBookReference;
            }

            return bookingReference;
        }

        public string BuildBookRequest(PropertyDetails propertyDetails, RoomDetails room)
        {
            var leadGuest = room.Passengers.First();

            var sbBookRequest = new OTA_HotelResV2Service
            {
                HotelReservationRequest =
                {
                    PrimaryLangID = _settings.LanguageCode(propertyDetails, propertyDetails.Source),
                    SequenceNmbr = room.ThirdPartyReference.Split('|')[0],
                    Pos = JuniperHelper.BuildPosNode(
                        _settings.AgentDutyCode(propertyDetails, propertyDetails.Source),
                        _settings.Password(propertyDetails, propertyDetails.Source)),
                    HotelReservations =
                    {
                        new HotelReservation
                        {
                            UniqueID =
                            {
                                IdContext = propertyDetails.BookingReference.Trim()
                            },
                            RoomStays =
                            {
                                new RoomStay
                                {
                                    RatePlans =
                                    {
                                        new RatePlan
                                        {
                                            RatePlanCode = room.ThirdPartyReference.Split('|')[1]
                                        }
                                    },
                                    RoomTypes =
                                    {
                                        new RoomType
                                        {
                                            RoomTypeExtension =
                                            {
                                                Guests = room.Passengers.Select(oPassenger => new Guest
                                                {
                                                    Name = oPassenger.FirstName,
                                                    Surname = oPassenger.LastName,
                                                    Age = ((oPassenger.PassengerType == PassengerType.Adult)? Constant.DefaultAdultAge
                                                        : ((oPassenger.PassengerType == PassengerType.Child)? oPassenger.Age
                                                        : Constant.DefaultInfantAge)).ToString()
                                                }).ToList()
                                            }
                                        }
                                    },
                                    TimeSpan =
                                    {
                                        Start = propertyDetails.ArrivalDate.ToString(Constant.DateTimeFormat),
                                        End = propertyDetails.DepartureDate.ToString(Constant.DateTimeFormat)
                                    },
                                    BasicPropertyInfo =
                                    {
                                        HotelCode = propertyDetails.TPKey
                                    },
                                    Total =
                                    {
                                        CurrencyCode = room.ThirdPartyReference.Split('|')[2],
                                        AmountAfterTax = room.LocalCost.ToString()
                                    },
                                    RoomStayExtension =
                                    {
                                        ExpectedPriceRange =
                                        {
                                            Min = "0",
                                            Max = room.LocalCost.ToString()
                                        },
                                        PaxCountry = _settings.PaxCountry(propertyDetails, propertyDetails.Source)
                                    },
                                     Comments = propertyDetails.Rooms.Where(x => !string.IsNullOrWhiteSpace(x.SpecialRequest)).Select(x => new Comment
                                    {
                                        Text = x.SpecialRequest
                                    }).ToList()
                                }
                            },
                            ResGuests =
                            {
                                new ResGuest
                                {
                                    Profiles =
                                    {
                                        new ProfileInfo
                                        {
                                            Profile =
                                            {
                                                ProfileType = "1",
                                                Customer =
                                                {
                                                    PersonName =
                                                    {
                                                        GivenName = leadGuest.FirstName,
                                                        Surname = leadGuest.LastName
                                                    },
                                                    Email = propertyDetails.LeadGuestEmail,
                                                    Address =
                                                    {
                                                        AddressLine = propertyDetails.LeadGuestAddress1,
                                                        PostalCode = propertyDetails.LeadGuestPostcode,
                                                        CityName = propertyDetails.LeadGuestTownCity,
                                                        StateProv = propertyDetails.LeadGuestCounty,
                                                        CountryName = propertyDetails.LeadGuestCountryCode // todo - should use TP lookup here but data not in DB yet
                                                    }
                                                }
                                            },

                                        }
                                    }
                                }
                            },
                            HotelReservationExtension =
                            {
                                PaxCountry = _settings.PaxCountry(propertyDetails, propertyDetails.Source),
                                ForceCurrency = _settings.CurrencyCode(propertyDetails, propertyDetails.Source)
                            }
                        }
                    }
                }
            };

            return JuniperHelper.BuildSoap(sbBookRequest, _serializer);
        }

        #endregion

        #region Cancellations

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse { Success = true };
            var cancellationReferences = new List<string>();

            var allBookingReferences = !string.Equals(propertyDetails.SourceReference, Constant.FailedToken)
                                            ? propertyDetails.SourceReference
                                            : propertyDetails.SourceSecondaryReference;

            foreach (var bookingReference in allBookingReferences.Split('|').Where(sRef => !string.Equals(sRef, Constant.FailedToken)))
            {
                var request = new Request();
                var response = new OTA_CancelServiceResponse();
                try
                {
                    var cancellationRequest = BuildCancellationRequest(bookingReference, propertyDetails);

                    request = JuniperHelper.BuildWebRequest(
                            JuniperHelper.ConstructUrl(
                                _settings.BaseURL(propertyDetails, propertyDetails.Source),
                                _settings.HotelCancelURL(propertyDetails, propertyDetails.Source)),
                            _settings.HotelCancelSOAPAction(propertyDetails, propertyDetails.Source),
                            cancellationRequest,
                            Constant.CancelLogFile,
                            propertyDetails.Source,
                            _settings.UseGZip(propertyDetails, propertyDetails.Source)
                        );

                    await request.Send(_httpClient, _logger);

                    var responseXml = _serializer.CleanXmlNamespaces(request.ResponseXML);
                    var responseBody = responseXml.SelectSingleNode("Envelope/Body").FirstChild.OuterXml;
                    response = _serializer.DeSerialize<OTA_CancelServiceResponse>(responseBody);

                    if (response.OTA_CancelRS.Success)
                    {
                        cancellationReferences.Add(bookingReference);
                    }

                    if (response.OTA_CancelRS.Errors.Any())
                    {
                        thirdPartyCancellationResponse.Success = false;
                        propertyDetails.Warnings.AddNew("Cancellation Error", response.OTA_CancelRS.Errors.First().ShortText);
                    }
                }
                catch (Exception ex)
                {
                    thirdPartyCancellationResponse.Success = false;
                    propertyDetails.Warnings.AddNew("Cancellation Exception", ex.Message);
                }
                finally
                {
                    propertyDetails.AddLog("Cancel", request);
                }
            }

            thirdPartyCancellationResponse.TPCancellationReference = string.Join("|", cancellationReferences);
            return thirdPartyCancellationResponse;
        }

        public string BuildCancellationRequest(string bookingReference, PropertyDetails propertyDetails)
        {
            var sbCancellationRequest = new OTA_CancelService
            {
                CancelRequest =
                {
                    PrimaryLangId = _settings.LanguageCode(propertyDetails, propertyDetails.Source),
                    Pos = JuniperHelper.BuildPosNode(
                        _settings.AgentDutyCode(propertyDetails, propertyDetails.Source),
                        _settings.Password(propertyDetails, propertyDetails.Source)),
                    UniqueId =
                    {
                        IdType = "14",
                        Id = bookingReference
                    }
                }
            };

            return JuniperHelper.BuildSoap(sbCancellationRequest, _serializer);
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        #endregion

        #region Request Builders

        public string BuildPreBookRequest(PropertyDetails propertyDetails, string tpReference)
        {
            var sbPreBookRequest = new OTA_HotelBookingRuleService
            {
                HotelBookingRuleRQ =
                {
                    PrimaryLangId = _settings.LanguageCode(propertyDetails, propertyDetails.Source),
                    SequenceNmbr = tpReference.Split('|')[0],
                    Pos = JuniperHelper.BuildPosNode(
                        _settings.AgentDutyCode(propertyDetails, propertyDetails.Source),
                        _settings.Password(propertyDetails, propertyDetails.Source)),
                    RuleMessage =
                    {
                        HotelCode = propertyDetails.TPKey,
                        StatusApplication =
                        {
                            RatePlanCode = tpReference.Split('|')[1],
                            Start = propertyDetails.ArrivalDate.ToString(Constant.DateTimeFormat),
                            End = propertyDetails.DepartureDate.ToString(Constant.DateTimeFormat)
                        },
                        TpaExtension =
                        {
                            ForceCurrency = _settings.CurrencyCode(propertyDetails, propertyDetails.Source),
                            ShowSupplements = "0",
                            PaxCountry = _settings.PaxCountry(propertyDetails, propertyDetails.Source)
                        }
                    }
                }
            };

            return JuniperHelper.BuildSoap(sbPreBookRequest, _serializer);
        }

        #endregion

        public string CreateReconciliationReference(string inputReference)
        {
            return "";
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }
    }
}