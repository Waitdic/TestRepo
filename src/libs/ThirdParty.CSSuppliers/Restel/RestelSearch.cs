namespace ThirdParty.CSSuppliers.Restel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Security;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using Models;
    using ThirdParty.Constants;
    using ThirdParty.Results;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;
    using static RestelCommon;

    public class RestelSearch : ThirdPartyPropertySearchBase
    {
        private readonly IRestelSettings _settings;
        private readonly ISerializer _serializer;
        private readonly ISecretKeeper _secretKeeper;

        public RestelSearch(
            IRestelSettings settings,
            ISerializer serializer,
            ISecretKeeper secretKeeper,
            ILogger<RestelSearch> logger)
            : base(logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
        }

        public override string Source => ThirdParties.RESTEL;

        public override bool SqlRequest => false;

        public override List<Request> BuildSearchRequests(SearchDetails searchDetails,  List<ResortSplit> resortSplits, bool saveLogs)
        {
            var requests = new List<Request>();

            string codusu = _settings.Codusu(searchDetails);

            foreach (var resortSplit in resortSplits)
            {
                var xmlRequest = CreateAvailabilityRequestXml(
                    resortSplit.ResortCode,
                    searchDetails.PropertyDuration,
                    searchDetails.ArrivalDate,
                    searchDetails.DepartureDate,
                    codusu,
                    searchDetails.RoomDetails.Select(roomDetail => $"{roomDetail.Adults}-{roomDetail.Children}"),
                    _serializer);

                // set a unique code. if the is one request we only need the source name
                string uniqueCode = Source;
                if (resortSplits.Count > 1)
                    uniqueCode = $"{Source}_{resortSplit.ResortCode}";

                // Build the Request Object
                var oRequest = CreateRequest(_settings, searchDetails, "Search");
                oRequest.CreateLog = saveLogs;
                oRequest.TimeoutInSeconds = RequestTimeOutSeconds(searchDetails);
                oRequest.ExtraInfo = new SearchExtraHelper(searchDetails, uniqueCode);
                oRequest.UseGZip = true;

                oRequest.SetRequest(xmlRequest);

                requests.Add(oRequest);
            }

            return requests;
        }

        public override TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var allResponses = requests
                .Where(r => r.Success)
                .Select(r =>_serializer.DeSerialize<RestelAvailabilityResponse>(_serializer.CleanXmlNamespaces(r.ResponseXML)))
                .ToList();

            // Find Max Child Age
            int maxChildAge = searchDetails.RoomDetails.SelectMany(oRoom => oRoom.ChildAges).Prepend(0).Max();

            int iRoomNumber = 1;
            var rooms = new Dictionary<string, int>(searchDetails.RoomDetails.Count);
            foreach (var roomBooking in searchDetails.RoomDetails)
            {
                string combo = $"{roomBooking.Adults}-{roomBooking.Children}";
                if (!rooms.ContainsKey(combo))
                {
                    rooms.Add(combo, iRoomNumber);
                }

                ++iRoomNumber;
            }

            transformedResults.TransformedResults.AddRange(allResponses.SelectMany(r => GetResultFromResponse(r, maxChildAge, rooms)));

            return transformedResults;
        }

        public List<TransformedResult> GetResultFromResponse(RestelAvailabilityResponse response, int maxChildAge, Dictionary<string, int> rooms)
        {
            var transformedResults = new List<TransformedResult>();

            // add response to collection; only do if any hotels in response
            if (response.Param.Hotls.Num == 0)
            {
                return transformedResults;
            }

            transformedResults.AddRange(from hot in response.Param.Hotls.Hot.Where(hot => hot.Enh.ToSafeInt() > maxChildAge)
            from pax in hot.Res.Pax
            from hab in pax.Hab
            from reg in hab.Reg.Where(reg => reg.Esr == "OK")
            let tpr = new RestelThirdPartyReference
            {
                RoomCode = hab.Cod,
                MealBasis = reg.Cod,
                ThirdPartyReferences = reg.Lin.ToList(),
                ResortCode = hot.Pro
            }
            select new TransformedResult
            {
                TPKey = hot.Cod,
                CurrencyCode = reg.Div,
                PropertyRoomBookingID = rooms.TryGetValue(pax.Cod, out int id)? id : default,
                RoomType = hab.Desc,
                MealBasisCode = reg.Cod,
                Amount = reg.Prr,
                TPReference = tpr.ToEncryptedString(_secretKeeper),
                NonRefundableRates = reg.Nr != "0"
            });

            return transformedResults;
        }

        public override bool SearchRestrictions(SearchDetails searchDetails)
        {
            // doesn't do more than 3 rooms
            if (searchDetails.Rooms > 3)
            {
                return true;
            }

            // no more than 15 nights
            if (searchDetails.PropertyDuration > 15)
            {
                return true;
            }

            if (searchDetails.PropertyArrivalDate < DateTime.Now.AddDays(2d))
            {
                return true;
            }

            // can only do multi room if pax combos are different;
            // if two rooms have the same combination of adults-children then it will only be possible to book the same type of room
            if (searchDetails.Rooms != 1)
            {
                var list = new List<string>();
                foreach (var room in searchDetails.RoomDetails)
                {
                    var pax = new StringBuilder();
                    pax.AppendFormat("{0}-{1}", room.Adults, room.Children);
                    if (!list.Contains(pax.ToString()))
                    {
                        list.Add(pax.ToString());
                    }
                    else
                    {
                        // if room combo exists, don't bother searching
                        return true;
                    }
                }
            }

            // Requests with infants can only be done over the phone so no infants
            if (searchDetails.TotalInfants > 0)
            {
                return true;
            }

            if (searchDetails.TotalChildren > 2)
            {
                return true;
            }

            // Maximum of 2 children and 7 pax in total, unless it is 8 adults, no children
            int totalAdults = searchDetails.TotalAdults;
            int totalChildrenAndInfants = searchDetails.TotalChildren + searchDetails.TotalInfants;
            if ((totalAdults + totalChildrenAndInfants > 7 | totalChildrenAndInfants > 2) & (totalAdults != 8 | totalChildrenAndInfants != 0))
            {
                return true;
            }

            return false;
        }

        public override bool ResponseHasExceptions(Request request)
        {
            return false;
        }
    }
}
