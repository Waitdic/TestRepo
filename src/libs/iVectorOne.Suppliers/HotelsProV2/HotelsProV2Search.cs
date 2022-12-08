namespace iVectorOne.Suppliers.HotelsProV2
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using iVector.Search.Property;
    using Newtonsoft.Json;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.HotelsProV2.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    public class HotelsProV2Search : IThirdPartySearch, ISingleSource
    {
        #region Properties

        private readonly IHotelsProV2Settings _settings;

        public string Source => ThirdParties.HOTELSPROV2;

        #endregion

        #region Constructors

        public HotelsProV2Search(IHotelsProV2Settings settings)
        {
            _settings = settings;
        }

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return searchDetails.Rooms > 1;
        }

        #endregion

        #region Search Funcitons

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            if (resortSplits.Count == 0)
            {
                return Task.FromResult(new List<Request>());
            }

            List<Request> requests = new();

            var hotels = resortSplits.SelectMany(split => split.Hotels);
            var useMultiHotelCodesSearch = _settings.UseMultiHotelCodesSearch(searchDetails);

            if (hotels.Count() < _settings.HotelSearchLimit(searchDetails)
                && useMultiHotelCodesSearch)
            {
                string resortCode = resortSplits.FirstOrDefault()?.ResortCode ?? "";
                string requestString = CreateSearchRequestString(searchDetails, resortCode, hotels, useMultiHotelCodesSearch);
                requests.Add(CreateRequest(requestString, searchDetails));
            }
            else
            {
                foreach (var resortSplit in resortSplits)
                {
                    string sRequestString = CreateSearchRequestString(searchDetails, resortSplit.ResortCode, hotels, useMultiHotelCodesSearch);
                    requests.Add(CreateRequest(sRequestString, searchDetails));
                }
            }

            return Task.FromResult(requests);
        }

        public Request CreateRequest(string requestString, SearchDetails searchDetails)
        {
            return new Request
            {
                EndPoint = $"{_settings.SearchURL(searchDetails)}{requestString}",
                Method = RequestMethod.GET,
                AuthenticationMode = AuthenticationMode.Basic,
                UserName = _settings.User(searchDetails),
                Password = _settings.Password(searchDetails),
                ExtraInfo = searchDetails,
                ContentType = ContentTypes.Application_x_www_form_urlencoded
            };
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var results = requests.SelectMany(request =>
            {
                var searchResponse = JsonConvert.DeserializeObject<ApiResonse<SearchResponse>>(request.ResponseString, HotelsProV2.GetJsonSerializerSettings());
                var responseCode = searchResponse.Code;
                return searchResponse.Results.SelectMany(result =>
                {
                    return result.Products.SelectMany(product =>
                    {
                        return product.Rooms
                            .Select((oRoom, roomNum) =>
                        {
                            var amount = product.Price.ToSafeDecimal() / product.Rooms.Count();

                            return new TransformedResult
                            {
                                MasterID = 0,
                                TPKey = result.HotelCode,
                                CurrencyCode = product.Currency,
                                PropertyRoomBookingID = roomNum + 1,
                                RoomType = oRoom.RoomType,
                                MealBasisCode = product.MealType,
                                Adults = oRoom.Pax.AdultQuantity,
                                ChildAgeCSV = string.Join(",", oRoom.Pax.ChildAges),
                                Infants = 0,
                                Amount = amount,
                                TPReference = $"{result.HotelCode}|{product.Code}|{responseCode}",
                                NonRefundableRates = product.NonRefundable
                            };
                        });
                    });
                });
            }).ToList();

            var transformedResults = new TransformedResultCollection();
            transformedResults.TransformedResults.AddRange(results);

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

        public string CreateSearchRequestString(SearchDetails searchDetails, string resortCode, IEnumerable<Hotel> hotels, bool useHotelCodes)
        {
            string nationality = _settings.LeadGuestNationality(searchDetails);
            string checkIn = searchDetails.ArrivalDate.ToString(Constant.DateFormat);
            string checkOut = searchDetails.DepartureDate.ToString(Constant.DateFormat);
            string currency = _settings.Currency(searchDetails);

            string sbRequest = "?";
            sbRequest += $"checkout={checkOut}&checkin={checkIn}";
            sbRequest += searchDetails.RoomDetails.Aggregate("", (all, oRoom) =>
            {
                string paxStr = oRoom.ChildAndInfantAges()
                                .Aggregate($"&pax={oRoom.Adults}", (sb, age) => $"{sb},{age}");
                return $"{all}{paxStr}";
            });

            if (useHotelCodes)
            {
                sbRequest += $"&hotel_code={string.Join(",", hotels.Select(h => h.TPKey))}";
            }
            else if (hotels.Count() == 1)
            {
                sbRequest += $"&hotel_code={hotels.First().TPKey}";
            }
            else
            {
                sbRequest += $"&destination_code={resortCode}";
            }

            sbRequest += $"&client_nationality={nationality}";
            sbRequest += $"&currency={currency}";

            return sbRequest;
        }

        #endregion
    }
}