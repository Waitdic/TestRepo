namespace ThirdParty.CSSuppliers.Jumbo
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using iVector.Search.Property;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.CSSuppliers.Jumbo.Models;

    public class JumboSearch : IThirdPartySearch
    {
        #region Constructor

        private readonly IJumboSettings _settings;

        private readonly ISerializer _serializer;

        public JumboSearch(IJumboSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.JUMBO;

        public bool SupportsNonRefundableTagging { get; } = false;

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails oSearchDetails)
        {

            bool bRestrictions = false;

            // Multi rooms was implemented however did not function correctly, Jumbo returns room combinations rather than available rooms
            if (oSearchDetails.Rooms > 1)
            {
                bRestrictions = true;
            }

            return bRestrictions;

        }

        #endregion

        #region SearchFunctions

        public List<Request> BuildSearchRequests(SearchDetails oSearchDetails, List<ResortSplit> oResortSplits, bool bSaveLogs)
        {

            var oRequests = new List<Request>();

            string sHotelID = "";
            string sURL = _settings.get_HotelBookingURL(oSearchDetails);
            var sb = new StringBuilder();

            sb.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:typ=\"http://xtravelsystem.com/v1_0rc1/hotel/types\">");
            sb.Append("<soapenv:Header/>");
            sb.Append("<soapenv:Body>");

            sb.Append("<typ:availableHotelsByMultiQueryV12>");

            sb.Append("<AvailableHotelsByMultiQueryRQ_1>");
            sb.AppendFormat("<agencyCode>{0}</agencyCode>", Jumbo.GetCredentials(oSearchDetails, oSearchDetails.LeadGuestNationalityID, "AgencyCode", _settings));
            sb.AppendFormat("<brandCode>{0}</brandCode>", Jumbo.GetCredentials(oSearchDetails, oSearchDetails.LeadGuestNationalityID, "BrandCode", _settings));
            sb.AppendFormat("<pointOfSaleId>{0}</pointOfSaleId>", Jumbo.GetCredentials(oSearchDetails, oSearchDetails.LeadGuestNationalityID, "POS", _settings));
            sb.AppendFormat("<checkin>{0}</checkin>", oSearchDetails.PropertyArrivalDate.ToString("yyyy-MM-ddThh:mm:ss"));
            sb.AppendFormat("<checkout>{0}</checkout>", oSearchDetails.PropertyDepartureDate.ToString("yyyy-MM-ddThh:mm:ss"));
            sb.AppendFormat("<fromPrice>{0}</fromPrice>", "0");
            sb.AppendFormat("<fromRow>{0}</fromRow>", "0");
            sb.AppendFormat("<includeEstablishmentData>{0}</includeEstablishmentData>", "false");
            sb.AppendFormat("<language>{0}</language>", _settings.get_Language(oSearchDetails));
            sb.AppendFormat("<maxRoomCombinationsPerEstablishment>{0}</maxRoomCombinationsPerEstablishment>", "10");
            sb.AppendFormat("<numRows>{0}</numRows>", "1000");

            foreach (RoomDetail oRoom in oSearchDetails.RoomDetails)
            {
                sb.Append("<occupancies>");
                sb.AppendFormat("<adults>{0}</adults>", oRoom.Adults);
                sb.AppendFormat("<children>{0}</children>", oRoom.Children + oRoom.Infants);
                foreach (int iChildAge in oRoom.ChildAndInfantAges())
                    sb.AppendFormat("<childrenAges>{0}</childrenAges>", iChildAge);

                sb.AppendFormat("<numberOfRooms>{0}</numberOfRooms>", "1");
                sb.Append("</occupancies>");
            }

            sb.AppendFormat("<onlyOnline>{0}</onlyOnline>", "true");
            sb.Append("<orderBy xsi:nil=\"true\" />");
            sb.Append("<productCode xsi:nil=\"true\" />");
            sb.AppendFormat("<toPrice>{0}</toPrice>", "9999");

            foreach (ResortSplit oResortSplit in oResortSplits)
            {

                string sResortCode = oResortSplit.ResortCode;

                if (oResortSplit.Hotels.Count == 1 & oResortSplits.Count == 1)
                {
                    sHotelID = oResortSplit.Hotels[0].TPKey.ToString();
                }
                else
                {
                    sHotelID = "0";
                }

                if (sHotelID.ToString() != "0" && !string.IsNullOrEmpty(sHotelID.ToString()))
                {
                    sb.AppendFormat("<establishmentId>{0}</establishmentId>", sHotelID.ToString());
                }
                else
                {
                    sb.AppendFormat("<areaCode>{0}</areaCode>", sResortCode.ToString());
                }

            }

            sb.Append("</AvailableHotelsByMultiQueryRQ_1>");

            sb.Append("</typ:availableHotelsByMultiQueryV12>");
            sb.Append("</soapenv:Body>");
            sb.Append("</soapenv:Envelope>");

            var oRequest = new Request();
            oRequest.EndPoint = sURL;
            oRequest.SetRequest(sb.ToString());
            oRequest.Method = eRequestMethod.POST;
            oRequest.ExtraInfo = oSearchDetails;

            oRequests.Add(oRequest);

            return oRequests;

        }

        public TransformedResultCollection TransformResponse(List<Request> oRequests, SearchDetails oSearchDetails, List<ResortSplit> oResortSplits)
        {

            var transformedResults = new TransformedResultCollection();
            string sResponse = _serializer.CleanXmlNamespaces(oRequests[0].ResponseXML).InnerXml.ToString();

            var oResponse = new XmlDocument();

            if (!sResponse.Contains("<faultcode>"))
            {

                var oResults = _serializer.DeSerialize<Envelope>(sResponse);

                transformedResults.TransformedResults.AddRange(oResults.Body.Content.result.availableHotels.SelectMany(o => CreateHotelResults(o)));

            }

            return transformedResults;

        }

        private IEnumerable<TransformedResult> CreateHotelResults(AvailableHotel hotel)
        {
            return hotel.roomCombinations.SelectMany(o => CreateRoomResults(o, hotel));
        }

        private IEnumerable<TransformedResult> CreateRoomResults(RoomCombination room, AvailableHotel hotel)
        {
            return room.prices.Where(o => (o.roomPrices.typeCode ?? "") == (room.rooms.typeCode ?? "") && o.roomPrices.paxes == room.rooms.adults + room.rooms.children).Select(price => CreateRoomResults(price, hotel, room));
        }

        private TransformedResult CreateRoomResults(Price price, AvailableHotel hotel, RoomCombination room)
        {
            return new TransformedResult()
            {
                MasterID = hotel.establishment.id,
                TPKey = hotel.establishment.id.ToString(),
                CurrencyCode = price.amount.currencyCode,
                PropertyRoomBookingID = 1,
                RoomType = room.rooms.typeName,
                MealBasisCode = price.boardTypeCode,
                Adults = room.rooms.adults,
                Children = room.rooms.children,
                ChildAgeCSV = string.Join(",", room.rooms.childrenAges),
                Infants = room.rooms.infants,
                Amount = price.roomPrices.price,
                TPReference = $"{room.rooms.typeCode}|{price.boardTypeCode}",
                NonRefundableRates = price.roomPrices.comments.Any(o => o.type == "Cancellation term" && o.text.InList("999 - 100.00%", "365 - 100.00%"))
            };
        }

        #endregion

        #region ResponseHasExceptions

        public bool ResponseHasExceptions(Request oRequest)
        {
            return false;
        }

        #endregion

        #region Helpers

        public XmlDocument DeDupeJumboResults(XmlDocument Result)
        {

            var oAddedRooms = new List<string>();
            var sbDeDupedXml = new StringBuilder();
            var oDeDupedXml = new XmlDocument();

            // add the rooms to the collection
            foreach (XmlNode oRoomResult in Result.SelectNodes("/Results/Result"))
            {

                bool bNewRoom = true;
                string sRoomResult = oRoomResult.OuterXml;

                if (!oAddedRooms.Contains(sRoomResult))
                {
                    oAddedRooms.Add(sRoomResult);
                }

            }

            // build up the new xml
            sbDeDupedXml.Append("<Results>");

            foreach (string sRoomXmlNode in oAddedRooms)
                sbDeDupedXml.Append(sRoomXmlNode);

            sbDeDupedXml.Append("</Results>");

            // load
            oDeDupedXml.LoadXml(sbDeDupedXml.ToString());

            return oDeDupedXml;

        }

        #endregion

    }

}