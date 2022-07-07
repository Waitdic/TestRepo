namespace ThirdParty.CSSuppliers.AmadeusHotels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using Models;
    using Models.Soap;
    using Support;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class AmadeusHotels : IThirdParty, ISingleSource
    {
        private readonly IAmadeusHotelsSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AmadeusHotels> _logger;
        private readonly ISerializer _serializer;
        private readonly AmadeusHelper _helper;

        public AmadeusHotels(
            IAmadeusHotelsSettings settings,
            ITPSupport support,
            HttpClient httpClient,
            ILogger<AmadeusHotels> logger,
            ISerializer serializer,
            ISecretKeeper secretKeeper)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _helper = new AmadeusHelper(settings, support, serializer, secretKeeper);
        }

        private const string FailedBookingReference = "failed";

        public string Source => ThirdParties.AMADEUSHOTELS;

        public bool SupportsRemarks => false;
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
            return _settings.RequiresVCard(info);
        }

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            try
            {
                var errata = new List<string>();
                var convertedList = new List<bool>();
                var liveAvailabilityResponses = new List<EnvelopeResponse<OTAHotelAvailRS>>();

                if (_settings.SplitMultiRoom(propertyDetails))
                {
                    foreach (var room in propertyDetails.Rooms)
                    {
                        var hotelLiveAvailabilityResponseXml = await SendRequestAsync(
                            _helper.MultiSingleAvailabilityHotelRequest(propertyDetails, new List<RoomDetails> { room }),
                            "LiveAvailabilityCheck",
                            propertyDetails,
                            AmadeusHotelsSoapActions.HotelMultiSingleHotelSoapAction);

                        var processHotelAvailRsLive = new ProcessHotelAvailRS(hotelLiveAvailabilityResponseXml, _serializer);

                        CheckAvailability(propertyDetails, room, processHotelAvailRsLive, out bool isConverted);
                        convertedList.Add(isConverted);

                        var hotelLiveAvailabilityResponse = processHotelAvailRsLive.HotelAvailResponse;
                        liveAvailabilityResponses.Add(hotelLiveAvailabilityResponse);

                        if (propertyDetails.Warnings.Any())
                        {
                            break;
                        }

                        UpdateBookingCode(room, processHotelAvailRsLive.GetNewBookingCode());
                    }
                }
                else
                {
                    var hotelLiveAvailabilityResponseXml = await SendRequestAsync(
                        _helper.MultiSingleAvailabilityHotelRequest(propertyDetails, propertyDetails.Rooms),
                        "LiveAvailabilityCheck",
                        propertyDetails,
                        AmadeusHotelsSoapActions.HotelMultiSingleHotelSoapAction);

                    var processHotelAvailRsLive = new ProcessHotelAvailRS(hotelLiveAvailabilityResponseXml, _serializer);

                    foreach (var room in propertyDetails.Rooms)
                    {
                        CheckAvailability(propertyDetails, room, processHotelAvailRsLive, out bool isConverted);
                        convertedList.Add(isConverted);

                        var hotelLiveAvailabilityResponse = processHotelAvailRsLive.HotelAvailResponse;
                        liveAvailabilityResponses.Add(hotelLiveAvailabilityResponse);

                        if (propertyDetails.Warnings.Any()) break;
                    }
                }

                var sessionIds = new List<string>();
                var paymentCodes = new List<string>();

                if (!propertyDetails.Warnings.Any())
                {
                    for (int roomIndex = 0; roomIndex < propertyDetails.Rooms.Count; roomIndex++)
                    {
                        var (sessionId, paymentCode) = await CheckRoomPriceAsync(
                            propertyDetails,
                            propertyDetails.Rooms[roomIndex],
                            roomIndex,
                            errata,
                            convertedList,
                            liveAvailabilityResponses);

                        sessionIds.Add(sessionId);
                        paymentCodes.Add(paymentCode);

                        if (propertyDetails.Warnings.Any())
                        {
                            break;
                        }
                    }
                }

                // Store session token and payment codes on tpref1 and tpref2 so that they can be used in the rest of the booking process
                propertyDetails.TPRef1 = string.Join(",", sessionIds);
                propertyDetails.TPRef2 = string.Join(",", paymentCodes);

                if (propertyDetails.Warnings.Any())
                {
                    return false;
                }
                else
                {
                    decimal totalCost = propertyDetails.Rooms.Sum(x => x.LocalCost);
                    if (totalCost > 0 && propertyDetails.LocalCost != totalCost)
                    {
                        propertyDetails.LocalCost = totalCost;
                        propertyDetails.GrossCost = totalCost;
                        propertyDetails.Warnings.AddNew($"{propertyDetails.Source} Prebook", "Price Changed");
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBookExceptionRS", ex.ToString());
                return false;
            }
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var bookingReferences = new List<string>();
            var supplierSourceReferences = new List<string>();
            string[] roomPaymentCodes = AmadeusHelper.SplitBuilder(propertyDetails.TPRef2);
            string[] sessionIds = AmadeusHelper.SplitBuilder(propertyDetails.TPRef1);

            if (_settings.SplitMultiRoom(propertyDetails))
            {
                for (int roomIndex = 0; roomIndex < propertyDetails.Rooms.Count; roomIndex++)
                {
                    var (bookingReference, supplierSourceReference) = await SplitMultiRoomBookRoomAsync(
                        propertyDetails, new List<RoomDetails> { propertyDetails.Rooms[roomIndex] },
                        sessionIds[roomIndex],
                        roomPaymentCodes[roomIndex],
                        FailedBookingReference);

                    bookingReferences.Add(bookingReference);
                    supplierSourceReferences.Add(supplierSourceReference);
                }
            }
            else
            {
                var (bookingReference, supplierSourceReference) = await BookRoomsAsync(
                    propertyDetails,
                    propertyDetails.Rooms.ToArray(),
                    roomPaymentCodes,
                    sessionIds[0],
                    FailedBookingReference);

                bookingReferences.Add(bookingReference);
                supplierSourceReferences.Add(supplierSourceReference);
            }

            propertyDetails.SupplierSourceReference = string.Join("|", supplierSourceReferences);
            propertyDetails.SourceSecondaryReference = string.Join("|", bookingReferences);

            if (bookingReferences.Any(r => r == FailedBookingReference)) return "failed";

            return propertyDetails.SourceSecondaryReference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var cancellationReferences = new List<string>();
            string[] bookingReferences = AmadeusHelper.SplitBuilder(propertyDetails.SourceSecondaryReference, '|');

            if (_settings.SplitMultiRoom(propertyDetails))
            {
                for (int roomIndex = 0; roomIndex < propertyDetails.Rooms.Count; roomIndex++)
                {
                    string bookingReference = bookingReferences[roomIndex];
                    if (bookingReference != FailedBookingReference)
                    {
                        cancellationReferences.Add(await CancelRoomsAsync(propertyDetails, bookingReference));
                    }
                }
            }
            else
            {
                string bookingReference = propertyDetails.SourceReference;
                if (bookingReference != FailedBookingReference)
                {
                    cancellationReferences.Add(await CancelRoomsAsync(propertyDetails, bookingReference));
                }
            }

            return new ThirdPartyCancellationResponse
            {
                Success = cancellationReferences.All(c => c != FailedBookingReference),
                CostRecievedFromThirdParty = false,
                TPCancellationReference = string.Join("|", cancellationReferences)
            };
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

        public void EndSession(PropertyDetails propertyDetails)
        {
            if (string.IsNullOrEmpty(propertyDetails.TPRef1))
            {
                propertyDetails.Warnings.AddNew("End Session", "No session IDs found when attempting to end the sessions.");
                return;
            }

            foreach (string sSessionId in propertyDetails.TPRef1.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                // todo - make async
                SendRequestAsync(
                    _helper.EndSession(propertyDetails, sSessionId),
                    "SignOut",
                    propertyDetails,
                    AmadeusHotelsSoapActions.SignOutSoapAction).RunSynchronously();
            }
        }

        public async Task<XmlDocument> SendRequestAsync(
            XmlDocument sRequest,
            string logFileName,
            PropertyDetails propertyDetails,
            string soapAction,
            IEnumerable<DataMaskDef>? dataMasking = null)
        {
            var webRequest = new Request
            {
                EndPoint = _settings.URL(propertyDetails),
                Method = RequestMethod.POST,
                Source = Source,
                CreateLog = true,
                LogFileName = logFileName,
                UseGZip = _settings.UseGZIP(propertyDetails),
                ContentType = ContentTypes.Text_xml,
                SOAP = true,
                SoapAction = soapAction,
                DataMasking = dataMasking?.ToList() ?? new()
            };
            webRequest.SetRequest(sRequest);
            await webRequest.Send(_httpClient, _logger);

            propertyDetails.AddLog(logFileName, webRequest);

            return _serializer.CleanXmlNamespaces(webRequest.ResponseXML);
        }

        private async Task<string> CancelRoomsAsync(PropertyDetails propertyDetails, string bookingReference)
        {
            string cancellationReference = FailedBookingReference;
            try
            {
                var cancelResponseXml = await SendRequestAsync(
                    _helper.CancelPNR(propertyDetails, bookingReference),
                    "CancelPNR",
                    propertyDetails,
                    AmadeusHotelsSoapActions.CancelPNRSoapAction);

                var response = _serializer.DeSerialize<EnvelopeResponse<PNRReply>>(cancelResponseXml).Body.Content;
                if (response.GeneralErrorInfo.Length > 0)
                {
                    propertyDetails.Warnings.AddNew("CancellationFailed", "Error in CancelResponse");
                }
                else
                {
                    cancellationReference = bookingReference;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("CancelException", ex.ToString());
            }

            return cancellationReference;
        }

        private static void CheckAvailability(
            PropertyDetails propertyDetails,
            RoomDetails room,
            ProcessHotelAvailRS processHotelAvailRsLive,
            out bool isConverted)
        {
            string[] references = AmadeusHelper.SplitBuilder(room.ThirdPartyReference, '|');
            string bookingCode = references[0];
            string ratePlanCode = references[1];
            string sMealBasisCode = references[3];
            isConverted = references[2].ToSafeBoolean();

            bool hasPropertyNode = processHotelAvailRsLive.FindLivePropertyNodes(
                bookingCode,
                ratePlanCode,
                room.RoomTypeCode,
                isConverted,
                sMealBasisCode,
                room.LocalCost);

            if (!hasPropertyNode)
            {
                propertyDetails.Warnings.AddNew($"{propertyDetails.Source} Prebook Failure", "RoomRate Not Returned");
            }
            else
            {
                isConverted = processHotelAvailRsLive.IsRoomTypeConverted();
                string availabilityStatus = processHotelAvailRsLive.AvailabilityStatus();

                if (availabilityStatus != "AvailableForSale")
                {
                    propertyDetails.Warnings.AddNew($"{propertyDetails.Source} Prebook Failure", $"Availability [{availabilityStatus}]");
                }
            }
        }

        public static void UpdateBookingCode(RoomDetails room, string newBookingCode)
        {
            string[] splitTpReference = AmadeusHelper.SplitBuilder(room.ThirdPartyReference, '|');
            splitTpReference[0] = newBookingCode;
            room.ThirdPartyReference = string.Join("|", splitTpReference);
        }

        private async Task<(string, string)> CheckRoomPriceAsync(
            PropertyDetails propertyDetails,
            RoomDetails room,
            int roomIndex,
            List<string> errata,
            List<bool> isConverted,
            List<EnvelopeResponse<OTAHotelAvailRS>> liveAvailabilityResponses)
        {
            string sessionId = GetSessionToken(liveAvailabilityResponses[roomIndex]);

            var priceCheckResponseXml = await SendRequestAsync(
                _helper.PricingCheckRequest(propertyDetails, room, sessionId, _settings.SplitMultiRoom(propertyDetails)),
                    "PriceCheck",
                    propertyDetails,
                    AmadeusHotelsSoapActions.EnhancedPricingSoapAction);

            var processHotelAvailRsPrice = new ProcessHotelAvailRS(priceCheckResponseXml, _serializer);

            bool hasPropertyNode = processHotelAvailRsPrice.FindPricePropertyNodes(
                room.RoomTypeCode,
                room.ThirdPartyReference.Split('|')[0],
                isConverted[roomIndex]);

            bool errataRequired = false;
            decimal totalCost = hasPropertyNode ? processHotelAvailRsPrice.GetTotalCost(out errataRequired) : 0;

            if (totalCost == 0)
            {
                propertyDetails.Warnings.AddNew($"{propertyDetails.Source} Prebook Failure",
                    "No Valid Supplier Cost");
            }
            else
            {
                room.LocalCost = totalCost;
                room.GrossCost = totalCost;

                var priceCheckResponse = processHotelAvailRsPrice.HotelAvailResponse;
                if (errataRequired)
                {
                    GetErrata(propertyDetails, priceCheckResponse);
                }

                // Update Booking code as the providers could return different booking code on the pricing response than the search response
                UpdateBookingCode(room, processHotelAvailRsPrice.GetNewBookingCode());
                propertyDetails.Cancellations = processHotelAvailRsPrice.GetCancellations(propertyDetails, errata);
            }

            string sessionIdRef = _helper.IncrementSession(sessionId);
            string paymentCode = processHotelAvailRsPrice.SetPaymentCode();

            return (sessionIdRef, paymentCode);
        }

        private string GetSessionToken(EnvelopeResponse<OTAHotelAvailRS> hotelLiveAvailabilityResponse)
        {
            var sessionToken = new AmadeusSessionToken(hotelLiveAvailabilityResponse.Header.Session);
            return _helper.IncrementSession(sessionToken.ToString());
        }

        private void GetErrata(PropertyDetails propertyDetails, EnvelopeResponse<OTAHotelAvailRS> response)
        {
            foreach (var vendorErrata in response.Body.Content.HotelStays
                         .Select(x => x.BasicPropertyInfo)
                         .SelectMany(b => b.VendorMessages)
                         .Select(v => v.SubSection.Paragraph))
            {
                if (!string.IsNullOrEmpty(vendorErrata.Text))
                {
                    propertyDetails.Errata.AddNew("Property Errata", vendorErrata.Text);
                }
            }
        }

        private async Task<(string, string)> SplitMultiRoomBookRoomAsync(
            PropertyDetails propertyDetails,
            List<RoomDetails> rooms,
            string sessionId,
            string paymentCode,
            string supplierSourceReference)
        {
            string bookingReference = FailedBookingReference;
            try
            {
                string bookingHolderTatooNumber = await InitialAddMultiElementsPnrAsync(propertyDetails, rooms.ToArray(), sessionId);

                // Create Hotel Reservation
                var hotelSellResponseXml = await SendRequestAsync(
                    _helper.CreateSplitMultiRoomHotelSellRequest(
                        propertyDetails,
                        rooms,
                        bookingHolderTatooNumber,
                        paymentCode,
                        ref sessionId),
                    "HotelSell",
                    propertyDetails,
                    AmadeusHotelsSoapActions.HotelSellSoapAction,
                    GetHotelSellDataMasking());

                (sessionId, supplierSourceReference, bookingReference) = await GetBookingReferencesAsync(
                    propertyDetails,
                    hotelSellResponseXml,
                    sessionId,
                    supplierSourceReference,
                    bookingReference);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("BookException", ex.ToString());
            }

            return (bookingReference, supplierSourceReference);
        }

        private async Task<string> InitialAddMultiElementsPnrAsync(PropertyDetails propertyDetails, RoomDetails[] rooms, string sessionId)
        {
            string bookingHolderTatooNumber = string.Empty;

            // Here we create the customer PNR. The following elements of the PNR are sent
            // 1.Name
            // 2.Contact Information(AP Element)
            var addMultiElementsInitialResponseXml = await SendRequestAsync(
                 _helper.PNRAddTravellerInfo(
                     propertyDetails,
                     rooms,
                     sessionId),
                 "PNR_AddMultiElements",
                 propertyDetails,
                 AmadeusHotelsSoapActions.PNRAddMultiElementSoapAction);

            // Add warnings if an error is returned in the response
            if (addMultiElementsInitialResponseXml.InnerXml.Contains("PNR_Reply"))
            {
                var response = _serializer.DeSerialize<EnvelopeResponse<PNRReply>>(addMultiElementsInitialResponseXml);
                if (!string.IsNullOrEmpty(response.Body.Content.GeneralErrorInfo.First().MessageErrorText.Text)
                    || !string.IsNullOrEmpty(response.Body.Content.GeneralErrorInfo.First().MessageErrorInformation.ErrorDetail
                        .ErrorCode))
                {
                    propertyDetails.Warnings.AddNew("PNR_AddMultiElementsFailed", response.Body.Content.GeneralErrorInfo.First().MessageErrorText.Text);
                    bookingHolderTatooNumber = response.Body.Content.TravellerInfo.ElementManagementPassenger.Reference.Number.ToString();
                }
            }

            if (addMultiElementsInitialResponseXml.InnerXml.Contains("Fault"))
            {
                var response = _serializer.DeSerialize<EnvelopeResponse<Fault>>(_serializer.CleanXmlNamespaces(addMultiElementsInitialResponseXml));
                propertyDetails.Warnings.AddNew("PNR_AddMultiElementsFailed", response.Body.Content.FaultString);
            }

            return bookingHolderTatooNumber;
        }

        private async Task<(string, string)> BookRoomsAsync(
            PropertyDetails propertyDetails,
            RoomDetails[] rooms,
            string[] roomPaymentCodes,
            string sessionId,
            string supplierSourceReference)
        {
            string bookingReference = FailedBookingReference;
            try
            {
                string bookingHolderTatooNumber = await InitialAddMultiElementsPnrAsync(propertyDetails, rooms, sessionId);

                // Create Hotel Reservation
                var hotelSellResponseXml = await SendRequestAsync(
                    _helper.CreateHotelSellRequest(
                        propertyDetails,
                        rooms,
                        bookingHolderTatooNumber,
                        roomPaymentCodes,
                        ref sessionId),
                    "HotelSell",
                    propertyDetails,
                    AmadeusHotelsSoapActions.HotelSellSoapAction,
                    GetHotelSellDataMasking());

                (sessionId, supplierSourceReference, bookingReference) = await GetBookingReferencesAsync(propertyDetails, hotelSellResponseXml, sessionId, supplierSourceReference, bookingReference);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("BookException", ex.ToString());
            }

            return (bookingReference, supplierSourceReference);
        }

        private static List<DataMaskDef> GetHotelSellDataMasking()
        {
            return new List<DataMaskDef>
            {
                new("/soap:Envelope/soap:Body/Hotel_Sell/roomStayData/roomList/guaranteeOrDeposit/groupCreditCardInfo/creditCardInfo/ccInfo/cardNumber", @"\d{12}"),
                new("/soap:Envelope/soap:Body/Hotel_Sell/roomStayData/roomList/guaranteeOrDeposit/groupCreditCardInfo/creditCardInfo/ccInfo/securityId", "[0-9]*")
            };
        }

        private async Task<(string, string, string)> GetBookingReferencesAsync(
            PropertyDetails propertyDetails,
            XmlDocument hotelSellResponse,
            string sessionId,
            string supplierSourceReference,
            string bookingReference)
        {
            if (hotelSellResponse.InnerXml.Contains("Fault"))
            {
                var response = _serializer.DeSerialize<EnvelopeResponse<Fault>>(hotelSellResponse);
                propertyDetails.Warnings.AddNew("Hotel_SellFailed", response.Body.Content.FaultString);
            }
            else
            {
                if (_settings.PostBookPNRAddMultiElement())
                {
                    // Create Final PNR With Ticketing
                    var pnrReservation = await SendRequestAsync(
                        _helper.PNREndTransaction(propertyDetails, ref sessionId),
                        "PNR_AddMultiElementsFinal",
                        propertyDetails,
                        AmadeusHotelsSoapActions.PNRAddMultiElementSoapAction);

                    var response = _serializer.DeSerialize<EnvelopeResponse<PNRReply>>(pnrReservation)
                        .Body
                        .Content;

                    if (!string.IsNullOrEmpty(response.PnrHeader.ReservationInfo.Reservation.ControlNumber))
                    {
                        bookingReference = response.PnrHeader.ReservationInfo.Reservation.ControlNumber;
                        supplierSourceReference = response.OriginDestinationDetails.ItineraryInfo.GeneralOption
                            .First(x => x.OptionDetail.Type == "CF")
                            .OptionDetail
                            .Type;
                    }
                }
                else
                {
                    var response = _serializer.DeSerialize<EnvelopeResponse<HotelSellReply>>(
                        _serializer.CleanXmlNamespaces(hotelSellResponse))
                        .Body
                        .Content;

                    if (!string.IsNullOrEmpty(response.RoomStayData.PnrInfo.ReservationControlInfoPNR.Reservation
                            .ControlNumber))
                    {
                        bookingReference = response.RoomStayData.PnrInfo.ReservationControlInfoPNR.Reservation.ControlNumber;
                    }

                    if (!string.IsNullOrEmpty(response.RoomStayData.GlobalBookingInfo.BookingInfo.Reservation
                            .ControlNumber))
                    {
                        bookingReference = response.RoomStayData.GlobalBookingInfo.BookingInfo.Reservation.ControlNumber;
                    }

                }

                // End the session
                await SendRequestAsync(
                    _helper.EndSession(propertyDetails, sessionId),
                    "SignOut",
                    propertyDetails,
                    AmadeusHotelsSoapActions.SignOutSoapAction);
            }

            return (sessionId, supplierSourceReference, bookingReference);
        }
    }
}