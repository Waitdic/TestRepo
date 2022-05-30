namespace ThirdParty.CSSuppliers.Stuba
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;

    public class StubaSearch : ThirdPartyPropertySearchBase
    {

        private readonly IStubaSettings _settings;

        private readonly ITPSupport _support;

        private readonly ISerializer _serializer;

        public override string Source { get; } = ThirdParties.STUBA;

        public override bool SqlRequest { get; } = false;

        public StubaSearch(IStubaSettings settings, ITPSupport support, ISerializer serializer, ILogger<StubaSearch> logger) : base(logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public override List<Request> BuildSearchRequests(SearchDetails oSearchDetails, List<ResortSplit> oResortSplits, bool bSaveLogs)
        {

            var oRequests = new List<Request>();

            var requestBodies = new List<string>();
            foreach (ResortSplit resortSplit in oResortSplits.Where(rs => !rs.ResortCode.Contains("|")))
                requestBodies.AddRange(BuildRequests(oSearchDetails, resortSplit.ResortCode, resortSplit.Hotels.Select(h => h.TPKey).ToList()));

            var resortCodesGroupedByCityCode = oResortSplits.Where(rs => rs.ResortCode.Contains("|")).ToLookup(rs => rs.ResortCode.Split('|')[0], rs => rs.ResortCode.Split('|')[1]);

            foreach (IGrouping<string, string> resortCodeGroup in resortCodesGroupedByCityCode)
            {
                string cityCode = resortCodeGroup.Key;
                string regionIdToUse;
                IEnumerable<string> hotelIDs;
                if (resortCodeGroup.Count() > 1)
                {
                    regionIdToUse = cityCode;
                    hotelIDs = from rs in oResortSplits.Where(rs => (rs.ResortCode.Split('|')[0] ?? "") == (regionIdToUse ?? ""))
                               from hotel in rs.Hotels
                               select hotel.TPKey;
                }
                else
                {
                    regionIdToUse = resortCodeGroup.Single();
                    hotelIDs = oResortSplits.Single(rs => (rs.ResortCode ?? "") == ($"{cityCode}|{regionIdToUse}" ?? "")).Hotels.Select(h => h.TPKey);
                }
                requestBodies.AddRange(BuildRequests(oSearchDetails, regionIdToUse, hotelIDs.ToList()));
            }

            foreach (string request in requestBodies)
            {
                var oRequest = new Request();
                oRequest.EndPoint = _settings.get_URL(oSearchDetails);
                oRequest.SetRequest(request);
                oRequest.Method = eRequestMethod.POST;
                oRequest.Source = ThirdParties.STUBA;
                oRequest.TimeoutInSeconds = RequestTimeOutSeconds(oSearchDetails);
                oRequest.LogFileName = "Search";
                oRequest.CreateLog = bSaveLogs;
                oRequest.ExtraInfo = new SearchExtraHelper() { SearchDetails = oSearchDetails };
                oRequest.UseGZip = true;
                oRequests.Add(oRequest);
            }
            return oRequests;
        }

        private IEnumerable<string> BuildRequests(SearchDetails oSearchDetails, string sResortCode, List<string> oHotelIDs)
        {
            int iMaxHotels = _settings.get_MaxHotelsPerRequest(oSearchDetails);
            if (iMaxHotels <= 0)
            {
                yield return BuildRequest(oSearchDetails, sResortCode, oHotelIDs);
            }
            else
            {
                int iBatchCount = Math.Ceiling(oHotelIDs.Count / (double)iMaxHotels).ToSafeInt();
                for (int i = 0, loopTo = iBatchCount - 1; i <= loopTo; i++)
                    yield return BuildRequest(oSearchDetails, sResortCode, oHotelIDs.Skip(i * iMaxHotels).Take(iMaxHotels));
            }
        }

        private string BuildRequest(SearchDetails oSearchDetails, string sResortCode, IEnumerable<string> oHotelIDs)
        {
            string sOrg = _settings.get_Organisation(oSearchDetails);
            string sUser = _settings.get_Username(oSearchDetails);
            string sPassword = _settings.get_Password(oSearchDetails);
            string sVersion = _settings.get_Version(oSearchDetails);
            string sCurrencyCode = _settings.get_Currency(oSearchDetails);
            string sNationality = _settings.get_Nationality(oSearchDetails);

            var request = new XElement("AvailabilitySearch",
                            new XElement("Authority",
                                new XElement("Org", sOrg),
                                new XElement("User", sUser),
                                new XElement("Password", sPassword),
                                new XElement("Currency", sCurrencyCode),
                                new XElement("Version", sVersion)
                            ),
                            new XElement("RegionId", sResortCode),
                            new XElement("Hotels", from id in oHotelIDs
                                                   select new XElement("Id", id)),
                            new XElement("HotelStayDetails",
                            new XElement("ArrivalDate", oSearchDetails.PropertyArrivalDate.ToString("yyyy-MM-dd")),
                            new XElement("Nights", oSearchDetails.PropertyDuration),
                            new XElement("Nationality", sNationality), from oRoom in oSearchDetails.RoomDetails
                                                                       select new XElement("Room",
                                                                                  new XElement("Guests", from adult in Enumerable.Range(0, oRoom.Adults)
                                                                                                         select new XElement("Adult"), from childAge in oRoom.ChildAges
                                                                                                                                       select new XElement("Child", new XAttribute("age", childAge)), from infant in Enumerable.Range(0, oRoom.Infants)
                                                                                                                                                                                                      select new XElement("Child", new XAttribute("age", "0")))))
                        );
            return request.ToString();
        }

        public override TransformedResultCollection TransformResponse(List<Request> oRequests, SearchDetails searchDetails, List<ResortSplit> oResortSplits)
        {

            var transformedCollection = new TransformedResultCollection();

            IEnumerable<StubaSearchResponse> results = oRequests.Select(o => _serializer.DeSerialize<StubaSearchResponse>(o.ResponseXML)).ToList();

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
            try
            {
                foreach (StubaSearchResponse stubaResult in results)
                {
                    foreach (HotelAvailability hotelAvail in stubaResult.HotelAvailability)
                    {
                        foreach (Result result in hotelAvail.Result)
                        {
                            for (int index = 0, loopTo = result.Room.Count(); index < loopTo; index++)
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
            }
            catch (Exception ex)
            {
                var test = ex.ToString();
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
                    hotelAvail.Result = hotelAvail.Result.Where(o => o.Room.Any(r => (r.CancellationPolicyStatus.ToLower() ?? "") == (policyStatus.ToLower() ?? ""))).ToList();
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

        public override bool SearchRestrictions(SearchDetails oSearchDetails)
        {
            return false;
        }

        public override bool ResponseHasExceptions(Request oRequest)
        {
            return false;
        }
    }
}