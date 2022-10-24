namespace iVectorOne.Suppliers.Italcamel
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Suppliers.Italcamel.Models.Common;
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;
    using iVectorOne.Interfaces;
    using System.Linq;
    using iVectorOne.Suppliers.Italcamel.Models.Search;

    public class ItalcamelSearch : IThirdPartySearch, ISingleSource
    {
        #region Properties
        
        private readonly IItalcamelSettings _settings;
        private readonly ISerializer _serializer;
        private readonly ItalcamelHelper _helper = new();

        public string Source  => ThirdParties.ITALCAMEL;

        public ItalcamelSearch(IItalcamelSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return (searchDetails.DepartureDate - searchDetails.ArrivalDate).Days > _settings.MaximumNumberOfNights(searchDetails)
                   || searchDetails.Rooms > _settings.MaximumRoomNumber(searchDetails)
                   || searchDetails.RoomDetails.Any(x => (x.Adults + x.Children) > _settings.MaximumRoomGuestNumber(searchDetails));
        }

        public bool ResponseHasExceptions(Intuitive.Helpers.Net.Request request)
        {
            return false;
        }

        #endregion

        #region SearchFunctions

        public Task<List<Intuitive.Helpers.Net.Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Intuitive.Helpers.Net.Request>();
            
            // get city code or hotel code
            var groupSearchIds = GetSearchIDs(resortSplits, _settings.MaximumHotelSearchNumber(searchDetails));
            
            // build request xml for each resort
            foreach (var group in groupSearchIds)
            {
                // work out whether to search by city or macro region
                var searchRequest = _helper.BuildSearchRequest(_settings, _serializer, searchDetails, group.ToArray());
                var soapAction = _settings.GenericURL(searchDetails).Replace("test.", "") + "/GETAVAILABILITY";

                // get response
                var request = new Intuitive.Helpers.Net.Request
                {
                    EndPoint = _settings.GenericURL(searchDetails),
                    Method = RequestMethod.POST,
                    ExtraInfo = searchDetails,
                    ContentType = ContentTypes.Text_xml,
                    UseGZip = true,
                    SoapAction = soapAction
                };
                request.SetRequest(searchRequest);
                
                requests.Add(request);
            }

            return Task.FromResult(requests);
        }

        public TransformedResultCollection TransformResponse(List<Intuitive.Helpers.Net.Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var responses = requests
                .Where(x => x.Success)
                .Select(r => _serializer.DeSerialize<Envelope<GetAvailabilityResponse>>(r.ResponseXML));

            var guests = BuildGuests(searchDetails.RoomDetails);

            transformedResults.TransformedResults.AddRange(GetResultFromResponse(responses, guests, searchDetails));

            return transformedResults;
        }

        public List<TransformedResult> GetResultFromResponse(
            IEnumerable<Envelope<GetAvailabilityResponse>> response,
            Guests guests,
            SearchDetails searchDetails)
        {
            var transformedResults = new List<TransformedResult>();
            var excludeNRF = _settings.ExcludeNRF(searchDetails);
            var packageRate = _settings.PackageRates(searchDetails);
            var accommodations = response.SelectMany(x => x.Body.Content.GetAvaibilityResult.Accommodations);
            
            foreach (var accommodation in accommodations)
            {
                var tpkey = accommodation.UID;
                for (var count = 0; count < guests.Rooms.Count; count++)
                {
                    foreach (var room in accommodations.Where(a => a.UID == tpkey).SelectMany(r => r.Rooms.Where(rm => rm.Available)))
                    {
                        if ((room.NotRefundable && excludeNRF) 
                            || (room.PackageRate && !packageRate))
                        {
                            continue;
                        }

                        foreach (var roomDetail in room.RoomDetails.Where(rd => rd.Adults == guests.Rooms[count].Adults 
                                                  && rd.Children == guests.Rooms[count].Children
                                                  && rd.ChildAge1 == guests.Rooms[count].ChildAge1
                                                  && rd.ChildAge2 == guests.Rooms[count].ChildAge2))
                        {
                            transformedResults.AddRange(roomDetail.Boards.Select(board => new TransformedResult
                            {
                                TPKey = tpkey,
                                CurrencyCode = "EUR",
                                PropertyRoomBookingID = count,
                                RoomType = room.Name,
                                MealBasisCode = board.Acronym,
                                Adults = guests.Rooms[count].Adults,
                                Children = guests.Rooms[count].Children,
                                ChildAgeCSV = guests.Rooms[count].HlpChildAgeCSV,
                                Amount = board.Amount,
                                TPReference = $"{room.MasterUID}|{board.UID}",
                                NonRefundableRates = room.NotRefundable,
                            }));
                        }
                    }
                }
            }

            return transformedResults;
        }

        #endregion

        #region Helpers

        private static List<Batch<string>> GetSearchIDs(List<ResortSplit> resortSplits, int limit)
        {
            return resortSplits.SelectMany(split => split.Hotels.Select(h => h.TPKey))
                .Distinct().Batch(limit).ToList();
        }

        public Guests BuildGuests(iVector.Search.Property.RoomDetails roomDetails)
        {
            return new Guests
            {
                Rooms = roomDetails.Select(room => new SearchRoom
                {
                    Adults = room.Adults,
                    Children = room.Children,
                    ChildAge1 = room.Children > 0 ? room.ChildAges[0] : 0,
                    ChildAge2 = room.Children > 1 ? room.ChildAges[1] : 0,
                    HlpChildAgeCSV = room.ChildAgeCSV
                }).ToList()
            };
        }

        #endregion
    }
}

