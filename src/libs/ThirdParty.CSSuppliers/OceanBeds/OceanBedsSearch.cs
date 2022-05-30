namespace ThirdParty.CSSuppliers.OceanBeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.CSSuppliers.OceanBeds.Models;
    using static OceanBedsHelper;

    public class OceanBedsSearch : ThirdPartyPropertySearchBase
    {
        private readonly IOceanBedsSettings _settings;
        private readonly ISerializer _serializer;

        public OceanBedsSearch(IOceanBedsSettings settings, ISerializer serializer, ILogger<OceanBedsSearch> logger)
            : base(logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public override string Source => ThirdParties.OCEANBEDS;

        public override bool SqlRequest => false;

        public override List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits, bool saveLogs)
        {
            var requests = from resort in resortSplits
                           select new OceanBedsPropertyDetails(searchDetails, resort.ResortCode)
                into propertyDetails
                           select BuildPropertyAvailabilityRequest(propertyDetails, searchDetails, _settings)
                into request
                           select BuildSearchRequest(searchDetails, request, saveLogs);

            return requests.ToList();
        }

        public override TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
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

        public override bool SearchRestrictions(SearchDetails searchDetails)
        {
            return searchDetails.Duration < 7;
        }

        public override bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        private Request BuildSearchRequest(SearchDetails searchDetails, PropertyAvailabilityRQ request, bool saveLogs)
        {
            var searchWebRequest = new Request();

            try
            {
                searchWebRequest.EndPoint = _settings.SearchEndPoint(searchDetails);
                searchWebRequest.Method = eRequestMethod.POST;
                searchWebRequest.Source = Source;
                searchWebRequest.LogFileName = "Search";
                searchWebRequest.CreateLog = saveLogs;
                searchWebRequest.TimeoutInSeconds = RequestTimeOutSeconds(searchDetails);
                searchWebRequest.ExtraInfo = searchDetails;
                searchWebRequest.SetRequest(_serializer.Serialize(request));
                searchWebRequest.UseGZip = _settings.UseGzip(searchDetails).ToSafeBoolean();
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
