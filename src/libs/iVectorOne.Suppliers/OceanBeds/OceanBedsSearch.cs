namespace ThirdParty.CSSuppliers.OceanBeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.OceanBeds.Models;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Results.Models;
    using static OceanBedsHelper;

    public class OceanBedsSearch : IThirdPartySearch, ISingleSource
    {
        private readonly IOceanBedsSettings _settings;
        private readonly ISerializer _serializer;

        public OceanBedsSearch(IOceanBedsSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public string Source => ThirdParties.OCEANBEDS;

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = from resort in resortSplits
                           select new OceanBedsPropertyDetails(searchDetails, resort.ResortCode)
                into propertyDetails
                           select BuildPropertyAvailabilityRequest(propertyDetails, searchDetails, _settings)
                into request
                           select BuildSearchRequest(searchDetails, request);

            return Task.FromResult(requests.ToList());
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResult = new TransformedResultCollection();
            var responses = requests
                .Where(x => x.Success)
                .Select(x => _serializer.DeSerialize<AvailabilityRS>(x.ResponseXML));

            transformedResult.TransformedResults.AddRange(responses
                .Where(r => r.Status.Message.ToLower() == "success")
                .SelectMany(r => GetResultFromResponse(r, searchDetails)));

            return transformedResult;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return searchDetails.Duration < 7;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        private Request BuildSearchRequest(SearchDetails searchDetails, PropertyAvailabilityRQ request)
        {
            var searchWebRequest = new Request();

            try
            {
                searchWebRequest.EndPoint = _settings.SearchEndPoint(searchDetails);
                searchWebRequest.Method = RequestMethod.POST;
                searchWebRequest.ExtraInfo = searchDetails;
                searchWebRequest.SetRequest(_serializer.Serialize(request));
                searchWebRequest.SOAP = false;
            }
            catch (Exception)
            {
                // ignored
            }

            return searchWebRequest;
        }


        private List<TransformedResult> GetResultFromResponse(
            AvailabilityRS rs,
            SearchDetails searchDetails)
        {
            var transformedResults = new List<TransformedResult>();

            foreach (var response in rs.Response)
            {
                foreach (var room in response.RoomList)
                {
                    string roomType;

                    if (room.BedRoom > 0)
                    {
                        roomType = $"{room.BedRoom} Bedroom";

                        if (room.BedRoom > 1)
                        {
                            roomType += "s";
                        }
                    }
                    else
                    {
                        roomType = room.Name;
                    }

                    // Board basis needs to default to Self Catering for individual homes
                    if (string.IsNullOrEmpty(response.BoardBasis) && response.Id.Contains('i'))
                    {
                        response.BoardBasis = "Self Catering";
                    }

                    for (int pos = 1; pos <= searchDetails.Rooms; pos++)
                    {
                        if (pos == 1 || (response.Id.Contains('h')
                                         && searchDetails.TotalAdults <= room.MaxAdults
                                         && searchDetails.TotalChildren <= room.MaxChildren))
                        {
                            transformedResults.Add(new TransformedResult
                            {
                                TPKey = response.Id,
                                CurrencyCode = _settings.Currency(searchDetails),
                                PropertyRoomBookingID = pos,
                                RoomType = roomType,
                                RoomTypeCode = $"{room.Id}|{room.Code}",
                                MealBasisCode = response.BoardBasis,
                                Amount = room.NetPrice,
                                SpecialOffer = room.OfferText,
                                TPReference = $"{response.Id}|{response.PropertyCode}|{room.Id}|{room.Code}"
                            });
                        }
                    }
                }
            }

            return transformedResults;
        }
    }
}
