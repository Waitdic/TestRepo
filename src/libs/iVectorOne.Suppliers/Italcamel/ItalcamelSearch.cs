namespace iVectorOne.Suppliers.Italcamel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Suppliers.Italcamel.Models.Common;
    using iVectorOne.Interfaces;
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;
    using iVectorOne.Models.Property.Booking;
    using System.Linq;
    using System.Xml;
    using iVectorOne.Suppliers.Italcamel.Models.Search;

    public class ItalcamelSearch : IThirdPartySearch, ISingleSource
    {
        #region Properties
        
        private readonly IItalcamelSettings _settings;
        private readonly ISerializer _serializer;
        public string Source  => ThirdParties.ITALCAMEL;

        public ItalcamelSearch(IItalcamelSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }
        
        #endregion
        
        #region SearchFunctions
        
        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Intuitive.Helpers.Net.Request>();
            
            // get city code or hotel code
            var searchIds = GetSearchIDs(resortSplits);
            
            // build request xml for each resort
            foreach (var searchId in searchIds)
            {
                // work out whether to search by city or macro region
                var searchRequest = BuildSearchRequest(searchDetails, searchId.Key, searchId.Value);
                var soapAction = _settings.URL(searchDetails).Replace("test.", "") + "/GETAVAILABILITYSPLITTED";
                
                // get response
                var request = new Intuitive.Helpers.Net.Request
                {
                    EndPoint = _settings.URL(searchDetails),
                    Method = RequestMethod.POST,
                    Source = Source,
                    LogFileName = "Search",
                    ExtraInfo = searchDetails,
                    ContentType = ContentTypes.Text_Xml_charset_utf_8,
                    UseGZip = true,
                    SoapAction = soapAction
                };
                request.SetRequest(searchRequest);
                
                requests.Add(request);
            }

            return Task.FromResult(requests);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var responses = requests
                .Where(x => x.Success)
                .Select(r => _serializer.DeSerialize<Envelope<GetAvailabilitySplittedResponse>>(r.ResponseXML));

            var guests = BuildGuests(searchDetails.RoomDetails);

            transformedResults.TransformedResults.AddRange(GetResultFromResponse(responses, guests));

            return transformedResults;
        }

        public List<TransformedResult> GetResultFromResponse(
            IEnumerable<Envelope<GetAvailabilitySplittedResponse>> response,
            Guests guests)
        {
            var transformedResults = new List<TransformedResult>();
            var accommodations = response.SelectMany(x => x.Body.Content.Response.Accommodations);
            foreach (var accommodation in accommodations)
            {
                var tpkey = accommodation.UID;
                for (var count = 1; count <= guests.Rooms.Count; count++)
                {
                    foreach (var room in accommodations.Where(a => a.UID == tpkey).SelectMany(r => r.Rooms.Where(rm => rm.Available)))
                    {
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
                                TPReference = $"{room.UID}|{board.UID}"
                            }));
                        }
                    }
                }
            }

            return transformedResults;
        }

        #endregion

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }
        
        private static Dictionary<string, SearchType> GetSearchIDs(List<ResortSplit> resortSplits)
        {
            var searchIds = new Dictionary<string, SearchType>();
            
            if (resortSplits.Count == 1 && resortSplits[0].Hotels.Count == 1)
                searchIds.Add(resortSplits[0].Hotels[0].TPKey, SearchType.Hotel);
            else
                foreach (var resortSplit in resortSplits)
                {
                    if (resortSplit.ResortCode.Split('|').Length > 1)
                    {
                        if (!searchIds.ContainsKey(resortSplit.ResortCode.Split('|')[1]))
                            searchIds.Add(resortSplit.ResortCode.Split('|')[1], SearchType.City);
                    }
                    else if (!searchIds.ContainsKey(resortSplit.ResortCode))
                        searchIds.Add(resortSplit.ResortCode, SearchType.City);
                }

            return searchIds;
        }

        private XmlDocument BuildSearchRequest(
            IThirdPartyAttributeSearch searchDetails,
            string searchCode,
            SearchType searchType,
            DateTime arrivalDate,
            DateTime departureDate,
            iVector.Search.Property.RoomDetails roomDetails)
        {
            var request = new Envelope<GetAvailabilitySplitted>
            {
                Body =
                {
                    Content =
                    {
                        Request =
                        {
                            Username = _settings.Username(searchDetails),
                            Password = _settings.Password(searchDetails),
                            LanguageuId = _settings.LanguageID(searchDetails),
                            AccomodationuId = searchType == SearchType.Hotel ? searchCode : string.Empty,
                            MacroregionuId = searchType == SearchType.MacroRegion ? searchCode : string.Empty,
                            CityuId =  searchType != SearchType.Hotel && searchType != SearchType.MacroRegion ? searchCode : string.Empty,
                            CheckIn = arrivalDate.ToString("yyyy-MM-dd"),
                            CheckOut = departureDate.ToString("yyyy-MM-dd"),
                            Rooms = roomDetails.Select(room => new Room
                            {
                                Adults = room.Adults,
                                Children = room.Children,
                                ChildAge1 = room.Children > 0 ? room.ChildAges[0] : 0,
                                ChildAge2 = room.Children > 1 ? room.ChildAges[1] : 0,
                            }).ToArray(),
                        }
                    }
                }
            };

            return _serializer.Serialize(request);
        }

        private XmlDocument BuildSearchRequest(SearchDetails searchDetails, string searchCode, SearchType searchType = SearchType.City)
        {
            return BuildSearchRequest(
                searchDetails,
                searchCode,
                searchType,
                searchDetails.ArrivalDate,
                searchDetails.DepartureDate,
                searchDetails.RoomDetails);
        }

        private XmlDocument BuildSearchRequest(PropertyDetails propertyDetails, string searchCode, SearchType searchType = SearchType.City)
        {
            return BuildSearchRequest(
                propertyDetails,
                searchCode,
                searchType,
                propertyDetails.ArrivalDate,
                propertyDetails.DepartureDate,
                GetRoomDetailsFromThirdPartyRoomDetails(propertyDetails.Rooms));
        }

        private iVector.Search.Property.RoomDetails GetRoomDetailsFromThirdPartyRoomDetails(List<RoomDetails> thirdPartyRoomDetails)
        {
            var roomDetails = new iVector.Search.Property.RoomDetails();

            foreach (var propertyRoomBooking in thirdPartyRoomDetails)
            {
                var oRoomDetail = new iVector.Search.Property.RoomDetail
                {
                    Adults = propertyRoomBooking.Adults,
                    Children = propertyRoomBooking.Children
                };

                for (var i = 0; i <= propertyRoomBooking.ChildAges.Count - 1; i++)
                    oRoomDetail.ChildAges.Add(propertyRoomBooking.ChildAges[i]);

                roomDetails.Add(oRoomDetail);
            }

            return roomDetails;
        }

        public Guests BuildGuests(iVector.Search.Property.RoomDetails roomDetails)
        {
            return new Guests
            {
                Rooms = roomDetails.Select(room => new Room
                {
                    Adults = room.Adults,
                    Children = room.Children,
                    ChildAge1 = room.Children > 0 ? room.ChildAges[0] : 0,
                    ChildAge2 = room.Children > 1 ? room.ChildAges[1] : 0,
                    HlpChildAgeCSV = room.ChildAgeCSV
                }).ToList()
            };
        }

        private enum SearchType
        {
            MacroRegion,
            City,
            Hotel
        }
    }
}

