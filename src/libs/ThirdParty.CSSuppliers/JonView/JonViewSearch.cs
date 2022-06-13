namespace ThirdParty.CSSuppliers.JonView
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

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

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            // build request xml for each resort
            foreach (var resort in resortSplits)
            {
                string cityCode = resort.ResortCode;

                // Build request url
                string url = BuildSearchURL(searchDetails, cityCode);

                var request = new Request
                {
                    EndPoint = _settings.get_URL(searchDetails) + url,
                    Method = eRequestMethod.POST,
                    Source = Source,
                    ExtraInfo = searchDetails,
                    UseGZip = true
                };

                requests.Add(request);
            }

            return requests;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var jonviewSearchResponses = new List<JonViewSearchResponse>();
            
            foreach (var request in requests)
            {
                var searchResponse = new JonViewSearchResponse();
                bool success = request.Success;

                if (success)
                {
                    searchResponse = _serializer.DeSerialize<JonViewSearchResponse>(request.ResponseString);

                    if (searchResponse.Response is not null)
                    {
                        jonviewSearchResponses.Add(searchResponse);
                    }
                }
            }

            transformedResults.TransformedResults
                .AddRange(jonviewSearchResponses
                    .Where(r => r.Response.Rooms.Count > 0)
                    .SelectMany(x => GetResultFromResponse(x)));

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

        private string BuildSearchURL(SearchDetails searchDetails, string cityCode)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("?actioncode=HOSTXML&clientlocseq={0}&userid={1}&" + "password={2}&message=<?xml version=\"1.0\" encoding=\"UTF-8\"?>", _settings.get_ClientLoc(searchDetails), _settings.get_UserID(searchDetails), _settings.get_Password(searchDetails));

            sb.AppendFormat("<message><actionseg>CT</actionseg><searchseg><prodtypecode>FIT</prodtypecode>" + "<searchtype>CITY</searchtype>");
            sb.AppendFormat("<citycode>{0}</citycode>", cityCode);
            sb.AppendFormat("<startdate>{0}</startdate>", searchDetails.PropertyArrivalDate.ToString("dd-MMM-yyyy"));
            sb.AppendFormat("<duration>{0}</duration>", searchDetails.PropertyDuration);
            sb.AppendFormat("<status>AVAILABLE</status>");
            sb.AppendFormat("<displayname>Y</displayname>");
            sb.AppendFormat("<displaynamedetails>Y</displaynamedetails>");
            sb.AppendFormat("<displayroomconf>Y</displayroomconf>");
            sb.AppendFormat("<displayprice>Y</displayprice>");
            sb.AppendFormat("<displaysuppliercd>Y</displaysuppliercd>");
            sb.AppendFormat("<displayavail>Y</displayavail>");
            sb.AppendFormat("<displaypolicy>Y</displaypolicy>");
            sb.AppendFormat("<displayrestriction>Y</displayrestriction>");
            sb.AppendFormat("<displaydynamicrates>Y</displaydynamicrates>");

            var room = searchDetails.RoomDetails[0];
            var sbChildAges = new StringBuilder();
            sbChildAges.Append(room.ChildAgeCSV.Replace(",", "/"));

            for (int i = 1; i <= room.Infants; i++)
            {
                sbChildAges.Append("/1");
            }

            string childAges = sbChildAges.ToString();
            if (!string.IsNullOrEmpty(childAges) && childAges.Substring(0, 1) == "/")
            {
                childAges = childAges.Substring(1, childAges.Length - 1);
            }

            sb.AppendFormat("<adults>{0}</adults>", room.Adults);
            sb.AppendFormat("<children>{0}</children>", room.Children + room.Infants);
            sb.AppendFormat("<childrenage>{0}</childrenage>", childAges);
            sb.AppendFormat("<displaypolicy>Y</displaypolicy>");
            sb.AppendFormat("</searchseg>");
            sb.AppendFormat("</message>");

            // Add the request body

            return sb.ToSafeString();
        }

        private List<TransformedResult> GetResultFromResponse(JonViewSearchResponse response)
        {
            var transformedResults = new List<TransformedResult>();

            foreach (var room in response.Response.Rooms)
            {
                var transformedResult = new TransformedResult
                {
                    TPKey = room.suppliercode,
                    CurrencyCode = room.currencycode,
                    RoomTypeCode = GetRoomType(room.productname),
                    MealBasisCode = "RO",
                    Amount = GetPrice(room.dayprice),
                    PropertyRoomBookingID = 1,
                    TPReference = room.prodcode + "_" + room.dayprice,
                    NonRefundableRates = room.cancellationPolicy.item.fromdays.Equals("999"),
                    RoomType = room.roomDetails.roomtype
                };

                transformedResults.Add(transformedResult);
            }

            return transformedResults;
        }

        private string GetRoomType(string productName)
        {
            if (productName.Split('-').Length > 1)
            {
                return productName.Split('-')[productName.Split('-').Length - 1].Trim();
            }
            else
            {
                return "Standard Room";
            }
        }

        private decimal GetPrice(string dayPrice)
        {
            decimal price = 0m;
            foreach (string priceString in dayPrice.Split('/'))
            {
                price += priceString.ToSafeDecimal();
            }

            return price;
        }

        #endregion
    }
}