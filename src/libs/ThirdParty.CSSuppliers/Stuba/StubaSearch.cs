namespace ThirdParty.CSSuppliers.Stuba
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    public class StubaSearch : IThirdPartySearch, ISingleSource
    {
        private readonly IStubaSettings _settings;

        private readonly ISerializer _serializer;

        public string Source => ThirdParties.STUBA;

        public StubaSearch(IStubaSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            var requestBodies = new List<string>();
            
            foreach (var resortSplit in resortSplits.Where(rs => !rs.ResortCode.Contains("|")))
            {
                requestBodies.AddRange(BuildRequests(searchDetails, resortSplit.ResortCode, resortSplit.Hotels.Select(h => h.TPKey).ToList()));
            }

            var resortCodesGroupedByCityCode = resortSplits
                    .Where(rs => rs.ResortCode.Contains("|"))
                    .ToLookup(rs => rs.ResortCode.Split('|')[0], rs => rs.ResortCode.Split('|')[1]);

            foreach (var resortCodeGroup in resortCodesGroupedByCityCode)
            {
                string cityCode = resortCodeGroup.Key;
                string regionIdToUse;
                IEnumerable<string> hotelIDs;

                if (resortCodeGroup.Count() > 1)
                {
                    regionIdToUse = cityCode;
                    hotelIDs = from rs in resortSplits.Where(rs => (rs.ResortCode.Split('|')[0] ?? "") == (regionIdToUse ?? ""))
                               from hotel in rs.Hotels
                               select hotel.TPKey;
                }
                else
                {
                    regionIdToUse = resortCodeGroup.Single();
                    hotelIDs = resortSplits.Single(rs => (rs.ResortCode ?? "") == ($"{cityCode}|{regionIdToUse}" ?? "")).Hotels.Select(h => h.TPKey);
                }

                requestBodies.AddRange(BuildRequests(searchDetails, regionIdToUse, hotelIDs.ToList()));
            }

            foreach (string requestBody in requestBodies)
            {
                var request = new Request
                {
                    EndPoint = _settings.get_URL(searchDetails),
                    Method = eRequestMethod.POST,
                    Source = ThirdParties.STUBA,
                };
                request.SetRequest(requestBody);
                requests.Add(request);
            }

            return Task.FromResult(requests);
        }

        private IEnumerable<string> BuildRequests(SearchDetails searchDetails, string resortCode, List<string> hotelIds)
        {
            int maxHotels = _settings.get_MaxHotelsPerRequest(searchDetails);

            if (maxHotels <= 0)
            {
                yield return BuildRequest(searchDetails, resortCode, hotelIds);
            }
            else
            {
                int batchCount = Math.Ceiling(hotelIds.Count / (double)maxHotels).ToSafeInt();
                
                for (int i = 0; i <= batchCount - 1; i++)
                {
                    yield return BuildRequest(searchDetails, resortCode, hotelIds.Skip(i * maxHotels).Take(maxHotels));
                }
            }
        }

        private string BuildRequest(SearchDetails searchDetails, string resortCode, IEnumerable<string> hotelIds)
        {
            string org = _settings.get_Organisation(searchDetails);
            string user = _settings.get_Username(searchDetails);
            string password = _settings.get_Password(searchDetails);
            string version = _settings.get_Version(searchDetails);
            string currencyCode = _settings.get_Currency(searchDetails);
            string nationality = _settings.get_Nationality(searchDetails);

            var request = new XElement("AvailabilitySearch",
                            new XElement("Authority",
                                new XElement("Org", org),
                                new XElement("User", user),
                                new XElement("Password", password),
                                new XElement("Currency", currencyCode),
                                new XElement("Version", version)
                            ),
                            new XElement("RegionId", resortCode),
                            new XElement("Hotels", from id in hotelIds
                                                   select new XElement("Id", id)),
                            new XElement("HotelStayDetails",
                            new XElement("ArrivalDate", searchDetails.PropertyArrivalDate.ToString("yyyy-MM-dd")),
                            new XElement("Nights", searchDetails.PropertyDuration),
                            new XElement("Nationality", nationality), from oRoom in searchDetails.RoomDetails
                                                                       select new XElement("Room",
                                                                                  new XElement("Guests", from adult in Enumerable.Range(0, oRoom.Adults)
                                                                                                         select new XElement("Adult"), from childAge in oRoom.ChildAges
                                                                                                                                       select new XElement("Child", new XAttribute("age", childAge)), from infant in Enumerable.Range(0, oRoom.Infants)
                                                                                                                                                                                                      select new XElement("Child", new XAttribute("age", "0")))))
                        );
            return request.ToString();
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedCollection = new TransformedResultCollection();

            var results = requests
                .Select(o =>
                    {
                        try
                        {
                            return _serializer.DeSerialize<StubaSearchResponse>(_serializer.CleanXmlNamespaces(o.ResponseXML));
                        }
                        catch
                        {
                            return null;
                        }
                    })
                .Where(o => o is not null)
                .ToList();

            bool removeNonRefundable = _settings.get_ExcludeNonRefundableRates(searchDetails);
            bool removeUnknownCancellations = _settings.get_ExcludeUnknownCancellationPolicys(searchDetails);

            if (removeNonRefundable)
            {
                RemoveNrfResults(results);
            }
            if (removeUnknownCancellations)
            {
                RemoveUnknownCancResults(results);
            }
            if (searchDetails.Rooms > 1)
            {
                RemoveNonCheapestRoomCombinations(results);
            }

            foreach (var stubaResult in results)
            {
                foreach (var hotelAvail in stubaResult.HotelAvailability)
                {
                    foreach (var result in hotelAvail.Result)
                    {
                        for (int index = 0; index < result.Room.Count(); index++)
                        {
                            var room = result.Room[index];

                            var transformedResult = new TransformedResult()
                            {
                                MasterID = hotelAvail.Hotel.id,
                                PropertyRoomBookingID = index + 1,
                                MealBasisCode = room.MealType.code.ToSafeString(),
                                TPKey = hotelAvail.Hotel.id.ToSafeString(),
                                CurrencyCode = stubaResult.Currency,
                                RoomType = room.RoomType.text,
                                Amount = room.Price.amt,
                                TPReference = $"{hotelAvail.hotelQuoteId}|{result.id}",
                                RoomTypeCode = room.RoomType.code.ToSafeString(),
                                NonRefundableRates = room.CancellationPolicyStatus == "NonRefundable"
                            };

                            transformedCollection.TransformedResults.Add(transformedResult);
                        }
                    }
                }
            }

            return transformedCollection;
        }

        private void RemoveUnknownCancResults(IEnumerable<StubaSearchResponse> results)
        {
            RemoveResultsByCancellationPolicy(results, "Unknown");
        }

        private void RemoveNrfResults(IEnumerable<StubaSearchResponse> results)
        {
            RemoveResultsByCancellationPolicy(results, "NonRefundable");
        }

        private void RemoveResultsByCancellationPolicy(IEnumerable<StubaSearchResponse> results, string policyStatus)
        {
            foreach (StubaSearchResponse response in results)
            {
                foreach (HotelAvailability hotelAvail in response.HotelAvailability)
                {
                    hotelAvail.Result = hotelAvail.Result
                        .Where(o => o.Room.Any(r => (r.CancellationPolicyStatus.ToLower() ?? "") == (policyStatus.ToLower() ?? "")))
                        .ToList();
                }
            }
        }

        private void RemoveNonCheapestRoomCombinations(IEnumerable<StubaSearchResponse> results)
        {
            foreach (HotelAvailability hotelAvail in results.SelectMany(o => o.HotelAvailability.Where(r => r.Result.Count > 0)))
            {
                var cheapestResults = new List<Result>();
                cheapestResults.Add(hotelAvail.Result.OrderBy(o => o.Room.Sum(r => r.Price.amt)).FirstOrDefault());
                hotelAvail.Result = cheapestResults;
            }
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }
    }
}