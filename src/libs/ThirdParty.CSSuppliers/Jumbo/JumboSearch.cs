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
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.Jumbo.Models;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    public class JumboSearch : IThirdPartySearch, ISingleSource
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

        public bool SupportsNonRefundableTagging => false;

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            bool restrictions = false;

            // Multi rooms was implemented however did not function correctly, Jumbo returns room combinations rather than available rooms
            if (searchDetails.Rooms > 1)
            {
                restrictions = true;
            }

            return restrictions;
        }

        #endregion

        #region SearchFunctions

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            string hotelId;
            string url = _settings.get_HotelBookingURL(searchDetails);
            var sb = new StringBuilder();

            sb.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:typ=\"http://xtravelsystem.com/v1_0rc1/hotel/types\">");
            sb.Append("<soapenv:Header/>");
            sb.Append("<soapenv:Body>");

            sb.Append("<typ:availableHotelsByMultiQueryV12>");

            sb.Append("<AvailableHotelsByMultiQueryRQ_1>");
            sb.AppendFormat("<agencyCode>{0}</agencyCode>", Jumbo.GetCredentials(searchDetails, searchDetails.LeadGuestNationalityID, "AgencyCode", _settings));
            sb.AppendFormat("<brandCode>{0}</brandCode>", Jumbo.GetCredentials(searchDetails, searchDetails.LeadGuestNationalityID, "BrandCode", _settings));
            sb.AppendFormat("<pointOfSaleId>{0}</pointOfSaleId>", Jumbo.GetCredentials(searchDetails, searchDetails.LeadGuestNationalityID, "POS", _settings));
            sb.AppendFormat("<checkin>{0}</checkin>", searchDetails.PropertyArrivalDate.ToString("yyyy-MM-ddThh:mm:ss"));
            sb.AppendFormat("<checkout>{0}</checkout>", searchDetails.PropertyDepartureDate.ToString("yyyy-MM-ddThh:mm:ss"));
            sb.AppendFormat("<fromPrice>{0}</fromPrice>", "0");
            sb.AppendFormat("<fromRow>{0}</fromRow>", "0");
            sb.AppendFormat("<includeEstablishmentData>{0}</includeEstablishmentData>", "false");
            sb.AppendFormat("<language>{0}</language>", _settings.get_Language(searchDetails));
            sb.AppendFormat("<maxRoomCombinationsPerEstablishment>{0}</maxRoomCombinationsPerEstablishment>", "10");
            sb.AppendFormat("<numRows>{0}</numRows>", "1000");

            foreach (var room in searchDetails.RoomDetails)
            {
                sb.Append("<occupancies>");
                sb.AppendFormat("<adults>{0}</adults>", room.Adults);
                sb.AppendFormat("<children>{0}</children>", room.Children + room.Infants);

                foreach (int childAge in room.ChildAndInfantAges())
                {
                    sb.AppendFormat("<childrenAges>{0}</childrenAges>", childAge);
                }

                sb.AppendFormat("<numberOfRooms>{0}</numberOfRooms>", "1");
                sb.Append("</occupancies>");
            }

            sb.AppendFormat("<onlyOnline>{0}</onlyOnline>", "true");
            sb.Append("<orderBy xsi:nil=\"true\" />");
            sb.Append("<productCode xsi:nil=\"true\" />");
            sb.AppendFormat("<toPrice>{0}</toPrice>", "9999");

            foreach (var resortSplit in resortSplits)
            {
                string resortCode = resortSplit.ResortCode;

                if (resortSplit.Hotels.Count == 1 & resortSplits.Count == 1)
                {
                    hotelId = resortSplit.Hotels[0].TPKey.ToString();
                }
                else
                {
                    hotelId = "0";
                }

                if (hotelId.ToString() != "0" && !string.IsNullOrEmpty(hotelId.ToString()))
                {
                    sb.AppendFormat("<establishmentId>{0}</establishmentId>", hotelId.ToString());
                }
                else
                {
                    sb.AppendFormat("<areaCode>{0}</areaCode>", resortCode.ToString());
                }

            }

            sb.Append("</AvailableHotelsByMultiQueryRQ_1>");

            sb.Append("</typ:availableHotelsByMultiQueryV12>");
            sb.Append("</soapenv:Body>");
            sb.Append("</soapenv:Envelope>");

            var request = new Request
            {
                EndPoint = url,
                Method = eRequestMethod.POST,
                ExtraInfo = searchDetails
            };

            request.SetRequest(sb.ToString());
            requests.Add(request);

            return requests;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            string responseBody = _serializer.CleanXmlNamespaces(requests[0].ResponseXML).InnerXml.ToString();

            var response = new XmlDocument();

            if (!responseBody.Contains("<faultcode>"))
            {
                var results = _serializer.DeSerialize<Envelope>(responseBody);

                transformedResults.TransformedResults.AddRange(results.Body.Content.result.availableHotels.SelectMany(o => CreateHotelResults(o)));
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

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion
    }
}