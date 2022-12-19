using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intuitive;
using Intuitive.Helpers.Net;
using Intuitive.Helpers.Serialization;
using iVectorOne.Constants;
using iVectorOne.Interfaces;
using iVectorOne.Models;
using iVectorOne.Search.Models;
using iVectorOne.Search.Results.Models;
using iVectorOne.Suppliers.PremierInn.Models;
using iVectorOne.Suppliers.PremierInn.Models.Search;
using iVectorOne.Suppliers.PremierInn.Models.Soap;

namespace iVectorOne.Suppliers.PremierInn
{
    public class PremierInnSearch : IThirdPartySearch, ISingleSource
    {
        #region Properties

        private readonly IPremierInnSettings _settings;
        private readonly ISerializer _serializer;

        public PremierInnSearch(IPremierInnSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public string Source => ThirdParties.PREMIERINN;

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return searchDetails.RoomDetails.Any(x => x.Adults > 2 || (x.Children + x.Infants) > 3);
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            if (resortSplits.Count == 0) return Task.FromResult(new List<Request>());

            var requests = new List<Request>();
            var groups = Helper.GetHotelCodes(resortSplits, _settings.HotelSearchLimit(searchDetails));

            for (var index = 0; index < searchDetails.Rooms; index++)
            {
                foreach (var group in groups)
                {
                    var request = BuildSearchRequest(searchDetails, group, index);

                    var webRequest = new Request
                    {
                        EndPoint = _settings.GenericURL(searchDetails),
                        Method = RequestMethod.POST,
                        ContentType = ContentTypes.Text_xml,
                        SoapAction = Models.Constants.SOAPAction,
                        ExtraInfo = index
                    };

                    webRequest.SetRequest(request);
                    requests.Add(webRequest);
                }
            }

            return Task.FromResult(requests);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var allResponses = requests
                .Where(x => x.Success)
                .Select(r => _serializer.DeSerialize<EnvelopeResponse<AvailabilityResponse>>(_serializer.CleanXmlNamespaces(r.ResponseXML)))
                .ToList();

            return transformedResults;
        }

        private string BuildSearchRequest(
            SearchDetails searchDetails,
            IEnumerable<string> hotelCodes,
            int index)
        {
            var room = searchDetails.RoomDetails[index];
            var request = new AvailabilityRequest
            {
                Login = Helper.BuildLogin(_settings, searchDetails),
                Parameters =
                {
                    MessageType = "AvailabilityRequest",
                    HotelCode = string.Join(',', hotelCodes),
                    StayDateRange =
                    {
                        Start = searchDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                        End = searchDetails.DepartureDate.ToString("yyyy-MM-dd")
                    },
                    Rooms = 
                    {
                        NumberofRooms = 1,
                        RoomDetails = 
                        {
                            Number = room.Children + room.Infants + room.Adults,
                            Adults = room.Adults,
                            Children = room.Children + room.Infants,
                            Cots = room.Infants != 0 ? "Yes" : "No",
                            Double = (room.Children == 0 && room.Infants < 2) ? "Yes" : "No",
                            Disabled = "No"
                        }
                    }
                }
            };
          
            var content = _serializer.CleanXmlNamespaces(_serializer.Serialize(request)).InnerXml
                .Replace("<AvailabilityRequest>", "")
                .Replace("</AvailabilityRequest>", "");

            return Helper.CreateEnvelope(_serializer, content);
        }
    }
}
