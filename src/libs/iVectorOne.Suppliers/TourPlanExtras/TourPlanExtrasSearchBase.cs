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

        private readonly ISerializer _serializer;
        private readonly ILogger<TourPlanExtrasSearchBase> _logger;
        public static readonly string ThirdPartySettingException = "The Third Party Setting: {0} must be provided.";

        public TourPlanExtrasSearchBase(
            ISerializer serializer,
            ILogger<TourPlanExtrasSearchBase> logger
           )
        {
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _settings = new InjectedTourPlanTransfersSettings();
        }

        public abstract string Source { get; }

        public Task<List<Request>> BuildSearchRequestsAsync(ExtraSearchDetails searchDetails, List<Extra> extras)
        {
            List<Request> requests = new List<Request>();
            if (extras == null)
            {
                return Task.FromResult(requests);
            }
            foreach (var payload in extras.Select(x => x.Payload).Distinct())
            {
                var request = BuildOptionInfoRequest(searchDetails, payload);
                request.ExtraInfo = payload;
                requests.Add(request);
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

        public TransformedExtraResultCollection TransformResponse(List<Request> requests, ExtraSearchDetails searchDetails, List<Extra> extras)
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
        private Request BuildOptionInfoRequest(ExtraSearchDetails searchDetails, string payload)
        {
            Request request = new Request();
            OptionInfoRequest optionInfoRequest = new OptionInfoRequest()
            {

                AgentID = _settings.AgentID,
                Password = _settings.Password,
                DateFrom = searchDetails.DepartureDate.ToString(Constant.DateTimeFormat),
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
