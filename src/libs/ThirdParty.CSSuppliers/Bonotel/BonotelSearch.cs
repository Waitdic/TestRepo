namespace ThirdParty.CSSuppliers.Bonotel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using iVector.Search.Property;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    public class BonotelSearch : IThirdPartySearch, ISingleSource
    {
        #region Properties

        private readonly IBonotelSettings _settings;

        private readonly ISerializer _serializer;

        public string Source => ThirdParties.BONOTEL;

        #endregion

        #region Constructors

        public BonotelSearch(IBonotelSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            bool restrictions = false;

            if (searchDetails.Duration > 30)
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

            foreach (var resortSplit in resortSplits)
            {
                // Dim oFinalResults As New Results
                var sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<availabilityRequest>");
                sb.Append("<control>");
                sb.AppendFormat("<userName>{0}</userName>", _settings.get_Username(searchDetails));
                sb.AppendFormat("<passWord>{0}</passWord>", _settings.get_Password(searchDetails));
                sb.Append("</control>");
                sb.AppendFormat("<checkIn>{0}</checkIn>", searchDetails.PropertyArrivalDate.ToString("dd-MMM-yyyy"));
                sb.AppendFormat("<checkOut>{0}</checkOut>", searchDetails.PropertyDepartureDate.ToString("dd-MMM-yyyy"));
                sb.AppendFormat("<noOfRooms>{0}</noOfRooms>", searchDetails.Rooms);
                sb.AppendFormat("<noOfNights>{0}</noOfNights>", searchDetails.PropertyDuration);
                sb.AppendFormat("<city>{0}</city>", resortSplit.ResortCode);
                sb.AppendFormat("<hotelCodes>");
                if (resortSplit.Hotels.Count == 1)
                {
                    sb.AppendFormat("<hotelCode>{0}</hotelCode>", resortSplit.Hotels[0].TPKey);
                }
                else
                {
                    sb.AppendFormat("<hotelCode>0</hotelCode>");
                }
                sb.AppendFormat("</hotelCodes>");
                sb.AppendFormat("<roomsInformation>");

                foreach (var room in searchDetails.RoomDetails)
                {
                    sb.AppendFormat("<roomInfo>");
                    sb.AppendFormat("<roomTypeId>0</roomTypeId>");
                    sb.AppendFormat("<bedTypeId>0</bedTypeId> ");
                    sb.AppendFormat("<adultsNum>{0}</adultsNum>", room.Adults);
                    sb.AppendFormat("<childNum>{0}</childNum>", room.Children + room.Infants);

                    if (room.Children + room.Infants > 0)
                    {
                        sb.AppendFormat("<childAges>");
                        foreach (int iChildAge in room.ChildAndInfantAges())
                            sb.AppendFormat("<childAge>{0}</childAge>", iChildAge);
                        sb.AppendFormat("</childAges>");
                    }

                    sb.AppendFormat("</roomInfo>");
                }

                sb.AppendFormat("</roomsInformation>");
                sb.Append("</availabilityRequest>");

                var request = new Request
                {
                    EndPoint = _settings.get_URL(searchDetails) + "GetAvailability.do",
                    ContentType = ContentTypes.Text_xml,
                    Method = eRequestMethod.POST,
                    ExtraInfo = searchDetails
                };

                request.SetRequest(sb.ToString());

                requests.Add(request);
            }

            return requests;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var responses = new List<AvailabilityResponse>();
            var transformedResults = new TransformedResultCollection();

            var results = requests.Select(o => _serializer.DeSerialize<AvailabilityResponse>(o.ResponseXML));
            var hotels = results.Where(o=> o.status == "Y").SelectMany(o => o.hotelList.hotel).ToList();

            transformedResults.TransformedResults.AddRange(hotels.SelectMany(o => CreateTransformedResponse(o)));

            return transformedResults;
        }

        public List<TransformedResult> CreateTransformedResponse(Hotel hotel)
        {
            var results = new List<TransformedResult>();

            decimal totalTax = 0;

            foreach (var room in hotel.roomInformation)
            {
                foreach (var tax in room.rateInformation.taxInformation)
                {
                    totalTax += tax.taxAmount;
                }
            }

            foreach (var room in hotel.roomInformation)
            {
                var result = new TransformedResult
                {
                    TPKey = hotel.hotelCode.ToSafeString(),
                    CurrencyCode = hotel.rateCurrencyCode,
                    PropertyRoomBookingID = room.roomNo.ToSafeInt(),
                    RoomType = room.roomType,
                    MealBasisCode = room.rateInformation.ratePlanCode,
                    Amount = room.rateInformation.totalRate,
                    TPReference = $"{room.roomCode}|{room.roomTypeCode}|{room.bedTypeCode}|{hotel.rateCurrencyCode}|{room.rateInformation.ratePlanCode}|{totalTax}"
                };

                results.Add(result);
            }

            return results;
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