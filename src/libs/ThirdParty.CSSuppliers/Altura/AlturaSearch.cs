namespace ThirdParty.CSSuppliers.Altura
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using iVector.Search.Property;
    using Microsoft.Extensions.Logging;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;
    using ThirdParty.CSSuppliers.Models.Altura;

    public class AlturaSearch : IThirdPartySearch
    {
        #region "Properties"

        private readonly IAlturaSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;

        public string Source => ThirdParties.ALTURA;

        #endregion

        #region "Constructors"

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

        #region "SearchRestrictions"

        public bool SearchRestrictions(SearchDetails oSearchDetails)
        {
            return oSearchDetails.Rooms > 1 && !_settings.SplitMultiRoom(oSearchDetails);
        }

        #endregion

        #region "SearchFunctions"

        public List<Request> BuildSearchRequests(SearchDetails oSearchDetails, List<ResortSplit> oResortSplits, bool bSaveLogs)
        {
            // Create for each resort a list of hotels
            var oHotels = oResortSplits.SelectMany(rs => rs.Hotels.Select(h => h.TPKey)).Distinct().ToList();
            List<Request> oRequests = new();
            // Create a request for each room 
            oRequests = oSearchDetails.RoomDetails.Select((oRoom, iPropertyRoomBookingID) =>
            {
                var oSearchRequest = GetSearchRequestXml(oSearchDetails, string.Join(",", oHotels), oRoom, ++iPropertyRoomBookingID);

                // Set a unique code. if the is one request we only need the source name
                var sUniqueCode = Source;

                var oExtraInfo = new SearchExtraHelper(oSearchDetails, sUniqueCode)
                {
                    ExtraInfo = iPropertyRoomBookingID.ToString()
                };

                var oRequest = new Request
                {
                    EndPoint = _settings.URL(oSearchDetails),
                    Method = eRequestMethod.POST,
                    Source = Source,
                    LogFileName = "Search",
                    Param = "xml",
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    ExtraInfo = oExtraInfo,
                    UseGZip = true,
                };

                oRequest.SetRequest(oSearchRequest);

                return oRequest;
            }).ToList();

            return oRequests;
        }

        #endregion


        #region "Transform Response"

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var oAlturaSearchResponses = requests
                .Where(oRequest => oRequest.Success && !oRequest.ResponseString.Contains("Error"))
                .OrderBy(oRequest => GetRoomBookingId(oRequest))
                .Select(oRequest =>
                {
                    var oSearchResponse = _serializer.DeSerialize<AlturaSearchResponses>(oRequest.ResponseXML);
                    oSearchResponse.PropertyRoomBookingID = GetRoomBookingId(oRequest);
                    return oSearchResponse;
                }).ToList();

            var oTransformedResults = new TransformedResultCollection();
            oTransformedResults.TransformedResults = oAlturaSearchResponses
                    .Where(r => r.Response.Hotels.SelectMany(h => h.Rates.Select(r => r.Rooms)).Count() > 0)
                    .SelectMany(x => GetResultFromResponse(x, searchDetails)).ToList();

            return oTransformedResults;
        }

        private int GetRoomBookingId(Request request)
            => SafeTypeExtensions.ToSafeInt((request.ExtraInfo as SearchExtraHelper)?.ExtraInfo);

        #endregion

        #region "Validators"

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }


        #endregion

        #region "Helpers"

        private XmlDocument GetSearchRequestXml(SearchDetails oSearchDetails, string sHotelList, RoomDetail oRoom, int iRoomCounter)
        {
            var sSellingCountry = _support.TPCountryCodeLookup(Source, oSearchDetails.SellingCountry);
            var sSourceMarket = !string.IsNullOrEmpty(sSellingCountry) ? sSellingCountry : _settings.SourceMarket(oSearchDetails);

            var sNationalityLookupValue = _support.TPNationalityLookup(Source, oSearchDetails.NationalityID);
            var sLeadGuestNationality = !string.IsNullOrEmpty(sNationalityLookupValue) ? sNationalityLookupValue : _settings.DefaultNationality(oSearchDetails);

            var liChildAges = oRoom.ChildAges ?? new List<int>()
                                .Concat(Enumerable.Range(0, oRoom.Infants).Select(_ => 1)).ToList();

            var oRequest = new AlturaSearchRequest
            {
                Request = {
                    RequestType = Constant.RequestTypeSearch,
                    Version = Constant.ApiVersion,
                    Currency = _settings.DefaultCurrency(oSearchDetails),
                    Session = GetSession(_settings, oSearchDetails),
                    Destination =
                    {
                        DestinationType = Constant.DestinationTypeHotel,
                        Content = sHotelList
                    },
                    Arrival = oSearchDetails.PropertyArrivalDate.ToString(Constant.ApiDateFormat),
                    Departure = oSearchDetails.PropertyDepartureDate.ToString(Constant.ApiDateFormat),
                    NumberOfRooms = 1,
                    RoomOccupancy =
                    {
                        new RequestRoom
                        {
                            Code = 1,
                            Adults = oRoom.Adults,
                            Children = oRoom.Children + oRoom.Infants,
                            ChildrenAges = liChildAges,

                        }
                    },
                    Market =
                    {
                        SourceMarket = sSourceMarket,
                        ClientNationality = string.IsNullOrEmpty(sLeadGuestNationality)?string.Empty : sLeadGuestNationality
                    }
                }
            };

            var xmlRequest = _serializer.Serialize(oRequest);
            return xmlRequest;
        }

        internal static Session GetSession(IAlturaSettings settings, IThirdPartyAttributeSearch oSearchDetails, string id = "")
            => new Session
            {
                Id = id,
                AgencyId = settings.AgencyId(oSearchDetails),
                Password = settings.Password(oSearchDetails),
                ExternalId = settings.ExternalId(oSearchDetails)
            };

        private List<TransformedResult> GetResultFromResponse(AlturaSearchResponses oResponse, SearchDetails oSearchDetails)
        {
            return oResponse.Response.Hotels.SelectMany(oHotel => oHotel.Rates
                .SelectMany(oRate => oRate.Rooms
                    .Select(oRoom => new TransformedResult
                    {
                        MasterID = SafeTypeExtensions.ToSafeInt(oHotel.Id),
                        TPKey = oHotel.Id,
                        CurrencyCode = oRate.Currency,
                        PropertyRoomBookingID = oResponse.PropertyRoomBookingID,
                        RoomType = oRoom.Name,
                        RoomTypeCode = oRoom.Mapping,
                        MealBasisCode = oRate.Board,
                        Amount = oRoom.Price / 100,
                        TPReference = $"{oResponse.Response.Session}|{oRate.Id}",
                        NonRefundableRates = string.Equals(oRate.NoRefundable, "1")
                    })
                )
            ).ToList();
        }

        #endregion
    }
}
