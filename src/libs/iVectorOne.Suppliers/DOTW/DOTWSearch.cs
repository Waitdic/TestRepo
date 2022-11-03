namespace iVectorOne.Suppliers.DOTW
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVectorOne.Search.Models;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.DOTW.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Search.Results.Models;

    public class DOTWSearch : IThirdPartySearch, ISingleSource
    {
        #region Constructor

        private readonly IDOTWSettings _settings;

        private readonly ITPSupport _support;

        private readonly IDOTWSupport _dotwSupport;

        private readonly ISerializer _serializer;

        public DOTWSearch(IDOTWSettings settings, ITPSupport support, IDOTWSupport dotwSupport, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _dotwSupport = Ensure.IsNotNull(dotwSupport, nameof(dotwSupport));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.DOTW;

        public bool SupportsNonRefundableTagging => false;

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails searchDetails, string source) => false;

        #endregion

        #region SearchFunctions

        public async Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            // get cities and sub locations
            var cities = new DOTWSupport.Cities();
            foreach (var resort in resortSplits)
            {
                if (resort.ResortCode.Contains('|'))
                {
                    int cityNumber = resort.ResortCode.Split('|')[0].ToSafeInt();
                    int locationId = resort.ResortCode.Split('|')[1].ToSafeInt();

                    if (!(cityNumber == 0) && cities.ContainsKey(cityNumber))
                    {
                        var city = cities[cityNumber];
                        if (!city.LocationIDs.Contains(locationId))
                        {
                            city.LocationIDs.Add(locationId);
                        }
                    }
                    else
                    {
                        var city = new DOTWSupport.City(cityNumber, locationId);
                        cities.Add(cityNumber, city);
                    }
                }
            }

            foreach (var cityKeyValue in cities)
            {
                // create the search request for this city
                var city = cityKeyValue.Value;

                string requestString = await BuildSearchRequestXMLAsync(searchDetails, city);

                var request = new Request
                {
                    EndPoint = _settings.GenericURL(searchDetails),
                    Method = RequestMethod.POST,
                    ExtraInfo = searchDetails,
                    UseGZip = true
                };

                request.SetRequest(requestString);

                requests.Add(request);
            }

            return requests;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var responses = new List<DOTWSearchResponse>();

            foreach (var request in requests)
            {
                var response = new DOTWSearchResponse();

                if (request.Success)
                {
                    response = _serializer.DeSerialize<DOTWSearchResponse>(request.ResponseString);

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

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion

        #region Helpers

        public async Task<string> BuildSearchRequestXMLAsync(SearchDetails searchDetails, DOTWSupport.City city)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<customer>");
            sb.AppendFormat("<username>{0}</username>", _settings.User(searchDetails));
            sb.AppendFormat("<password>{0}</password>", _dotwSupport.MD5Password(_settings.Password(searchDetails)));
            sb.AppendFormat("<id>{0}</id>", _settings.CompanyCode(searchDetails));
            sb.AppendLine("<source>1</source>");
            sb.AppendLine("<product>hotel</product>");
            sb.AppendLine("<request command=\"searchhotels\" debug=\"0\">");
            sb.AppendLine("<bookingDetails>");
            sb.AppendFormat("<fromDate>{0}</fromDate>", searchDetails.ArrivalDate.ToString("yyyy-MM-dd"));
            sb.AppendFormat("<toDate>{0}</toDate>", searchDetails.DepartureDate.ToString("yyyy-MM-dd"));
            sb.AppendFormat("<currency>{0}</currency>", _dotwSupport.GetCurrencyID(searchDetails));

            sb.AppendFormat("<rooms no = \"{0}\">", searchDetails.Rooms);

            int roomRunNo = 0;

            foreach (var roomDetail in searchDetails.RoomDetails)
            {
                var childAndInfantAges = roomDetail.ChildAndInfantAges(1).Where(i => i <= 12).ToList();

                int adults = roomDetail.Adults + roomDetail.Children + roomDetail.Infants - childAndInfantAges.Count;
                int children = childAndInfantAges.Count;

                sb.AppendFormat("<room runno = \"{0}\">", roomRunNo);
                sb.AppendFormat("<adultsCode>{0}</adultsCode>", adults);
                sb.AppendFormat("<children no = \"{0}\">", children);

                // append the children
                int childRunNo = 0;
                foreach (int childAge in childAndInfantAges)
                {
                    sb.AppendFormat("<child runno=\"{0}\">{1}</child>", childRunNo, childAge);
                    childRunNo += 1;
                }

                sb.AppendLine("</children>");
                sb.AppendLine("<extraBed>0</extraBed>");
                sb.AppendLine("<rateBasis>-1</rateBasis>");

                // Nationality and Country of residence
                if (_settings.Version(searchDetails) == 2)
                {
                    string nationality = await DOTW.GetNationalityAsync(searchDetails.ISONationalityCode, searchDetails, _support, _settings);
                    string countryCode = DOTW.GetCountryOfResidence(nationality, searchDetails, _settings);

                    if (!string.IsNullOrEmpty(nationality))
                    {
                        sb.AppendFormat("<passengerNationality>{0}</passengerNationality>", nationality);
                    }

                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        sb.AppendFormat("<passengerCountryOfResidence>{0}</passengerCountryOfResidence>", countryCode);
                    }
                }

                sb.AppendLine("</room>");
                roomRunNo += 1;
            }

            sb.AppendLine("</rooms>");
            sb.AppendLine("</bookingDetails>");
            sb.AppendLine("<return>");
            sb.AppendLine("<sorting order = \"asc\">sortByPrice</sorting>");
            sb.AppendLine("<getRooms>true</getRooms>");
            sb.AppendLine("<filters xmlns:a = \"http://us.dotwconnect.com/xsd/atomicCondition\" xmlns:c = \"http://us.dotwconnect.com/xsd/complexCondition\">");
            sb.AppendFormat("<city>{0}</city>", city.CityNumber);
            sb.AppendLine("<nearbyCities>false</nearbyCities>");

            // conditions
            sb.AppendLine("<c:condition>");

            // availability = not on request
            sb.AppendLine("<a:condition>");
            sb.AppendLine("<fieldName>onRequest</fieldName>");
            sb.AppendLine("<fieldTest>equals</fieldTest> ");
            sb.AppendLine("<fieldValues>");
            sb.AppendLine("<fieldValue>0</fieldValue> ");
            sb.AppendLine("</fieldValues>");
            sb.AppendLine("</a:condition>");

            // put in the city locations
            // only need to do this is there is more than one location
            if (city.LocationIDs.Count > 1)
            {
                sb.AppendLine("<operator>AND</operator>");
                sb.AppendLine("<a:condition>");
                sb.AppendLine("<fieldName>locationId</fieldName>");
                sb.AppendLine("<fieldTest>in</fieldTest>");
                sb.AppendLine("<fieldValues>");

                foreach (int iLocationID in city.LocationIDs)
                {
                    sb.AppendFormat("<fieldValue>{0}</fieldValue>", iLocationID);
                }

                sb.AppendLine("</fieldValues>");
                sb.AppendLine("</a:condition>");
            }

            sb.AppendLine("</c:condition>");
            sb.AppendLine("</filters>");
            sb.AppendLine("<fields>");
            sb.AppendLine("<field>hotelName</field>");
            sb.AppendLine("<field>noOfRooms</field>");
            sb.AppendLine("<roomField>name</roomField>");
            sb.AppendLine("<roomField>including</roomField>");
            sb.AppendLine("<roomField>minStay</roomField>");
            sb.AppendLine("</fields>");
            sb.AppendLine("</return>");
            sb.AppendLine("</request>");
            sb.AppendLine("</customer>");

            return sb.ToString();
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

                            if (string.IsNullOrEmpty(rateBasis.MinStay) &&
                                (rateBasis.RateType.Type == "1" ||
                                    rateBasis.RateType.Type == "2" ||
                                    !_settings.ExcludeDOTWThirdParties(searchDetails)))
                            {

                                var amount = _settings.UseMinimumSellingPrice(searchDetails) && rateBasis.TotalMinSelling != null
                                        && !string.IsNullOrEmpty(rateBasis.TotalMinSelling.Total) ?
                                        rateBasis.TotalMinSelling.Total.ToSafeDecimal() : rateBasis.Total.TotalCost.ToSafeDecimal();

                                var transformedResult = new TransformedResult()
                                {
                                    MasterID = hotel.HotelID.ToSafeInt(),
                                    TPKey = hotel.HotelID,
                                    CurrencyCode = rateBasis.RateType.CurrencyID,
                                    PropertyRoomBookingID = room.RoomNum + 1,
                                    RoomType = roomtype.Name,
                                    RoomTypeCode = roomtype.Code,
                                    MealBasisCode = rateBasis.ID,
                                    Amount = amount,
                                    DynamicProperty = rateBasis.WithinCancellationDeadline == "yes",
                                    TPReference = roomtype.Code + "|" + rateBasis.ID
                                };

                                transformedResults.Add(transformedResult);
                            }
                        }
                    }
                }
            }

            return transformedResults;
        }

        #endregion
    }
}