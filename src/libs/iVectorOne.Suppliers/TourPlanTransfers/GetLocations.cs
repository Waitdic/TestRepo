namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using System.Xml;
    using System.Collections.Generic;
    using System.Net.Http;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using Intuitive;
    using System.Threading.Tasks;
    using Intuitive.Data;
    using Newtonsoft.Json;
    using MoreLinq;
    using System.Linq;
    using System;
    using iVectorOne.Suppliers.BedsWithEase.Models.Common;
    using FluentValidation.Validators;

    public class GetLocations
    {
        private readonly ISql _sql;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GetLocations> _logger;

        private const string AgentID = "GOWAUD";
        private const string Password = "koala12";
        private const string Source = "GowaySydneyTransfers";

        public GetLocations(
            ISqlFactory sqlFactory,
            HttpClient httpClient,
            ILogger<GetLocations> logger)
        {
            _sql = Ensure.IsNotNull(sqlFactory, nameof(sqlFactory)).CreateSqlContext();
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public async Task AddNewLocations()
        {
            var locationCodes = await GetLocationCodes();

            var locationNames = new List<string>();

            foreach (string code in locationCodes)
            {
                locationNames.AddRange(await GetLocationNames(code));
            }

            await AddNewLocationsToTable(locationNames);

            return;
        }

        private async Task<List<string>> GetLocationCodes()
        {
            List<string> locationCodes = new List<string>();

            var request = $"<!DOCTYPE Request SYSTEM \"hostConnect_3_00_000.dtd\">\r\n<Request>\r\n    <GetLocationsRequest>\r\n        <AgentID>{AgentID}</AgentID>\r\n        <Password>{Password}</Password>\r\n    </GetLocationsRequest>\r\n</Request>";

            var webRequest = new Request
            {
                EndPoint = "https://pa-gowsyd.nx.tourplan.net/iCom_Test/servlet/conn",
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml,
            };

            webRequest.SetRequest(request);
            await webRequest.Send(_httpClient, _logger);

            XmlDocument locationResponse = webRequest.ResponseXML;

            foreach (XmlNode node in locationResponse.SelectNodes($"Reply/GetLocationsReply/Locations/Location/Code")) 
            {
                locationCodes.Add(node.InnerText);
            }

            return locationCodes;
        }

        private async Task<List<string>> GetLocationNames(string code)
        {
            var locationNames = new List<string>();

            var serviceCode = "TR";
            var infoCode = "GIST";
            string optCode = $"{code}{serviceCode}????????????";
            string date = "2023-11-07";

            var request = $"<?xml version=\"1.0\"?>\r\n<!DOCTYPE Request SYSTEM \"hostConnect_3_00_000.dtd\">\r\n<Request>\r\n    <OptionInfoRequest>\r\n        <AgentID>{AgentID}</AgentID>\r\n        <Password>{Password}</Password>\r\n        <Opt>{optCode}</Opt>\r\n        <Info>{infoCode}</Info>\r\n        <DateFrom>{date}</DateFrom>\r\n        <RoomConfigs>\r\n            <RoomConfig>\r\n                <Adults>2</Adults>\r\n                <Children>0</Children>\r\n                <Infants>0</Infants>\r\n            </RoomConfig>\r\n        </RoomConfigs>\r\n    </OptionInfoRequest>\r\n</Request>";

            var webRequest = new Request
            {
                EndPoint = "https://pa-gowsyd.nx.tourplan.net/iCom_Test/servlet/conn",
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml,
            };

            webRequest.SetRequest(request);
            await webRequest.Send(_httpClient, _logger);

            XmlDocument searchResponse = webRequest.ResponseXML;

            _logger.Log(LogLevel.Trace, $"Code: {code}, ResponseCount: {searchResponse.SelectNodes($"Reply/OptionInfoReply/Option/OptGeneral/Description").Count}");

            foreach (XmlNode node in searchResponse.SelectNodes($"Reply/OptionInfoReply/Option/OptGeneral/Description"))
            {
                locationNames.AddRange(SplitDescription(code, node.InnerText));
            }

            return locationNames;
        }

        private async Task AddNewLocationsToTable(List<string> locations)
        {
            var addedCount = await _sql.ReadScalarAsync<int>(
                    "Transfer_AddNewLocations",
                    new CommandSettings()
                        .IsStoredProcedure()
                        .WithParameters(new
                        {
                            source = Source,
                            locations = locations.ToDelimitedString(","),
                        }));
        }

        private List<string> SplitDescription(string code, string description)
        {
            try
            {
                description = description.Replace(",", "");

                var list = new List<string>();
                var strings = description.Split(" to ");

                if (strings.Length == 2)
                {
                    list.Add($"{code}: {strings[0]}");
                    list.Add($"{code}: {strings[1].Replace(" Transfer", "")}");
                } else
                {
                    list.Add($"{code}: {description} - Full Description");
                }

                return list;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

    }
}
