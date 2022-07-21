namespace iVectorOne.CSSuppliers.YouTravel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    public class YouTravelSearch : IThirdPartySearch, ISingleSource
    {
        #region Constructor

        private readonly IYouTravelSettings _settings;

        private readonly ISerializer _serializer;

        public YouTravelSearch(IYouTravelSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.YOUTRAVEL;

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            bool restrictions = false;

            // no more than 3 rooms allowed
            if (searchDetails.RoomDetails.Count > 3)
            {
                restrictions = true;
            }

            return restrictions;
        }

        #endregion

        #region SearchFunctions

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            // if a single resort do that, else find distinct destination and loop through these
            var urls = new RequestURLs();

            if (resortSplits.Count == 1)
            {
                if (resortSplits[0].Hotels.Count == 1)
                {
                    string hotelCode = resortSplits[0].Hotels[0].TPKey;
                    string hotelURL = _settings.SearchURL(searchDetails) + string.Format("?HID={0}", hotelCode);
                    urls.Add(hotelURL, "HID", hotelCode);
                }

                string resortCode = resortSplits[0].ResortCode.Split('_')[1];
                string resortURL = _settings.SearchURL(searchDetails) + string.Format("?RSRT={0}", resortCode);
                urls.Add(resortURL, "RSRT", resortCode);
            }
            else
            {
                var destinations = YouTravelSupport.GetDistinctDestinations(resortSplits);
                foreach (string destination in destinations)
                {
                    string destinationURL = _settings.SearchURL(searchDetails) + string.Format("?DSTN={0}", destination);
                    urls.Add(destinationURL, "DSTN", destination);
                }
            }

            // build string with addition critera (dates, passengers)
            var sb = new StringBuilder();
            sb.AppendFormat("&LangID={0}", _settings.LanguageCode(searchDetails));
            sb.AppendFormat("&Username={0}", _settings.User(searchDetails));
            sb.AppendFormat("&Password={0}", _settings.Password(searchDetails));
            sb.AppendFormat("&Checkin_Date={0}", YouTravelSupport.FormatDate(searchDetails.ArrivalDate));
            sb.AppendFormat("&Nights={0}", searchDetails.Duration);
            sb.AppendFormat("&Rooms={0}", searchDetails.Rooms);

            // adults and children
            int roomIndex = 0;
            foreach (var room in searchDetails.RoomDetails)
            {
                roomIndex += 1;
                sb.AppendFormat("&ADLTS_{0}={1}", roomIndex, room.Adults);

                if (room.Children + room.Infants > 0)
                {
                    sb.AppendFormat("&CHILD_{0}={1}", roomIndex, room.Children + room.Infants);
                    int childCount = 1;
                    foreach (int childAge in room.ChildAndInfantAges(1))
                    {
                        sb.AppendFormat("&ChildAgeR{0}C{1}={2}", roomIndex, childCount, childAge);
                        childCount += 1;
                    }
                }
            }

            sb.Append("&CanxPol=1");

            foreach (var url in urls)
            {
                // set a unique code. if the is one request we only need the source name
                string uniqueCode = Source;
                if (urls.Count > 1)
                {
                    uniqueCode = url.UniqueRequestID(Source);
                }

                var request = new Request
                {
                    EndPoint = url.URL + sb.ToString(),
                    Method = RequestMethod.GET,
                };

                requests.Add(request);
            }

            return Task.FromResult(requests);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var results = requests.Select(o => _serializer.DeSerialize<YouTravelSearchResponse>(_serializer.CleanXmlNamespaces(o.ResponseXML)));
            var sessions = results.Where(o => o.Success.ToSafeBoolean()).Select(o => o.session).ToList();

            var transformedResults = new TransformedResultCollection();

            transformedResults.TransformedResults.AddRange(sessions.SelectMany(o => CreateTransformedResponse(o)));

            return transformedResults;
        }

        private List<TransformedResult> CreateTransformedResponse(Session session)
        {
            var results = new List<TransformedResult>();

            string currencyCode = session.Currency;
            string sessionId = session.id;
            results.AddRange(session.Hotel.SelectMany(o => CreateHotelResults(o, currencyCode, sessionId)));

            return results;
        }

        private List<TransformedResult> CreateHotelResults(Hotel hotel, string currencyCode, string sessionId)
        {
            var results = new List<TransformedResult>();

            string tpKey = hotel.ID;
            results.AddRange(CreateRoomsFromRoomResultCollection(hotel.Room_1.Room, 1, currencyCode, tpKey, sessionId));

            if (hotel.Room_2 is not null)
            {
                results.AddRange(CreateRoomsFromRoomResultCollection(hotel.Room_2.Room, 2, currencyCode, tpKey, sessionId));
            }

            if (hotel.Room_3 is not null)
            {
                results.AddRange(CreateRoomsFromRoomResultCollection(hotel.Room_3.Room, 3, currencyCode, tpKey, sessionId));
            }

            return results;
        }

        private List<TransformedResult> CreateRoomsFromRoomResultCollection(Room[] rooms, int propertyRoomBookingId, string currencyCode, string tpKey, string sessionId)
        {
            var results = new List<TransformedResult>();
            foreach (var room in rooms)
            {
                var result = new TransformedResult
                {
                    TPKey = tpKey,
                    CurrencyCode = currencyCode,
                    PropertyRoomBookingID = propertyRoomBookingId,
                    RoomType = room.Type,
                    MealBasisCode = room.Board,
                    Amount = room.Rates.Final_Rate,
                    NonRefundableRates = !room.Refundable.ToSafeBoolean(),
                    TPReference = $"{sessionId}_{room.Id}_{room.CanxPolicy.token}"
                };
                results.Add(result);
            }
            return results;
        }

        #endregion

        #region ResponseHasExceptions
        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }
        #endregion

        private class RequestURLs : List<RequestURL>
        {
            public void Add(string url, string requestType, string requestId)
            {
                var requestUrl = new RequestURL(url, requestType, requestId);
                Add(requestUrl);
            }
        }

        private class RequestURL
        {
            public string URL = string.Empty;
            public string RequestType = string.Empty;
            public string RequestID = string.Empty;

            public RequestURL(string url, string requestType, string requestId)
            {
                URL = url;
                RequestType = requestType;
                RequestID = requestId;
            }

            public string UniqueRequestID(string source) => $"{source}_{RequestType}_{RequestID}";
        }
    }
}