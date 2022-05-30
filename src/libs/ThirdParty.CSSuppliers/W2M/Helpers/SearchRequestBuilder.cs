namespace ThirdParty.CSSuppliers.Helpers.W2M
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Constants;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;
    using ThirdParty.CSSuppliers.Models.W2M;
    using Microsoft.Extensions.Logging;

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
                Username = _settings.Username(searchDetails),
                Password = _settings.Password(searchDetails),
                Language = _settings.LangID(searchDetails),
                Endpoint = _settings.SearchUrl(searchDetails),
                SoapPrefix = _settings.SoapActionPrefix(searchDetails)
            };

            var startDate = searchDetails.PropertyArrivalDate.ToString(Constants.DateTimeFormat);
            var endDate = searchDetails.PropertyDepartureDate.ToString(Constants.DateTimeFormat);

            var hotelsCodes = GetHotelCodes(searchDetails, resortSplits);

            var hotelsSearchLimit = _settings.HotelSearchLimit(searchDetails);
            var hotelsCodesLists = hotelsCodes.Batch(hotelsSearchLimit);

            var leadGuestNationality = _settings.DefaultNationality(searchDetails);

            foreach (var codes in hotelsCodesLists)
            {
                for (int i = 0; i < searchDetails.RoomDetails.Count; i++)
                {
                    int propertyRoomBookingID = i + 1;
                    var room = searchDetails.RoomDetails[i];

                    var searchRequest = _xmlRequestBuilder.BuildAvailabilityRequest(parameters, startDate, endDate, room, leadGuestNationality, codes);
                    var request = BuildJuniperWebRequest(searchRequest, Constants.RequestNames.Search, parameters, Constants.SoapActions.AvailabilityCheck);
                    var uniqueCode = $"{source}_{Guid.NewGuid()}";
                    var searchHelper = new SearchExtraHelper(searchDetails, uniqueCode)
                    {
                        ExtraInfo = propertyRoomBookingID.ToString()
                    };
                    request.ExtraInfo = searchHelper;
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
                Method = eRequestMethod.POST,
                ContentType = ContentTypes.Text_Xml_charset_utf_8,
                LogFileName = requestName,
                UseGZip = true,
                CreateLog = parameters.CreateLogs
            };
            webRequest.SetRequest(request);
            webRequest.Headers.AddNew("Accept-Encoding", "gzip, deflate");

            return webRequest;
        }

        internal virtual string GetAvailabilityCheckResponse(string hotelAvailCheck, BaseRequestParameters parameters)
        {
            var request = BuildJuniperWebRequest(hotelAvailCheck, Constants.RequestNames.AvailabilityCheck, parameters,
                Constants.SoapActions.HotelAvailabilityCheck);
            request.Source = ThirdParties.W2M;
            request.Send(_httpClient, _logger).RunSynchronously();

            return request.ResponseString;
        }
    }
}
