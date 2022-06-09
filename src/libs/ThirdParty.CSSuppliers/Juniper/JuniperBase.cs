namespace ThirdParty.CSSuppliers.Juniper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.CSSuppliers.Juniper.Model;
    using Microsoft.Extensions.Logging;

    public abstract class JuniperBase : IThirdParty
    {
        #region "Properties"

        private readonly IJuniperBaseSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public abstract string Source { get; }

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
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

        public JuniperBase(IJuniperBaseSettings settings, ISerializer serializer, HttpClient httpClient, ILogger logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region "PreBook"

        public bool PreBook(PropertyDetails propertyDetails)
        {
            bool bSuccess = false;
            string hotelBookRuleURL = JuniperHelper.ConstructUrl(_settings.BaseURL(propertyDetails), _settings.HotelBookingRuleURL(propertyDetails));
            string sSOAPAction = _settings.HotelBookingRuleSOAPAction(propertyDetails);
            bool useGzip = _settings.UseGZip(propertyDetails);

            foreach (var oRoom in propertyDetails.Rooms)
            {
                Request oRequest = null;

                try
                {
                    string sPreBookRequest = BuildPreBookRequest(propertyDetails, oRoom.ThirdPartyReference);
                    oRequest = JuniperHelper.BuildWebRequest(hotelBookRuleURL, sSOAPAction, sPreBookRequest, Constant.PreBookLogFile, Source, useGzip);
                    oRequest.Send(_httpClient, _logger).RunSynchronously();

                    var oResponseXml = _serializer.CleanXmlNamespaces(oRequest.ResponseXML);
                    var sResponse = oResponseXml.SelectSingleNode("Envelope/Body").FirstChild.OuterXml;
                    var oResponse = _serializer.DeSerialize<OTA_HotelBookingRuleServiceResponse>(sResponse);
                    var oBookingRule = oResponse.HotelBookingRuleRS.RuleMessage.BookingRules.First();

                    var nCost = SafeTypeExtensions.ToSafeDecimal(oBookingRule.TpaExtensions.TotalPrice.Content);

                    if (nCost == 0)
                    {
                        throw new Exception("Rate no longer available");
                    }
                    else if (!decimal.Equals(oRoom.LocalCost, nCost))
                    {
                        oRoom.LocalCost = nCost;
                        oRoom.GrossCost = nCost;
                    }

                    ProcessCancellationPolicies(propertyDetails, oRoom, oBookingRule);

                    var sDescription = oBookingRule.Description.Text;
                    sDescription = Regex.Replace(sDescription, @"[\]<][^>]*>", string.Empty);

                    if (!string.IsNullOrEmpty(sDescription))
                    {
                        propertyDetails.Errata.AddNew("Important Information", sDescription);
                    }

                    bSuccess = true;
                }
                catch (Exception ex)
                {
                    propertyDetails.Warnings.AddNew("Prebook Exception", ex.Message);
                    bSuccess = false;
                    break;
                }
                finally
                {
                    if (oRequest is not null)
                    {
                        if (!string.IsNullOrEmpty(oRequest.RequestLog))
                        {
                            propertyDetails.Logs.AddNew(Source, $"{Source} Prebook Request", oRequest.RequestLog);
                        }

                        if (!string.IsNullOrEmpty(oRequest.ResponseLog))
                        {
                            propertyDetails.Logs.AddNew(Source, $"{Source} Prebook Response", oRequest.ResponseLog);
                        }
                    }
                }
            }

            //'merge policies
            propertyDetails.Cancellations.Solidify(SolidifyType.Sum);

            return bSuccess;
        }

        public void ProcessCancellationPolicies(PropertyDetails propertyDetails, RoomDetails oRoom, BookingRule bookingRule)
        {
            var bRefundable = true;
            foreach (var oCancellationPolicy in bookingRule.CancelPenalties)
            {
                string sPolicy = oCancellationPolicy?.PenaltyDescription.Text ?? "";
                if (sPolicy.ToLower().Contains("non") && sPolicy.ToLower().Contains("refundable"))
                {
                    propertyDetails.Cancellations.AddNew(DateTime.Now, new DateTime(2099, 12, 25), oRoom.LocalCost);
                    bRefundable = false;
                }
            }
            if (bRefundable)
            {
                foreach (var oPolicyRule in bookingRule.TpaExtensions.CancellationPolicyRules)
                {
                    if (!string.IsNullOrEmpty(oPolicyRule.DateFrom))
                    {
                        DateTime dateFrom = oPolicyRule.DateFrom.ToSafeDate();
                        DateTime dateTo = string.IsNullOrEmpty(oPolicyRule.DateTo)
                            ? new DateTime(2099, 12, 25)
                            : oPolicyRule.DateTo.ToSafeDate().AddDays(-1);

                        decimal nAmount = 0;

                        if (oPolicyRule.FixedPrice.ToSafeDecimal() > 0)
                        {
                            nAmount = oPolicyRule.FixedPrice.ToSafeDecimal();
                        }
                        else if (oPolicyRule.PercentPrice.ToSafeDecimal() > 0)
                        {
                            nAmount = oRoom.LocalCost * oPolicyRule.PercentPrice.ToSafeDecimal() / 100;
                        }
                        else if (oPolicyRule.FirstNightPrice.ToSafeDecimal() > 0)
                        {
                            nAmount = oPolicyRule.FirstNightPrice.ToSafeDecimal();
                        }
                        else if (oPolicyRule.Nights.ToSafeInt() > 0)
                        {
                            nAmount = oRoom.LocalCost / propertyDetails.Duration * oPolicyRule.Nights.ToSafeInt();
                        }

                        propertyDetails.Cancellations.AddNew(dateFrom, dateTo, nAmount);
                    }
                }
            }
        }

        #endregion

        #region "Book"

        public string Book(PropertyDetails propertyDetails)
        {
            var oReference = new List<string>();
            var sBookRequest = "";
            var oResponseXML = new XmlDocument();

            foreach (var oRoom in propertyDetails.Rooms)
            {
                try
                {
                    sBookRequest = BuildBookRequest(propertyDetails, oRoom);

                    var oRequest = new Request
                    {
                        EndPoint = JuniperHelper.ConstructUrl(_settings.BaseURL(propertyDetails), _settings.HotelBookURL(propertyDetails)),
                        SOAP = true,
                        SoapAction = _settings.HotelBookSOAPAction(propertyDetails),
                        Method = eRequestMethod.POST,
                        Source = Source,
                        LogFileName = Constant.BookLogFile,
                        CreateLog = true,
                        UseGZip = _settings.UseGZip(propertyDetails),
                    };

                    oRequest.SetRequest(sBookRequest);
                    oRequest.Send(_httpClient, _logger).RunSynchronously();

                    oResponseXML = _serializer.CleanXmlNamespaces(oRequest.ResponseXML);
                    var sResponse = oResponseXML.SelectSingleNode("Envelope/Body").FirstChild.OuterXml;
                    var oResponse = _serializer.DeSerialize<OTA_HotelResV2ServiceResponse>(sResponse);

                    if (oResponse.OTA_HotelResRS?.Success ?? false)
                    {
                        var oHotelReservation = oResponse.OTA_HotelResRS.HotelReservations.First();
                        var sBookingStatus = oHotelReservation.ResStatus;
                        var sBookingReference = oHotelReservation.UniqueId.Id;

                        var bookingStatuses = new[] { "Pag", "Ok", "Con", "Con*" };

                        if (!string.IsNullOrEmpty(sBookingStatus)
                            && bookingStatuses.Contains(sBookingStatus)
                            && !string.IsNullOrEmpty(sBookingReference))
                        {
                            oReference.Add(sBookingReference);
                        }
                        else
                        {
                            oReference.Add(Constant.FailedToken);
                        }
                    }
                    else
                    {
                        var sError = oResponse.OTA_HotelResRS.Errors?.First()?.ShortText;
                        if (!string.IsNullOrEmpty(sError))
                        {
                            propertyDetails.Warnings.AddNew("Third Party Error", sError, WarningType.ThirdPartyError);
                        }
                        oReference.Add(Constant.FailedToken);
                    }
                }
                catch (Exception ex)
                {
                    oReference.Add(Constant.FailedToken);
                    propertyDetails.Warnings.AddNew("Book Exception", ex.Message);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(sBookRequest))
                    {
                        propertyDetails.Logs.AddNew(Source, $"{Source} Book Request", sBookRequest);
                    }
                    if (!string.IsNullOrEmpty(oResponseXML.InnerXml))
                    {
                        propertyDetails.Logs.AddNew(Source, $"{Source} Book Response", oResponseXML.InnerXml);
                    }
                }
            }

            var sBookingRef = string.Join("|", oReference);
            if (oReference.Contains(Constant.FailedToken))
            {
                propertyDetails.SourceSecondaryReference = sBookingRef;
                return Constant.FailedBookReference;
            }

            return sBookingRef;
        }

        public string BuildBookRequest(PropertyDetails propertyDetails, RoomDetails oRoom)
        {
            var oLeadGuest = oRoom.Passengers.First();

            var sbBookRequest = new OTA_HotelResV2Service
            {
                HotelReservationRequest =
                {
                    PrimaryLangID = _settings.LanguageCode(propertyDetails),
                    SequenceNmbr = oRoom.ThirdPartyReference.Split('|')[0],
                    Pos = JuniperHelper.BuildPosNode(
                        _settings.AgentDutyCode(propertyDetails),
                        _settings.Password(propertyDetails)),
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
                                            RatePlanCode = oRoom.ThirdPartyReference.Split('|')[1]
                                        }
                                    },
                                    RoomTypes =
                                    {
                                        new RoomType
                                        {
                                            RoomTypeExtension =
                                            {
                                                Guests = oRoom.Passengers.Select(oPassenger => new Guest
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
                                        CurrencyCode = oRoom.ThirdPartyReference.Split('|')[2],
                                        AmountAfterTax = oRoom.LocalCost.ToString()
                                    },
                                    RoomStayExtension =
                                    {
                                        ExpectedPriceRange =
                                        {
                                            Min = "0",
                                            Max = oRoom.LocalCost.ToString()
                                        },
                                        PaxCountry = _settings.PaxCountry(propertyDetails)
                                    },
                                    Comments = propertyDetails.BookingComments.Select(oComment => new Comment
                                    {
                                        Text = oComment.Text
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
                                                        GivenName = oLeadGuest.FirstName,
                                                        Surname = oLeadGuest.LastName
                                                    },
                                                    Email = propertyDetails.LeadGuestEmail,
                                                    Address =
                                                    {
                                                        AddressLine = propertyDetails.LeadGuestAddress1,
                                                        PostalCode = propertyDetails.LeadGuestPostcode,
                                                        CityName = propertyDetails.LeadGuestTownCity,
                                                        StateProv = propertyDetails.LeadGuestCounty,
                                                        CountryName = propertyDetails.LeadGuestBookingCountry
                                                    }
                                                }
                                            },

                                        }
                                    }
                                }
                            },
                            HotelReservationExtension =
                            {
                                PaxCountry = _settings.PaxCountry(propertyDetails),
                                ForceCurrency = _settings.CurrencyCode(propertyDetails)
                            }
                        }
                    }
                }
            };

            return JuniperHelper.BuildSoap(sbBookRequest, _serializer);
        }

        #endregion

        #region "Cancellations"

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails)
        {
            var oThirdPartyCancellationResponse = new ThirdPartyCancellationResponse { Success = true };
            var oCancellationReferences = new List<string>();

            var allBookingReferences = !string.Equals(propertyDetails.SourceReference, Constant.FailedToken)
                                            ? propertyDetails.SourceReference
                                            : propertyDetails.SourceSecondaryReference;

            foreach (var sBookingRef in allBookingReferences.Split('|').Where(sRef => !string.Equals(sRef, Constant.FailedToken)))
            {
                var oRequest = new Request();
                var oResponse = new OTA_CancelServiceResponse();
                try
                {
                    var sCancellationRequest = BuildCancellationRequest(sBookingRef, propertyDetails);

                    oRequest = JuniperHelper.BuildWebRequest(
                            JuniperHelper.ConstructUrl(_settings.BaseURL(propertyDetails), _settings.HotelCancelURL(propertyDetails)),
                            _settings.HotelCancelSOAPAction(propertyDetails),
                            sCancellationRequest,
                            Constant.CancelLogFile,
                            Source,
                            _settings.UseGZip(propertyDetails)
                        );

                    oRequest.Send(_httpClient, _logger).RunSynchronously();

                    var oResponseXml = _serializer.CleanXmlNamespaces(oRequest.ResponseXML);
                    var sResponse = oResponseXml.SelectSingleNode("Envelope/Body").FirstChild.OuterXml;
                    oResponse = _serializer.DeSerialize<OTA_CancelServiceResponse>(sResponse);

                    if (oResponse.OTA_CancelRS.Success)
                    {
                        oCancellationReferences.Add(sBookingRef);
                    }

                    if (oResponse.OTA_CancelRS.Errors.Any())
                    {
                        oThirdPartyCancellationResponse.Success = false;
                        propertyDetails.Warnings.AddNew("Cancellation Error", oResponse.OTA_CancelRS.Errors.First().ShortText);
                    }
                }
                catch (Exception ex)
                {
                    oThirdPartyCancellationResponse.Success = false;
                    propertyDetails.Warnings.AddNew("Cancellation Exception", ex.Message);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(oRequest.RequestLog))
                    {
                        propertyDetails.Logs.AddNew(Source, $"{Source} Cancel Request", oRequest.RequestLog);
                    }
                    if (!string.IsNullOrEmpty(oRequest.ResponseLog))
                    {
                        propertyDetails.Logs.AddNew(Source, $"{Source} Cancel Response", oRequest.ResponseLog);
                    }
                }
            }
            oThirdPartyCancellationResponse.TPCancellationReference = string.Join("|", oCancellationReferences);
            return oThirdPartyCancellationResponse;
        }

        public string BuildCancellationRequest(string bookingReference, IThirdPartyAttributeSearch searchDetails)
        {
            var sbCancellationRequest = new OTA_CancelService
            {
                CancelRequest =
                {
                    PrimaryLangId = _settings.LanguageCode(searchDetails),
                    Pos = JuniperHelper.BuildPosNode(_settings.AgentDutyCode(searchDetails), _settings.Password(searchDetails)),
                    UniqueId =
                    {
                        IdType = "14",
                        Id = bookingReference
                    }
                }
            };

            return JuniperHelper.BuildSoap(sbCancellationRequest, _serializer);
        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails)
        {
            return new();
        }

        #endregion

        #region "Request Builders"

        public string BuildPreBookRequest(PropertyDetails propertyDetails, string sTPReference)
        {
            var sbPreBookRequest = new OTA_HotelBookingRuleService
            {
                HotelBookingRuleRQ =
                {
                    PrimaryLangId = _settings.LanguageCode(propertyDetails),
                    SequenceNmbr = sTPReference.Split('|')[0],
                    Pos = JuniperHelper.BuildPosNode(_settings.AgentDutyCode(propertyDetails), _settings.Password(propertyDetails)),
                    RuleMessage =
                    {
                        HotelCode = propertyDetails.TPKey,
                        StatusApplication =
                        {
                            RatePlanCode = sTPReference.Split('|')[1],
                            Start = propertyDetails.ArrivalDate.ToString(Constant.DateTimeFormat),
                            End = propertyDetails.DepartureDate.ToString(Constant.DateTimeFormat)
                        },
                        TpaExtension =
                        {
                            ForceCurrency = _settings.CurrencyCode(propertyDetails),
                            ShowSupplements = "0",
                            PaxCountry = _settings.PaxCountry(propertyDetails)
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