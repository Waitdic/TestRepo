namespace iVectorOne.Suppliers.JonView
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Suppliers.JonView.Models;
    using iVectorOne.Models.Property.Booking;
    using System;
    using System.Text.RegularExpressions;

    public class JonViewSearch : IThirdPartySearch, ISingleSource
    {
        #region Properties

        private readonly IJonViewSettings _settings;
        private readonly ISerializer _serializer;

        public string Source => ThirdParties.JONVIEW;

        public JonViewSearch(IJonViewSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            bool restrictions = false;

            if (searchDetails.RoomDetails.Count > 1)
            {
                int adults = searchDetails.RoomDetails[0].Adults;
                int children = searchDetails.RoomDetails[0].Children;
                int infants = searchDetails.RoomDetails[0].Infants;
                string childAgesCsv = searchDetails.RoomDetails[0].ChildAgeCSV;

                foreach (var roomDetails in searchDetails.RoomDetails)
                {
                    if (!(roomDetails.Adults == adults &&
                        roomDetails.Children == children &&
                        roomDetails.Infants == infants &&
                        (roomDetails.ChildAgeCSV ?? "") == (childAgesCsv ?? "")))
                    {
                        restrictions = true;
                    }
                }
            }

            return restrictions;
        }

        #endregion

        #region SearchFunctions

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            // build request xml for each resort
            foreach (var resort in resortSplits)
            {
                string cityCode = resort.ResortCode;

                // Build request url
                string xmlBody = BuildSearchRequest(searchDetails, cityCode);

                var request = new Request
                {
                    EndPoint = _settings.GenericURL(searchDetails),
                    SoapAction = Constant.RequestSoapAction,
                    Method = RequestMethod.POST,
                    Source = Source,
                    ExtraInfo = string.Join(",", resort.Hotels.Select(x=>x.TPKey)),
                    UseGZip = true
                };
                request.SetRequest(xmlBody);
                requests.Add(request);
            }

            return Task.FromResult(requests);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var jonviewSearchResponses = new List<SearchResponse>();
          
           
            foreach (var request in requests)
            {
                bool success = request.Success;
                string[] TPKeys = (request.ExtraInfo as string).Split(',');

                if (success)
                {
                    var message = JonView.ExtractEnvelopeContent<SearchResponse>(request, _serializer);

                    if (message.AlternateList != null)
                    {
                        //increasing performance, transform requested hotels only
                        var roomRecords = TPKeys.Any()
                            ? message.AlternateList.RoomRecords.Where(x => TPKeys.Contains(x.SupplierCode)).ToList()
                            : message.AlternateList.RoomRecords;

                        var transformedRooms = roomRecords.SelectMany(room =>
                        {
                            decimal localCost = room.DayPrice.Split('/').Sum(x => x.ToSafeDecimal());

                            var cancellations = room.CancellationPolicy.Select(cancelItem =>
                                    GetCancellation(room, cancelItem, searchDetails.ArrivalDate,
                                        searchDetails.TotalAdults + searchDetails.TotalChildren)).ToList();

                            var cancellationSet = new Cancellations();
                            cancellationSet.AddRange(cancellations);
                            cancellationSet.Solidify(SolidifyType.Max, new DateTime(2009, 12, 31), localCost);

                            string roomTypeCode = room.ProductName.Contains("-")
                                                    ? room.ProductName.Split('-').Last().Trim()
                                                    : "Standard Room";

                            bool nfr = room.CancellationPolicy.All(c => string.Equals(c.FromDays, "999"));

                            return Enumerable.Range(1, searchDetails.Rooms).Select(prbid => new TransformedResult
                            {
                                TPKey = room.SupplierCode,
                                CurrencyCode = room.CurrencyCode,
                                RoomTypeCode = roomTypeCode,
                                MealBasisCode = room.ProductDetails.Board,
                                Amount = localCost,
                                PropertyRoomBookingID = prbid,
                                TPReference = room.ProdCode,
                                NonRefundableRates = nfr,
                                RoomType = room.ProductDetails.RoomType,
                                Cancellations = cancellationSet
                            });
                        });
                        transformedResults.TransformedResults.AddRange(transformedRooms);
                    }
                }
            }

            return transformedResults;
        }

        #endregion

        #region ResponseHasExceptions

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion

        #region Helper classes

        private string BuildSearchRequest(SearchDetails searchDetails, string cityCode) 
        {
            var room = searchDetails.RoomDetails[0];

            var availRQ = new SearchRequest 
            {
                ActionSeg = "CT",
                SearchSeg = 
                {
                    ProdTypeCode = "FIT",
                    SearchType = "CITY",
                    CityCode = cityCode,
                    StartDate = searchDetails.ArrivalDate.ToString(Constant.DateFormat),
                    Duration = searchDetails.Duration,
                    Status = "AVAILABLE",
                    DisplayName = "Y",
                    DisplayNameDetails = "Y",
                    DisplayRoomConf = "Y",
                    DisplayPrice = "Y",
                    DisplaySupplierCd = "Y",
                    DisplayAvail = "Y",
                    DisplayPolicy = "Y",
                    DisplayRestriction = "Y",
                    DisplayDynamicRates = "Y",
                    Adults = room.Adults,
                    Children = room.Children + room.Infants,
                    ChildrenAge = string.Join("/", room.ChildAges.Concat(Enumerable.Repeat(1, room.Infants))),
                    Rooms = searchDetails.Rooms
                }
            };

            var message = JonView.CreateSoapRequest(_serializer, searchDetails, _settings, availRQ);

            return message.OuterXml;
        }

        private static Cancellation GetCancellation(RoomRecord room, CancelicyItem cancelItem, DateTime arrivalDate, int personCount) 
        {
            var fromDaysBeforeArrival = cancelItem.FromDays.ToSafeInt();
            var toDaysBeforeArrival = cancelItem.ToDays.ToSafeInt();

            var startDate = toDaysBeforeArrival < 0
                ? arrivalDate
                : arrivalDate.AddDays(-fromDaysBeforeArrival);

            var endDate = toDaysBeforeArrival < 0
                ? arrivalDate
                : arrivalDate.AddDays(-toDaysBeforeArrival);

            decimal[] dayPrice = room.DayPrice.Split('/').Select(p => p.ToSafeDecimal()).ToArray();
            decimal localCost = dayPrice.Sum();

            List<decimal> baseAmounts = new();

            switch (cancelItem.ChargeType) 
            {
                case ChargeType.EntireItem:
                    baseAmounts = new List<decimal> { localCost };
                    break;
                case ChargeType.Daily:
                    var match = Regex.Match(cancelItem.CanNote, @"on first (\d+) day\(s\) Price");
                    int numberOfDays = match.Groups[1].Value.ToSafeInt();
                    baseAmounts = dayPrice.Take(numberOfDays).ToList();
                    break;
                case ChargeType.PerPerson:
                case ChargeType.NotAvailable:
                    baseAmounts = Enumerable.Repeat(localCost / personCount, personCount).ToList();
                    break;
            }

            var canRate = cancelItem.Canrate.ToSafeDecimal();

            var finalAmountForThisRule = baseAmounts.Sum(amount =>
            {
                return string.Equals(cancelItem.RateType, RateType.Percentage)
                    ? amount * (canRate / 100.0M)
                    : amount;
            });

            var cancellation =  new Cancellation
            {
                StartDate = startDate,
                EndDate = endDate,
                Amount = finalAmountForThisRule
            };

            return cancellation;
        }

        #endregion
    }
}