namespace ThirdParty.CSSuppliers.BedsWithEase
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using Models;
    using Models.Common;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    public class BedsWithEaseSearch : IThirdPartySearch
    {
        private readonly IBedsWithEaseSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BedsWithEaseSearch> _logger;

        public BedsWithEaseSearch(
            IBedsWithEaseSettings settings,
            ISerializer serializer,
            IHttpClientFactory httpclient,
            ILogger<BedsWithEaseSearch> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpclient, nameof(httpclient)).CreateClient();
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public string Source => ThirdParties.BEDSWITHEASE;

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits, bool saveLogs)
        {
            var requests = new List<Request>();
            string sessionId = BedsWithEaseHelper.GetSessionId(searchDetails, _settings, _serializer, _httpClient, _logger);

            foreach (var resortSplit in resortSplits)
            {
                var request = new Request
                {
                    EndPoint = _settings.URL(searchDetails),
                    Method = eRequestMethod.POST,
                    SoapAction = _settings.SOAPAvailableHotels(searchDetails),
                    SOAP = true,
                    Source = Source,
                    LogFileName = "Search",
                    CreateLog = saveLogs,
                    UseGZip = _settings.UseGZIP(searchDetails)
                };

                var xmlRequest = BuildAvailableRequest(sessionId, searchDetails, resortSplit.ResortCode);
                request.SetRequest(xmlRequest);

                requests.Add(request);
            }

            return requests;
        }

        private XmlDocument BuildAvailableRequest(string sessionId, SearchDetails searchDetails, string resortCode)
        {
            string operatorCode = _settings.OperatorCode(searchDetails);
            var roomDetails = searchDetails.RoomDetails;

            var envelope = new Envelope<HotelAvailabilityRequest>
            {
                Body =
                {
                    Content =
                    {
                        StayDateRange =
                        {
                            Start = $"{searchDetails.PropertyArrivalDate:yyyy-MM-dd}",
                            End = $"{searchDetails.PropertyDepartureDate:yyyy-MM-dd}"
                        },
                        SessionId = sessionId,
                        HotelSearchCriterion =
                        {
                            Locations = new Location[] { new() { ResortCode = resortCode } }
                        }
                    }
                }
            };

            var request = envelope.Body.Content;

            if (!string.IsNullOrEmpty(operatorCode))
                request.Operators = new[] { operatorCode };

            int roomIndex = 0;
            foreach (var room in roomDetails)
            {
                ++roomIndex;

                var perRoom = new PerRoom
                {
                    PerRoomRecordNumber = roomIndex,
                };

                for (int i = 0; i < room.Adults; i++)
                {
                    perRoom.Adults.Add(new Person());
                }

                if (room.Children > 0)
                {
                    perRoom.Children = room.ChildAges.Select(
                        childAge => new Person
                        {
                            Age = new Age
                            {
                                Value = childAge
                            }
                        }).ToArray();
                }

                if (room.Infants > 0)
                {
                    perRoom.Infants = room.ChildAges.Select(
                        _ => new Person
                        {
                            Age = new Age
                            {
                                Value = 1
                            }
                        }).ToArray();
                }

                request.RoomStayCandidate.GuestCounts.Add(perRoom);
            }

            return _serializer.Serialize(envelope);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var responses =
                from request in requests
                where request.Success
                select _serializer.DeSerialize<Envelope<HotelAvailabilityResponse>>(request.ResponseXML);

            transformedResults.TransformedResults.AddRange(responses.SelectMany(r => GetResultFromResponse(r.Body.Content)));

            return transformedResults;
        }

        private static IEnumerable<TransformedResult> GetResultFromResponse(HotelAvailabilityResponse response)
        {
            var transformedResults = new List<TransformedResult>();
            foreach (var hotel in response.Hotels)
            {
                foreach (var roomStay in hotel.RoomStays)
                {
                    foreach (var allocationVariant in roomStay.AllocationVariants)
                    {
                        foreach (var rateWithBoard in allocationVariant.RoomType.RatesWithBoard)
                        {
                            int offerIndex = 0;
                            var specialOffer = new StringBuilder();
                            foreach (var offer in rateWithBoard.Offers)
                            {
                                ++offerIndex;
                                specialOffer.Append(offerIndex == rateWithBoard.Offers.Count
                                    ? offer.Description
                                    : $"{offer.Description}|");
                            }

                            transformedResults.Add(new TransformedResult()
                            {
                                TPKey = $"{hotel.HotelInfo.Code}|{hotel.HotelInfo.HotelGUID}",
                                CurrencyCode = rateWithBoard.CurrencyCode,
                                RoomType = allocationVariant.RoomType.Description,
                                MealBasisCode = rateWithBoard.BoardType.Code,
                                Amount = rateWithBoard.TotalPrice.ToSafeDecimal(),
                                TPReference = $"{rateWithBoard.BookCode}|{hotel.HotelInfo.HotelGUID}|{rateWithBoard.Contract}|{response.SessionId}",
                                SpecialOffer = specialOffer.ToString(),
                                AvailableRooms = allocationVariant.RoomType.NumberOfRoomsAvailable.Value,
                                PropertyRoomBookingID = roomStay.PerRoomRecordNumber
                            });
                        }
                    }
                }
            }

            return transformedResults;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(SearchDetails searchDetails)
        {
            return false;
        }
    }
}
