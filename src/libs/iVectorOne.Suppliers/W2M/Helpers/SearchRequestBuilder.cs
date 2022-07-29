namespace iVectorOne.Suppliers.Helpers.W2M
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.Models.W2M;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;

    internal class SearchRequestBuilder
    {
        private readonly SoapRequestXmlBuilder _xmlRequestBuilder;
        private readonly IW2MSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        internal SearchRequestBuilder(
            IW2MSettings settings,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger logger)
        {
            _xmlRequestBuilder = new SoapRequestXmlBuilder(serializer);
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        internal List<Request> BuildSearchRequests(SearchDetails searchDetails, string source, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            var parameters = new BaseRequestParameters
            {
                Username = _settings.User(searchDetails),
                Password = _settings.Password(searchDetails),
                Language = _settings.LanguageCode(searchDetails),
                Endpoint = _settings.SearchURL(searchDetails),
                SoapPrefix = _settings.SoapActionPrefix(searchDetails)
            };

            var startDate = searchDetails.ArrivalDate.ToString(Constants.DateTimeFormat);
            var endDate = searchDetails.DepartureDate.ToString(Constants.DateTimeFormat);

            var hotelsCodes = GetHotelCodes(searchDetails, resortSplits);

            var hotelsSearchLimit = _settings.HotelBatchLimit(searchDetails);
            var hotelsCodesLists = hotelsCodes.Batch(hotelsSearchLimit);

            var leadGuestNationality = _settings.LeadGuestNationality(searchDetails);

            foreach (var codes in hotelsCodesLists)
            {
                for (int i = 0; i < searchDetails.RoomDetails.Count; i++)
                {
                    int propertyRoomBookingID = i + 1;
                    var room = searchDetails.RoomDetails[i];

                    var searchRequest = _xmlRequestBuilder.BuildAvailabilityRequest(parameters, startDate, endDate, room, leadGuestNationality, codes);
                    var request = BuildJuniperWebRequest(searchRequest, Constants.RequestNames.Search, parameters, Constants.SoapActions.AvailabilityCheck);
                    
                    request.ExtraInfo = propertyRoomBookingID;
                    requests.Add(request);
                }
            }

            return requests;
        }

        private static IEnumerable<string> GetHotelCodes(SearchDetails searchDetails, List<ResortSplit> resortSplits) =>
            resortSplits
                .SelectMany(x => x.Hotels)
                .Select(x => x.TPKey);

        internal Request BuildJuniperWebRequest(string request, string requestName, BaseRequestParameters parameters, string soapAction)
        {
            var webRequest = new Request
            {
                Source = ThirdParties.W2M,
                EndPoint = parameters.Endpoint,
                SOAP = true,
                SoapAction = $"{parameters.SoapPrefix}{soapAction}",
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_Xml_charset_utf_8,
                LogFileName = requestName,
                UseGZip = true,
                CreateLog = parameters.CreateLogs
            };
            webRequest.SetRequest(request);
            webRequest.Headers.AddNew("Accept-Encoding", "gzip, deflate");

            return webRequest;
        }

        internal async Task<string> GetAvailabilityCheckResponseAsync(string hotelAvailCheck, BaseRequestParameters parameters)
        {
            var request = BuildJuniperWebRequest(hotelAvailCheck, Constants.RequestNames.AvailabilityCheck, parameters,
                Constants.SoapActions.HotelAvailabilityCheck);
            request.Source = ThirdParties.W2M;
            await request.Send(_httpClient, _logger);

            return request.ResponseString;
        }
    }
}