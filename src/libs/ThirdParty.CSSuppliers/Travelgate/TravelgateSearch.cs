using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Intuitive;
using Intuitive.Helpers.Extensions;
using Intuitive.Helpers.Security;
using Intuitive.Helpers.Serialization;
using Intuitive.Net.WebRequests;
using Microsoft.Extensions.Logging;
using ThirdParty.Constants;
using ThirdParty.Lookups;
using ThirdParty.Models;
using ThirdParty.Results;
using ThirdParty.Search.Models;

namespace ThirdParty.CSSuppliers
{

    public class TravelgateSearch : ThirdPartyPropertySearchBase
    {

        #region Constructor

        public TravelgateSearch(ITravelgateSettings settings, ITPSupport support, ISecretKeeper secretKeeper, ISerializer serializer, ILogger<TravelgateSearch> logger) : base(logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region Properties

        private readonly ITravelgateSettings _settings;

        private readonly ITPSupport _support;

        private readonly ISecretKeeper _secretKeeper;

        private readonly ISerializer _serializer;

        public override string Source { get; } = ThirdParties.TRAVELGATE;

        public override bool SqlRequest { get; } = false;

        #endregion

        #region Search Restrictions

        public override bool SearchRestrictions(SearchDetails oSearchDetails)
        {
            return false;
        }

        #endregion

        #region SearchFunctions

        public override List<Request> BuildSearchRequests(SearchDetails oSearchDetails, List<ResortSplit> oResortSplits, bool bSaveLogs)
        {

            var oRequests = new List<Request>();
            int iSalesChannelID = oSearchDetails.SalesChannelID;
            int iBrandID = oSearchDetails.BrandID;

            // Remove any suppliers in the dictionary that are attempting a restricted search
            // Needs to be done here since
            var oFilteredSuppliers = new List<string>();

            int iMaximumRoomNumber = _settings.get_MaximumRoomNumber(oSearchDetails);
            int iMaximumRoomGuestNumber = _settings.get_MaximumRoomGuestNumber(oSearchDetails);
            int iMinimumStay = _settings.get_MinimumStay(oSearchDetails);

            bool bSearchExceedsGuestCount = false;

            foreach (iVector.Search.Property.RoomDetail oRoom in oSearchDetails.RoomDetails)
            {
                if (oRoom.Adults + oRoom.Children + oRoom.Infants > iMaximumRoomGuestNumber)
                {
                    bSearchExceedsGuestCount = true;
                }
            }

            if (!(oSearchDetails.Rooms > iMaximumRoomNumber || oSearchDetails.Duration < iMinimumStay || bSearchExceedsGuestCount))
            {
                oFilteredSuppliers.Add(Source);
            }

            if (oFilteredSuppliers.Count > 0)
            {

                var sbSearchRequest = new StringBuilder();

                {
                    ref var withBlock = ref sbSearchRequest;

                    withBlock.Append("<soapenv:Envelope xmlns:soapenv = \"http://schemas.xmlsoap.org/soap/envelope/\" ");
                    withBlock.Append("xmlns:ns = \"http://schemas.xmltravelgate.com/hub/2012/06\" ");
                    withBlock.Append("xmlns:wsse = \"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">");
                    withBlock.Append("<soapenv:Header>");
                    withBlock.Append("<wsse:Security>");
                    withBlock.Append("<wsse:UsernameToken>");
                    withBlock.AppendFormat("<wsse:Username>{0}</wsse:Username>", _settings.get_Username(oSearchDetails));
                    withBlock.AppendFormat("<wsse:Password>{0}</wsse:Password>", _settings.get_Password(oSearchDetails));
                    withBlock.Append("</wsse:UsernameToken>");
                    withBlock.Append("</wsse:Security>");
                    withBlock.Append("</soapenv:Header>");
                    withBlock.Append("<soapenv:Body>");
                    withBlock.Append("<ns:Avail>");
                    withBlock.Append("<ns:availRQ>");
                    withBlock.Append("<ns:timeoutMilliseconds>25000</ns:timeoutMilliseconds>"); // max general search timeout is 25s
                    withBlock.Append("<ns:version>1</ns:version>");
                    withBlock.Append("<ns:providerRQs>");

                    int iRequestCount = 1;

                    // Some third parties don't support searches by TPKey, so use these to check what kind of search we want to be doing
                    int iMaximumHotelSearchNumber = _settings.get_MaximumHotelSearchNumber(oSearchDetails);
                    int iMaximumCitySearchNumber = _settings.get_MaximumCitySearchNumber(oSearchDetails);
                    // We generally prefer hotel based searches, but if a third party has a small maximum number of hotels per search
                    // we leave it up to the discretion of the user to determine if they would rather perform city based searches (where allowed)
                    bool bAllowHotelSearch = _settings.get_AllowHotelSearch(oSearchDetails);
                    // Whether to try to search for a zone (region) instead of individual resorts (cities)
                    bool bUseZoneSearch = oResortSplits.Count > 1 && _settings.get_UseZoneSearch(oSearchDetails);

                    var oSearchBatchDetails = new SearchBatchDetails();
                    oSearchBatchDetails.Source = Source;
                    oSearchBatchDetails.ResortSplits = oResortSplits;

                    // Check how many hotels we have - if only one we can ignore the allow hotel search boolean
                    int iHotelCount = 0;
                    foreach (ResortSplit oResortSplit in oResortSplits)
                        iHotelCount += oResortSplit.Hotels.Count;

                    // Get a count of our resorts as well
                    int iResortCount = oResortSplits.Count;

                    // Set which search type we will be using - if hotel searches are allowed and either the allow flag is set to true,
                    // or else city searches aren't allowed or we're only searching for one hotel, search by hotel
                    // Otherwiwse search by city

                    if (iMaximumHotelSearchNumber > 0 && (bAllowHotelSearch || iMaximumCitySearchNumber == 0 || iHotelCount == 1))
                    {

                        oSearchBatchDetails.SearchByHotel = true;

                        // Get the batch size and count for hotel searches
                        oSearchBatchDetails.BatchSize = iMaximumHotelSearchNumber;
                        oSearchBatchDetails.SearchItemCount = iHotelCount;
                    }

                    else if (iMaximumCitySearchNumber > 0)
                    {

                        oSearchBatchDetails.SearchByHotel = false;

                        if (bUseZoneSearch)
                        {
                            oSearchBatchDetails.SetZoneSearchID();
                            oSearchBatchDetails.UseZoneSearch = oSearchBatchDetails.SearchItemIDs.Count == 1;
                        }

                        if (oSearchBatchDetails.UseZoneSearch)
                        {
                            oSearchBatchDetails.BatchSize = 1;
                            oSearchBatchDetails.SearchItemCount = 1;
                        }
                        else
                        {
                            // Get the batch size and count for city searches
                            oSearchBatchDetails.BatchSize = iMaximumCitySearchNumber;
                            oSearchBatchDetails.SearchItemCount = iResortCount;
                        }
                    }

                    oSearchBatchDetails.CalculateBatchCount();

                    if (!oSearchBatchDetails.UseZoneSearch)
                    {
                        oSearchBatchDetails.SetSearchIDs();
                    }

                    BuildSearchBatch(oSearchDetails, oSearchBatchDetails, ref sbSearchRequest, ref iRequestCount);

                    // Next

                    withBlock.Append("</ns:providerRQs>");
                    withBlock.Append("</ns:availRQ>");
                    withBlock.Append("</ns:Avail>");
                    withBlock.Append("</soapenv:Body>");
                    withBlock.Append("</soapenv:Envelope>");

                }

                // Build Request Object
                var oRequest = new Request();
                oRequest.EndPoint = _settings.get_URL(oSearchDetails);
                oRequest.SoapAction = _settings.get_SearchSOAPAction(oSearchDetails);
                oRequest.Headers.AddNew("SOAPAction", _settings.get_SearchSOAPAction(oSearchDetails));
                oRequest.Method = eRequestMethod.POST;
                oRequest.Source = Source;
                oRequest.LogFileName = "Search";
                oRequest.CreateLog = bSaveLogs;
                oRequest.TimeoutInSeconds = RequestTimeOutSeconds(oSearchDetails);
                oRequest.ExtraInfo = oSearchDetails;
                oRequest.SuppressExpectHeaders = true;
                oRequest.SetRequest(sbSearchRequest.ToString());
                oRequest.UseGZip = _settings.get_UseGZip(oSearchDetails);

                oRequests.Add(oRequest);

            }

            return oRequests;

        }

        public void BuildSearchBatch(SearchDetails SearchDetails, SearchBatchDetails SearchBatchDetails, ref StringBuilder SearchRequest, ref int RequestCount)
        {

            int iSalesChannelID = SearchDetails.SalesChannelID;
            int iBrandID = SearchDetails.BrandID;

            // Index to keep track of where we're at
            int iIndex = 0;


            // Loop through the batches
            for (int iBatchNumber = 1, loopTo = SearchBatchDetails.BatchCount; iBatchNumber <= loopTo; iBatchNumber++)
            {

                SearchRequest.Append("<ns:ProviderRQ>");
                SearchRequest.AppendFormat("<ns:code>{0}</ns:code>", _settings.get_ProviderCode(SearchDetails));
                SearchRequest.AppendFormat("<ns:id>{0}</ns:id>", RequestCount);
                SearchRequest.Append("<ns:rqXML>");
                SearchRequest.Append("<AvailRQ>");
                SearchRequest.AppendFormat("<timeoutMilliseconds>{0}</timeoutMilliseconds>", _settings.get_SearchRequestTimeout(SearchDetails));
                SearchRequest.Append("<source>");
                SearchRequest.AppendFormat("<languageCode>{0}</languageCode>", _settings.get_LanguageCode(SearchDetails));
                SearchRequest.Append("</source>");
                SearchRequest.Append("<filterAuditData>");
                SearchRequest.Append("<registerTransactions>false</registerTransactions>");
                SearchRequest.Append("</filterAuditData>");
                SearchRequest.Append("<Configuration>");
                SearchRequest.AppendFormat("<User>{0}</User>", _settings.get_ProviderUsername(SearchDetails));
                SearchRequest.AppendFormat("<Password>{0}</Password>", _settings.get_ProviderPassword(SearchDetails));
                SearchRequest.Append(AppendURLs(SearchDetails));
                SearchRequest.Append(HttpUtility.HtmlDecode(_settings.get_Parameters(SearchDetails)));

                SearchRequest.Append("</Configuration>");

                SearchRequest.Append("<OnRequest>false</OnRequest>");

                SearchRequest.Append("<AvailDestinations>");

                // Get the last item for our current batch
                int iLastBatchItem = 0;

                if (iIndex + SearchBatchDetails.BatchSize > SearchBatchDetails.SearchItemCount)
                {
                    iLastBatchItem = SearchBatchDetails.SearchItemCount - 1;
                }
                else
                {
                    iLastBatchItem = iIndex + SearchBatchDetails.BatchSize - 1;
                }

                for (int iPosition = iIndex, loopTo1 = iLastBatchItem; iPosition <= loopTo1; iPosition++)
                {

                    if (SearchBatchDetails.SearchByHotel)
                    {
                        SearchRequest.AppendFormat("<Destination type = \"HOT\" code = \"{0}\"/>", SearchBatchDetails.SearchItemIDs.ElementAt(iIndex));
                    }
                    else if (SearchBatchDetails.UseZoneSearch)
                    {
                        SearchRequest.AppendFormat("<Destination type = \"ZON\" code = \"{0}\"/>", SearchBatchDetails.SearchItemIDs.ElementAt(iIndex));
                    }
                    else
                    {
                        SearchRequest.AppendFormat("<Destination type = \"CTY\" code = \"{0}\"/>", SearchBatchDetails.SearchItemIDs.ElementAt(iIndex));
                    }

                    iIndex += 1;

                }

                SearchRequest.Append("</AvailDestinations>");

                SearchRequest.AppendFormat("<StartDate>{0}</StartDate>", SearchDetails.ArrivalDate.ToString("dd/MM/yyyy"));
                SearchRequest.AppendFormat("<EndDate>{0}</EndDate>", SearchDetails.DepartureDate.ToString("dd/MM/yyyy"));
                SearchRequest.AppendFormat("<Currency>{0}</Currency>", _settings.get_CurrencyCode(SearchDetails));

                string sNationality = _support.TPNationalityLookup(ThirdParties.TRAVELGATE, SearchDetails.NationalityID);
                if (string.IsNullOrEmpty(sNationality))
                {
                    sNationality = _settings.get_DefaultNationality(SearchDetails, false);
                }

                if (!string.IsNullOrEmpty(sNationality))
                {
                    SearchRequest.AppendFormat("<Nationality>{0}</Nationality>", sNationality);
                    SearchRequest.AppendFormat("<Markets><Market>{0}</Market></Markets>", sNationality);
                }
                else
                {
                    string sDefaultNationality = _settings.get_DefaultNationality(SearchDetails, false);
                    if (!string.IsNullOrEmpty(sDefaultNationality))
                    {
                        SearchRequest.AppendFormat("<Markets><Market>{0}</Market></Markets>", sDefaultNationality);
                    }
                }

                string sMarkets = _settings.get_Markets(SearchDetails);
                if (sMarkets.Length > 0)
                {
                    SearchRequest.Append("<Markets>");
                    foreach (string sMarket in sMarkets.Split(','))
                        SearchRequest.AppendFormat("<Market>{0}</Market>", sMarket);
                    SearchRequest.Append("</Markets>");
                }
                else if (!string.IsNullOrEmpty(sNationality))
                {
                    SearchRequest.AppendFormat("<Markets><Market>{0}</Market></Markets>", sNationality);
                }

                SearchRequest.Append("<RoomCandidates>");

                int iRoomIndex = 1;
                foreach (iVector.Search.Property.RoomDetail oRoomDetails in SearchDetails.RoomDetails)
                {

                    SearchRequest.AppendFormat("<RoomCandidate id = \"{0}\">", iRoomIndex);
                    SearchRequest.Append("<Paxes>");

                    int iPaxCount = 1;

                    for (int i = 1, loopTo2 = oRoomDetails.Adults; i <= loopTo2; i++)
                    {
                        SearchRequest.AppendFormat("<Pax age = \"30\" id = \"{0}\"/>", iPaxCount);
                        iPaxCount += 1;
                    }

                    if (oRoomDetails.Children > 0)
                    {
                        foreach (string sChildAge in oRoomDetails.ChildAgeCSV.Split(','))
                        {
                            SearchRequest.AppendFormat("<Pax age = \"{0}\" id = \"{1}\"/>", sChildAge, iPaxCount);
                            iPaxCount += 1;
                        }
                    }

                    if (oRoomDetails.Infants > 0)
                    {
                        for (int i = 1, loopTo3 = oRoomDetails.Infants; i <= loopTo3; i++)
                        {
                            SearchRequest.AppendFormat("<Pax age = \"1\" id = \"{0}\"/>", iPaxCount);
                            iPaxCount += 1;
                        }
                    }

                    SearchRequest.Append("</Paxes>");
                    SearchRequest.Append("</RoomCandidate>");

                    iRoomIndex += 1;
                }

                SearchRequest.Append("</RoomCandidates>");

                SearchRequest.Append("</AvailRQ>");
                SearchRequest.Append("</ns:rqXML>");
                SearchRequest.Append("</ns:ProviderRQ>");

                RequestCount += 1;


            }

        }

        public override TransformedResultCollection TransformResponse(List<Request> oRequests, SearchDetails oSearchDetails, List<ResortSplit> oResortSplits)
        {

            var oTransformedResults = new TransformedResultCollection();
            var oAvailabilityResponses = new List<TravelgateSearchResponse>();
            var oEnvelopeSerializer = new XmlSerializer(typeof(TravelgateResponseEnvelope));
            var oResponseSerializer = new XmlSerializer(typeof(TravelgateSearchResponse));

            foreach (Request oRequest in oRequests)
            {

                if (oRequest.Success)
                {
                    var oResponseXML = _serializer.CleanXmlNamespaces(oRequest.ResponseXML);

                    // Retrieve response envelope
                    var oResponseEnvelope = new TravelgateResponseEnvelope();

                    // Deserialize the response Envelope
                    using (TextReader oReader = new StringReader(oResponseXML.InnerXml))
                    {
                        oResponseEnvelope = (TravelgateResponseEnvelope)oEnvelopeSerializer.Deserialize(oReader);
                    }

                    string sResponses = oResponseEnvelope.Body.Response.Result.ProviderResults.FirstOrDefault().Results.Result;

                    // Decoded Xml if response encoded
                    if (sResponses.Contains("&gt;"))
                    {
                        HttpUtility.HtmlDecode(sResponses);
                    }

                    // Deserialize Response Body
                    var oResponse = new TravelgateSearchResponse();
                    using (TextReader oReader = new StringReader(sResponses))
                    {
                        oResponse = (TravelgateSearchResponse)oResponseSerializer.Deserialize(oReader);
                    }

                    oAvailabilityResponses.Add(oResponse);
                }
            }

            // Extract search results from responses
            oTransformedResults.TransformedResults.AddRange(oAvailabilityResponses.Where(r => r.Hotels.Count > 0).SelectMany(x => GetResultFromResponse(x)));

            return oTransformedResults;

        }

        #endregion

        #region ResponseHasExceptions

        public override bool ResponseHasExceptions(Request oRequest)
        {
            return false;
        }

        #endregion

        #region Helper methods

        public class SearchBatchDetails
        {

            public string Source;
            public List<ResortSplit> ResortSplits;
            public bool SearchByHotel;
            public bool UseZoneSearch;
            public int BatchSize;
            public int BatchCount;
            public int SearchItemCount;
            public List<string> SearchItemIDs = new List<string>();

            public void CalculateBatchCount()
            {
                BatchCount = (int)Math.Round(Math.Ceiling(SearchItemCount / (double)BatchSize));
            }

            public void SetZoneSearchID()
            {
                // Only try to extract the region/zone code if code has 3 parts separated by #
                var sCode = ResortSplits[0].ResortCode.Split('#');
                if (sCode.Length == 3)
                {
                    SearchItemIDs.Add(sCode[0] + "#" + sCode[1]);
                }
            }

            public void SetSearchIDs()
            {
                // Create a list of all properties/cities that we will be searching (easier to keep track of this way)
                foreach (ResortSplit oResortSplit in ResortSplits)
                {
                    if (SearchByHotel)
                    {
                        foreach (iVector.Search.Property.Hotel oHotel in oResortSplit.Hotels)
                            SearchItemIDs.Add(oHotel.TPKey);
                    }
                    else
                    {
                        SearchItemIDs.Add(oResortSplit.ResortCode);
                    }
                }
            }

        }

        public string AppendURLs(IThirdPartyAttributeSearch SearchDetails)
        {

            var sbURLXML = new StringBuilder();

            sbURLXML.AppendFormat("<UrlReservation>{0}</UrlReservation>", _settings.get_UrlReservation(SearchDetails));
            sbURLXML.AppendFormat("<UrlGeneric>{0}</UrlGeneric>", _settings.get_UrlGeneric(SearchDetails));
            sbURLXML.AppendFormat("<UrlAvail>{0}</UrlAvail>", _settings.get_UrlAvail(SearchDetails));
            sbURLXML.AppendFormat("<UrlValuation>{0}</UrlValuation>", _settings.get_UrlValuation(SearchDetails));

            return sbURLXML.ToString();

        }

        private List<TransformedResult> GetResultFromResponse(TravelgateSearchResponse oResponse)
        {
            var oTransformedResults = new List<TransformedResult>();

            foreach (TravelgateSearchResponse.Hotel oHotel in oResponse.Hotels)
            {
                foreach (TravelgateSearchResponse.Meal oMealPlans in oHotel.Meals)
                {
                    foreach (TravelgateSearchResponse.OptionDetails oOption in oMealPlans.Options)
                    {
                        string encryptedParamters = EncryptParamters(oOption.Parameters);

                        foreach (TravelgateSearchResponse.Room oRoom in oOption.Rooms)
                        {
                            var oTransformedResult = new TransformedResult();
                            oTransformedResult.TPKey = oHotel.TPKey;
                            oTransformedResult.CurrencyCode = oOption.Price.Currency;
                            oTransformedResult.RoomType = oRoom.RoomType;
                            oTransformedResult.RoomTypeCode = oRoom.RoomTypeCode;
                            oTransformedResult.MealBasisCode = oMealPlans.MealBaisCode;
                            oTransformedResult.Amount = oOption.Price.Amount.ToSafeDecimal();
                            oTransformedResult.PropertyRoomBookingID = oRoom.PropertyRoomBookingID.ToSafeInt();
                            oTransformedResult.CommissionPercentage = oOption.Price.Commission.ToSafeDecimal();
                            oTransformedResult.NonRefundableRates = IsNonRefundable(oOption.RateRules, oRoom.NonRefunfable);
                            oTransformedResult.FixPrice = IsFixedPrice(oOption.Price.Binding, oOption.Price.Commission);
                            oTransformedResult.SellingPrice = GetSellingPrice(oOption.Price.Binding, oOption.Price.Amount);
                            oTransformedResult.NetPrice = CalculateNetPrice(oOption.Price.Binding, oOption.Price.Amount, oOption.Price.Commission);
                            oTransformedResult.TPRateCode = GetTPRateCode(oResponse.DailyRatePlans);
                            oTransformedResult.TPReference = oRoom.ID + "~" + oRoom.RoomTypeCode + "~" + oRoom.RoomType + "~" + oOption.PaymentType + "~" + encryptedParamters + "~" + oOption.Price.Commission + "~" + oOption.Price.Binding + "~" + oMealPlans.MealBaisCode;

                            oTransformedResults.Add(oTransformedResult);
                        }
                    }
                }
            }

            return oTransformedResults;
        }

        private string EncryptParamters(List<TravelgateSearchResponse.Parameter> oParameters)
        {
            return _secretKeeper.Encrypt("<Parameters>" + GetParamters(oParameters) + "</Parameters>");
        }

        private string GetParamters(List<TravelgateSearchResponse.Parameter> oParameters)
        {
            var sbParamters = new StringBuilder();
            foreach (TravelgateSearchResponse.Parameter oParameter in oParameters)
                sbParamters.AppendFormat("<Parameter key=\"{0}\" value=\"{1}\"></Parameter>", oParameter.Key, oParameter.Value);

            return sbParamters.ToSafeString();
        }

        private bool IsNonRefundable(List<TravelgateSearchResponse.RateRule> oRateRules, string sIsNonRefundable)
        {
            if (oRateRules.Any())
            {
                return oRateRules.First().Rules.FirstOrDefault().RateType.Equals("NonRefundable") | !string.IsNullOrEmpty(sIsNonRefundable) & sIsNonRefundable.Equals("true");
            }

            return false;
        }

        private bool IsFixedPrice(string sBinding, string sComissions)
        {
            return sBinding.Equals("true") & !sComissions.Equals("-1");
        }

        private string GetSellingPrice(string sBinding, string sAmount)
        {
            if (sBinding.Equals("true"))
            {
                return sAmount;
            }

            return "0";
        }

        private string CalculateNetPrice(string sBinding, string sAmount, string sComission)
        {
            if (sBinding.Equals("true"))
            {
                return (sAmount.ToSafeDecimal() * ((100m - sComission.ToSafeDecimal()) / 100m)).ToSafeString();
            }

            return "0";
        }

        private string GetTPRateCode(List<TravelgateSearchResponse.RatePlan> oDailyRatePlans)
        {
            if (oDailyRatePlans.Any())
            {
                return oDailyRatePlans.First().TPRateCode;
            }

            return string.Empty;
        }

        #endregion

    }
}