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
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;


    public class BonotelSearch : ThirdPartyPropertySearchBase
    {

        #region Properties

        private readonly IBonotelSettings _settings;

        private readonly ITPSupport _support;

        private readonly ISerializer _serializer;

        public override string Source { get; } = ThirdParties.BONOTEL;

        public override bool SqlRequest { get; } = false;

        public override bool SupportsNonRefundableTagging { get; } = false;

        #endregion

        #region Constructors

        public BonotelSearch(IBonotelSettings settings, ITPSupport support, ISerializer serializer, ILogger<BonotelSearch> logger) : base(logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region SearchRestrictions

        public override bool SearchRestrictions(SearchDetails oSearchDetails)
        {

            bool bRestrictions = false;

            if (oSearchDetails.Duration > 30)
            {
                bRestrictions = true;
            }

            return bRestrictions;

        }

        #endregion

        #region SearchFunctions

        public override List<Request> BuildSearchRequests(SearchDetails oSearchDetails, List<ResortSplit> oResortSplits, bool bSaveLogs)
        {

            var oRequests = new List<Request>();

            foreach (ResortSplit oResortSplit in oResortSplits)
            {


                // Dim oFinalResults As New Results
                var sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<availabilityRequest>");
                sb.Append("<control>");
                sb.AppendFormat("<userName>{0}</userName>", _settings.get_Username(oSearchDetails));
                sb.AppendFormat("<passWord>{0}</passWord>", _settings.get_Password(oSearchDetails));
                sb.Append("</control>");
                sb.AppendFormat("<checkIn>{0}</checkIn>", oSearchDetails.PropertyArrivalDate.ToString("dd-MMM-yyyy"));
                sb.AppendFormat("<checkOut>{0}</checkOut>", oSearchDetails.PropertyDepartureDate.ToString("dd-MMM-yyyy"));
                sb.AppendFormat("<noOfRooms>{0}</noOfRooms>", oSearchDetails.Rooms);
                sb.AppendFormat("<noOfNights>{0}</noOfNights>", oSearchDetails.PropertyDuration);
                sb.AppendFormat("<city>{0}</city>", oResortSplit.ResortCode);
                sb.AppendFormat("<hotelCodes>");
                if (oResortSplit.Hotels.Count == 1)
                {
                    sb.AppendFormat("<hotelCode>{0}</hotelCode>", oResortSplit.Hotels[0].TPKey);
                }
                else
                {
                    sb.AppendFormat("<hotelCode>0</hotelCode>");
                }
                sb.AppendFormat("</hotelCodes>");
                sb.AppendFormat("<roomsInformation>");
                foreach (RoomDetail oRoom in oSearchDetails.RoomDetails)
                {
                    sb.AppendFormat("<roomInfo>");
                    sb.AppendFormat("<roomTypeId>0</roomTypeId>");
                    sb.AppendFormat("<bedTypeId>0</bedTypeId> ");
                    sb.AppendFormat("<adultsNum>{0}</adultsNum>", oRoom.Adults);
                    sb.AppendFormat("<childNum>{0}</childNum>", oRoom.Children + oRoom.Infants);
                    if (oRoom.Children + oRoom.Infants > 0)
                    {
                        sb.AppendFormat("<childAges>");
                        foreach (int iChildAge in oRoom.ChildAndInfantAges())
                            sb.AppendFormat("<childAge>{0}</childAge>", iChildAge);
                        sb.AppendFormat("</childAges>");
                    }
                    sb.AppendFormat("</roomInfo>");
                }
                sb.AppendFormat("</roomsInformation>");
                sb.Append("</availabilityRequest>");



                var oRequest = new Request();
                oRequest.EndPoint = _settings.get_URL(oSearchDetails) + "GetAvailability.do";
                oRequest.Method = eRequestMethod.POST;
                oRequest.Source = ThirdParties.BONOTEL;
                oRequest.LogFileName = "Search";
                oRequest.CreateLog = bSaveLogs;
                oRequest.ExtraInfo = oSearchDetails;
                oRequest.SetRequest(sb.ToString());
                oRequest.ContentType = ContentTypes.Text_xml;

                oRequests.Add(oRequest);

            }

            return oRequests;

        }

        public override TransformedResultCollection TransformResponse(List<Request> oRequests, SearchDetails oSearchDetails, List<ResortSplit> oResortSplits)
        {
            var oResponses = new List<AvailabilityResponse>();
            var oTransformedResults = new TransformedResultCollection();

            var results = oRequests.Select(o => _serializer.DeSerialize<AvailabilityResponse>(o.ResponseXML));
            var oHotels = results.Where(o=> o.status == "Y").SelectMany(o => o.hotelList.hotel).ToList();

            oTransformedResults.TransformedResults.AddRange(oHotels.SelectMany(o => CreateTransformedResponse(o)));

            return oTransformedResults;
        }

        public List<TransformedResult> CreateTransformedResponse(Hotel hotel)
        {

            var results = new List<TransformedResult>();

            var totalTax = default(decimal);

            foreach (RoomInformation room in hotel.roomInformation)
            {
                foreach (Tax tax in room.rateInformation.taxInformation)
                    totalTax += tax.taxAmount;
            }

            foreach (RoomInformation room in hotel.roomInformation)
            {
                var result = new TransformedResult();

                result.TPKey = hotel.hotelCode.ToSafeString();
                result.CurrencyCode = hotel.rateCurrencyCode;
                result.PropertyRoomBookingID = room.roomNo.ToSafeInt();
                result.RoomType = room.roomType;
                result.MealBasisCode = room.rateInformation.ratePlanCode;
                result.Amount = room.rateInformation.totalRate;
                result.TPReference = $"{room.roomCode}|{room.roomTypeCode}|{room.bedTypeCode}|{hotel.rateCurrencyCode}|{room.rateInformation.ratePlanCode}|{totalTax}";

                results.Add(result);

            }

            return results;

        }

        #endregion

        #region ResponseHasExceptions
        public override bool ResponseHasExceptions(Request oRequest)
        {
            return false;
        }
        #endregion

        #region Helpers



        #endregion

    }
}