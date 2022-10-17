﻿namespace iVectorOne.Suppliers.Italcamel
{
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
                var searchRequest = _helper.BuildSearchRequest(_settings, _serializer, searchDetails, group);
                var soapAction = _settings.GenericURL(searchDetails).Replace("test.", ".") + "/GETAVAILABILITY";
                
                // get response
                var request = new Intuitive.Helpers.Net.Request
                {
                    EndPoint = _settings.GenericURL(searchDetails),
                    Method = RequestMethod.POST,
                    Source = Source,
                    LogFileName = "Search",
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
                for (var count = 1; count <= guests.Rooms.Count; count++)
                {
                    foreach (var room in accommodations.Where(a => a.UID == tpkey).SelectMany(r => r.Rooms.Where(rm => rm.Available)))
                    {
                        bool? nonRef = room.NotRefundable ? room.NotRefundable : null;
                        
                        if ((nonRef != null && excludeNRF) 
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
                                NonRefundableRates = nonRef,
                            }));
                        }
                    }
                }
            }

            return transformedResults;
        }

        #endregion

        #region Helpers

        private static List<string[]> GetSearchIDs(List<ResortSplit> resortSplits, int limit)
        {
            //var searchIds = new Dictionary<string, ItalcamelHelper.SearchType>();
            var searchIds = new List<string>();
            var groupsSearchIds = new List<string[]>();

            //if (resortSplits.Count == 1 && resortSplits[0].Hotels.Count == 1)
            //{
            //    searchIds.Add(resortSplits[0].Hotels[0].TPKey, ItalcamelHelper.SearchType.Hotel);
            //    groupsSearchIds.Add(searchIds);
            //}
            //else
            //{
            //    foreach (var resortSplit in resortSplits)
            //    {
            //        if (resortSplit.ResortCode.Split('|').Length > 1)
            //        {
            //            if (!searchIds.ContainsKey(resortSplit.ResortCode.Split('|')[1]))
            //                searchIds.Add(resortSplit.ResortCode.Split('|')[1], ItalcamelHelper.SearchType.Hotel);
            //        }
            //        else if (!searchIds.ContainsKey(resortSplit.ResortCode))
            //            searchIds.Add(resortSplit.ResortCode, ItalcamelHelper.SearchType.Hotel);
            //    }
            //}

            //if (resortSplits.Count == 1 && resortSplits[0].Hotels.Count == 1)
            //{
            //    searchIds.Add(resortSplits[0].Hotels[0].TPKey, ItalcamelHelper.SearchType.Hotel);
            //    groupsSearchIds.Add(searchIds);
            //}

            var count = 0;
            foreach (var hotel in resortSplits.SelectMany(x => x.Hotels))
            {
                if (count < limit)
                {
                    searchIds.Add(hotel.TPKey);
                    count++;
                }
                else
                {
                    groupsSearchIds.Add(searchIds.ToArray());
                    count = 0;
                    searchIds = new List<string> {hotel.TPKey};
                }
            }

            if (searchIds.Count != 0)
            {
                groupsSearchIds.Add(searchIds.ToArray());
            }
            
            //else
            //{
            //    foreach (var resortSplit in resortSplits)
            //    {
            //        if (resortSplit.ResortCode.Split('|').Length > 1)
            //        {
            //            if (!searchIds.ContainsKey(resortSplit.ResortCode.Split('|')[1]))
            //                searchIds.Add(resortSplit.ResortCode.Split('|')[1], ItalcamelHelper.SearchType.Hotel);
            //        }
            //        else if (!searchIds.ContainsKey(resortSplit.ResortCode))
            //            searchIds.Add(resortSplit.ResortCode, ItalcamelHelper.SearchType.Hotel);
            //    }
            //}

            return groupsSearchIds;
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

