namespace iVectorOne.Suppliers.SunHotels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVector.Search.Property;
    using Microsoft.Extensions.Caching.Memory;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Models.Property;

    public class SunHotelsSearch : IThirdPartySearch, ISingleSource
    {
        #region Properties

        public string Source => ThirdParties.SUNHOTELS;

        private readonly ISunHotelsSettings _settings;
        private readonly ISerializer _serializer;
        private readonly IMemoryCache _cache;

        #endregion

        #region Constructor

        public SunHotelsSearch(ISunHotelsSettings settings, ISerializer serializer, IMemoryCache cache)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _cache = Ensure.IsNotNull(cache, nameof(cache));
        }

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            bool restrictions = false;

            if (searchDetails.Rooms > 1)
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
            var searchCodes = new Dictionary<List<string>, string>();
            var hotelCodes = new List<string>();

            foreach (var resortSplit in resortSplits)
            {
                if (resortSplit.Hotels.Any())
                {
                    hotelCodes.AddRange(resortSplit.Hotels.Select(x => x.TPKey));
                }
            }

            searchCodes.Add(hotelCodes, "HOTEL");

            foreach (var searchCode in searchCodes)
            {
                // set a unique code. if there is one request we only need the source name
                string uniqueCode = Source;

                if (searchCodes.Count > 1)
                {
                    uniqueCode = string.Format("{0}_{1}_{2}", Source, searchCode.Key, searchCode.Value);
                }

                int propertyRoomBookingId = 1;

                int hotelRequestLimit = _settings.HotelBatchLimit(searchDetails);

                foreach (var roomDetail in searchDetails.RoomDetails)
                {
                    int total = searchCode.Key.Count;
                    int from = 0;
                    int numberToTake = total;

                    if (hotelRequestLimit > 0)
                    {
                        numberToTake = Math.Min(hotelRequestLimit, total);
                    }

                    while (from < total)
                    {
                        string requestBody = BuildSearchRequestString(
                            _settings.SearchURL(searchDetails),
                            _settings.User(searchDetails),
                            _settings.Password(searchDetails),
                            _settings.LanguageCode(searchDetails),
                            _settings.Currency(searchDetails),
                            searchDetails,
                            roomDetail,
                            searchCode.Key.Skip(from).Take(numberToTake).ToList(),
                            searchCode.Value,
                            _settings.CustomerCountryCode(searchDetails),
                            _settings.RequestPackageRates(searchDetails));

                        var request = new Request
                        {
                            EndPoint = requestBody.ToString(),
                            Method = RequestMethod.GET,
                            ExtraInfo = propertyRoomBookingId
                        };

                        requests.Add(request);

                        from += numberToTake;
                    }

                    propertyRoomBookingId++;
                }
            }

            return Task.FromResult(requests);
        }

        public string BuildSearchRequestString(
            string url,
            string username,
            string password,
            string language,
            string currency,
            SearchDetails searchDetails,
            RoomDetail roomDetail,
            List<string> searchCode,
            string searchCodeType,
            string nationality,
            bool requestPackageRates)
        {
            // build the request url
            var sb = new StringBuilder();

            string searchCodeCsv = string.Join(",", searchCode);

            sb.Append(url);
            sb.AppendFormat("userName={0}", username);
            sb.AppendFormat("&password={0}", password);
            sb.AppendFormat("&language={0}", language);
            sb.AppendFormat("&currencies={0}", currency);
            sb.AppendFormat("&checkInDate={0}", SunHotels.GetSunHotelsDate(searchDetails.ArrivalDate));
            sb.AppendFormat("&checkOutDate={0}", SunHotels.GetSunHotelsDate(searchDetails.DepartureDate));
            sb.AppendFormat("&numberOfRooms={0}", "1");
            sb.Append("&destination=");

            // check whether we have a hotel, resort or region code
            switch (searchCodeType ?? "")
            {
                case "HOTEL":
                    {
                        sb.Append("&destinationID=");
                        sb.AppendFormat("&hotelIDs={0}", searchCodeCsv);
                        sb.Append("&resortIDs=");
                        break;
                    }

                case "RESORT":
                    {
                        sb.Append("&destinationID=");
                        sb.Append("&hotelIDs=");
                        sb.AppendFormat("&resortIDs={0}", searchCodeCsv);
                        break;
                    }

                case "REGION":
                    {
                        sb.AppendFormat("&destinationID={0}", searchCodeCsv);
                        sb.Append("&hotelIDs=");
                        sb.Append("&resortIDs=");
                        break;
                    }
            }

            sb.AppendFormat("&accommodationTypes=");
            sb.AppendFormat("&numberOfAdults={0}", GetAdultsFromRoomDetail(roomDetail));
            sb.AppendFormat("&numberOfChildren={0}", GetChildrenFromRoomDetail(roomDetail));
            sb.AppendFormat("&childrenAges={0}", roomDetail.ChildAgeCSV);
            sb.AppendFormat("&infant={0}", IsInfantIncluded(roomDetail));
            sb.Append("&sortBy=Price&sortOrder=Ascending");
            sb.Append("&exactDestinationMatch=");
            sb.Append("&blockSuperdeal=false");
            sb.Append("&showTransfer=");
            sb.Append("&mealIds=");
            sb.Append("&showCoordinates=");
            sb.Append("&showReviews=");
            sb.Append("&referencePointLatitude=");
            sb.Append("&referencePointLongitude=");
            sb.Append("&maxDistanceFromReferencePoint=");
            sb.Append("&minStarRating=");
            sb.Append("&maxStarRating=");
            sb.Append("&featureIds=");
            sb.Append("&minPrice=");
            sb.Append("&maxPrice=");
            sb.Append("&themeIds=");
            sb.Append("&excludeSharedRooms=");
            sb.Append("&excludeSharedFacilities=");
            sb.Append("&prioritizedHotelIds=");
            sb.Append("&totalRoomsInBatch=");
            sb.Append("&paymentMethodId=1");
            sb.AppendFormat("&customerCountry={0}", nationality);

            sb.AppendFormat("&B2C={0}", requestPackageRates ? "0" : "");

            return sb.ToString();
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedList = new TransformedResultCollection();

            var responses = new List<SunhotelsSearchResponse>();
            string currency = _settings.Currency(searchDetails);

            // Sunhotels have a separate lookup xml for room type name("http://xml.sunhotels.net/15/PostGet/NonStaticXMLAPI.asmx/GetRoomTypes?")
            var roomTypes = GetRoomTypes();

            // order by property room booking id - stored in extra info
            foreach (var request in requests.OrderBy(o => o.ExtraInfo.ToSafeInt()))
            {
                SunhotelsSearchResponse response = null!;

                try
                {
                    response = _serializer.DeSerialize<SunhotelsSearchResponse>(request.ResponseXML);
                }
                catch (Exception ex)
                {
                    var exception = ex.ToString();
                }

                response.PropertyRoomBookingID = request.ExtraInfo.ToSafeInt();
                var roomDetail = searchDetails.RoomDetails.Single(o => o.PropertyRoomBookingID == response.PropertyRoomBookingID);

                foreach (var hotel in response.hotels)
                {
                    foreach (var roomType in hotel.roomtypes)
                    {
                        string roomTypeName = string.Empty;
                        roomTypes.TryGetValue(roomType.roomtypeID.ToSafeString(), out roomTypeName);

                        foreach (var room in roomType.rooms)
                        {
                            var nonRefundable = false;
                            var cancellations = new List<Cancellation>();

                            for (int index = 0; index < room.cancellation_policies.Count(); index++)
                            {
                                var cancellation = room.cancellation_policies[index];

                                bool nullDeadline = string.IsNullOrEmpty(cancellation.deadline);
                                decimal percentage = cancellation.percentage.ToSafeDecimal();

                                var hours = new TimeSpan(cancellation.deadline.ToSafeInt(), 0, 0);
                                DateTime startDate, endDate;

                                // for 100% cancellations we don't get an hours before
                                // so force the start date to be from now
                                if (hours.TotalHours == 0d)
                                {
                                    startDate = DateTime.Now.Date;
                                }
                                else
                                {
                                    startDate = searchDetails.ArrivalDate.Subtract(hours);
                                }

                                if (!nonRefundable && percentage == 100m && nullDeadline)
                                {
                                    nonRefundable = true;
                                }

                                Cancellation_policy nextCancellation = null!;

                                if (index + 1 < room.cancellation_policies.Count())
                                {
                                    nextCancellation = room.cancellation_policies[index + 1];
                                }

                                if (nextCancellation != null)
                                {
                                    // the hours of the end dates need to be rounded to the nearest 24 hours to stop them overlapping with the start of the next cancellation policy and making
                                    // the charges add together
                                    int deadline = nextCancellation.deadline.ToSafeInt();
                                    var endHours = new TimeSpan(SunHotels.RoundHoursUpToTheNearest24Hours(deadline), 0, 0);

                                    endDate = searchDetails.ArrivalDate.Subtract(endHours);
                                    endDate = endDate.AddDays(-1);
                                }
                                else
                                {
                                    endDate = new DateTime(2099, 1, 1);
                                }

                                cancellations.Add(new Cancellation(startDate, endDate, percentage));
                            }

                            foreach (var meal in room.meals)
                            {
                                var resultCancellations = new List<iVectorOne.Models.Cancellation>();
                                foreach (var cancellation in cancellations)
                                {
                                    resultCancellations.Add(new iVectorOne.Models.Cancellation()
                                    {
                                        Amount = cancellation.Percentage,
                                        StartDate = cancellation.StartDate,
                                        EndDate = cancellation.EndDate
                                    });
                                }

                                transformedList.TransformedResults.Add(new TransformedResult()
                                {
                                    MasterID = hotel.hotelid,
                                    TPKey = hotel.hotelid.ToSafeString(),
                                    CurrencyCode = currency,
                                    PropertyRoomBookingID = response.PropertyRoomBookingID,
                                    RoomType = roomTypeName,
                                    MealBasisCode = string.IsNullOrEmpty(meal.labelId.ToSafeString()) ? meal.id.ToSafeString() : $"{meal.id}|{meal.labelId}",
                                    Adults = roomDetail.Adults,
                                    Children = roomDetail.Children,
                                    ChildAgeCSV = roomDetail.ChildAgeCSV,
                                    Infants = roomDetail.Infants,
                                    Amount = meal.prices.price.Value.ToSafeDecimal(),
                                    TPReference = $"{room.id}_{meal.id}_{roomType.roomtypeID}_{room.paymentMethods.paymentMethod.id}",
                                    NonRefundableRates = nonRefundable || room.isSuperDeal,
                                    Cancellations = resultCancellations
                                });
                            }
                        }
                    }
                }
            }

            return transformedList;
        }

        #endregion

        #region ResponseHasExceptions

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion

        #region Helpers

        public Dictionary<string, string> GetRoomTypes()
        {
            // todo - replace large xml file with web api call
            return _cache.GetOrCreate(
                "SunHotelsRoomTypes",
                () => _serializer.DeSerialize<getRoomTypesResult>(SunHotelsRes.SunHotelsRoomType).roomTypes.ToDictionary(o => o.id, o => o.name),
                9999);
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
            return (roomDetail.Adults + roomDetail.ChildAges.Count(age => age > 17)).ToString();
        }

        public static string GetChildrenFromRoomDetail(RoomDetail roomDetail)
        {
            return roomDetail.ChildAges.Count(age => age <= 17).ToString();
        }

        public static int IsInfantIncluded(RoomDetail roomDetail)
        {
            return roomDetail.Infants > 0 ? 1 : 0;
        }

        public class Cancellation
        {
            public Cancellation(DateTime startDate, DateTime endDate, decimal percentage)
            {
                StartDate = startDate;
                EndDate = endDate;
                Percentage = percentage;
            }

            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal Percentage { get; set; }
        }
    }
}