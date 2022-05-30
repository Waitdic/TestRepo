using System;
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
using ThirdParty.Lookups;
using ThirdParty.Models;
using ThirdParty.Models.Property.Booking;

namespace ThirdParty.CSSuppliers
{

    public abstract class Travelgate : IThirdParty
    {

        #region Constructor

        public Travelgate(ITravelgateSettings settings, ITPSupport support, HttpClient httpClient, ISecretKeeper secretKeeper, ISerializer serializer, ILogger<Travelgate> logger)
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

        public abstract string Source { get; set; }

        private readonly ITravelgateSettings _settings;

        private readonly ITPSupport _support;

        private readonly HttpClient _httpClient;

        private readonly ISecretKeeper _secretKeeper;

        private readonly ISerializer _serializer;

        private readonly ILogger<Travelgate> _logger;

        private int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.get_OffsetCancellationDays(searchDetails, false);
        }

        int IThirdParty.OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails) => OffsetCancellationDays(searchDetails);

        public bool SupportsBookingSearch
        {
            get
            {
                return false;
            }
        }

        private bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.get_AllowCancellations(searchDetails);
        }

        bool IThirdParty.SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source) => SupportsLiveCancellation(searchDetails, source);

        public bool SupportsRemarks
        {
            get
            {
                return false;
            }
        }

        private bool TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails)
        {
            return false;
        }

        bool IThirdParty.TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails) => TakeSavingFromCommissionMargin(searchDetails);

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return _settings.get_RequiresVCard(info);
        }

        private char get_ReferenceDelimiter(IThirdPartyAttributeSearch SearchDetails)
        {
            string sReferenceDelimiter = _settings.get_ReferenceDelimiter(SearchDetails, false);
            if (string.IsNullOrEmpty(sReferenceDelimiter))
            {
                sReferenceDelimiter = "~";
            }

            return sReferenceDelimiter[0];
        }


        #endregion

        #region Prebook
        public bool PreBook(PropertyDetails oPropertyDetails)
        {

            var oRequestXML = new XmlDocument();
            var oResponseXML = new XmlDocument();
            bool bSuccess = false;

            try
            {

                // Send request
                string sRequest = GenericRequest(BuildPrebookRequest(oPropertyDetails), oPropertyDetails);
                oRequestXML.LoadXml(sRequest);
                oResponseXML = SendRequest(sRequest, "Prebook", oPropertyDetails);

                // Retrieve, decode and clean provider response
                string sDecodedProviderResponse = oResponseXML.SafeNodeValue("Envelope/Body/ValuationResponse/ValuationResult/providerRS/rs");
                var oProviderResponse = new XmlDocument();
                oProviderResponse.LoadXml(sDecodedProviderResponse);

                // Responses only contain a cost for the entire booking - so no individual room prices
                // If there's a difference in total we split the new cost against all the rooms
                decimal nBookingCost = oProviderResponse.SelectSingleNode("ValuationRS/Price/@amount").InnerText.ToSafeDecimal();

                if (nBookingCost == 0m)
                {
                    return false;
                }

                if (nBookingCost != oPropertyDetails.GrossCost)
                {

                    decimal nPerRoomCost = nBookingCost / oPropertyDetails.Rooms.Count;

                    foreach (RoomDetails oRoomDetails in oPropertyDetails.Rooms)
                    {
                        oRoomDetails.LocalCost = nPerRoomCost;
                        oRoomDetails.GrossCost = nPerRoomCost;
                    }

                }

                oPropertyDetails.LocalCost = nBookingCost;

                string errata = oProviderResponse.SafeNodeValue("ValuationRS/Remarks");
                if (!string.IsNullOrWhiteSpace(errata))
                {
                    oPropertyDetails.Errata.Add(new Erratum("Remarks", RemoveHtmlTags(errata)));
                }

                // Cancellations
                foreach (XmlNode oCancellationNode in oProviderResponse.SelectNodes("ValuationRS/CancelPenalties/CancelPenalty"))
                {

                    int iCancellationHours = oCancellationNode.SelectSingleNode("HoursBefore").InnerText.ToSafeInt();
                    string sCancellationType = oCancellationNode.SelectSingleNode("Penalty/@type").InnerText;
                    decimal nCancellationValue = oCancellationNode.SelectSingleNode("Penalty").InnerText.ToSafeDecimal();
                    decimal nCancellationCost = 0m;

                    // Three cancellation types: Importe - we are given a fixed fee; Porcentaje - a percentage of room cost; and Noches - a number of nights.
                    switch (sCancellationType ?? "")
                    {
                        case "Importe":
                            {
                                nCancellationCost = nCancellationValue;
                                break;
                            }
                        case "Porcentaje":
                            {
                                nCancellationCost = nBookingCost * (nCancellationValue / 100m);
                                break;
                            }
                        case "Noches":
                            {
                                nCancellationCost = nBookingCost / oPropertyDetails.Duration * nCancellationValue;
                                break;
                            }
                    }

                    var dFromDate = oPropertyDetails.ArrivalDate.AddHours(-1 * iCancellationHours);
                    var dEndDate = oPropertyDetails.ArrivalDate;
                    if (dFromDate > dEndDate)
                    {
                        dEndDate = new DateTime(2099, 12, 31);
                    }

                    var oCancellationPolicy = new Cancellation(dFromDate, dEndDate, nCancellationCost);

                    oPropertyDetails.Cancellations.Add(oCancellationPolicy);

                }

                // If no cancellation policies but considered a non-refundable rate, set a 100% cancellation policy effective from today
                bool bNonRefundableRate = oProviderResponse.SafeNodeValue("ValuationRS/CancelPenalties/@nonRefundable").ToSafeBoolean();

                if (oPropertyDetails.Cancellations.Count == 0 && bNonRefundableRate)
                {

                    var oCancellationPolicy = new Cancellation(DateTime.Now, new DateTime(2099, 12, 31), nBookingCost);
                    oPropertyDetails.Cancellations.Add(oCancellationPolicy);

                }

                oPropertyDetails.Cancellations.Solidify(SolidifyType.Max);

                // Lastly, grab any parameters we may need for the booking, put an encrypted version in the TPRef
                if (oProviderResponse.SelectSingleNode("ValuationRS/Parameters") is not null)
                {
                    string sEncryptedParameters = _secretKeeper.Encrypt(oProviderResponse.SelectSingleNode("ValuationRS/Parameters").OuterXml);
                    oPropertyDetails.TPRef1 = sEncryptedParameters;
                }
                else
                {
                    oPropertyDetails.TPRef1 = "";
                }

                bSuccess = true;
            }

            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                bSuccess = false;
            }

            // Lastly, add any logs
            if (!string.IsNullOrEmpty(oRequestXML.InnerXml))
            {
                oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, string.Format("{0} PreBook Request", oPropertyDetails.Source), oRequestXML);
            }

            if (!string.IsNullOrEmpty(oResponseXML.InnerXml))
            {
                oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, string.Format("{0} PreBook Response", oPropertyDetails.Source), oResponseXML);
            }

            return bSuccess;
        }
        #endregion

        #region Book
        public string Book(PropertyDetails oPropertyDetails)
        {

            var oRequestXML = new XmlDocument();
            var oResponseXML = new XmlDocument();
            string sReference = string.Empty;

            try
            {

                // Send request
                string sRequest = GenericRequest(BuildBookRequest(oPropertyDetails), oPropertyDetails);
                oRequestXML.LoadXml(sRequest);
                oResponseXML = SendRequest(sRequest, "Book", oPropertyDetails);

                // Retrieve, decode and clean provider response
                string sDecodedProviderResponse = oResponseXML.SafeNodeValue("Envelope/Body/ReservationResponse/ReservationResult/providerRS/rs");
                var oProviderResponse = new XmlDocument();
                oProviderResponse.LoadXml(sDecodedProviderResponse);

                if (oProviderResponse.SelectSingleNode("ReservationRS/ResStatus").InnerText.ToString() == "OK")
                {

                    string sBookingReference = oProviderResponse.SelectSingleNode("ReservationRS/ProviderLocator").InnerText.ToString();
                    sReference = sBookingReference;
                }
                else
                {
                    sReference = "Failed";
                }
            }

            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
                sReference = "Failed";
            }

            // Lastly, add any logs
            if (!string.IsNullOrEmpty(oRequestXML.InnerXml))
            {
                oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, string.Format("{0} Book Request", oPropertyDetails.Source), oRequestXML);
            }

            if (!string.IsNullOrEmpty(oResponseXML.InnerXml))
            {
                oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, string.Format("{0} Book Response", oPropertyDetails.Source), oResponseXML);
            }

            return sReference;

        }

        #endregion

        #region Cancellation
        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails oPropertyDetails)
        {

            var oRequestXML = new XmlDocument();
            var oResponseXML = new XmlDocument();
            var oCancellationResponse = new ThirdPartyCancellationResponse();

            try
            {

                // Send request
                string sRequest = GenericRequest(BuildCancellationRequest(oPropertyDetails), oPropertyDetails);
                oRequestXML.LoadXml(sRequest);
                oResponseXML = SendRequest(sRequest, "Cancellation", oPropertyDetails);

                // Retrieve, decode and clean provider response
                string sDecodedProviderResponse = oResponseXML.SafeNodeValue("Envelope/Body/CancelResponse/CancelResult/providerRS/rs");
                var oProviderResponse = new XmlDocument();
                oProviderResponse.LoadXml(sDecodedProviderResponse);

                if (oProviderResponse.SafeNodeValue("CancelRS/TransactionStatus/ResStatus") == "CN")
                {

                    string sCancellationReference = oProviderResponse.SafeNodeValue("CancelId");

                    if (!string.IsNullOrEmpty(sCancellationReference))
                    {
                        oCancellationResponse.TPCancellationReference = sCancellationReference;
                    }
                    else
                    {
                        oCancellationResponse.TPCancellationReference = oPropertyDetails.SourceReference;
                    }

                    oCancellationResponse.Success = true;
                }
            }

            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString());
                oCancellationResponse.Success = false;
            }

            // Lastly, add any logs
            if (!string.IsNullOrEmpty(oRequestXML.InnerXml))
            {
                oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, string.Format("{0} Cancellation Request", oPropertyDetails.Source), oRequestXML);
            }

            if (!string.IsNullOrEmpty(oResponseXML.InnerXml))
            {
                oPropertyDetails.Logs.AddNew(oPropertyDetails.Source, string.Format("{0} Cancellation Response", oPropertyDetails.Source), oResponseXML);
            }

            return oCancellationResponse;

        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails oPropertyDetails)
        {
            return new ThirdPartyCancellationFeeResult();
        }
        #endregion

        #region Booking Search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails oBookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public string CreateReconciliationReference(string sInputReference)
        {
            return "";
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails oPropertyDetails)
        {
            return null;
        }
        #endregion

        #region Helpers

        public XmlDocument SendRequest(string RequestString, string RequestType, PropertyDetails PropertyDetails)
        {

            var oResponseXML = new XmlDocument();

            var oWebRequest = new Request();


            switch (RequestType.ToLower() ?? "")
            {
                case "prebook":
                    {
                        oWebRequest.SoapAction = _settings.get_PrebookSOAPAction(PropertyDetails);
                        break;
                    }
                case "book":
                    {
                        oWebRequest.SoapAction = _settings.get_BookSOAPAction(PropertyDetails);
                        break;
                    }
                case "cancellation":
                    {
                        oWebRequest.SoapAction = _settings.get_CancelSOAPAction(PropertyDetails);
                        break;
                    }
            }

            oWebRequest.EndPoint = _settings.get_URL(PropertyDetails);
            oWebRequest.Method = eRequestMethod.POST;
            oWebRequest.Source = PropertyDetails.Source;
            oWebRequest.LogFileName = RequestType;
            oWebRequest.CreateLog = true;
            oWebRequest.SetRequest(RequestString);
            oWebRequest.UseGZip = true;
            oWebRequest.Send(_httpClient, _logger);

            oResponseXML = _serializer.CleanXmlNamespaces(oWebRequest.ResponseXML);

            return oResponseXML;

        }

        public string RemoveHtmlTags(string text)
        {
            return Regex.Replace(text, @"<(?:[^>=]|='[^']*'|=""[^""]*""|=[^'""][^\s>]*)*>", "");
        }

        #endregion

        #region Request builders

        public string BuildPrebookRequest(PropertyDetails PropertyDetails)
        {

            var sbPrebookRequest = new StringBuilder();

            // Create valuation request
            sbPrebookRequest.Append("<ns:Valuation>");
            sbPrebookRequest.Append("<ns:valuationRQ>");
            sbPrebookRequest.Append("<ns:timeoutMilliseconds>180000</ns:timeoutMilliseconds>"); // max general timeout 180s
            sbPrebookRequest.Append("<ns:version>1</ns:version>");

            sbPrebookRequest.Append("<ns:providerRQ>");
            sbPrebookRequest.AppendFormat("<ns:code>{0}</ns:code>", _settings.get_ProviderCode(PropertyDetails));
            sbPrebookRequest.AppendFormat("<ns:id>{0}</ns:id>", 1);
            sbPrebookRequest.Append("<ns:rqXML>");

            sbPrebookRequest.Append("<ValuationRQ>");

            sbPrebookRequest.Append("<timeoutMilliseconds>179700</timeoutMilliseconds>"); // has to be lower than general timeout
            sbPrebookRequest.Append("<source>");
            sbPrebookRequest.AppendFormat("<languageCode>{0}</languageCode>", _settings.get_LanguageCode(PropertyDetails));
            sbPrebookRequest.Append("</source>");
            sbPrebookRequest.Append("<filterAuditData>");
            sbPrebookRequest.Append("<registerTransactions>true</registerTransactions>");
            sbPrebookRequest.Append("</filterAuditData>");

            sbPrebookRequest.Append("<Configuration>");
            sbPrebookRequest.AppendFormat("<User>{0}</User>", _settings.get_ProviderUsername(PropertyDetails));
            sbPrebookRequest.AppendFormat("<Password>{0}</Password>", _settings.get_ProviderPassword(PropertyDetails));
            sbPrebookRequest.Append(AppendURLs(PropertyDetails));
            sbPrebookRequest.Append(HttpUtility.HtmlDecode(_settings.get_Parameters(PropertyDetails)));
            sbPrebookRequest.Append("</Configuration>");

            sbPrebookRequest.AppendFormat("<StartDate>{0}</StartDate>", PropertyDetails.ArrivalDate.ToString("dd/MM/yyyy"));
            sbPrebookRequest.AppendFormat("<EndDate>{0}</EndDate>", PropertyDetails.DepartureDate.ToString("dd/MM/yyyy"));

            // Add hotel option parameters
            // Check if we have any first before we add the parameters node
            // Don't do multiroom bookings for rooms across different options, so can just pluck out the first room's parameters
            string sReferenceDelimiter = _settings.get_ReferenceDelimiter(PropertyDetails, false);
            if (string.IsNullOrEmpty(sReferenceDelimiter))
            {
                sReferenceDelimiter = "~";
            }
            string sEncryptedParameters = PropertyDetails.Rooms[0].ThirdPartyReference.Split(sReferenceDelimiter[0])[4];

            if (!string.IsNullOrEmpty(sEncryptedParameters))
            {
                var oParameters = new XmlDocument();
                oParameters.LoadXml(_secretKeeper.Decrypt(sEncryptedParameters));
                sbPrebookRequest.Append(oParameters.OuterXml);
            }

            sbPrebookRequest.AppendFormat("<MealPlanCode>{0}</MealPlanCode>", PropertyDetails.Rooms[0].ThirdPartyReference.Split(sReferenceDelimiter[0])[7]);
            sbPrebookRequest.AppendFormat("<HotelCode>{0}</HotelCode>", PropertyDetails.TPKey);
            sbPrebookRequest.AppendFormat("<PaymentType>{0}</PaymentType>", PropertyDetails.Rooms[0].ThirdPartyReference.Split(sReferenceDelimiter[0])[3]);
            sbPrebookRequest.Append("<OptionType>Hotel</OptionType>");
            string sNationality = "";
            string sNationalityLookupValue = _support.TPNationalityLookup(ThirdParties.TRAVELGATE, PropertyDetails.NationalityID);
            string sDefaultNationality = _settings.get_DefaultNationality(PropertyDetails, false);

            if (!string.IsNullOrEmpty(sNationalityLookupValue))
            {
                sNationality = sNationalityLookupValue;
            }
            else if (!string.IsNullOrEmpty(sDefaultNationality))
            {
                sNationality = sDefaultNationality;
            }
            if (!string.IsNullOrEmpty(sNationality))
            {
                sbPrebookRequest.AppendFormat("<Nationality>{0}</Nationality>", sNationality);
            }

            sbPrebookRequest.Append("<Rooms>");

            int iRoomID = 1;
            foreach (RoomDetails oRoom in PropertyDetails.Rooms)
            {
                sbPrebookRequest.AppendFormat("<Room id = \"{0}\" roomCandidateRefId = \"{3}\" code = \"{1}\" description = \"{2}\"/>", oRoom.ThirdPartyReference.Split(sReferenceDelimiter[0])[0], oRoom.ThirdPartyReference.Split(sReferenceDelimiter[0])[1], oRoom.ThirdPartyReference.Split(sReferenceDelimiter[0])[2], iRoomID);

                iRoomID += 1;
            }

            sbPrebookRequest.Append("</Rooms>");
            sbPrebookRequest.Append("<RoomCandidates>");

            iRoomID = 1;
            int iPassengerID;

            foreach (RoomDetails oRoom in PropertyDetails.Rooms)
            {
                sbPrebookRequest.AppendFormat("<RoomCandidate id = \"{0}\">", iRoomID);
                sbPrebookRequest.Append("<Paxes>");

                iPassengerID = 1; // every room starts with PaxID 1
                foreach (Passenger oPassenger in oRoom.Passengers)
                {
                    var iAge = default(int);
                    switch (oPassenger.PassengerType)
                    {
                        case PassengerType.Adult:
                            {
                                iAge = 30;
                                break;
                            }
                        case PassengerType.Child:
                            {
                                iAge = oPassenger.Age;
                                break;
                            }
                        case PassengerType.Infant:
                            {
                                iAge = 1;
                                break;
                            }
                    }

                    sbPrebookRequest.AppendFormat("<Pax age = \"{0}\" id = \"{1}\"/>", iAge, iPassengerID);

                    iPassengerID += 1;
                }

                sbPrebookRequest.Append("</Paxes>");
                sbPrebookRequest.Append("</RoomCandidate>");

                iRoomID += 1;
            }
            sbPrebookRequest.Append("</RoomCandidates>");
            sbPrebookRequest.Append("</ValuationRQ>");

            sbPrebookRequest.Append("</ns:rqXML>");
            sbPrebookRequest.Append("</ns:providerRQ>");
            sbPrebookRequest.Append("</ns:valuationRQ>");

            sbPrebookRequest.Append("</ns:Valuation>");

            return sbPrebookRequest.ToString();

        }

        public string BuildBookRequest(PropertyDetails oPropertyDetails)
        {

            string sSource = oPropertyDetails.Source;

            char cDelimiter = get_ReferenceDelimiter(oPropertyDetails);

            var sbBookRequest = new StringBuilder();

            string sPaymentType = oPropertyDetails.Rooms[0].ThirdPartyReference.Split(cDelimiter)[3];

            sbBookRequest.Append("<ns:Reservation>");
            sbBookRequest.Append("<ns:reservationRQ>");
            sbBookRequest.Append("<ns:timeoutMilliseconds>180000</ns:timeoutMilliseconds>"); // max general timeout 180s
            sbBookRequest.Append("<ns:version>1</ns:version>");

            sbBookRequest.Append("<ns:providerRQ>");
            sbBookRequest.AppendFormat("<ns:code>{0}</ns:code>", _settings.get_ProviderCode(oPropertyDetails));
            sbBookRequest.AppendFormat("<ns:id>{0}</ns:id>", 1);
            sbBookRequest.Append("<ns:rqXML>");

            sbBookRequest.Append("<ReservationRQ>");

            sbBookRequest.Append("<timeoutMilliseconds>179700</timeoutMilliseconds>"); // has to be lower than general timeout
            sbBookRequest.Append("<source>");
            sbBookRequest.AppendFormat("<languageCode>{0}</languageCode>", _settings.get_LanguageCode(oPropertyDetails));
            sbBookRequest.Append("</source>");
            sbBookRequest.Append("<filterAuditData>");
            sbBookRequest.Append("<registerTransactions>true</registerTransactions>");
            sbBookRequest.Append("</filterAuditData>");

            sbBookRequest.Append("<Configuration>");
            sbBookRequest.AppendFormat("<User>{0}</User>", _settings.get_ProviderUsername(oPropertyDetails));
            sbBookRequest.AppendFormat("<Password>{0}</Password>", _settings.get_ProviderPassword(oPropertyDetails));
            sbBookRequest.Append(AppendURLs(oPropertyDetails));
            sbBookRequest.Append(HttpUtility.HtmlDecode(_settings.get_Parameters(oPropertyDetails)));
            sbBookRequest.Append("</Configuration>");
            if (_settings.get_SendGUIDReference(oPropertyDetails))
            {
                sbBookRequest.AppendFormat("<ClientLocator>{0}</ClientLocator>", oPropertyDetails.BookingReference.TrimEnd() + Guid.NewGuid().ToString());
            }
            else
            {
                sbBookRequest.AppendFormat("<ClientLocator>{0}</ClientLocator>", oPropertyDetails.BookingReference);
            }
            sbBookRequest.AppendLine(BuildCardDetails(oPropertyDetails));

            sbBookRequest.AppendFormat("<StartDate>{0}</StartDate>", oPropertyDetails.ArrivalDate.ToString("dd/MM/yyyy"));
            sbBookRequest.AppendFormat("<EndDate>{0}</EndDate>", oPropertyDetails.DepartureDate.ToString("dd/MM/yyyy"));

            // Add hotel option parameters
            // Check if we have any first before we add the parameters node
            // Don't do multiroom bookings for rooms across different options, so can just pluck out the first room's parameters
            string sEncryptedParameters = oPropertyDetails.TPRef1;

            if (!string.IsNullOrEmpty(sEncryptedParameters))
            {
                var oParameters = new XmlDocument();
                oParameters.LoadXml(_secretKeeper.Decrypt(sEncryptedParameters));
                sbBookRequest.Append(oParameters.OuterXml);
            }

            sbBookRequest.AppendFormat("<MealPlanCode>{0}</MealPlanCode>", oPropertyDetails.Rooms[0].ThirdPartyReference.Split(cDelimiter)[7]);
            sbBookRequest.AppendFormat("<HotelCode>{0}</HotelCode>", oPropertyDetails.TPKey);

            string sNationality = "";
            string sNationalityLookupValue = _support.TPNationalityLookup(Source, oPropertyDetails.NationalityID);
            string sDefaultNationality = _settings.get_DefaultNationality(oPropertyDetails, false);

            if (!string.IsNullOrEmpty(sNationalityLookupValue))
            {
                sNationality = sNationalityLookupValue;
            }
            else if (!string.IsNullOrEmpty(sDefaultNationality))
            {
                sNationality = sDefaultNationality;
            }
            if (!string.IsNullOrEmpty(sNationality))
            {
                sbBookRequest.AppendFormat("<Nationality>{0}</Nationality>", sNationality);
            }

            string sCurrencyCode = _support.TPCurrencyLookup(sSource, oPropertyDetails.CurrencyCode);
            // send gross cost as gross net down can be used (local is the net)
            sbBookRequest.AppendFormat("<Price currency = \"{0}\" amount = \"{1}\" binding = \"{2}\" commission = \"{3}\"/>", sCurrencyCode, oPropertyDetails.GrossCost, oPropertyDetails.Rooms[0].ThirdPartyReference.Split(cDelimiter)[6], oPropertyDetails.Rooms[0].ThirdPartyReference.Split(cDelimiter)[5]);
            sbBookRequest.Append("<ResGuests>");
            sbBookRequest.Append("<Guests>");

            int iPassengerCount;
            int iRoomCount = 1;
            foreach (RoomDetails oRoom in oPropertyDetails.Rooms)
            {

                iPassengerCount = 1;
                foreach (Passenger oPassenger in oRoom.Passengers)
                {

                    sbBookRequest.AppendFormat("<Guest roomCandidateId = \"{0}\" paxId = \"{1}\">", iRoomCount, iPassengerCount);
                    sbBookRequest.AppendFormat("<GivenName>{0}</GivenName>", oPassenger.FirstName);
                    sbBookRequest.AppendFormat("<SurName>{0}</SurName>", oPassenger.LastName);
                    sbBookRequest.Append("</Guest>");

                    iPassengerCount += 1;
                }
                iRoomCount += 1;
            }
            sbBookRequest.Append("</Guests>");
            sbBookRequest.Append("</ResGuests>");
            sbBookRequest.AppendFormat("<PaymentType>{0}</PaymentType>", sPaymentType);

            sbBookRequest.Append("<Rooms>");

            iRoomCount = 1;

            foreach (RoomDetails oRoom in oPropertyDetails.Rooms)
            {
                sbBookRequest.AppendFormat("<Room id = \"{0}\" roomCandidateRefId = \"{3}\" code = \"{1}\" description = \"{2}\"/>", oRoom.ThirdPartyReference.Split(cDelimiter)[0], oRoom.ThirdPartyReference.Split(cDelimiter)[1], oRoom.ThirdPartyReference.Split(cDelimiter)[2], iRoomCount);

                iRoomCount += 1;
            }
            sbBookRequest.Append("</Rooms>");
            sbBookRequest.Append("<RoomCandidates>");

            iRoomCount = 1;
            int iPassengerID;

            foreach (RoomDetails oRoom in oPropertyDetails.Rooms)
            {
                sbBookRequest.AppendFormat("<RoomCandidate id = \"{0}\">", iRoomCount);
                sbBookRequest.Append("<Paxes>");

                iPassengerID = 1;
                foreach (Passenger oPassenger in oRoom.Passengers)
                {
                    var iAge = default(int);
                    switch (oPassenger.PassengerType)
                    {
                        case PassengerType.Adult:
                            {
                                iAge = 30;
                                break;
                            }
                        case PassengerType.Child:
                            {
                                iAge = oPassenger.Age;
                                break;
                            }
                        case PassengerType.Infant:
                            {
                                iAge = 1;
                                break;
                            }
                    }

                    sbBookRequest.AppendFormat("<Pax age = \"{0}\" id = \"{1}\"/>", iAge, iPassengerID);

                    iPassengerID += 1;
                }

                sbBookRequest.Append("</Paxes>");
                sbBookRequest.Append("</RoomCandidate>");

                iRoomCount += 1;
            }

            sbBookRequest.Append("</RoomCandidates>");
            if (!string.IsNullOrEmpty(oPropertyDetails.BookingComments.ToString()))
            {
                sbBookRequest.Append("<Remarks>");
                sbBookRequest.Append(oPropertyDetails.BookingComments.ToString().Trim());
                sbBookRequest.Append("</Remarks>");
            }
            sbBookRequest.Append("</ReservationRQ>");

            sbBookRequest.Append("</ns:rqXML>");
            sbBookRequest.Append("</ns:providerRQ>");
            sbBookRequest.Append("</ns:reservationRQ>");
            sbBookRequest.Append("</ns:Reservation>");

            return sbBookRequest.ToString();

        }

        public string BuildCancellationRequest(PropertyDetails PropertyDetails)
        {

            var sbCancellationRequest = new StringBuilder();

            sbCancellationRequest.Append("<ns:Cancel>");
            sbCancellationRequest.Append("<ns:cancelRQ>");
            sbCancellationRequest.Append("<ns:timeoutMilliseconds>180000</ns:timeoutMilliseconds>"); // max general timeout 180s
            sbCancellationRequest.Append("<ns:version>1</ns:version>");

            sbCancellationRequest.Append("<ns:providerRQ>");
            sbCancellationRequest.AppendFormat("<ns:code>{0}</ns:code>", _settings.get_ProviderCode(PropertyDetails));
            sbCancellationRequest.AppendFormat("<ns:id>{0}</ns:id>", 1);
            sbCancellationRequest.Append("<ns:rqXML>");

            sbCancellationRequest.AppendFormat("<CancelRQ  hotelCode=\"{0}\">", PropertyDetails.TPKey);

            sbCancellationRequest.Append("<timeoutMilliseconds>179700</timeoutMilliseconds>"); // has to be lower than general timeout
            sbCancellationRequest.Append("<source>");
            sbCancellationRequest.AppendFormat("<languageCode>{0}</languageCode>", _settings.get_LanguageCode(PropertyDetails));
            sbCancellationRequest.Append("</source>");
            sbCancellationRequest.Append("<filterAuditData>");
            sbCancellationRequest.Append("<registerTransactions>true</registerTransactions>");
            sbCancellationRequest.Append("</filterAuditData>");

            sbCancellationRequest.Append("<Configuration>");
            sbCancellationRequest.AppendFormat("<User>{0}</User>", _settings.get_ProviderUsername(PropertyDetails));
            sbCancellationRequest.AppendFormat("<Password>{0}</Password>", _settings.get_ProviderPassword(PropertyDetails));
            sbCancellationRequest.Append(AppendURLs(PropertyDetails));
            sbCancellationRequest.Append(HttpUtility.HtmlDecode(_settings.get_Parameters(PropertyDetails)));
            sbCancellationRequest.Append("</Configuration>");

            sbCancellationRequest.Append("<Locators>");
            sbCancellationRequest.AppendFormat("<Client>{0}</Client>", PropertyDetails.BookingReference);
            sbCancellationRequest.AppendFormat("<Provider>{0}</Provider>", PropertyDetails.SourceReference);
            sbCancellationRequest.Append("</Locators>");
            sbCancellationRequest.AppendFormat("<StartDate>{0}</StartDate>", PropertyDetails.ArrivalDate.ToString("dd/MM/yyyy"));
            sbCancellationRequest.AppendFormat("<EndDate>{0}</EndDate>", PropertyDetails.DepartureDate.ToString("dd/MM/yyyy"));
            sbCancellationRequest.Append("</CancelRQ>");

            sbCancellationRequest.Append("</ns:rqXML>");
            sbCancellationRequest.Append("</ns:providerRQ>");
            sbCancellationRequest.Append("</ns:cancelRQ>");
            sbCancellationRequest.Append("</ns:Cancel>");

            return sbCancellationRequest.ToString();

        }

        public string GenericRequest(string SpecificRequest, IThirdPartyAttributeSearch SearchDetails)
        {

            var sbRequest = new StringBuilder();

            sbRequest.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" ");
            sbRequest.Append("xmlns:ns=\"http://schemas.xmltravelgate.com/hub/2012/06\" ");
            sbRequest.Append("xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">");
            sbRequest.Append("<soapenv:Header>");
            sbRequest.Append("<wsse:Security>");
            sbRequest.Append("<wsse:UsernameToken>");
            sbRequest.AppendFormat("<wsse:Username>{0}</wsse:Username>", _settings.get_Username(SearchDetails));
            sbRequest.AppendFormat("<wsse:Password>{0}</wsse:Password>", _settings.get_Password(SearchDetails));
            sbRequest.Append("</wsse:UsernameToken>");
            sbRequest.Append("</wsse:Security>");
            sbRequest.Append("</soapenv:Header>");
            sbRequest.Append("<soapenv:Body>");
            sbRequest.Append(SpecificRequest);
            sbRequest.Append("</soapenv:Body>");

            sbRequest.Append("</soapenv:Envelope>");
            return sbRequest.ToString();

        }

        public string AppendURL(string URLType, string Source, IThirdPartyAttributeSearch SearchDetails)
        {

            string sURL = _settings.get_URL(SearchDetails);

            if (string.IsNullOrEmpty(sURL))
            {
                return "";
            }
            else
            {
                string sNodeName = "";
                switch (URLType ?? "")
                {
                    case "URLReservation":
                        {
                            sNodeName = "UrlReservation";
                            break;
                        }
                    case "URLGeneric":
                        {
                            sNodeName = "UrlGeneric";
                            break;
                        }
                    case "URLAvail":
                        {
                            sNodeName = "UrlAvail";
                            break;
                        }
                    case "URLValuation":
                        {
                            sNodeName = "UrlValuation";
                            break;
                        }
                }

                return string.Format("<{0}>{1}</{0}>", sNodeName, sURL);
            }

        }

        public string BuildCardDetails(PropertyDetails oPropertyDetails)
        {

            string sSource = oPropertyDetails.Source;

            char cDelimiter = get_ReferenceDelimiter(oPropertyDetails);

            string sPaymentType = oPropertyDetails.Rooms[0].ThirdPartyReference.Split(cDelimiter)[3];
            bool bRequiresVCard = _settings.get_RequiresVCard(oPropertyDetails);

            var sb = new StringBuilder();


            if (bRequiresVCard && sPaymentType == "CardBookingPay" | sPaymentType == "CardCheckInPay")
            {

                string sCardHolderName = oPropertyDetails.GeneratedVirtualCard.CardHolderName;
                if (string.IsNullOrEmpty(sCardHolderName))
                {
                    sCardHolderName = _settings.get_CardHolderName(oPropertyDetails);
                }

                sb.AppendLine("<CardInfo>");
                sb.AppendLine($"<CardCode>{_support.TPCreditCardLookup(sSource, oPropertyDetails.GeneratedVirtualCard.CardTypeID)}</CardCode>");
                sb.AppendLine($"<Number>{oPropertyDetails.GeneratedVirtualCard.CardNumber}</Number>");
                sb.AppendLine($"<Holder>{sCardHolderName}</Holder>");
                sb.AppendLine("<ValidityDate>");
                sb.AppendLine($"<Month>{oPropertyDetails.GeneratedVirtualCard.ExpiryMonth.PadLeft(2, '0')}</Month>");
                sb.AppendLine($"<Year>{oPropertyDetails.GeneratedVirtualCard.ExpiryYear.Substring(2)}</Year>");
                sb.AppendLine("</ValidityDate>");
                sb.AppendLine($"<CVC>{oPropertyDetails.GeneratedVirtualCard.CVV}</CVC>");
                sb.AppendLine("</CardInfo>");
            }

            else if (!bRequiresVCard)
            {

                string sEncrytedCardDetails = _settings.get_EncryptedCardDetails(oPropertyDetails);
                if (string.IsNullOrWhiteSpace(sEncrytedCardDetails))
                    return string.Empty;

                // Card details are encryted in the form 'CardCode|CardNumber|ExpiryDate|CVC|CardHolderName'
                var sDecryptedCardDetails = _secretKeeper.Decrypt(sEncrytedCardDetails).Split('|');
                if (sDecryptedCardDetails.Count() != 5)
                    return string.Empty;

                var dtExpiryDate = sDecryptedCardDetails[2].ToSafeDate();

                sb.AppendLine("<CardInfo>");
                sb.AppendLine($"<CardCode>{sDecryptedCardDetails[0]}</CardCode>");
                sb.AppendLine($"<Number>{sDecryptedCardDetails[1]}</Number>");
                sb.AppendLine($"<Holder>{sDecryptedCardDetails[4]}</Holder>");
                sb.AppendLine("<ValidityDate>");
                sb.AppendLine($"<Month>{dtExpiryDate.Month.ToString().PadLeft(2, '0')}</Month>");
                sb.AppendLine($"<Year>{dtExpiryDate.Year}</Year>");
                sb.AppendLine("</ValidityDate>");
                sb.AppendLine($"<CVC>{sDecryptedCardDetails[3]}</CVC>");
                sb.AppendLine("</CardInfo>");

            }

            return sb.ToString();
        }

        #endregion

        #region Helper class

        public string AppendURLs(IThirdPartyAttributeSearch SearchDetails)
        {

            var sbURLXML = new StringBuilder();

            sbURLXML.AppendFormat("<UrlReservation>{0}</UrlReservation>", _settings.get_UrlReservation(SearchDetails));
            sbURLXML.AppendFormat("<UrlGeneric>{0}</UrlGeneric>", _settings.get_UrlGeneric(SearchDetails));
            sbURLXML.AppendFormat("<UrlAvail>{0}</UrlAvail>", _settings.get_UrlAvail(SearchDetails));
            sbURLXML.AppendFormat("<UrlValuation>{0}</UrlValuation>", _settings.get_UrlValuation(SearchDetails));

            return sbURLXML.ToString();

        }

        #endregion

        #region End session

        public void EndSession(PropertyDetails oPropertyDetails)
        {

        }

        #endregion

    }
}