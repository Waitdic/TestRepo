namespace iVectorOne.Suppliers.TourPlanExtras
{
    using Intuitive;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Extra;
    using iVectorOne.Interfaces;
    using iVectorOne.Models.Extra;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models.Extra;
    using iVectorOne.Suppliers.TourPlanExtras.Models;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public abstract class TourPlanExtrasSearchBase : IThirdPartySearch, ISingleSource
    {
        private readonly ITourPlanTransfersSettings _settings;

        private readonly HttpClient _httpClient;
        private readonly ISerializer _serializer;
        private readonly ILogger<TourPlanExtrasSearchBase> _logger;
        public static readonly string ThirdPartySettingException = "The Third Party Setting: {0} must be provided.";

        public TourPlanExtrasSearchBase(
            HttpClient httpClient,
            ISerializer serializer,
            ILogger<TourPlanExtrasSearchBase> logger
           )
        {
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _settings = new InjectedTourPlanTransfersSettings();
        }

        public abstract string Source { get; }

        public Task<List<Request>> BuildSearchRequestsAsync(ExtraSearchDetails searchDetails, List<Extras> extras)
        {
            List<Request> requests = new List<Request>();
            if (extras is not null && extras.Any())
            {
                foreach (var payload in extras.Select(x => x.Payload).Distinct())
                {
                    var Outbound = BuildOptionInfoRequest(searchDetails, payload, searchDetails.DepartureDate);
                    Outbound.ExtraInfo = Constant.Outbound;
                    requests.Add(Outbound);
                    if (!searchDetails.OneWay)
                    {
                        var returnBuildOptionInfoRequest = BuildOptionInfoRequest(searchDetails, payload, searchDetails.ReturnDate);
                        returnBuildOptionInfoRequest.ExtraInfo = string.Empty;
                        requests.Add(returnBuildOptionInfoRequest);
                    }
                }
            }
            return Task.FromResult(requests);
        }

        public bool ResponseHasExceptions(Request request)
        {
            throw new NotImplementedException();
        }

        public bool SearchRestrictions(ExtraSearchDetails searchDetails)
        {
            return false;
        }

        public TransformedExtraResultCollection TransformResponse(List<Request> requests, ExtraSearchDetails searchDetails, List<Extras> extras)
        {
            throw new NotImplementedException();
        }

        public bool ValidateSettings(ExtraSearchDetails searchDetails)
        {
            if (!_settings.SetThirdPartySettings(searchDetails.ThirdPartySettings))
            {
                searchDetails.Warnings.AddRange(_settings.GetWarnings());
                return false;
            }
            return true;
        }

        #region private methods
        private Request BuildOptionInfoRequest(ExtraSearchDetails searchDetails, string payload, DateTime dateFrom)
        {
            Request request = new Request();
            OptionInfoRequest optionInfoRequest = new OptionInfoRequest()
            {

                AgentID = _settings.AgentID,
                Password = _settings.Password,
                DateFrom = dateFrom.ToString(Constant.DateTimeFormat),
                Info = Constant.Info,
                Opt = payload + Constant.ExtraOptText,
                RoomConfigs = new List<RoomConfiguration>()
                {
                   new RoomConfiguration() {
                   Adults = searchDetails.Adults,
                   Children = searchDetails.Children,
                   Infants = searchDetails.Children
                   }
                }
            };

            request = GetXMLRequest();
            var xmlDocument = Helpers.Serialize(optionInfoRequest, _serializer);
            request.SetRequest(xmlDocument);

            return request;
        }
        private Request GetXMLRequest()
        {
            return new Request()
            {
                EndPoint = _settings.URL,
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml

            };
        }
        #endregion
    }
}
