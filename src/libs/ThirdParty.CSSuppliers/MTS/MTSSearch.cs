namespace ThirdParty.CSSuppliers.MTS
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.MTS.Models;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;

    public class MTSSearch : IThirdPartySearch
    {
        #region Constructor

        private readonly IMTSSettings _settings;
        private readonly ISerializer _serializer;

        public MTSSearch(IMTSSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.MTS;

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails searchDetails)
        {
            return false;
        }

        #endregion

        #region SearchFunctions

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits, bool saveLogs)
        {
            var requests = new List<Request>();
            var regions = new Dictionary<string, string>();

            var overrideCountriesList = new List<string>();
            if (overrideCountriesList.Count == 0)
            {
                string overrideCountries = _settings.OverrideCountries(searchDetails);
                if (!string.IsNullOrWhiteSpace(overrideCountries))
                {
                    // split the string and add each one to _overrideCountries
                    foreach (string country in overrideCountries.Split('|'))
                    {
                        overrideCountriesList.Add(country);
                    }
                }
                else
                {
                    overrideCountriesList.Add("United Arab Emirates");
                    overrideCountriesList.Add("Turkey");
                    overrideCountriesList.Add("Egypt");
                }
            }

            if (resortSplits.Count == 1)
            {
                // how many hotels? if >1, search by resort, if =1 search by hotelcode
                if (resortSplits[0].Hotels.Count == 1)
                {
                    string country = resortSplits[0].ResortCode.Split('|')[0];
                    string hotelCode = resortSplits[0].Hotels[0].TPKey;
                    string countryAndHotelCode = country + "|" + hotelCode;

                    regions.Add(countryAndHotelCode, "CountryAndHotelCode");
                }

                if (resortSplits[0].Hotels.Count > 1)
                {
                    string resortPath = resortSplits[0].ResortCode;

                    // save resort if new
                    if (!regions.ContainsKey(resortPath))
                    {
                        regions.Add(resortPath.ToString(), "Resort");
                    }
                }
            }

            if (resortSplits.Count > 1)
            {
                // select region and save; for each new different region, save that region as well
                foreach (var resortSplit in resortSplits)
                {
                    // select region
                    string country = resortSplit.ResortCode.Split('|')[0];
                    string region = resortSplit.ResortCode.Split('|')[1];
                    string regionPath = country + "|" + region;

                    // save region if new
                    if (!regions.ContainsKey(regionPath))
                    {
                        regions.Add(regionPath, "Region");
                    }
                }
            }

            // need to send off a request for each resort and store them in an array
            // build request
            foreach (var search in regions)
            {
                // get the third party resorts
                // once get IPs confirmed, ie not now
                var searchKey = search.Key.Split('|');

                bool useOverrideId = overrideCountriesList.Contains(search.Key.Split('|')[0]);

                // build the request string
                var sbRequest = new StringBuilder();

                sbRequest.Append("<OTA_HotelAvailRQ xmlns = \"http://www.opentravel.org/OTA/2003/05\" Version = \"0.1\">");
                sbRequest.Append("<POS>");
                sbRequest.Append("<Source>");

                if (useOverrideId)
                {
                    // If country is Egypt, Turkey or UAE, search with second ID. This returns in contracted currency
                    sbRequest.AppendFormat("<RequestorID Instance = \"{0}\" ID_Context = \"{1}\" ID = \"{2}\" Type = \"{3}\"/>", _settings.Instance(searchDetails), _settings.ID_Context(searchDetails), _settings.OverRideID(searchDetails), _settings.Type(searchDetails));
                }
                else
                {
                    // Anything else, search with main ID; returns in Euros
                    sbRequest.AppendFormat("<RequestorID Instance = \"{0}\" ID_Context = \"{1}\" ID = \"{2}\" Type = \"{3}\"/>", _settings.Instance(searchDetails), _settings.ID_Context(searchDetails), _settings.ID(searchDetails), _settings.Type(searchDetails));
                }

                sbRequest.Append("<BookingChannel Type = \"2\"/>");
                sbRequest.Append("</Source>");

                sbRequest.Append("<Source>");
                sbRequest.AppendFormat("<RequestorID Type=\"{0}\" ID=\"{1}\" MessagePassword=\"{2}\"/>", _settings.AuthenticationType(searchDetails), _settings.AuthenticationID(searchDetails), _settings.MessagePassword(searchDetails));
                sbRequest.Append("</Source>");

                sbRequest.Append("</POS>");

                sbRequest.Append("<AvailRequestSegments>");
                sbRequest.Append("<AvailRequestSegment InfoSource='1*2*4*5*'>");
                sbRequest.AppendFormat("<StayDateRange End=\"{0}\" Start=\"{1}\"></StayDateRange>", searchDetails.PropertyDepartureDate.ToString("yyyy-MM-dd"), searchDetails.PropertyArrivalDate.ToString("yyyy-MM-dd"));
                sbRequest.Append("<RoomStayCandidates>");

                // loop through the rooms
                int roomCount = 0;
                foreach (var roomBooking in searchDetails.RoomDetails)
                {
                    sbRequest.AppendFormat("<RoomStayCandidate RPH=\"{0}\">", roomCount + 1);
                    sbRequest.Append("<GuestCounts>");

                    // Adults
                    sbRequest.Append("<GuestCount AgeQualifyingCode=\"10\" ");
                    sbRequest.AppendFormat(" Count=\"{0}\" ", roomBooking.Adults);
                    sbRequest.Append("></GuestCount>");

                    // Children
                    foreach (int childAge in roomBooking.ChildAges)
                    {
                        sbRequest.Append("<GuestCount AgeQualifyingCode=\"8\" ");
                        sbRequest.AppendFormat("Age=\"{0}\" ", childAge);
                        sbRequest.AppendFormat("Count=\"1\"");
                        sbRequest.Append("></GuestCount>");
                    }

                    // Infants
                    if (roomBooking.Infants > 0)
                    {
                        sbRequest.Append("<GuestCount AgeQualifyingCode=\"7\" ");
                        sbRequest.AppendFormat("Age=\"1\" ");
                        sbRequest.AppendFormat("Count=\"{0}\"", roomBooking.Infants);
                        sbRequest.Append("></GuestCount>");
                    }

                    sbRequest.Append("</GuestCounts>");
                    sbRequest.Append("</RoomStayCandidate>");

                    roomCount++;
                }

                sbRequest.Append("</RoomStayCandidates>");
                sbRequest.Append("<HotelSearchCriteria>");

                if (search.Value == "CountryAndHotelCode")
                {
                    sbRequest.Append("<Criterion ExactMatch = \"true\">");
                    sbRequest.AppendFormat("<HotelRef HotelCode=\"{0}\"></HotelRef>", search.Key.Split('|')[1]);
                }
                else
                {
                    sbRequest.Append("<Criterion>");
                    sbRequest.AppendFormat("<RefPoint CodeContext = \"Country\">{0}</RefPoint>", search.Key.Split('|')[0]);
                    sbRequest.AppendFormat("<RefPoint CodeContext = \"Region\">{0}</RefPoint>", search.Key.Split('|')[1]);

                    // Check if is a resort-level search
                    if (search.Value == "Resort")
                    {
                        sbRequest.AppendFormat("<RefPoint CodeContext = \"Resort\">{0}</RefPoint>", search.Key.Split('|')[2]);
                    }
                }

                sbRequest.Append("</Criterion>");
                sbRequest.Append("</HotelSearchCriteria>");
                sbRequest.Append("</AvailRequestSegment>");
                sbRequest.Append("</AvailRequestSegments>");

                sbRequest.Append("</OTA_HotelAvailRQ>");

                // if we have one regions then we can simply use the source here
                string uniqueCode = Source;

                // unless there is more than one
                if (regions.Count > 1)
                {
                    // get a "unique" key for this thread (if regions and resort names overlap then we are in trouble)
                    string uniqueCodeKey = "All";

                    if (searchKey.Length > 0)
                    {
                        uniqueCodeKey = searchKey[searchKey.Length - 1];
                    }

                    uniqueCode = string.Format("{0}_{1}", Source, uniqueCodeKey);
                }

                var request = new Request
                {
                    EndPoint = _settings.BaseURL(searchDetails),
                    Method = eRequestMethod.POST,
                    ExtraInfo = new SearchExtraHelper(searchDetails, uniqueCode),
                    ContentType = ContentTypes.Application_json
                };

                request.SetRequest(sbRequest.ToString());

                requests.Add(request);
            }

            return requests;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var allResponses = new List<MTSSearchResponse>();

            foreach (var request in requests)
            {
                var response = new MTSSearchResponse();
                bool success = request.Success;

                if (success)
                {
                    response = _serializer.DeSerialize<MTSSearchResponse>(request.ResponseString);

                    allResponses.Add(response);
                }
            }

            transformedResults.TransformedResults
                .AddRange(allResponses.Where(o => o.Hotels.Count() > 0)
                .SelectMany(r => GetResultFromResponse(searchDetails, r)));

            return transformedResults;
        }


        private List<TransformedResult> GetResultFromResponse(SearchDetails searchDetails, MTSSearchResponse response)
        {
            var transformedResults = new List<TransformedResult>();

            foreach (var hotel in response.Hotels)
            {
                foreach (var room in response.Rooms)
                {
                    if (hotel.Info.HotelCode == room.Info.HotelCode)
                    {
                        foreach (var area in response.Areas)
                        {
                            if (area.AreaID == hotel.Info.AreaID)
                            {
                                string country = area.Descriptions.Where(x => x.Name == "Country").FirstOrDefault().Text;

                                foreach (var roomType in room.RoomTypes)
                                {
                                    foreach (var roomRate in room.RoomRates)
                                    {
                                        var amount = roomRate.Rates.FirstOrDefault().Total.AmountAfterTax;

                                        if (roomRate.NumberOfUnits < searchDetails.Rooms || amount == 0)
                                        {
                                            continue;
                                        }

                                        transformedResults.Add(new TransformedResult()
                                        {
                                            MasterID = 0,
                                            TPKey = hotel.Info.HotelCode,
                                            CurrencyCode = roomRate.Rates.FirstOrDefault().Total.CurrencyCode,
                                            PropertyRoomBookingID = room.ID,
                                            NonRefundableRates = roomType.Code.Substring(10) == "N",
                                            RoomType = roomType.RoomDescription.Text,
                                            MealBasisCode = roomRate.Features.FirstOrDefault().Descriptions.FirstOrDefault().Text,
                                            Amount = amount,
                                            TPReference = roomType.Code + "|" + roomRate.Features.FirstOrDefault().Descriptions.FirstOrDefault().Text + "|" + country,
                                            RoomTypeCode = roomRate.RoomTypeCode
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return transformedResults;
        }

        #endregion

        #region ResponseHasExceptions

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion
    }
}