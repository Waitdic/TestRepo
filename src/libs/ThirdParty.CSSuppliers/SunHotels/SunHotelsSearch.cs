namespace ThirdParty.CSSuppliers.SunHotels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using iVector.Search.Property;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;

    public class SunHotelsSearch : IThirdPartySearch
    {
        #region Properties

        public string Source => ThirdParties.SUNHOTELS;
        private readonly ITPSupport _support;
        private readonly ISunHotelsSettings _settings;
        private readonly ISerializer _serializer;
        private readonly IMemoryCache _cache;
        
        #endregion

        #region Constructor

        public SunHotelsSearch(ISunHotelsSettings settings, ITPSupport support, ISerializer serializer, IMemoryCache cache)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _cache = Ensure.IsNotNull(cache, nameof(cache));
        }

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails oSearchDetails)
        {
            bool bRestrictions = false;

            if (oSearchDetails.Rooms > 1)
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
            var oSearchCodes = new Dictionary<List<string>, string>();
            var oHotelCodes = new List<string>();

            foreach (ResortSplit oResortSplit in oResortSplits)
            {
                if (oResortSplit.Hotels.Any())
                {
                    oHotelCodes.AddRange(oResortSplit.Hotels.Select(x => x.TPKey));
                }
            }

            oSearchCodes.Add(oHotelCodes, "HOTEL");

            // loop through each searchcode
            foreach (KeyValuePair<List<string>, string> oSearchCode in oSearchCodes)
            {

                // set a unique code. if there is one request we only need the source name
                string sUniqueCode = Source;
                if (oSearchCodes.Count > 1)
                    sUniqueCode = string.Format("{0}_{1}_{2}", Source, oSearchCode.Key, oSearchCode.Value);

                int iPropertyRoomBookingID = 1;

                int iHotelRequestLimit = _settings.get_HotelRequestLimit(oSearchDetails);

                foreach (RoomDetail oRoomDetail in oSearchDetails.RoomDetails)
                {

                    var oExtraInfo = new SearchExtraHelper(oSearchDetails, sUniqueCode);

                    // record the room id for the transform
                    oExtraInfo.ExtraInfo = iPropertyRoomBookingID.ToString();

                    int iTotal = oSearchCode.Key.Count;
                    var iFrom = default(int);
                    int numberToTake = iTotal;

                    if (iHotelRequestLimit > 0)
                    {
                        numberToTake = Math.Min(iHotelRequestLimit, iTotal);
                    }

                    while (iFrom < iTotal)
                    {

                        string request = BuildSearchRequestString(_settings.get_SearchURL(oSearchDetails), _settings.get_Username(oSearchDetails), _settings.get_Password(oSearchDetails), _settings.get_Language(oSearchDetails), _settings.get_Currency(oSearchDetails), oSearchDetails, oRoomDetail, oSearchCode.Key.Skip(iFrom).Take(numberToTake).ToList(), oSearchCode.Value, _settings.get_Nationality(oSearchDetails), _settings.get_RequestPackageRates(oSearchDetails));

                        var oRequest = new Request
                        {
                            EndPoint = request.ToString(),
                            Method = eRequestMethod.GET,
                            ExtraInfo = oExtraInfo
                        };

                        oRequests.Add(oRequest);

                        iPropertyRoomBookingID += 1;

                        iFrom += numberToTake;
                    }
                }

            }

            return oRequests;

        }

        public string BuildSearchRequestString(string url, string username, string password, string language, string currency, SearchDetails searchDetails, RoomDetail roomDetail, List<string> searchCode, string searchCodeType, string nationality, bool requestPackageRates)
        {

            // build the request url
            var searchRequestUrlBuilder = new StringBuilder();

            string searchCodeCsv = string.Join(",", searchCode);

            searchRequestUrlBuilder.Append(url);
            searchRequestUrlBuilder.AppendFormat("userName={0}", username);
            searchRequestUrlBuilder.AppendFormat("&password={0}", password);
            searchRequestUrlBuilder.AppendFormat("&language={0}", language);
            searchRequestUrlBuilder.AppendFormat("&currencies={0}", currency);
            searchRequestUrlBuilder.AppendFormat("&checkInDate={0}", SunHotels.GetSunHotelsDate(searchDetails.PropertyArrivalDate));
            searchRequestUrlBuilder.AppendFormat("&checkOutDate={0}", SunHotels.GetSunHotelsDate(searchDetails.PropertyDepartureDate));
            searchRequestUrlBuilder.AppendFormat("&numberOfRooms={0}", "1");
            searchRequestUrlBuilder.Append("&destination=");

            // check whether we have a hotel, resort or region code
            switch (searchCodeType ?? "")
            {
                case "HOTEL":
                    {
                        searchRequestUrlBuilder.Append("&destinationID=");
                        searchRequestUrlBuilder.AppendFormat("&hotelIDs={0}", searchCodeCsv);
                        searchRequestUrlBuilder.Append("&resortIDs=");
                        break;
                    }

                case "RESORT":
                    {
                        searchRequestUrlBuilder.Append("&destinationID=");
                        searchRequestUrlBuilder.Append("&hotelIDs=");
                        searchRequestUrlBuilder.AppendFormat("&resortIDs={0}", searchCodeCsv);
                        break;
                    }

                case "REGION":
                    {
                        searchRequestUrlBuilder.AppendFormat("&destinationID={0}", searchCodeCsv);
                        searchRequestUrlBuilder.Append("&hotelIDs=");
                        searchRequestUrlBuilder.Append("&resortIDs=");
                        break;
                    }

            }

            searchRequestUrlBuilder.AppendFormat("&accommodationTypes=");
            searchRequestUrlBuilder.AppendFormat("&numberOfAdults={0}", GetAdultsFromRoomDetail(roomDetail));
            searchRequestUrlBuilder.AppendFormat("&numberOfChildren={0}", GetChildrenFromRoomDetail(roomDetail));
            searchRequestUrlBuilder.AppendFormat("&childrenAges={0}", roomDetail.ChildAgeCSV);
            searchRequestUrlBuilder.AppendFormat("&infant={0}", IsInfantIncluded(roomDetail));
            searchRequestUrlBuilder.Append("&sortBy=Price&sortOrder=Ascending");
            searchRequestUrlBuilder.Append("&exactDestinationMatch=");
            searchRequestUrlBuilder.Append("&blockSuperdeal=false");
            searchRequestUrlBuilder.Append("&showTransfer=");
            searchRequestUrlBuilder.Append("&mealIds=");
            searchRequestUrlBuilder.Append("&showCoordinates=");
            searchRequestUrlBuilder.Append("&showReviews=");
            searchRequestUrlBuilder.Append("&referencePointLatitude=");
            searchRequestUrlBuilder.Append("&referencePointLongitude=");
            searchRequestUrlBuilder.Append("&maxDistanceFromReferencePoint=");
            searchRequestUrlBuilder.Append("&minStarRating=");
            searchRequestUrlBuilder.Append("&maxStarRating=");
            searchRequestUrlBuilder.Append("&featureIds=");
            searchRequestUrlBuilder.Append("&minPrice=");
            searchRequestUrlBuilder.Append("&maxPrice=");
            searchRequestUrlBuilder.Append("&themeIds=");
            searchRequestUrlBuilder.Append("&excludeSharedRooms=");
            searchRequestUrlBuilder.Append("&excludeSharedFacilities=");
            searchRequestUrlBuilder.Append("&prioritizedHotelIds=");
            searchRequestUrlBuilder.Append("&totalRoomsInBatch=");
            searchRequestUrlBuilder.Append("&paymentMethodId=1");
            searchRequestUrlBuilder.AppendFormat("&customerCountry={0}", nationality);

            searchRequestUrlBuilder.AppendFormat("&B2C={0}", requestPackageRates ? "0" : "");

            return searchRequestUrlBuilder.ToString();
        }

        public TransformedResultCollection TransformResponse(List<Request> oRequests, SearchDetails oSearchDetails, List<ResortSplit> oResortSplits)
        {
            var oTransformedList = new TransformedResultCollection();
            try
            {
                var oResponses = new List<SunhotelsSearchResponse>();
                string sCurrency = _settings.get_Currency(oSearchDetails);

                // Sunhotels have a separate lookup xml for room type name("http://xml.sunhotels.net/15/PostGet/NonStaticXMLAPI.asmx/GetRoomTypes?")
                var oRoomTypes = new Dictionary<string, string>();
                oRoomTypes = GetRoomTypes();
                // order by property room booking id - stored in extra info
                foreach (Request oRequest in oRequests.OrderBy(o => ((SearchExtraHelper)o.ExtraInfo).ExtraInfo.ToSafeInt()))
                {
                    SunhotelsSearchResponse response = null;
                    try
                    {
                        response = _serializer.DeSerialize<SunhotelsSearchResponse>(oRequest.ResponseXML);
                    }
                    catch (Exception ex)
                    {
                        var exception = ex.ToString();
                    }
                    response.PropertyRoomBookingID = ((SearchExtraHelper)oRequest.ExtraInfo).ExtraInfo.ToSafeInt();
                    var oRoomDetail = oSearchDetails.RoomDetails.Single(o => o.PropertyRoomBookingID == response.PropertyRoomBookingID);

                    foreach (Hotel hotel in response.hotels)
                    {
                        foreach (Roomtype roomType in hotel.roomtypes)
                        {
                            string sRoomTypeName = string.Empty;
                            oRoomTypes.TryGetValue(roomType.roomtypeID.ToSafeString(), out sRoomTypeName);
                            foreach (Room room in roomType.rooms)
                            {
                                var bNonRef = default(bool);
                                var aCancellations = new List<Cancellation>();

                                for (int index = 0, loopTo = room.cancellation_policies.Count(); index < loopTo; index++)
                                {
                                    var cancellation = room.cancellation_policies[index];

                                    bool bNullDeadline = string.IsNullOrEmpty(cancellation.deadline);
                                    decimal nPercentage = cancellation.percentage.ToSafeDecimal();

                                    var oHours = new TimeSpan(cancellation.deadline.ToSafeInt(), 0, 0);
                                    DateTime dStartDate, dEndDate;

                                    // for 100% cancellations we don't get an hours before
                                    // so force the start date to be from now
                                    if (oHours.TotalHours == 0d)
                                    {
                                        dStartDate = DateTime.Now.Date;
                                    }
                                    else
                                    {
                                        dStartDate = oSearchDetails.PropertyArrivalDate.Subtract(oHours);
                                    }

                                    if (!bNonRef && nPercentage == 100m && bNullDeadline)
                                    {
                                        bNonRef = true;
                                    }

                                    Cancellation_policy nextCancellation = null;

                                    if (index + 1 < loopTo)
                                    {
                                        nextCancellation = room.cancellation_policies[index + 1];
                                    }

                                    if (nextCancellation != null)
                                    {
                                        // the hours of the end dates need to be rounded to the nearest 24 hours to stop them overlapping with the start of the next cancellation policy and making
                                        // the charges add together
                                        int iEndHours = nextCancellation.deadline.ToSafeInt();
                                        var oEndHours = new TimeSpan(SunHotels.RoundHoursUpToTheNearest24Hours(iEndHours), 0, 0);

                                        dEndDate = oSearchDetails.PropertyArrivalDate.Subtract(oEndHours);
                                        dEndDate = dEndDate.AddDays(-1);
                                    }
                                    else
                                    {
                                        dEndDate = new DateTime(2099, 1, 1);
                                    }

                                    aCancellations.Add(new Cancellation(dStartDate, dEndDate, nPercentage));
                                }

                                foreach (Meal meal in room.meals)
                                {

                                    var aResultCancellations = new List<ThirdParty.Models.Property.Booking.Cancellation>();
                                    foreach (Cancellation oCancellation in aCancellations)
                                        aResultCancellations.Add(new ThirdParty.Models.Property.Booking.Cancellation()
                                        {
                                            Amount = oCancellation.Percentage,
                                            StartDate = oCancellation.StartDate,
                                            EndDate = oCancellation.EndDate
                                        });

                                    oTransformedList.TransformedResults.Add(new TransformedResult()
                                    {
                                        MasterID = hotel.hotelid,
                                        TPKey = hotel.hotelid.ToSafeString(),
                                        CurrencyCode = sCurrency,
                                        PropertyRoomBookingID = response.PropertyRoomBookingID,
                                        RoomType = sRoomTypeName,
                                        MealBasisCode = string.IsNullOrEmpty(meal.labelId.ToSafeString()) ? meal.id.ToSafeString() : $"{meal.id}|{meal.labelId}",
                                        Adults = oRoomDetail.Adults,
                                        Children = oRoomDetail.Children,
                                        ChildAgeCSV = oRoomDetail.ChildAgeCSV,
                                        Infants = oRoomDetail.Infants,
                                        Amount = meal.prices.price.ToSafeDecimal(),
                                        TPReference = $"{room.id}_{meal.id}_{roomType.roomtypeID}_{room.paymentMethods.paymentMethod.id}",
                                        NonRefundableRates = bNonRef || room.isSuperDeal,
                                        Cancellations = aResultCancellations
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                var exception = ex.ToString();
            }
            return oTransformedList;
        }

        #endregion

        #region ResponseHasExceptions

        public bool ResponseHasExceptions(Request oRequest)
        {
            return false;
        }

        #endregion

        #region Helpers
        public Dictionary<string, string> GetRoomTypes()
        {

            var oRoomTypes = _cache.GetOrCreate("SunHotelsRoomTypes", () =>
                                {
                                    var roomTypes = _serializer.DeSerialize<getRoomTypesResult>(SunHotelsRes.SunHotelsRoomType).roomTypes.ToDictionary(o => o.id, o => o.name);
                                    return roomTypes;
                                }, 9999);
            return oRoomTypes;

        }

        #endregion

        public class getRoomTypesResult
        {
            [XmlArray("roomTypes")]
            [XmlArrayItem("roomType")]
            public List<SunhotelRoomType> roomTypes { get; set; } = new List<SunhotelRoomType>();
        }

        public class SunhotelRoomType
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public static string GetAdultsFromRoomDetail(RoomDetail roomDetail)
        {

            int iAdultCount = roomDetail.Adults;

            foreach (int iAge in roomDetail.ChildAges)
            {
                if (iAge > 17)
                    iAdultCount += 1;
            }

            return iAdultCount.ToString();

        }

        public static string GetChildrenFromRoomDetail(RoomDetail roomDetail)
        {

            int iChildCount = 0;

            foreach (int iAge in roomDetail.ChildAges)
            {
                if (iAge <= 17)
                    iChildCount += 1;
            }

            return iChildCount.ToString();

        }

        public static int IsInfantIncluded(RoomDetail roomDetail)
        {
            int iInfantIncluded = 0;
            if (roomDetail.Infants > 0)
            {
                iInfantIncluded = 1;
            }
            return iInfantIncluded;
        }

        public class Cancellation
        {
            public Cancellation(DateTime dStartDate, DateTime dEndDate, decimal nPercentage)
            {
                StartDate = dStartDate;
                EndDate = dEndDate;
                Percentage = nPercentage;
            }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal Percentage { get; set; }
        }
    }
}