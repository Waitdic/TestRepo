namespace ThirdParty.CSSuppliers.TeamAmerica
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.CSSuppliers.TeamAmerica.Models;
    using Microsoft.Extensions.Logging;

    public class TeamAmerica : IThirdParty
    {
        #region "Properties"

        private readonly ITeamAmericaSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TeamAmerica> _logger;

        public string Source { get => ThirdParties.TEAMAMERICA; }
        public bool SupportsBookingSearch => true;
        public bool SupportsRemarks => true;

        #endregion

        #region "Constructors"

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

        #region "PreBook"
        public bool PreBook(PropertyDetails oPropertyDetails)
        {
            bool bReturn = true;
            var oSearchRequests = new List<Request>();

            //'check availability/cancelllation policy for each room
            var bAvailable = true;

            try
            {
                //'check availability/cancelllation policy for each room
                foreach (var oRoomDetails in oPropertyDetails.Rooms)
                {
                    //'Build/send search
                    var sRequest = BuildSearchXml(oPropertyDetails, oRoomDetails);
                    var oSearchRequest = SendRequest(Constant.SoapActionPreBook, sRequest, oPropertyDetails, Constant.SoapActionPreBook, oPropertyDetails.CreateLogs);
                    var oHotelOffer = Envelope<PriceSearchResponse>.DeSerialize(oSearchRequest.ResponseXML, _serializer).HotelSearchResponse.Offers.First();

                    // 'check availability
                    if (!IsEveryNightAvailable(oHotelOffer))
                    {
                        bAvailable = false;
                    }
                    else
                    {
                        //'check the price

                        //'get the occupancy returned at search (Single, Double, Triple or Quad)
                        string sOccupancy = GetOccupancy(oRoomDetails, true);

                        decimal nCost = oHotelOffer.NightlyInfos.Select(night => SafeTypeExtensions.ToSafeDecimal(
                            night.Prices.FirstOrDefault(x => string.Equals(x.Occupancy, sOccupancy))?.AdultPrice ?? "0")).Sum();

                        //'Compare with the original price
                        if (nCost > 0 && SafeTypeExtensions.ToSafeDecimal(oRoomDetails.LocalCost) != nCost)
                        {
                            oRoomDetails.LocalCost = SafeTypeExtensions.ToSafeDecimal(nCost);
                            oRoomDetails.GrossCost = SafeTypeExtensions.ToSafeDecimal(nCost);
                        }

                        string avarageNightRate = oHotelOffer.AverageRates.FirstOrDefault(rate => string.Equals(rate.Occupancy, sOccupancy))?.AverageNightlyRate ?? "";

                        oRoomDetails.ThirdPartyReference += $"|{avarageNightRate}";
                    }

                    oSearchRequests.Add(oSearchRequest);

                    if (!bAvailable)
                    {
                        bReturn = false;
                    }
                    else
                    {
                        GetCancellationCosts(oPropertyDetails, oHotelOffer);
                    }
                }
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("PreBookExcpetion", ex.Message);
                bReturn = false;
            }
            finally
            {
                //'Add the xml to the booking
                foreach (var oSearchRequest in oSearchRequests)
                {
                    if (!string.IsNullOrEmpty(oSearchRequest.RequestLog))
                    {
                        oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, "TeamAmerica Price Check Request", oSearchRequest.RequestLog);
                    }
                    if (!string.IsNullOrEmpty(oSearchRequest.ResponseLog))
                    {
                        oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, "TeamAmerica Price Check Response", oSearchRequest.ResponseLog);
                    }
                }
            }

            return bReturn;
        }

        #endregion

        #region"Book"

        public string Book(PropertyDetails oPropertyDetails)
        {
            string sReference = "";
            var oRequest = new Request();

            try
            {
                //'Build/send Confirmation
                string sRequest = BuildBookXml(oPropertyDetails);
                oRequest = SendRequest(Constant.SoapActionBook, sRequest, oPropertyDetails, "Book", oPropertyDetails.CreateLogs);
                var oResponse = Envelope<NewMultiItemReservationResponse>.DeSerialize(oRequest.ResponseXML, _serializer).ReservationResponse;
                string sResponse = oRequest.ResponseString;

                //'Check for errors or pending bookings
                if (sResponse.ToLower().Contains("error") || string.Equals(oResponse.ReservationInformation.ReservationStatus, "RQ"))
                {
                    sReference = Constant.Failed;
                }
                else
                {
                    //'get the confirmation code
                    sReference = oResponse.ReservationInformation.ReservationNumber;
                }
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("BookException", ex.Message);
                sReference = Constant.Failed;
            }
            finally
            {
                //'Attach the logs to the booking
                if (!string.IsNullOrEmpty(oRequest.RequestLog))
                {
                    oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, "TeamAmerica Book Request", oRequest.RequestLog);
                }
                if (!string.IsNullOrEmpty(oRequest.ResponseLog))
                {
                    oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, "TeamAmerica Book Response", oRequest.ResponseLog);
                }
            }

            return sReference;
        }

        #endregion

        #region "CancelBooking"
        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails oPropertyDetails)
        {
            var TPCancelResponse = new ThirdPartyCancellationResponse
            {
                Success = true
            };

            var oRequest = new Request();

            try
            {
                var sRequest = BuildCancelXml(oPropertyDetails);
                oRequest = SendRequest(Constant.SoapActionCancel, sRequest, oPropertyDetails, Constant.SoapActionCancel, oPropertyDetails.CreateLogs);

                var oResponse = Envelope<CancelReservationResponse>.DeSerialize(oRequest.ResponseXML, _serializer).CancelResponse;

                if (string.Equals(oResponse.ReservationStatusCode, "CX"))
                {
                    TPCancelResponse.TPCancellationReference = oPropertyDetails.SourceReference;
                }
                else
                {
                    TPCancelResponse.TPCancellationReference = Constant.Failed;
                    TPCancelResponse.Success = false;
                }
            }
            catch (Exception)
            {
                TPCancelResponse.TPCancellationReference = Constant.Failed;
                TPCancelResponse.Success = false;
            }
            finally
            {
                //'Attach the logs to the booking
                if (!string.IsNullOrEmpty(oRequest.RequestLog))
                {
                    oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, "TeamAmerica Cancellation Request", oRequest.RequestLog);
                }
                if (!string.IsNullOrEmpty(oRequest.ResponseLog))
                {
                    oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, "TeamAmerica Cancellation Response", oRequest.ResponseLog);
                }
            }

            return TPCancelResponse;
        }

        #endregion

        #region "Other methods"

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails)
        {
            return new();
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

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }
        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public bool TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails)
        {
            return false;
        }

        #endregion

        #region "Helpers"

        public static bool IsEveryNightAvailable(HotelOffer oHotelOffer)
        {
            return oHotelOffer.NightlyInfos.All(night => string.Equals(night.Status, Constant.Available));
        }

        public Request SendRequest(string sSoapAction, string sXml, IThirdPartyAttributeSearch SearchDetails, string sLogFile, bool bLog)
        {
            var oRequest = new Request
            {
                EndPoint = _settings.URL(SearchDetails),
                SoapAction = $"{_settings.URL(SearchDetails)}/{sSoapAction}",
                Method = eRequestMethod.POST,
                Source = Source,
                ContentType = ContentTypes.Text_Xml_charset_utf_8,
                LogFileName = sLogFile,
                CreateLog = bLog
            };
            oRequest.SetRequest(sXml);
            oRequest.Send(_httpClient, _logger).RunSynchronously();
            return oRequest;
        }

        public string BuildSearchXml(PropertyDetails oPropertyDetails, RoomDetails oRoomDetails)
        {
            var request = new PriceSearch
            {
                UserName = _settings.Username(oPropertyDetails),
                Password = _settings.Password(oPropertyDetails),
                CityCode = oPropertyDetails.ResortCode,
                ProductCode = oRoomDetails.ThirdPartyReference.Split('|')[0],
                RequestType = Constant.SearchTypeHotel,
                Occupancy = "",
                ArrivalDate = oPropertyDetails.ArrivalDate.ToString(Constant.DateTimeFormat),
                NumberOfNights = oPropertyDetails.Duration,
                NumberOfRooms = oPropertyDetails.Rooms.Count,
                DisplayClosedOut = Constant.TokenNo,
                DisplayOnRequest = Constant.TokenNo,
                VendorIds = { oPropertyDetails.TPKey }
            };

            return Envelope<PriceSearch>.Serialize(request, _serializer);
        }

        public void GetCancellationCosts(PropertyDetails oPropertyDetails, HotelOffer oHotelOffer)
        {
            foreach (var oPolicy in oHotelOffer.CancellationPolicies)
            {
                var oCancellation = new Cancellation
                {
                    StartDate = oPropertyDetails.ArrivalDate.AddDays(-oPolicy.NumberDaysPrior),
                    EndDate = oPropertyDetails.ArrivalDate,
                };

                var nPenaltyAmount = SafeTypeExtensions.ToSafeDecimal(oPolicy.PenaltyAmount);

                switch (oPolicy.PenaltyType)
                {
                    case "Nights":
                        oCancellation.Amount = nPenaltyAmount * oPropertyDetails.LocalCost / oPropertyDetails.Duration;
                        break;
                    case "Percent":
                        oCancellation.Amount = nPenaltyAmount * oPropertyDetails.LocalCost / 100;
                        break;
                    case "Dollars":
                        oCancellation.Amount = nPenaltyAmount;
                        break;
                }

                oPropertyDetails.Cancellations.Add(oCancellation);
            }

            oPropertyDetails.Cancellations.Solidify(SolidifyType.LatestStartDate);
        }

        public string GetOccupancy(RoomDetails oRoomDetails, bool bAdultOnly)
        {
            string sOccupancy = "";
            string sFamilyPlan = oRoomDetails.ThirdPartyReference.Split('|')[1];
            int iChildAge = SafeTypeExtensions.ToSafeInt(oRoomDetails.ThirdPartyReference.Split('|')[2]);

            int iAdults;
            int iChildren = 0;

            if (string.Equals(sFamilyPlan, Constant.TokenYes))
            {
                iAdults = oRoomDetails.AdultsSetAgeOrOver(iChildAge + 1);
                iChildren = bAdultOnly ? 0 : oRoomDetails.ChildrenUnderSetAge(iChildAge + 1) + oRoomDetails.Infants;
            }
            else
            {
                iAdults = oRoomDetails.Adults + oRoomDetails.Children + oRoomDetails.Infants;
            }

            switch (iAdults)
            {
                case 1:
                    switch (iChildren)
                    {
                        case 0:
                            sOccupancy = "Single";
                            break;
                        case 1:
                            sOccupancy = "SGL+1CH";
                            break;
                        //'iAdults must be greater or equal to iChildren, else use DBL room
                        case 2:
                            sOccupancy = "DBL+1CH";
                            break;
                        case 3:
                            sOccupancy = "DBL+2CH";
                            break;
                    }
                    break;
                case 2:
                    switch (iChildren)
                    {
                        case 0:
                            sOccupancy = "Double";
                            break;
                        case 1:
                            sOccupancy = "DBL+1CH";
                            break;
                        case 2:
                            sOccupancy = "DBL+2CH";
                            break;
                    }
                    break;
                case 3:
                    switch (iChildren)
                    {
                        case 0:
                            sOccupancy = "Triple";
                            break;
                        case 1:
                            sOccupancy = "TPL+1CH";
                            break;
                    }
                    break;
                case 4:
                    sOccupancy = "Quad";
                    break;
            }

            return sOccupancy;
        }

        public string BuildBookXml(PropertyDetails oPropertyDetails)
        {
            var bookRequest = new NewMultiItemReservation
            {
                UserName = _settings.Username(oPropertyDetails),
                Password = _settings.Password(oPropertyDetails),
                AgentName = _settings.CompanyName(oPropertyDetails),
                AgentEmail = _settings.CompanyAddressEmail(oPropertyDetails),
                ClientReference = oPropertyDetails.BookingReference,
                RoomItems = oPropertyDetails.Rooms.Select(oRoomDetails =>
                {
                    return new RoomItem
                    {
                        ProductCode = oRoomDetails.ThirdPartyReference.Split('|')[0],
                        ProductDate = oPropertyDetails.ArrivalDate.ToString(Constant.DateTimeFormat),
                        Occupancy = GetOccupancy(oRoomDetails, false),
                        NumberOfNights = oPropertyDetails.Duration,
                        Language = Constant.ENG,
                        Quantity = 1,
                        ItemRemarks = oPropertyDetails.BookingComments.ToString(),
                        RateExpected = oRoomDetails.ThirdPartyReference.Split('|')[3],
                        Passangers = oRoomDetails.Passengers.Select(oPassenger =>
                        {
                            string title = string.Equals(oPassenger.Title, "Master")
                                    ? "Mstr"
                                    : ((oPassenger.Title.Length > 4)
                                        ? oPassenger.Title.Substring(0, 4)
                                        : oPassenger.Title);

                            string sFamilyPlan = oRoomDetails.ThirdPartyReference.Split('|')[1];
                            int iChildAge = SafeTypeExtensions.ToSafeInt(oRoomDetails.ThirdPartyReference.Split('|')[2]);
                            string sPassangerType = (oPassenger.PassengerType == PassengerType.Adult
                                                    || string.Equals(sFamilyPlan, Constant.TokenNo))
                                                    || oPassenger.Age > iChildAge
                                ? Constant.AdultCode
                                : Constant.ChildCode;

                            int passangerAge = oPassenger.Age;
                            if (passangerAge == 0)
                            {
                                if (oPassenger.PassengerType == PassengerType.Adult) passangerAge = Constant.AgeAdult;
                                if (oPassenger.PassengerType == PassengerType.Child) passangerAge = Constant.AgeChild;
                                if (oPassenger.PassengerType == PassengerType.Infant) passangerAge = Constant.AgeInfant;
                            }

                            string sNationalityCode = (oPassenger.NationalityID != 0)
                                ? _support.TPNationalityLookup(Source, oPassenger.NationalityID)
                                : _settings.DefaultNationalityCode(oPropertyDetails);

                            return new NewPassanger
                            {
                                Salutation = title,
                                FamilyName = oPassenger.LastName,
                                FirstName = oPassenger.FirstName,
                                PassengerType = sPassangerType,
                                NationalityCode = sNationalityCode,
                                PassengerAge = passangerAge
                            };
                        }).ToList()
                    };
                }).ToList()
            };
            return Envelope<NewMultiItemReservation>.Serialize(bookRequest, _serializer);
        }

        public string BuildCancelXml(PropertyDetails oPropertyDetails)
        {
            var cancelResponse = new CancelReservation
            {
                UserName = _settings.Username(oPropertyDetails),
                Password = _settings.Password(oPropertyDetails),
                ReservationNumber = oPropertyDetails.SourceReference
            };

            return Envelope<CancelReservation>.Serialize(cancelResponse, _serializer);
        }

        #endregion
    }
}