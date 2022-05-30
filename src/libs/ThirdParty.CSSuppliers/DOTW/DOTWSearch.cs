namespace ThirdParty.CSSuppliers.DOTW
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using Intuitive.Net.WebRequests;
    using Newtonsoft.Json;
    using ThirdParty.Search.Models;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Models;
    using ThirdParty.Lookups;
    using ThirdParty.Search.Support;
    using ThirdParty.Results;
    using iVector.Search.Property;
    using ThirdParty.Models.Property.Booking;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Security;
    using Microsoft.Extensions.Logging;
    using System.Text;
    using ThirdParty.CSSuppliers.DOTW.Models;

    public class DOTWSearch : ThirdPartyPropertySearchBase
    {

        #region Constructor

        public DOTWSearch(IDOTWSettings settings, ITPSupport support, ISecretKeeper secretKeeper, ILogger<DOTWSearch> logger, IDOTWSupport dotwSupport) : base(logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _dotwSupport = dotwSupport;
        }

        public DOTWSearch(IDOTWSettings settings, ITPSupport support, ISecretKeeper secretKeeper, ILogger<DOTWSearch> logger) : base(logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _dotwSupport = new DOTWSupport();
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
        }


        #endregion

        #region Properties

        public override string Source => ThirdParties.DOTW;

        public override bool SqlRequest => false;

        public override bool SupportsNonRefundableTagging => false;

        private readonly IDOTWSettings _settings;

        private readonly ITPSupport _support;

        private readonly ISecretKeeper _secretKeeper;

        private IDOTWSupport _dotwSupport { get; set; }

        #endregion

        #region SearchRestrictions

        public override bool SearchRestrictions(SearchDetails searchDetails)
        {
            return false;
        }

        #endregion

        #region SearchFunctions

        public override List<Intuitive.Net.WebRequests.Request> BuildSearchRequests(SearchDetails oSearchDetails, List<ResortSplit> oResortSplits, bool bSaveLogs)
        {

            var oRequests = new List<Intuitive.Net.WebRequests.Request>();

            // get cities and sub locations
            var oCities = new DOTWSupport.Cities();
            foreach (ResortSplit oResort in oResortSplits)
            {
                if (oResort.ResortCode.Contains('|'))
                {

                    int iCityNumber = (oResort.ResortCode.Split('|')[0]).ToSafeInt();
                    int iLocationID = (oResort.ResortCode.Split('|')[1]).ToSafeInt();

                    if (!(iCityNumber == 0) && oCities.ContainsKey(iCityNumber))
                    {
                        DOTWSupport.City oCity = oCities[iCityNumber];
                        if (!oCity.LocationIDs.Contains(iLocationID))
                        {
                            oCity.LocationIDs.Add(iLocationID);
                        }
                    }
                    else
                    {
                        var oCity = new DOTWSupport.City(iCityNumber, iLocationID);
                        oCities.Add(iCityNumber, oCity);
                    }

                }
            }

            foreach (KeyValuePair<int, DOTWSupport.City> oCityKeyValue in oCities)
            {


                // create the search request for this city
                var oCity = oCityKeyValue.Value;

                string sRequest = BuildSearchRequestXML(oSearchDetails, oCity, _dotwSupport);


                var oRequest = new Request();
                oRequest.EndPoint = _settings.ServerURL(oSearchDetails);
                oRequest.Method = eRequestMethod.POST;
                oRequest.Source = Source;
                oRequest.LogFileName = "Search";
                oRequest.CreateLog = bSaveLogs;
                oRequest.TimeoutInSeconds = this.RequestTimeOutSeconds(oSearchDetails) - 2;
                oRequest.ExtraInfo = oSearchDetails;
                oRequest.SetRequest(sRequest);
                oRequest.UseGZip = true;

                oRequests.Add(oRequest);

            }

            return oRequests;
        }

        public override TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var responses = new List<DOTWSearchResponse>();


            foreach (var request in requests)
            {
                var response = new DOTWSearchResponse();
                bool success = request.Success;

                if (success)
                {
                    response = JsonConvert.DeserializeObject<DOTWSearchResponse>(request.ResponseString);
                    responses.Add(response);
                }
            }

            transformedResults.TransformedResults
            .AddRange(responses.Where(o => o.Hotels.Count() > 0)
            .SelectMany(r => GetResultFromResponse(searchDetails, r)));

            return transformedResults;
        }

        #endregion

        #region ResponseHasExceptions
        public override bool ResponseHasExceptions(Request oRequest)
        {
            return false;
        }
        #endregion

        #region Helpers

        public string BuildSearchRequestXML(SearchDetails oSearchDetails, DOTWSupport.City oCity, IDOTWSupport oSupport)
        {

            int iSalesChannelID = oSearchDetails.SalesChannelID;
            int iBrandID = oSearchDetails.BrandID;

            var oSB = new StringBuilder();


            oSB.AppendLine("<customer>");
            oSB.AppendFormat("<username>{0}</username>", _settings.Username(oSearchDetails));
            oSB.AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(oSearchDetails)));
            oSB.AppendFormat("<id>{0}</id>", _settings.CompanyCode(oSearchDetails));
            oSB.AppendLine("<source>1</source>");
            oSB.AppendLine("<product>hotel</product>");
            oSB.AppendLine("<request command=\"searchhotels\" debug=\"0\">");
            oSB.AppendLine("<bookingDetails>");
            oSB.AppendFormat("<fromDate>{0}</fromDate>", oSearchDetails.PropertyArrivalDate.ToString("yyyy-MM-dd"));
            oSB.AppendFormat("<toDate>{0}</toDate>", oSearchDetails.PropertyDepartureDate.ToString("yyyy-MM-dd"));
            oSB.AppendFormat("<currency>{0}</currency>", oSupport.GetCachedCurrencyID(oSearchDetails, _support, oSearchDetails.CurrencyCode, _settings));

            oSB.AppendFormat("<rooms no = \"{0}\">", oSearchDetails.Rooms);

            int iRoomRunNo = 0;
            foreach (RoomDetail oRoomDetail in oSearchDetails.RoomDetails)
            {

                List<int> oChildAndInfantAges = oRoomDetail.ChildAndInfantAges(1).Where(i => i <= 12).ToList();

                int iAdults = oRoomDetail.Adults + oRoomDetail.Children + oRoomDetail.Infants - oChildAndInfantAges.Count;
                int iChildren = oChildAndInfantAges.Count;

                oSB.AppendFormat("<room runno = \"{0}\">", iRoomRunNo);
                oSB.AppendFormat("<adultsCode>{0}</adultsCode>", iAdults);
                oSB.AppendFormat("<children no = \"{0}\">", iChildren);

                // append the children
                int iChildRunNo = 0;
                foreach (int iChildAge in oChildAndInfantAges)
                {
                    oSB.AppendFormat("<child runno=\"{0}\">{1}</child>", iChildRunNo, iChildAge);
                    iChildRunNo += 1;
                }

                oSB.AppendLine("</children>");
                oSB.AppendLine("<extraBed>0</extraBed>");
                oSB.AppendLine("<rateBasis>-1</rateBasis>");

                // Nationality and Country of residence				
                if (_settings.Version(oSearchDetails) == "2")
                {

                    string sNationality = DOTW.GetNationality(oSearchDetails.NationalityID, oSearchDetails, _support, _settings);
                    string sCountryCode = DOTW.GetCountryOfResidence(sNationality, oSearchDetails, _settings);

                    if (!string.IsNullOrEmpty(sNationality))
                    {
                        oSB.AppendFormat("<passengerNationality>{0}</passengerNationality>", sNationality);
                    }

                    if (!string.IsNullOrEmpty(sCountryCode))
                    {
                        oSB.AppendFormat("<passengerCountryOfResidence>{0}</passengerCountryOfResidence>", sCountryCode);
                    }
                }

                oSB.AppendLine("</room>");
                iRoomRunNo += 1;

            }

            oSB.AppendLine("</rooms>");
            oSB.AppendLine("</bookingDetails>");
            oSB.AppendLine("<return>");
            oSB.AppendLine("<sorting order = \"asc\">sortByPrice</sorting>");
            oSB.AppendLine("<getRooms>true</getRooms>");
            oSB.AppendLine("<filters xmlns:a = \"http://us.dotwconnect.com/xsd/atomicCondition\" xmlns:c = \"http://us.dotwconnect.com/xsd/complexCondition\">");
            oSB.AppendFormat("<city>{0}</city>", oCity.CityNumber);
            oSB.AppendLine("<nearbyCities>false</nearbyCities>");


            // conditions
            oSB.AppendLine("<c:condition>");


            // availability = not on request
            oSB.AppendLine("<a:condition>");
            oSB.AppendLine("<fieldName>onRequest</fieldName>");
            oSB.AppendLine("<fieldTest>equals</fieldTest> ");
            oSB.AppendLine("<fieldValues>");
            oSB.AppendLine("<fieldValue>0</fieldValue> ");
            oSB.AppendLine("</fieldValues>");
            oSB.AppendLine("</a:condition>");


            // put in the city locations
            // only need to do this is there is more than one location
            if (oCity.LocationIDs.Count > 1)
            {
                oSB.AppendLine("<operator>AND</operator>");
                oSB.AppendLine("<a:condition>");
                oSB.AppendLine("<fieldName>locationId</fieldName>");
                oSB.AppendLine("<fieldTest>in</fieldTest>");
                oSB.AppendLine("<fieldValues>");
                foreach (int iLocationID in oCity.LocationIDs)
                    oSB.AppendFormat("<fieldValue>{0}</fieldValue>", iLocationID);
                oSB.AppendLine("</fieldValues>");
                oSB.AppendLine("</a:condition>");
            }

            oSB.AppendLine("</c:condition>");
            oSB.AppendLine("</filters>");
            oSB.AppendLine("<fields>");
            oSB.AppendLine("<field>hotelName</field>");
            oSB.AppendLine("<field>noOfRooms</field>");
            oSB.AppendLine("<roomField>name</roomField>");
            oSB.AppendLine("<roomField>including</roomField>");
            oSB.AppendLine("<roomField>minStay</roomField>");
            oSB.AppendLine("</fields>");
            oSB.AppendLine("</return>");
            oSB.AppendLine("</request>");
            oSB.AppendLine("</customer>");

            return oSB.ToString();

        }

        private List<TransformedResult> GetResultFromResponse(SearchDetails searchDetails, DOTWSearchResponse response)
        {
            List<TransformedResult> transformedResults = new List<TransformedResult>();

            foreach (var hotel in response.Hotels)
            {
                foreach (var room in hotel.Rooms)
                {
                    foreach (var roomtype in room.RoomTypes) 
                    { 
                  
                        foreach (var rateBasis in roomtype.RateBases)
                        {
                            // Some DOTW Rooms have a minimum stay Length so I need to remove any that don't fulfil the criteria
                            // exclude DOTW's third parties if necessary. 1 is their own static stock, 2 is their own dynamic stock and 
                            // 3 is their third party dynamic stock

                            if (!string.IsNullOrEmpty(rateBasis.MinStay) ||!(rateBasis.RateType.Type == "1" || 
                                rateBasis.RateType.Type == "2" || !_settings.ExcludeDOTWThirdParties(searchDetails))) { continue; }

                            var amount = _settings.UseMinimumSellingPrice(searchDetails) && rateBasis.TotalMinSelling != null
                                    && !string.IsNullOrEmpty(rateBasis.TotalMinSelling.Total) ?
                                    rateBasis.TotalMinSelling.Total.ToSafeDecimal() : rateBasis.Total.TotalCost.ToSafeDecimal(); 

                            TransformedResult transformedResult = new TransformedResult()
                            {
                                MasterID = hotel.HotelID.ToSafeInt(), 
                                TPKey = hotel.HotelID, 
                                CurrencyCode = rateBasis.RateType.CurrencyID, 
                                PropertyRoomBookingID = room.RoomNum, 
                                RoomType = roomtype.Name,
                                RoomTypeCode = roomtype.Code,
                                MealBasisCode = rateBasis.ID,
                                Amount = amount,
                                DynamicProperty = rateBasis.WithinCancellationDeadline == "yes",
                                TPReference = roomtype.Code + rateBasis.ID
                            };
                        }
                    }
                }
            }


            return transformedResults; 
        }

        #endregion

    }
}
