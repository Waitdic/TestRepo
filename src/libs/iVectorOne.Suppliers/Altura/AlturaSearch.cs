namespace iVectorOne.Suppliers.Altura
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVector.Search.Property;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.Models.Altura;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

    public class AlturaSearch : IThirdPartySearch, ISingleSource
    {
        #region Properties

        private readonly IAlturaSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;

        public string Source => ThirdParties.ALTURA;

        #endregion

        #region Constructors

        public AlturaSearch(
            IAlturaSettings settings,
            ITPSupport support,
            ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return searchDetails.Rooms > 1 && !_settings.EnableMultiRoomSearch(searchDetails);
        }

        #endregion

        #region SearchFunctions

        public async Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            // Create for each resort a list of hotels
            var hotels = resortSplits.SelectMany(rs => rs.Hotels.Select(h => h.TPKey)).Distinct().ToList();
            List<Request> requests = new();

            foreach (var room in searchDetails.RoomDetails)
            {
                var searchRequest = await GetSearchRequestXmlAsync(searchDetails, string.Join(",", hotels), room);

                var request = new Request
                {
                    EndPoint = _settings.GenericURL(searchDetails),
                    Method = RequestMethod.POST,
                    Param = "xml",
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    ExtraInfo = room.PropertyRoomBookingID,
                };

                request.SetRequest(searchRequest);
                requests.Add(request);
            }

            return requests;
        }

        #endregion

        #region Transform Response

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var alturaSearchResponses = requests
                .Where(request => request.Success && !request.ResponseString.Contains("Error"))
                .OrderBy(request => GetRoomBookingId(request))
                .Select(request =>
                {
                    var searchResponse = _serializer.DeSerialize<AlturaSearchResponses>(request.ResponseXML);
                    searchResponse.PropertyRoomBookingID = GetRoomBookingId(request);
                    return searchResponse;
                }).ToList();

            var transformedResults = new TransformedResultCollection();
            transformedResults.TransformedResults = alturaSearchResponses
                    .Where(r => r.Response.Hotels.SelectMany(h => h.Rates.Select(r => r.Rooms)).Count() > 0)
                    .SelectMany(x => GetResultFromResponse(x)).ToList();

            return transformedResults;
        }

        private int GetRoomBookingId(Request request)
            => request.ExtraInfo.ToSafeInt();

        #endregion

        #region Validators

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion

        #region Helpers

        private async Task<XmlDocument> GetSearchRequestXmlAsync(SearchDetails searchDetails, string hotelList, RoomDetail room)
        {
            var sellingCountry = await _support.TPCountryCodeLookupAsync(Source, searchDetails.SellingCountry, searchDetails.SubscriptionID);
            var sourceMarket = !string.IsNullOrEmpty(sellingCountry) ? sellingCountry : _settings.SourceMarket(searchDetails);

            var nationalityLookupValue = await _support.TPNationalityLookupAsync(Source, searchDetails.ISONationalityCode);
            var leadGuestNationality = !string.IsNullOrEmpty(nationalityLookupValue) ? nationalityLookupValue : _settings.LeadGuestNationality(searchDetails);

            var childAges = room.ChildAges ?? new List<int>()
                .Concat(Enumerable.Range(0, room.Infants).Select(_ => 1))
                .ToList();

            var request = new AlturaSearchRequest
            {
                Request = {
                    RequestType = Constant.RequestTypeSearch,
                    Version = Constant.ApiVersion,
                    Currency = _settings.Currency(searchDetails),
                    Session = GetSession(_settings, searchDetails),
                    Destination =
                    {
                        DestinationType = Constant.DestinationTypeHotel,
                        Content = hotelList
                    },
                    Arrival = searchDetails.ArrivalDate.ToString(Constant.ApiDateFormat),
                    Departure = searchDetails.DepartureDate.ToString(Constant.ApiDateFormat),
                    NumberOfRooms = 1,
                    RoomOccupancy =
                    {
                        new RequestRoom
                        {
                            Code = 1,
                            Adults = room.Adults,
                            Children = room.Children + room.Infants,
                            ChildrenAges = childAges,
                        }
                    },
                    Market =
                    {
                        SourceMarket = sourceMarket,
                        ClientNationality = string.IsNullOrEmpty(leadGuestNationality)?string.Empty : leadGuestNationality
                    }
                }
            };

            var xmlRequest = _serializer.CleanXmlNamespaces(_serializer.Serialize(request));
            return xmlRequest;
        }

        internal static Session GetSession(IAlturaSettings settings, IThirdPartyAttributeSearch searchDetails, string id = "")
            => new Session
            {
                Id = id,
                AgencyId = settings.AgencyId(searchDetails),
                Password = settings.Password(searchDetails),
                ExternalId = settings.ExternalId(searchDetails)
            };

        private IEnumerable<TransformedResult> GetResultFromResponse(AlturaSearchResponses response)
        {
            return response.Response.Hotels.SelectMany(hotel => hotel.Rates
                .SelectMany(rate => rate.Rooms
                    .Select(room => new TransformedResult
                    {
                        MasterID = hotel.Id.ToSafeInt(),
                        TPKey = hotel.Id,
                        CurrencyCode = rate.Currency,
                        PropertyRoomBookingID = response.PropertyRoomBookingID,
                        RoomType = room.Name,
                        RoomTypeCode = room.Mapping,
                        MealBasisCode = rate.Board,
                        Amount = room.Price / 100,
                        TPReference = $"{response.Response.Session}|{rate.Id}",
                        NonRefundableRates = string.Equals(rate.NoRefundable, "1")
                    })));
        }

        #endregion
    }
}