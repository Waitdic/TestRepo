namespace ThirdParty.CSSuppliers.JonView
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using iVector.Search.Property;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    public class JonViewSearch : IThirdPartySearch
    {

        #region Properties

        private readonly IJonViewSettings _settings;

        public string Source => ThirdParties.JONVIEW;

        public JonViewSearch(IJonViewSettings settings)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
        }

        public bool SupportsNonRefundableTagging { get; } = false;

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails oSearchDetails)
        {

            bool bRestrictions = false;

            if (oSearchDetails.RoomDetails.Count > 1)
            {

                int iAdults = oSearchDetails.RoomDetails[0].Adults;
                int iChildren = oSearchDetails.RoomDetails[0].Children;
                int iInfants = oSearchDetails.RoomDetails[0].Infants;
                string sChildAgesCSV = oSearchDetails.RoomDetails[0].ChildAgeCSV;

                foreach (RoomDetail oRoomDetails in oSearchDetails.RoomDetails)
                {
                    if (!(oRoomDetails.Adults == iAdults && oRoomDetails.Children == iChildren && oRoomDetails.Infants == iInfants && (oRoomDetails.ChildAgeCSV ?? "") == (sChildAgesCSV ?? "")))
                    {
                        bRestrictions = true;
                    }
                }


            }

            return bRestrictions;

        }

        #endregion

        #region SearchFunctions

        public List<Request> BuildSearchRequests(SearchDetails oSearchDetails, List<ResortSplit> oResortSplits)
        {
            var oRequests = new List<Request>();

            // build request xml for each resort
            foreach (ResortSplit oResort in oResortSplits)
            {

                string sCityCode = oResort.ResortCode;

                // Build request url
                string url = BuildSearchURL(oSearchDetails, sCityCode);

                var oRequest = new Request
                {
                    EndPoint = _settings.get_URL(oSearchDetails) + url,
                    Method = eRequestMethod.POST,
                    Source = Source,
                    ExtraInfo = oSearchDetails,
                    UseGZip = true
                };

                oRequests.Add(oRequest);

            }

            return oRequests;

        }

        public TransformedResultCollection TransformResponse(List<Request> oRequests, SearchDetails oSearchDetails, List<ResortSplit> oResortSplits)
        {

            var transformedResults = new TransformedResultCollection();
            var jonviewSearchResponses = new List<JonViewSearchResponse>();
            var serializer = new XmlSerializer(typeof(JonViewSearchResponse));

            foreach (Request request in oRequests)
            {

                var searchResponse = new JonViewSearchResponse();
                bool success = request.Success;

                if (success)
                {
                    using (TextReader reader = new StringReader(request.ResponseString))
                    {
                        searchResponse = (JonViewSearchResponse)serializer.Deserialize(reader);
                    }
                    if (searchResponse.Response is not null)
                    {
                        jonviewSearchResponses.Add(searchResponse);
                    }
                }

            }

            transformedResults.TransformedResults.AddRange(jonviewSearchResponses.Where(r => r.Response.Rooms.Count > 0).SelectMany(x => GetResultFromResponse(x)));

            return transformedResults;

        }

        #endregion

        #region ResponseHasExceptions

        public bool ResponseHasExceptions(Request oRequest)
        {
            return false;
        }

        #endregion

        #region Helper classes

        private string BuildSearchURL(SearchDetails searchDetails, string cityCode)
        {

            var url = new StringBuilder();

            url.AppendFormat("?actioncode=HOSTXML&clientlocseq={0}&userid={1}&" + "password={2}&message=<?xml version=\"1.0\" encoding=\"UTF-8\"?>", _settings.get_ClientLoc(searchDetails), _settings.get_UserID(searchDetails), _settings.get_Password(searchDetails));

            url.AppendFormat("<message><actionseg>CT</actionseg><searchseg><prodtypecode>FIT</prodtypecode>" + "<searchtype>CITY</searchtype>");
            url.AppendFormat("<citycode>{0}</citycode>", cityCode);
            url.AppendFormat("<startdate>{0}</startdate>", searchDetails.PropertyArrivalDate.ToString("dd-MMM-yyyy"));
            url.AppendFormat("<duration>{0}</duration>", searchDetails.PropertyDuration);
            url.AppendFormat("<status>AVAILABLE</status>");
            url.AppendFormat("<displayname>Y</displayname>");
            url.AppendFormat("<displaynamedetails>Y</displaynamedetails>");
            url.AppendFormat("<displayroomconf>Y</displayroomconf>");
            url.AppendFormat("<displayprice>Y</displayprice>");
            url.AppendFormat("<displaysuppliercd>Y</displaysuppliercd>");
            url.AppendFormat("<displayavail>Y</displayavail>");
            url.AppendFormat("<displaypolicy>Y</displaypolicy>");
            url.AppendFormat("<displayrestriction>Y</displayrestriction>");
            url.AppendFormat("<displaydynamicrates>Y</displaydynamicrates>");

            var room = searchDetails.RoomDetails[0];
            var childAges = new StringBuilder();
            childAges.Append(room.ChildAgeCSV.Replace(",", "/"));
            for (int i = 1, loopTo = room.Infants; i <= loopTo; i++)
                childAges.Append("/1");
            string sChildAges = childAges.ToString();
            if (!string.IsNullOrEmpty(sChildAges) && sChildAges.Substring(0, 1) == "/")
            {
                sChildAges = sChildAges.Substring(1, sChildAges.Length - 1);
            }

            url.AppendFormat("<adults>{0}</adults>", room.Adults);
            url.AppendFormat("<children>{0}</children>", room.Children + room.Infants);
            url.AppendFormat("<childrenage>{0}</childrenage>", childAges);
            url.AppendFormat("<displaypolicy>Y</displaypolicy>");
            url.AppendFormat("</searchseg>");
            url.AppendFormat("</message>");

            // Add the request body

            return url.ToSafeString();

        }

        private List<TransformedResult> GetResultFromResponse(JonViewSearchResponse response)
        {
            var transformedResults = new List<TransformedResult>();

            foreach (JonViewSearchResponse.Room room in response.Response.Rooms)
            {
                var transformedResult = new TransformedResult();
                transformedResult.TPKey = room.suppliercode;
                transformedResult.CurrencyCode = room.currencycode;
                transformedResult.RoomTypeCode = GetRoomType(room.productname);
                transformedResult.MealBasisCode = "RO";
                transformedResult.Amount = GetPrice(room.dayprice);
                transformedResult.PropertyRoomBookingID = 1;
                transformedResult.TPReference = room.prodcode + "_" + room.dayprice;
                transformedResult.NonRefundableRates = room.cancellationPolicy.item.fromdays.Equals("999");
                transformedResult.RoomType = room.roomDetails.roomtype;

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
            foreach (string sPrice in dayPrice.Split('/'))
                price += sPrice.ToSafeDecimal();

            return price;
        }

        #endregion

    }
}