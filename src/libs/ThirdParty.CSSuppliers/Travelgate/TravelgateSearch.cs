namespace ThirdParty.CSSuppliers.Travelgate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.Travelgate.Models;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    // todo - allow travelgate itself as a third party
    // todo - refactor to avoid boilerplate code for shared tp interfaces
    public abstract class TravelgateSearch : IThirdPartySearch
    {
        #region Constructor

        private readonly ITravelgateSettings _settings;

        private readonly ITPSupport _support;

        private readonly ISecretKeeper _secretKeeper;

        private readonly ISerializer _serializer;

        public TravelgateSearch(ITravelgateSettings settings, ITPSupport support, ISecretKeeper secretKeeper, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public abstract string Source { get; }

        #endregion

        #region Search Restrictions

        public bool SearchRestrictions(SearchDetails searchDetails)
        {
            return false;
        }

        #endregion

        #region SearchFunctions

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            // Remove any suppliers in the dictionary that are attempting a restricted search
            // Needs to be done here since
            var filteredSuppliers = new List<string>();

            int maximumRoomNumber = _settings.get_MaximumRoomNumber(searchDetails);
            int maximumRoomGuestNumber = _settings.get_MaximumRoomGuestNumber(searchDetails);
            int minimumStay = _settings.get_MinimumStay(searchDetails);

            bool searchExceedsGuestCount = false;

            foreach (var room in searchDetails.RoomDetails)
            {
                if (room.Adults + room.Children + room.Infants > maximumRoomGuestNumber)
                {
                    searchExceedsGuestCount = true;
                }
            }

            if (!(searchDetails.Rooms > maximumRoomNumber || searchDetails.Duration < minimumStay || searchExceedsGuestCount))
            {
                filteredSuppliers.Add(Source);
            }

            if (filteredSuppliers.Count > 0)
            {
                var sbSearchRequest = new StringBuilder();

                sbSearchRequest.Append("<soapenv:Envelope xmlns:soapenv = \"http://schemas.xmlsoap.org/soap/envelope/\" ");
                sbSearchRequest.Append("xmlns:ns = \"http://schemas.xmltravelgate.com/hub/2012/06\" ");
                sbSearchRequest.Append("xmlns:wsse = \"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">");
                sbSearchRequest.Append("<soapenv:Header>");
                sbSearchRequest.Append("<wsse:Security>");
                sbSearchRequest.Append("<wsse:UsernameToken>");
                sbSearchRequest.AppendFormat("<wsse:Username>{0}</wsse:Username>", _settings.get_Username(searchDetails));
                sbSearchRequest.AppendFormat("<wsse:Password>{0}</wsse:Password>", _settings.get_Password(searchDetails));
                sbSearchRequest.Append("</wsse:UsernameToken>");
                sbSearchRequest.Append("</wsse:Security>");
                sbSearchRequest.Append("</soapenv:Header>");
                sbSearchRequest.Append("<soapenv:Body>");
                sbSearchRequest.Append("<ns:Avail>");
                sbSearchRequest.Append("<ns:availRQ>");
                sbSearchRequest.Append("<ns:timeoutMilliseconds>25000</ns:timeoutMilliseconds>"); // max general search timeout is 25s
                sbSearchRequest.Append("<ns:version>1</ns:version>");
                sbSearchRequest.Append("<ns:providerRQs>");

                int requestCount = 1;

                // Some third parties don't support searches by TPKey, so use these to check what kind of search we want to be doing
                int maximumHotelSearchNumber = _settings.get_MaximumHotelSearchNumber(searchDetails);
                int maximumCitySearchNumber = _settings.get_MaximumCitySearchNumber(searchDetails);
                // We generally prefer hotel based searches, but if a third party has a small maximum number of hotels per search
                // we leave it up to the discretion of the user to determine if they would rather perform city based searches (where allowed)
                bool allowHotelSearch = _settings.get_AllowHotelSearch(searchDetails);
                // Whether to try to search for a zone (region) instead of individual resorts (cities)
                bool useZoneSearch = resortSplits.Count > 1 && _settings.get_UseZoneSearch(searchDetails);

                var searchBatchDetails = new SearchBatchDetails();
                searchBatchDetails.Source = Source;
                searchBatchDetails.ResortSplits = resortSplits;

                // Check how many hotels we have - if only one we can ignore the allow hotel search boolean
                int hotelCount = 0;
                foreach (var resortSplit in resortSplits)
                {
                    hotelCount += resortSplit.Hotels.Count;
                }

                // Get a count of our resorts as well
                int resortCount = resortSplits.Count;

                // Set which search type we will be using - if hotel searches are allowed and either the allow flag is set to true,
                // or else city searches aren't allowed or we're only searching for one hotel, search by hotel
                // Otherwiwse search by city

                if (maximumHotelSearchNumber > 0 && (allowHotelSearch || maximumCitySearchNumber == 0 || hotelCount == 1))
                {
                    searchBatchDetails.SearchByHotel = true;

                    // Get the batch size and count for hotel searches
                    searchBatchDetails.BatchSize = maximumHotelSearchNumber;
                    searchBatchDetails.SearchItemCount = hotelCount;
                }
                else if (maximumCitySearchNumber > 0)
                {
                    searchBatchDetails.SearchByHotel = false;

                    if (useZoneSearch)
                    {
                        searchBatchDetails.SetZoneSearchID();
                        searchBatchDetails.UseZoneSearch = searchBatchDetails.SearchItemIDs.Count == 1;
                    }

                    if (searchBatchDetails.UseZoneSearch)
                    {
                        searchBatchDetails.BatchSize = 1;
                        searchBatchDetails.SearchItemCount = 1;
                    }
                    else
                    {
                        // Get the batch size and count for city searches
                        searchBatchDetails.BatchSize = maximumCitySearchNumber;
                        searchBatchDetails.SearchItemCount = resortCount;
                    }
                }

                searchBatchDetails.CalculateBatchCount();

                if (!searchBatchDetails.UseZoneSearch)
                {
                    searchBatchDetails.SetSearchIDs();
                }

                BuildSearchBatch(searchDetails, searchBatchDetails, ref sbSearchRequest, ref requestCount);

                // Next

                sbSearchRequest.Append("</ns:providerRQs>");
                sbSearchRequest.Append("</ns:availRQ>");
                sbSearchRequest.Append("</ns:Avail>");
                sbSearchRequest.Append("</soapenv:Body>");
                sbSearchRequest.Append("</soapenv:Envelope>");

                // Build Request Object
                var request = new Request
                {
                    EndPoint = _settings.get_URL(searchDetails),
                    SoapAction = _settings.get_SearchSOAPAction(searchDetails),
                    Method = eRequestMethod.POST,
                    ExtraInfo = searchDetails,
                    SuppressExpectHeaders = true,
                    UseGZip = _settings.get_UseGZip(searchDetails)
                };
                request.Headers.AddNew("SOAPAction", _settings.get_SearchSOAPAction(searchDetails));
                request.SetRequest(sbSearchRequest.ToString());

                requests.Add(request);
            }

            return requests;
        }

        public void BuildSearchBatch(SearchDetails searchDetails, SearchBatchDetails searchBatchDetails, ref StringBuilder searchRequest, ref int requestCount)
        {
            // Index to keep track of where we're at
            int index = 0;

            // Loop through the batches
            for (int batchNumber = 1; batchNumber <= searchBatchDetails.BatchCount; batchNumber++)
            {
                searchRequest.Append("<ns:ProviderRQ>");
                searchRequest.AppendFormat("<ns:code>{0}</ns:code>", _settings.get_ProviderCode(searchDetails));
                searchRequest.AppendFormat("<ns:id>{0}</ns:id>", requestCount);
                searchRequest.Append("<ns:rqXML>");
                searchRequest.Append("<AvailRQ>");
                searchRequest.AppendFormat("<timeoutMilliseconds>{0}</timeoutMilliseconds>", _settings.get_SearchRequestTimeout(searchDetails));
                searchRequest.Append("<source>");
                searchRequest.AppendFormat("<languageCode>{0}</languageCode>", _settings.get_LanguageCode(searchDetails));
                searchRequest.Append("</source>");
                searchRequest.Append("<filterAuditData>");
                searchRequest.Append("<registerTransactions>false</registerTransactions>");
                searchRequest.Append("</filterAuditData>");
                searchRequest.Append("<Configuration>");
                searchRequest.AppendFormat("<User>{0}</User>", _settings.get_ProviderUsername(searchDetails));
                searchRequest.AppendFormat("<Password>{0}</Password>", _settings.get_ProviderPassword(searchDetails));
                searchRequest.Append(AppendURLs(searchDetails));
                searchRequest.Append(HttpUtility.HtmlDecode(_settings.get_Parameters(searchDetails)));

                searchRequest.Append("</Configuration>");
                searchRequest.Append("<OnRequest>false</OnRequest>");
                searchRequest.Append("<AvailDestinations>");

                // Get the last item for our current batch
                int lastBatchItem;

                if (index + searchBatchDetails.BatchSize > searchBatchDetails.SearchItemCount)
                {
                    lastBatchItem = searchBatchDetails.SearchItemCount - 1;
                }
                else
                {
                    lastBatchItem = index + searchBatchDetails.BatchSize - 1;
                }

                for (int position = index; position <= lastBatchItem; position++)
                {
                    if (searchBatchDetails.SearchByHotel)
                    {
                        searchRequest.AppendFormat("<Destination type = \"HOT\" code = \"{0}\"/>", searchBatchDetails.SearchItemIDs.ElementAt(index));
                    }
                    else if (searchBatchDetails.UseZoneSearch)
                    {
                        searchRequest.AppendFormat("<Destination type = \"ZON\" code = \"{0}\"/>", searchBatchDetails.SearchItemIDs.ElementAt(index));
                    }
                    else
                    {
                        searchRequest.AppendFormat("<Destination type = \"CTY\" code = \"{0}\"/>", searchBatchDetails.SearchItemIDs.ElementAt(index));
                    }

                    index++;
                }

                searchRequest.Append("</AvailDestinations>");

                searchRequest.AppendFormat("<StartDate>{0}</StartDate>", searchDetails.ArrivalDate.ToString("dd/MM/yyyy"));
                searchRequest.AppendFormat("<EndDate>{0}</EndDate>", searchDetails.DepartureDate.ToString("dd/MM/yyyy"));
                searchRequest.AppendFormat("<Currency>{0}</Currency>", _settings.get_CurrencyCode(searchDetails));

                string nationality = _support.TPNationalityLookup(ThirdParties.TRAVELGATE, searchDetails.NationalityID);
                if (string.IsNullOrEmpty(nationality))
                {
                    nationality = _settings.get_DefaultNationality(searchDetails, false);
                }

                if (!string.IsNullOrEmpty(nationality))
                {
                    searchRequest.AppendFormat("<Nationality>{0}</Nationality>", nationality);
                    searchRequest.AppendFormat("<Markets><Market>{0}</Market></Markets>", nationality);
                }
                else
                {
                    string sDefaultNationality = _settings.get_DefaultNationality(searchDetails, false);
                    if (!string.IsNullOrEmpty(sDefaultNationality))
                    {
                        searchRequest.AppendFormat("<Markets><Market>{0}</Market></Markets>", sDefaultNationality);
                    }
                }

                string markets = _settings.get_Markets(searchDetails);
                if (markets.Length > 0)
                {
                    searchRequest.Append("<Markets>");
                    foreach (string sMarket in markets.Split(','))
                    {
                        searchRequest.AppendFormat("<Market>{0}</Market>", sMarket);
                    }

                    searchRequest.Append("</Markets>");
                }
                else if (!string.IsNullOrEmpty(nationality))
                {
                    searchRequest.AppendFormat("<Markets><Market>{0}</Market></Markets>", nationality);
                }

                searchRequest.Append("<RoomCandidates>");

                int roomIndex = 1;
                foreach (var roomDetails in searchDetails.RoomDetails)
                {
                    searchRequest.AppendFormat("<RoomCandidate id = \"{0}\">", roomIndex);
                    searchRequest.Append("<Paxes>");

                    int paxCount = 1;

                    for (int i = 1; i <= roomDetails.Adults; i++)
                    {
                        searchRequest.AppendFormat("<Pax age = \"30\" id = \"{0}\"/>", paxCount);
                        paxCount += 1;
                    }

                    if (roomDetails.Children > 0)
                    {
                        foreach (string sChildAge in roomDetails.ChildAgeCSV.Split(','))
                        {
                            searchRequest.AppendFormat("<Pax age = \"{0}\" id = \"{1}\"/>", sChildAge, paxCount);
                            paxCount += 1;
                        }
                    }

                    if (roomDetails.Infants > 0)
                    {
                        for (int i = 1, loopTo3 = roomDetails.Infants; i <= loopTo3; i++)
                        {
                            searchRequest.AppendFormat("<Pax age = \"1\" id = \"{0}\"/>", paxCount);
                            paxCount += 1;
                        }
                    }

                    searchRequest.Append("</Paxes>");
                    searchRequest.Append("</RoomCandidate>");

                    roomIndex += 1;
                }

                searchRequest.Append("</RoomCandidates>");

                searchRequest.Append("</AvailRQ>");
                searchRequest.Append("</ns:rqXML>");
                searchRequest.Append("</ns:ProviderRQ>");

                requestCount += 1;
            }
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var availabilityResponses = new List<TravelgateSearchResponse>();
            
            foreach (var request in requests)
            {
                if (request.Success)
                {
                    var responseXml = _serializer.CleanXmlNamespaces(request.ResponseXML);

                    // Retrieve response envelope
                    var responseEnvelope = new TravelgateResponseEnvelope();

                    // Deserialize the response Envelope
                    responseEnvelope = _serializer.DeSerialize<TravelgateResponseEnvelope>(responseXml.InnerXml);

                    string responses = responseEnvelope.Body.Response.Result.ProviderResults.FirstOrDefault().Results.Result;

                    // Decoded Xml if response encoded
                    if (responses.Contains("&gt;"))
                    {
                        HttpUtility.HtmlDecode(responses);
                    }

                    // Deserialize Response Body
                    var response = _serializer.DeSerialize<TravelgateSearchResponse>(responses);

                    availabilityResponses.Add(response);
                }
            }

            // Extract search results from responses
            transformedResults.TransformedResults.AddRange(availabilityResponses.Where(r => r.Hotels.Count > 0).SelectMany(x => GetResultFromResponse(x)));

            return transformedResults;
        }

        #endregion

        #region ResponseHasExceptions

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion

        #region Helper methods

        public class SearchBatchDetails
        {
            public string Source { get; set; }
            public List<ResortSplit> ResortSplits { get; set; }
            public bool SearchByHotel { get; set; }
            public bool UseZoneSearch { get; set; }
            public int BatchSize { get; set; }
            public int BatchCount { get; set; }
            public int SearchItemCount { get; set; }
            public List<string> SearchItemIDs { get; set; } = new();

            public void CalculateBatchCount()
            {
                BatchCount = (int)Math.Round(Math.Ceiling(SearchItemCount / (double)BatchSize));
            }

            public void SetZoneSearchID()
            {
                // Only try to extract the region/zone code if code has 3 parts separated by #
                var code = ResortSplits[0].ResortCode.Split('#');
                if (code.Length == 3)
                {
                    SearchItemIDs.Add(code[0] + "#" + code[1]);
                }
            }

            public void SetSearchIDs()
            {
                // Create a list of all properties/cities that we will be searching (easier to keep track of this way)
                foreach (var resortSplit in ResortSplits)
                {
                    if (SearchByHotel)
                    {
                        foreach (var hotel in resortSplit.Hotels)
                        {
                            SearchItemIDs.Add(hotel.TPKey);
                        }
                    }
                    else
                    {
                        SearchItemIDs.Add(resortSplit.ResortCode);
                    }
                }
            }
        }

        public string AppendURLs(IThirdPartyAttributeSearch searchDetails)
        {
            var sbURLXML = new StringBuilder();

            sbURLXML.AppendFormat("<UrlReservation>{0}</UrlReservation>", _settings.get_UrlReservation(searchDetails));
            sbURLXML.AppendFormat("<UrlGeneric>{0}</UrlGeneric>", _settings.get_UrlGeneric(searchDetails));
            sbURLXML.AppendFormat("<UrlAvail>{0}</UrlAvail>", _settings.get_UrlAvail(searchDetails));
            sbURLXML.AppendFormat("<UrlValuation>{0}</UrlValuation>", _settings.get_UrlValuation(searchDetails));

            return sbURLXML.ToString();
        }

        private List<TransformedResult> GetResultFromResponse(TravelgateSearchResponse response)
        {
            var transformedResults = new List<TransformedResult>();

            foreach (var hotel in response.Hotels)
            {
                foreach (var mealPlans in hotel.Meals)
                {
                    foreach (var option in mealPlans.Options)
                    {
                        string encryptedParamters = EncryptParamters(option.Parameters);

                        foreach (var room in option.Rooms)
                        {
                            var transformedResult = new TransformedResult
                            {
                                TPKey = hotel.TPKey,
                                CurrencyCode = option.Price.Currency,
                                RoomType = room.RoomType,
                                RoomTypeCode = room.RoomTypeCode,
                                MealBasisCode = mealPlans.MealBaisCode,
                                Amount = option.Price.Amount.ToSafeDecimal(),
                                PropertyRoomBookingID = room.PropertyRoomBookingID.ToSafeInt(),
                                CommissionPercentage = option.Price.Commission.ToSafeDecimal(),
                                NonRefundableRates = IsNonRefundable(option.RateRules, room.NonRefunfable),
                                FixPrice = IsFixedPrice(option.Price.Binding, option.Price.Commission),
                                SellingPrice = GetSellingPrice(option.Price.Binding, option.Price.Amount),
                                NetPrice = CalculateNetPrice(option.Price.Binding, option.Price.Amount, option.Price.Commission),
                                TPRateCode = GetTPRateCode(response.DailyRatePlans),
                                TPReference = room.ID + "~" + room.RoomTypeCode + "~" + room.RoomType + "~" + option.PaymentType + "~" + encryptedParamters + "~" + option.Price.Commission + "~" + option.Price.Binding + "~" + mealPlans.MealBaisCode
                            };

                            transformedResults.Add(transformedResult);
                        }
                    }
                }
            }

            return transformedResults;
        }

        private string EncryptParamters(List<TravelgateSearchResponse.Parameter> parameters)
        {
            return _secretKeeper.Encrypt("<Parameters>" + GetParamters(parameters) + "</Parameters>");
        }

        private string GetParamters(List<TravelgateSearchResponse.Parameter> parameters)
        {
            var sbParamters = new StringBuilder();
            foreach (var parameter in parameters)
            {
                sbParamters.AppendFormat("<Parameter key=\"{0}\" value=\"{1}\"></Parameter>", parameter.Key, parameter.Value);
            }

            return sbParamters.ToSafeString();
        }

        private bool IsNonRefundable(List<TravelgateSearchResponse.RateRule> rateRules, string isNonRefundable)
        {
            if (rateRules.Any())
            {
                return rateRules.First().Rules.FirstOrDefault().RateType.Equals("NonRefundable") ||
                    (!string.IsNullOrEmpty(isNonRefundable) && isNonRefundable.Equals("true"));
            }

            return false;
        }

        private bool IsFixedPrice(string binding, string comissions)
        {
            return binding.Equals("true") && !comissions.Equals("-1");
        }

        private string GetSellingPrice(string binding, string amount)
        {
            if (binding.Equals("true"))
            {
                return amount;
            }

            return "0";
        }

        private string CalculateNetPrice(string binding, string amount, string commission)
        {
            if (binding.Equals("true"))
            {
                return (amount.ToSafeDecimal() * ((100m - commission.ToSafeDecimal()) / 100m)).ToSafeString();
            }

            return "0";
        }

        private string GetTPRateCode(List<TravelgateSearchResponse.RatePlan> dailyRatePlans)
        {
            if (dailyRatePlans.Any())
            {
                return dailyRatePlans.First().TPRateCode;
            }

            return string.Empty;
        }

        #endregion
    }
}