namespace ThirdParty.CSSuppliers.Travelgate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class Travelgate : IThirdParty, IMultiSource
    {
        #region Constructor

        private readonly ITravelgateSettings _settings;

        private readonly ITPSupport _support;

        private readonly HttpClient _httpClient;

        private readonly ISecretKeeper _secretKeeper;

        private readonly ISerializer _serializer;

        private readonly ILogger<Travelgate> _logger;

        public Travelgate(
            ITravelgateSettings settings,
            ITPSupport support,
            HttpClient httpClient,
            ISecretKeeper secretKeeper,
            ISerializer serializer,
            ILogger<Travelgate> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region Properties

        public List<string> Sources => TravelgateSupport.TravelgateSources;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
            => _settings.get_OffsetCancellationDays(searchDetails, source);

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
            => _settings.get_AllowCancellations(searchDetails, source);

        public bool SupportsRemarks => false;

        public bool RequiresVCard(VirtualCardInfo info, string source)
            => _settings.get_RequiresVCard(info, source);

        private char get_ReferenceDelimiter(IThirdPartyAttributeSearch searchDetails, string source)
        {
            string referenceDelimiter = _settings.get_ReferenceDelimiter(searchDetails, source);
            if (string.IsNullOrEmpty(referenceDelimiter))
            {
                referenceDelimiter = "~";
            }

            return referenceDelimiter[0];
        }

        #endregion

        #region Prebook

        public bool PreBook(PropertyDetails propertyDetails)
        {
            var requestXml = new XmlDocument();
            var responseXml = new XmlDocument();
            bool success;

            try
            {
                // Send request
                string request = GenericRequest(BuildPrebookRequest(propertyDetails), propertyDetails);
                requestXml.LoadXml(request);
                responseXml = SendRequest(request, "Prebook", propertyDetails);

                // Retrieve, decode and clean provider response
                string decodedProviderResponse = responseXml.SafeNodeValue("Envelope/Body/ValuationResponse/ValuationResult/providerRS/rs");
                var providerResponse = new XmlDocument();
                providerResponse.LoadXml(decodedProviderResponse);

                // Responses only contain a cost for the entire booking - so no individual room prices
                // If there's a difference in total we split the new cost against all the rooms
                decimal bookingCost = providerResponse.SelectSingleNode("ValuationRS/Price/@amount").InnerText.ToSafeDecimal();

                if (bookingCost == 0m)
                {
                    return false;
                }

                if (bookingCost != propertyDetails.GrossCost)
                {
                    decimal perRoomCost = bookingCost / propertyDetails.Rooms.Count;

                    foreach (var roomDetails in propertyDetails.Rooms)
                    {
                        roomDetails.LocalCost = perRoomCost;
                        roomDetails.GrossCost = perRoomCost;
                    }
                }

                propertyDetails.LocalCost = bookingCost;

                string errata = providerResponse.SafeNodeValue("ValuationRS/Remarks");
                if (!string.IsNullOrWhiteSpace(errata))
                {
                    propertyDetails.Errata.Add(new Erratum("Remarks", RemoveHtmlTags(errata)));
                }

                // Cancellations
                foreach (XmlNode cancellationNode in providerResponse.SelectNodes("ValuationRS/CancelPenalties/CancelPenalty"))
                {
                    int cancellationHours = cancellationNode.SelectSingleNode("HoursBefore").InnerText.ToSafeInt();
                    string cancellationType = cancellationNode.SelectSingleNode("Penalty/@type").InnerText;
                    decimal cancellationValue = cancellationNode.SelectSingleNode("Penalty").InnerText.ToSafeDecimal();
                    decimal cancellationCost = 0m;

                    // Three cancellation types: Importe - we are given a fixed fee; Porcentaje - a percentage of room cost; and Noches - a number of nights.
                    switch (cancellationType ?? "")
                    {
                        case "Importe":
                            {
                                cancellationCost = cancellationValue;
                                break;
                            }
                        case "Porcentaje":
                            {
                                cancellationCost = bookingCost * (cancellationValue / 100m);
                                break;
                            }
                        case "Noches":
                            {
                                cancellationCost = bookingCost / propertyDetails.Duration * cancellationValue;
                                break;
                            }
                    }

                    var fromDate = propertyDetails.ArrivalDate.AddHours(-1 * cancellationHours);
                    var endDate = propertyDetails.ArrivalDate;
                    if (fromDate > endDate)
                    {
                        endDate = new DateTime(2099, 12, 31);
                    }

                    var cancellationPolicy = new Cancellation(fromDate, endDate, cancellationCost);

                    propertyDetails.Cancellations.Add(cancellationPolicy);
                }

                // If no cancellation policies but considered a non-refundable rate, set a 100% cancellation policy effective from today
                bool nonRefundableRate = providerResponse.SafeNodeValue("ValuationRS/CancelPenalties/@nonRefundable").ToSafeBoolean();

                if (propertyDetails.Cancellations.Count == 0 && nonRefundableRate)
                {
                    var cancellationPolicy = new Cancellation(DateTime.Now, new DateTime(2099, 12, 31), bookingCost);
                    propertyDetails.Cancellations.Add(cancellationPolicy);
                }

                propertyDetails.Cancellations.Solidify(SolidifyType.Max);

                // Lastly, grab any parameters we may need for the booking, put an encrypted version in the TPRef
                if (providerResponse.SelectSingleNode("ValuationRS/Parameters") is not null)
                {
                    string encryptedParameters = _secretKeeper.Encrypt(providerResponse.SelectSingleNode("ValuationRS/Parameters").OuterXml);
                    propertyDetails.TPRef1 = encryptedParameters;
                }
                else
                {
                    propertyDetails.TPRef1 = "";
                }

                success = true;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                success = false;
            }

            // Lastly, add any logs
            if (!string.IsNullOrEmpty(requestXml.InnerXml))
            {
                propertyDetails.Logs.AddNew(propertyDetails.Source, string.Format("{0} PreBook Request", propertyDetails.Source), requestXml);
            }

            if (!string.IsNullOrEmpty(responseXml.InnerXml))
            {
                propertyDetails.Logs.AddNew(propertyDetails.Source, string.Format("{0} PreBook Response", propertyDetails.Source), responseXml);
            }

            return success;
        }

        #endregion

        #region Book

        public string Book(PropertyDetails propertyDetails)
        {
            var requestXml = new XmlDocument();
            var responseXml = new XmlDocument();
            string reference;

            try
            {
                // Send request
                string request = GenericRequest(BuildBookRequest(propertyDetails), propertyDetails);
                requestXml.LoadXml(request);
                responseXml = SendRequest(request, "Book", propertyDetails);

                // Retrieve, decode and clean provider response
                string decodedProviderResponse = responseXml.SafeNodeValue("Envelope/Body/ReservationResponse/ReservationResult/providerRS/rs");
                var providerResponse = new XmlDocument();
                providerResponse.LoadXml(decodedProviderResponse);

                if (providerResponse.SelectSingleNode("ReservationRS/ResStatus").InnerText.ToString() == "OK")
                {
                    string bookingReference = providerResponse.SelectSingleNode("ReservationRS/ProviderLocator").InnerText.ToString();
                    reference = bookingReference;
                }
                else
                {
                    reference = "Failed";
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
                reference = "Failed";
            }

            // Lastly, add any logs
            if (!string.IsNullOrEmpty(requestXml.InnerXml))
            {
                propertyDetails.Logs.AddNew(propertyDetails.Source, string.Format("{0} Book Request", propertyDetails.Source), requestXml);
            }

            if (!string.IsNullOrEmpty(responseXml.InnerXml))
            {
                propertyDetails.Logs.AddNew(propertyDetails.Source, string.Format("{0} Book Response", propertyDetails.Source), responseXml);
            }

            return reference;
        }

        #endregion

        #region Cancellation

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails)
        {
            var requestXml = new XmlDocument();
            var responseXml = new XmlDocument();
            var cancellationResponse = new ThirdPartyCancellationResponse();

            try
            {
                // Send request
                string request = GenericRequest(BuildCancellationRequest(propertyDetails), propertyDetails);
                requestXml.LoadXml(request);
                responseXml = SendRequest(request, "Cancellation", propertyDetails);

                // Retrieve, decode and clean provider response
                string decodedProviderResponse = responseXml.SafeNodeValue("Envelope/Body/CancelResponse/CancelResult/providerRS/rs");
                var providerResponse = new XmlDocument();
                providerResponse.LoadXml(decodedProviderResponse);

                if (providerResponse.SafeNodeValue("CancelRS/TransactionStatus/ResStatus") == "CN")
                {
                    string cancellationReference = providerResponse.SafeNodeValue("CancelId");

                    if (!string.IsNullOrEmpty(cancellationReference))
                    {
                        cancellationResponse.TPCancellationReference = cancellationReference;
                    }
                    else
                    {
                        cancellationResponse.TPCancellationReference = propertyDetails.SourceReference;
                    }

                    cancellationResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString());
                cancellationResponse.Success = false;
            }

            // Lastly, add any logs
            if (!string.IsNullOrEmpty(requestXml.InnerXml))
            {
                propertyDetails.Logs.AddNew(propertyDetails.Source, string.Format("{0} Cancellation Request", propertyDetails.Source), requestXml);
            }

            if (!string.IsNullOrEmpty(responseXml.InnerXml))
            {
                propertyDetails.Logs.AddNew(propertyDetails.Source, string.Format("{0} Cancellation Response", propertyDetails.Source), responseXml);
            }

            return cancellationResponse;
        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails)
        {
            return new ThirdPartyCancellationFeeResult();
        }

        #endregion

        #region Booking Search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return null;
        }

        #endregion

        #region Helpers

        public XmlDocument SendRequest(string requestString, string requestType, PropertyDetails propertyDetails)
        {
            string soapAction = string.Empty;
            switch (requestType.ToLower() ?? "")
            {
                case "prebook":
                    {
                        soapAction = _settings.get_PrebookSOAPAction(propertyDetails, propertyDetails.Source);
                        break;
                    }
                case "book":
                    {
                        soapAction = _settings.get_BookSOAPAction(propertyDetails, propertyDetails.Source);
                        break;
                    }
                case "cancellation":
                    {
                        soapAction = _settings.get_CancelSOAPAction(propertyDetails, propertyDetails.Source);
                        break;
                    }
            }

            var webRequest = new Request
            {
                EndPoint = _settings.get_URL(propertyDetails, propertyDetails.Source),
                Method = eRequestMethod.POST,
                Source = propertyDetails.Source,
                LogFileName = requestType,
                CreateLog = true,
                UseGZip = true,
                SoapAction = soapAction
            };
            webRequest.SetRequest(requestString);
            webRequest.Send(_httpClient, _logger);

            var responseXML = _serializer.CleanXmlNamespaces(webRequest.ResponseXML);

            return responseXML;
        }

        public string RemoveHtmlTags(string text)
        {
            return Regex.Replace(text, @"<(?:[^>=]|='[^']*'|=""[^""]*""|=[^'""][^\s>]*)*>", "");
        }

        #endregion

        #region Request builders

        public string BuildPrebookRequest(PropertyDetails propertyDetails)
        {
            var sbPrebookRequest = new StringBuilder();

            // Create valuation request
            sbPrebookRequest.Append("<ns:Valuation>");
            sbPrebookRequest.Append("<ns:valuationRQ>");
            sbPrebookRequest.Append("<ns:timeoutMilliseconds>180000</ns:timeoutMilliseconds>"); // max general timeout 180s
            sbPrebookRequest.Append("<ns:version>1</ns:version>");

            sbPrebookRequest.Append("<ns:providerRQ>");
            sbPrebookRequest.AppendFormat("<ns:code>{0}</ns:code>", _settings.get_ProviderCode(propertyDetails, propertyDetails.Source));
            sbPrebookRequest.AppendFormat("<ns:id>{0}</ns:id>", 1);
            sbPrebookRequest.Append("<ns:rqXML>");

            sbPrebookRequest.Append("<ValuationRQ>");

            sbPrebookRequest.Append("<timeoutMilliseconds>179700</timeoutMilliseconds>"); // has to be lower than general timeout
            sbPrebookRequest.Append("<source>");
            sbPrebookRequest.AppendFormat("<languageCode>{0}</languageCode>", _settings.get_LanguageCode(propertyDetails, propertyDetails.Source));
            sbPrebookRequest.Append("</source>");
            sbPrebookRequest.Append("<filterAuditData>");
            sbPrebookRequest.Append("<registerTransactions>true</registerTransactions>");
            sbPrebookRequest.Append("</filterAuditData>");

            sbPrebookRequest.Append("<Configuration>");
            sbPrebookRequest.AppendFormat("<User>{0}</User>", _settings.get_ProviderUsername(propertyDetails, propertyDetails.Source));
            sbPrebookRequest.AppendFormat("<Password>{0}</Password>", _settings.get_ProviderPassword(propertyDetails, propertyDetails.Source));
            sbPrebookRequest.Append(AppendURLs(propertyDetails));
            sbPrebookRequest.Append(HttpUtility.HtmlDecode(_settings.get_Parameters(propertyDetails, propertyDetails.Source)));
            sbPrebookRequest.Append("</Configuration>");

            sbPrebookRequest.AppendFormat("<StartDate>{0}</StartDate>", propertyDetails.ArrivalDate.ToString("dd/MM/yyyy"));
            sbPrebookRequest.AppendFormat("<EndDate>{0}</EndDate>", propertyDetails.DepartureDate.ToString("dd/MM/yyyy"));

            // Add hotel option parameters
            // Check if we have any first before we add the parameters node
            // Don't do multiroom bookings for rooms across different options, so can just pluck out the first room's parameters
            string referenceDelimiter = _settings.get_ReferenceDelimiter(propertyDetails, propertyDetails.Source);

            if (string.IsNullOrEmpty(referenceDelimiter))
            {
                referenceDelimiter = "~";
            }

            string encryptedParameters = propertyDetails.Rooms[0].ThirdPartyReference.Split(referenceDelimiter[0])[4];

            if (!string.IsNullOrEmpty(encryptedParameters))
            {
                var parameters = new XmlDocument();
                parameters.LoadXml(_secretKeeper.Decrypt(encryptedParameters));
                sbPrebookRequest.Append(parameters.OuterXml);
            }

            sbPrebookRequest.AppendFormat("<MealPlanCode>{0}</MealPlanCode>", propertyDetails.Rooms[0].ThirdPartyReference.Split(referenceDelimiter[0])[7]);
            sbPrebookRequest.AppendFormat("<HotelCode>{0}</HotelCode>", propertyDetails.TPKey);
            sbPrebookRequest.AppendFormat("<PaymentType>{0}</PaymentType>", propertyDetails.Rooms[0].ThirdPartyReference.Split(referenceDelimiter[0])[3]);
            sbPrebookRequest.Append("<OptionType>Hotel</OptionType>");

            string nationality = "";
            string nationalityLookupValue = _support.TPNationalityLookup(ThirdParties.TRAVELGATE, propertyDetails.NationalityID);
            string defaultNationality = _settings.get_DefaultNationality(propertyDetails, propertyDetails.Source);

            if (!string.IsNullOrEmpty(nationalityLookupValue))
            {
                nationality = nationalityLookupValue;
            }
            else if (!string.IsNullOrEmpty(defaultNationality))
            {
                nationality = defaultNationality;
            }
            if (!string.IsNullOrEmpty(nationality))
            {
                sbPrebookRequest.AppendFormat("<Nationality>{0}</Nationality>", nationality);
            }

            sbPrebookRequest.Append("<Rooms>");

            int roomId = 1;
            foreach (var room in propertyDetails.Rooms)
            {
                sbPrebookRequest.AppendFormat(
                    "<Room id = \"{0}\" roomCandidateRefId = \"{3}\" code = \"{1}\" description = \"{2}\"/>",
                    room.ThirdPartyReference.Split(referenceDelimiter[0])[0],
                    room.ThirdPartyReference.Split(referenceDelimiter[0])[1],
                    room.ThirdPartyReference.Split(referenceDelimiter[0])[2],
                    roomId);

                roomId += 1;
            }

            sbPrebookRequest.Append("</Rooms>");
            sbPrebookRequest.Append("<RoomCandidates>");

            roomId = 1;
            int passengerId;

            foreach (var room in propertyDetails.Rooms)
            {
                sbPrebookRequest.AppendFormat("<RoomCandidate id = \"{0}\">", roomId);
                sbPrebookRequest.Append("<Paxes>");

                passengerId = 1; // every room starts with PaxID 1
                foreach (var passenger in room.Passengers)
                {
                    var age = default(int);
                    switch (passenger.PassengerType)
                    {
                        case PassengerType.Adult:
                            {
                                age = 30;
                                break;
                            }
                        case PassengerType.Child:
                            {
                                age = passenger.Age;
                                break;
                            }
                        case PassengerType.Infant:
                            {
                                age = 1;
                                break;
                            }
                    }

                    sbPrebookRequest.AppendFormat("<Pax age = \"{0}\" id = \"{1}\"/>", age, passengerId);

                    passengerId += 1;
                }

                sbPrebookRequest.Append("</Paxes>");
                sbPrebookRequest.Append("</RoomCandidate>");

                roomId += 1;
            }

            sbPrebookRequest.Append("</RoomCandidates>");
            sbPrebookRequest.Append("</ValuationRQ>");

            sbPrebookRequest.Append("</ns:rqXML>");
            sbPrebookRequest.Append("</ns:providerRQ>");
            sbPrebookRequest.Append("</ns:valuationRQ>");

            sbPrebookRequest.Append("</ns:Valuation>");

            return sbPrebookRequest.ToString();
        }

        public string BuildBookRequest(PropertyDetails propertyDetails)
        {
            string source = propertyDetails.Source;

            char delimiter = get_ReferenceDelimiter(propertyDetails, source);

            var sbBookRequest = new StringBuilder();

            string paymentType = propertyDetails.Rooms[0].ThirdPartyReference.Split(delimiter)[3];

            sbBookRequest.Append("<ns:Reservation>");
            sbBookRequest.Append("<ns:reservationRQ>");
            sbBookRequest.Append("<ns:timeoutMilliseconds>180000</ns:timeoutMilliseconds>"); // max general timeout 180s
            sbBookRequest.Append("<ns:version>1</ns:version>");

            sbBookRequest.Append("<ns:providerRQ>");
            sbBookRequest.AppendFormat("<ns:code>{0}</ns:code>", _settings.get_ProviderCode(propertyDetails, source));
            sbBookRequest.AppendFormat("<ns:id>{0}</ns:id>", 1);
            sbBookRequest.Append("<ns:rqXML>");

            sbBookRequest.Append("<ReservationRQ>");

            sbBookRequest.Append("<timeoutMilliseconds>179700</timeoutMilliseconds>"); // has to be lower than general timeout
            sbBookRequest.Append("<source>");
            sbBookRequest.AppendFormat("<languageCode>{0}</languageCode>", _settings.get_LanguageCode(propertyDetails, source));
            sbBookRequest.Append("</source>");
            sbBookRequest.Append("<filterAuditData>");
            sbBookRequest.Append("<registerTransactions>true</registerTransactions>");
            sbBookRequest.Append("</filterAuditData>");

            sbBookRequest.Append("<Configuration>");
            sbBookRequest.AppendFormat("<User>{0}</User>", _settings.get_ProviderUsername(propertyDetails, source));
            sbBookRequest.AppendFormat("<Password>{0}</Password>", _settings.get_ProviderPassword(propertyDetails, source));
            sbBookRequest.Append(AppendURLs(propertyDetails));
            sbBookRequest.Append(HttpUtility.HtmlDecode(_settings.get_Parameters(propertyDetails, source)));
            sbBookRequest.Append("</Configuration>");

            if (_settings.get_SendGUIDReference(propertyDetails, source))
            {
                sbBookRequest.AppendFormat("<ClientLocator>{0}</ClientLocator>", propertyDetails.BookingReference.TrimEnd() + Guid.NewGuid().ToString());
            }
            else
            {
                sbBookRequest.AppendFormat("<ClientLocator>{0}</ClientLocator>", propertyDetails.BookingReference);
            }

            sbBookRequest.AppendLine(BuildCardDetails(propertyDetails));

            sbBookRequest.AppendFormat("<StartDate>{0}</StartDate>", propertyDetails.ArrivalDate.ToString("dd/MM/yyyy"));
            sbBookRequest.AppendFormat("<EndDate>{0}</EndDate>", propertyDetails.DepartureDate.ToString("dd/MM/yyyy"));

            // Add hotel option parameters
            // Check if we have any first before we add the parameters node
            // Don't do multiroom bookings for rooms across different options, so can just pluck out the first room's parameters
            string encryptedParameters = propertyDetails.TPRef1;

            if (!string.IsNullOrEmpty(encryptedParameters))
            {
                var parameters = new XmlDocument();
                parameters.LoadXml(_secretKeeper.Decrypt(encryptedParameters));
                sbBookRequest.Append(parameters.OuterXml);
            }

            sbBookRequest.AppendFormat("<MealPlanCode>{0}</MealPlanCode>", propertyDetails.Rooms[0].ThirdPartyReference.Split(delimiter)[7]);
            sbBookRequest.AppendFormat("<HotelCode>{0}</HotelCode>", propertyDetails.TPKey);

            string nationality = "";
            string nationalityLookupValue = _support.TPNationalityLookup(source, propertyDetails.NationalityID);
            string defaultNationality = _settings.get_DefaultNationality(propertyDetails, source);

            if (!string.IsNullOrEmpty(nationalityLookupValue))
            {
                nationality = nationalityLookupValue;
            }
            else if (!string.IsNullOrEmpty(defaultNationality))
            {
                nationality = defaultNationality;
            }
            if (!string.IsNullOrEmpty(nationality))
            {
                sbBookRequest.AppendFormat("<Nationality>{0}</Nationality>", nationality);
            }

            string currencyCode = _support.TPCurrencyLookup(source, propertyDetails.CurrencyCode);
            // send gross cost as gross net down can be used (local is the net)
            sbBookRequest.AppendFormat("<Price currency = \"{0}\" amount = \"{1}\" binding = \"{2}\" commission = \"{3}\"/>", currencyCode, propertyDetails.GrossCost, propertyDetails.Rooms[0].ThirdPartyReference.Split(delimiter)[6], propertyDetails.Rooms[0].ThirdPartyReference.Split(delimiter)[5]);
            sbBookRequest.Append("<ResGuests>");
            sbBookRequest.Append("<Guests>");

            int passengerCount;
            int roomCount = 1;
            foreach (var room in propertyDetails.Rooms)
            {
                passengerCount = 1;

                foreach (var passenger in room.Passengers)
                {
                    sbBookRequest.AppendFormat("<Guest roomCandidateId = \"{0}\" paxId = \"{1}\">", roomCount, passengerCount);
                    sbBookRequest.AppendFormat("<GivenName>{0}</GivenName>", passenger.FirstName);
                    sbBookRequest.AppendFormat("<SurName>{0}</SurName>", passenger.LastName);
                    sbBookRequest.Append("</Guest>");

                    passengerCount++;
                }

                roomCount++;
            }

            sbBookRequest.Append("</Guests>");
            sbBookRequest.Append("</ResGuests>");
            sbBookRequest.AppendFormat("<PaymentType>{0}</PaymentType>", paymentType);

            sbBookRequest.Append("<Rooms>");

            roomCount = 1;

            foreach (var room in propertyDetails.Rooms)
            {
                sbBookRequest.AppendFormat("<Room id = \"{0}\" roomCandidateRefId = \"{3}\" code = \"{1}\" description = \"{2}\"/>", room.ThirdPartyReference.Split(delimiter)[0], room.ThirdPartyReference.Split(delimiter)[1], room.ThirdPartyReference.Split(delimiter)[2], roomCount);

                roomCount++;
            }

            sbBookRequest.Append("</Rooms>");
            sbBookRequest.Append("<RoomCandidates>");

            roomCount = 1;
            int passengerId;

            foreach (var room in propertyDetails.Rooms)
            {
                sbBookRequest.AppendFormat("<RoomCandidate id = \"{0}\">", roomCount);
                sbBookRequest.Append("<Paxes>");

                passengerId = 1;
                foreach (var passenger in room.Passengers)
                {
                    var age = default(int);
                    switch (passenger.PassengerType)
                    {
                        case PassengerType.Adult:
                            {
                                age = 30;
                                break;
                            }
                        case PassengerType.Child:
                            {
                                age = passenger.Age;
                                break;
                            }
                        case PassengerType.Infant:
                            {
                                age = 1;
                                break;
                            }
                    }

                    sbBookRequest.AppendFormat("<Pax age = \"{0}\" id = \"{1}\"/>", age, passengerId);

                    passengerId += 1;
                }

                sbBookRequest.Append("</Paxes>");
                sbBookRequest.Append("</RoomCandidate>");

                roomCount += 1;
            }

            sbBookRequest.Append("</RoomCandidates>");

            if (!string.IsNullOrEmpty(propertyDetails.BookingComments.ToString()))
            {
                sbBookRequest.Append("<Remarks>");
                sbBookRequest.Append(propertyDetails.BookingComments.ToString().Trim());
                sbBookRequest.Append("</Remarks>");
            }

            sbBookRequest.Append("</ReservationRQ>");

            sbBookRequest.Append("</ns:rqXML>");
            sbBookRequest.Append("</ns:providerRQ>");
            sbBookRequest.Append("</ns:reservationRQ>");
            sbBookRequest.Append("</ns:Reservation>");

            return sbBookRequest.ToString();
        }

        public string BuildCancellationRequest(PropertyDetails propertyDetails)
        {
            var sbCancellationRequest = new StringBuilder();

            sbCancellationRequest.Append("<ns:Cancel>");
            sbCancellationRequest.Append("<ns:cancelRQ>");
            sbCancellationRequest.Append("<ns:timeoutMilliseconds>180000</ns:timeoutMilliseconds>"); // max general timeout 180s
            sbCancellationRequest.Append("<ns:version>1</ns:version>");

            sbCancellationRequest.Append("<ns:providerRQ>");
            sbCancellationRequest.AppendFormat("<ns:code>{0}</ns:code>", _settings.get_ProviderCode(propertyDetails, propertyDetails.Source));
            sbCancellationRequest.AppendFormat("<ns:id>{0}</ns:id>", 1);
            sbCancellationRequest.Append("<ns:rqXML>");

            sbCancellationRequest.AppendFormat("<CancelRQ  hotelCode=\"{0}\">", propertyDetails.TPKey);

            sbCancellationRequest.Append("<timeoutMilliseconds>179700</timeoutMilliseconds>"); // has to be lower than general timeout
            sbCancellationRequest.Append("<source>");
            sbCancellationRequest.AppendFormat("<languageCode>{0}</languageCode>", _settings.get_LanguageCode(propertyDetails, propertyDetails.Source));
            sbCancellationRequest.Append("</source>");
            sbCancellationRequest.Append("<filterAuditData>");
            sbCancellationRequest.Append("<registerTransactions>true</registerTransactions>");
            sbCancellationRequest.Append("</filterAuditData>");

            sbCancellationRequest.Append("<Configuration>");
            sbCancellationRequest.AppendFormat("<User>{0}</User>", _settings.get_ProviderUsername(propertyDetails, propertyDetails.Source));
            sbCancellationRequest.AppendFormat("<Password>{0}</Password>", _settings.get_ProviderPassword(propertyDetails, propertyDetails.Source));
            sbCancellationRequest.Append(AppendURLs(propertyDetails));
            sbCancellationRequest.Append(HttpUtility.HtmlDecode(_settings.get_Parameters(propertyDetails, propertyDetails.Source)));
            sbCancellationRequest.Append("</Configuration>");

            sbCancellationRequest.Append("<Locators>");
            sbCancellationRequest.AppendFormat("<Client>{0}</Client>", propertyDetails.BookingReference);
            sbCancellationRequest.AppendFormat("<Provider>{0}</Provider>", propertyDetails.SourceReference);
            sbCancellationRequest.Append("</Locators>");
            sbCancellationRequest.AppendFormat("<StartDate>{0}</StartDate>", propertyDetails.ArrivalDate.ToString("dd/MM/yyyy"));
            sbCancellationRequest.AppendFormat("<EndDate>{0}</EndDate>", propertyDetails.DepartureDate.ToString("dd/MM/yyyy"));
            sbCancellationRequest.Append("</CancelRQ>");

            sbCancellationRequest.Append("</ns:rqXML>");
            sbCancellationRequest.Append("</ns:providerRQ>");
            sbCancellationRequest.Append("</ns:cancelRQ>");
            sbCancellationRequest.Append("</ns:Cancel>");

            return sbCancellationRequest.ToString();
        }

        public string GenericRequest(string specificRequest, PropertyDetails propertyDetails)
        {
            var sbRequest = new StringBuilder();

            sbRequest.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" ");
            sbRequest.Append("xmlns:ns=\"http://schemas.xmltravelgate.com/hub/2012/06\" ");
            sbRequest.Append("xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">");
            sbRequest.Append("<soapenv:Header>");
            sbRequest.Append("<wsse:Security>");
            sbRequest.Append("<wsse:UsernameToken>");
            sbRequest.AppendFormat("<wsse:Username>{0}</wsse:Username>", _settings.get_Username(propertyDetails, propertyDetails.Source));
            sbRequest.AppendFormat("<wsse:Password>{0}</wsse:Password>", _settings.get_Password(propertyDetails, propertyDetails.Source));
            sbRequest.Append("</wsse:UsernameToken>");
            sbRequest.Append("</wsse:Security>");
            sbRequest.Append("</soapenv:Header>");
            sbRequest.Append("<soapenv:Body>");
            sbRequest.Append(specificRequest);
            sbRequest.Append("</soapenv:Body>");
            sbRequest.Append("</soapenv:Envelope>");

            return sbRequest.ToString();
        }

        public string BuildCardDetails(PropertyDetails propertyDetails)
        {
            string source = propertyDetails.Source;

            char delimiter = get_ReferenceDelimiter(propertyDetails, source);

            string paymentType = propertyDetails.Rooms[0].ThirdPartyReference.Split(delimiter)[3];
            bool requiresVCard = _settings.get_RequiresVCard(propertyDetails, source);

            var sb = new StringBuilder();

            if ((requiresVCard && paymentType == "CardBookingPay") || paymentType == "CardCheckInPay")
            {
                string cardHolderName = propertyDetails.GeneratedVirtualCard.CardHolderName;

                if (string.IsNullOrEmpty(cardHolderName))
                {
                    cardHolderName = _settings.get_CardHolderName(propertyDetails, source);
                }

                sb.AppendLine("<CardInfo>");
                sb.AppendLine($"<CardCode>{_support.TPCreditCardLookup(source, propertyDetails.GeneratedVirtualCard.CardTypeID)}</CardCode>");
                sb.AppendLine($"<Number>{propertyDetails.GeneratedVirtualCard.CardNumber}</Number>");
                sb.AppendLine($"<Holder>{cardHolderName}</Holder>");
                sb.AppendLine("<ValidityDate>");
                sb.AppendLine($"<Month>{propertyDetails.GeneratedVirtualCard.ExpiryMonth.PadLeft(2, '0')}</Month>");
                sb.AppendLine($"<Year>{propertyDetails.GeneratedVirtualCard.ExpiryYear.Substring(2)}</Year>");
                sb.AppendLine("</ValidityDate>");
                sb.AppendLine($"<CVC>{propertyDetails.GeneratedVirtualCard.CVV}</CVC>");
                sb.AppendLine("</CardInfo>");
            }
            else if (!requiresVCard)
            {
                string encrytedCardDetails = _settings.get_EncryptedCardDetails(propertyDetails, source);
                if (string.IsNullOrWhiteSpace(encrytedCardDetails))
                {
                    return string.Empty;
                }

                // Card details are encryted in the form 'CardCode|CardNumber|ExpiryDate|CVC|CardHolderName'
                var decryptedCardDetails = _secretKeeper.Decrypt(encrytedCardDetails).Split('|');
                if (decryptedCardDetails.Count() != 5)
                {
                    return string.Empty;
                }

                var expiryDate = decryptedCardDetails[2].ToSafeDate();

                sb.AppendLine("<CardInfo>");
                sb.AppendLine($"<CardCode>{decryptedCardDetails[0]}</CardCode>");
                sb.AppendLine($"<Number>{decryptedCardDetails[1]}</Number>");
                sb.AppendLine($"<Holder>{decryptedCardDetails[4]}</Holder>");
                sb.AppendLine("<ValidityDate>");
                sb.AppendLine($"<Month>{expiryDate.Month.ToString().PadLeft(2, '0')}</Month>");
                sb.AppendLine($"<Year>{expiryDate.Year}</Year>");
                sb.AppendLine("</ValidityDate>");
                sb.AppendLine($"<CVC>{decryptedCardDetails[3]}</CVC>");
                sb.AppendLine("</CardInfo>");
            }

            return sb.ToString();
        }

        #endregion

        #region Helper class

        public string AppendURLs(PropertyDetails propertyDetails)
        {
            var sbURLXML = new StringBuilder();

            sbURLXML.AppendFormat("<UrlReservation>{0}</UrlReservation>", _settings.get_UrlReservation(propertyDetails, propertyDetails.Source));
            sbURLXML.AppendFormat("<UrlGeneric>{0}</UrlGeneric>", _settings.get_UrlGeneric(propertyDetails, propertyDetails.Source));
            sbURLXML.AppendFormat("<UrlAvail>{0}</UrlAvail>", _settings.get_UrlAvail(propertyDetails, propertyDetails.Source));
            sbURLXML.AppendFormat("<UrlValuation>{0}</UrlValuation>", _settings.get_UrlValuation(propertyDetails, propertyDetails.Source));

            return sbURLXML.ToString();
        }

        #endregion

        #region End session

        public void EndSession(PropertyDetails propertyDetails)
        {

        }

        #endregion
    }
}