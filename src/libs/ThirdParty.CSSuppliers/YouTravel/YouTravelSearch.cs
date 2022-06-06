namespace ThirdParty.CSSuppliers.YouTravel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using iVector.Search.Property;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;

    public class YouTravelSearch : IThirdPartySearch
    {
        #region Constructor

        public YouTravelSearch(IYouTravelSettings settings, ITPSupport support, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region Properties

        private readonly IYouTravelSettings _settings;

        private readonly ITPSupport _support;

        private readonly ISerializer _serializer;

        public string Source => ThirdParties.YOUTRAVEL;

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails oSearchDetails)
        {

            bool bRestrictions = false;

            // no more than 3 rooms allowed
            if (oSearchDetails.RoomDetails.Count > 3)
            {
                bRestrictions = true;
            }

            return bRestrictions;

        }

        #endregion

        #region SearchFunctions

        public List<Request> BuildSearchRequests(SearchDetails oSearchDetails, List<ResortSplit> oResortSplits, bool bSaveLogs)
        {

            var oRequests = new List<Request>();

            // if a single resort do that, else find distinct destination and loop through these
            var oURLs = new RequestURLs();

            if (oResortSplits.Count == 1)
            {
                if (oResortSplits[0].Hotels.Count == 1)
                {
                    string sHotelCode = oResortSplits[0].Hotels[0].TPKey;
                    string sHotelURL = _settings.get_SearchURL(oSearchDetails) + string.Format("?HID={0}", sHotelCode);
                    oURLs.Add(sHotelURL, "HID", sHotelCode);
                }

                string sResortCode = oResortSplits[0].ResortCode.Split('_')[1];
                string sResortURL = _settings.get_SearchURL(oSearchDetails) + string.Format("?RSRT={0}", sResortCode);
                oURLs.Add(sResortURL, "RSRT", sResortCode);
            }

            else
            {
                var oDestinations = YouTravelSupport.GetDistinctDestinations(oResortSplits);
                foreach (string sDestination in oDestinations)
                {
                    string sDestinationURL = _settings.get_SearchURL(oSearchDetails) + string.Format("?DSTN={0}", sDestination);
                    oURLs.Add(sDestinationURL, "DSTN", sDestination);
                }
            }

            // build string with addition critera (dates, passengers)
            var sbSuffix = new StringBuilder();
            sbSuffix.AppendFormat("&LangID={0}", _settings.get_LangID(oSearchDetails));
            sbSuffix.AppendFormat("&Username={0}", _settings.get_Username(oSearchDetails));
            sbSuffix.AppendFormat("&Password={0}", _settings.get_Password(oSearchDetails));
            sbSuffix.AppendFormat("&Checkin_Date={0}", YouTravelSupport.FormatDate(oSearchDetails.PropertyArrivalDate));
            sbSuffix.AppendFormat("&Nights={0}", oSearchDetails.PropertyDuration);
            sbSuffix.AppendFormat("&Rooms={0}", oSearchDetails.Rooms);


            // adults and children
            int iRoomIndex = 0;
            foreach (RoomDetail oRoom in oSearchDetails.RoomDetails)
            {
                iRoomIndex += 1;
                sbSuffix.AppendFormat("&ADLTS_{0}={1}", iRoomIndex, oRoom.Adults);

                if (oRoom.Children + oRoom.Infants > 0)
                {
                    sbSuffix.AppendFormat("&CHILD_{0}={1}", iRoomIndex, oRoom.Children + oRoom.Infants);
                    int iChildCount = 1;
                    foreach (int iChildAge in oRoom.ChildAndInfantAges(1))
                    {
                        sbSuffix.AppendFormat("&ChildAgeR{0}C{1}={2}", iRoomIndex, iChildCount, iChildAge);
                        iChildCount += 1;
                    }
                }
            }
            sbSuffix.Append("&CanxPol=1");

            foreach (RequestURL oURL in oURLs)
            {

                // set a unique code. if the is one request we only need the source name
                string sUniqueCode = Source;
                if (oURLs.Count > 1)
                    sUniqueCode = oURL.UniqueRequestID(Source);

                var oRequest = new Request();
                oRequest.EndPoint = oURL.URL + sbSuffix.ToString();
                oRequest.Method = eRequestMethod.GET;
                oRequest.Source = Source;
                oRequest.LogFileName = "Search";
                oRequest.CreateLog = bSaveLogs;
                oRequest.ExtraInfo = new SearchExtraHelper(oSearchDetails, sUniqueCode);
                oRequest.UseGZip = true;

                oRequests.Add(oRequest);

            }

            return oRequests;

        }


        public TransformedResultCollection TransformResponse(List<Request> oRequests, SearchDetails oSearchDetails, List<ResortSplit> oResortSplits)
        {

            var results = oRequests.Select(o => _serializer.DeSerialize<YouTravelSearchResponse>(o.ResponseXML));
            var sessions = results.Where(o => o.Success.ToSafeBoolean()).Select(o => o.session).ToList();

            var oTransformedResults = new TransformedResultCollection();

            oTransformedResults.TransformedResults.AddRange(sessions.SelectMany(o => CreateTransformedResponse(o)));

            if (oSearchDetails.StarRating is not null)
            {
                oSearchDetails.StarRating = oSearchDetails.StarRating.Replace("+", "");
            }

            return oTransformedResults;

        }

        private List<TransformedResult> CreateTransformedResponse(Session oSession)
        {
            var results = new List<TransformedResult>();

            string currencyCode = oSession.Currency;
            string sessionId = oSession.id;
            results.AddRange(oSession.Hotel.SelectMany(o => CreateHotelResults(o, currencyCode, sessionId)));

            return results;
        }

        private List<TransformedResult> CreateHotelResults(Hotel oHotel, string sCurrencyCode, string sSessionId)
        {
            var results = new List<TransformedResult>();

            string tpKey = oHotel.ID;
            results.AddRange(CreateRoomsFromRoomResultCollection(oHotel.Room_1.Room, 1, sCurrencyCode, tpKey, sSessionId));
            results.AddRange(CreateRoomsFromRoomResultCollection(oHotel.Room_2.Room, 2, sCurrencyCode, tpKey, sSessionId));
            results.AddRange(CreateRoomsFromRoomResultCollection(oHotel.Room_3.Room, 3, sCurrencyCode, tpKey, sSessionId));

            return results;
        }

        private List<TransformedResult> CreateRoomsFromRoomResultCollection(Room[] oRooms, int iPropertyRoomBookingID, string sCurrencyCode, string sTPKey, string sSessionId)
        {
            var results = new List<TransformedResult>();
            foreach (Room room in oRooms)
            {
                var result = new TransformedResult();
                result.TPKey = sTPKey;
                result.CurrencyCode = sCurrencyCode;
                result.PropertyRoomBookingID = iPropertyRoomBookingID;
                result.RoomType = room.Type;
                result.MealBasisCode = room.Board;
                result.Amount = room.Rates.Final_Rate;
                result.NonRefundableRates = !room.Refundable.ToSafeBoolean();
                result.TPReference = $"{sSessionId}_{room.Id}_{room.CanxPolicy.token}";
                results.Add(result);
            }
            return results;
        }

        #endregion

        #region ResponseHasExceptions
        public bool ResponseHasExceptions(Request oRequest)
        {
            return false;
        }
        #endregion

        private class RequestURLs : List<RequestURL>
        {

            public void Add(string URL, string RequestType, string RequestID)
            {
                var oURL = new RequestURL(URL, RequestType, RequestID);
                Add(oURL);
            }

        }

        private class RequestURL
        {
            public string URL = string.Empty;
            public string RequestType = string.Empty;
            public string RequestID = string.Empty;

            public RequestURL(string URL, string RequestType, string RequestID)
            {
                this.URL = URL;
                this.RequestType = RequestType;
                this.RequestID = RequestID;
            }

            public string UniqueRequestID(string Source)
            {
                return string.Format("{0}_{1}_{2}", Source, RequestType, RequestID);
            }

        }

    }
}