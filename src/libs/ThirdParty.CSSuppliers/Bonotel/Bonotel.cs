namespace ThirdParty.CSSuppliers.Bonotel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;


    public class Bonotel : IThirdParty
    {

        private readonly HttpClient _httpclient;

        private readonly ISerializer _serializer;

        private readonly ILogger<Bonotel> _logger;

        #region Constructor

        public Bonotel(IBonotelSettings settings, ITPSupport support, HttpClient httpClient, ISerializer serializer, ILogger<Bonotel> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpclient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region Properties

        private IBonotelSettings _settings { get; set; }

        private ITPSupport _support { get; set; }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.get_AllowCancellations(searchDetails, false);
        }

        public bool SupportsRemarks
        {
            get
            {
                return true;
            }
        }

        public bool SupportsBookingSearch
        {
            get
            {
                return false;
            }
        }

        public string Source => ThirdParties.BONOTEL;

        private bool TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails)
        {
            return false;
        }

        bool IThirdParty.TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails) => TakeSavingFromCommissionMargin(searchDetails);

        private int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.get_OffsetCancellationDays(searchDetails, false);
        }

        int IThirdParty.OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails) => OffsetCancellationDays(searchDetails);

        private bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        bool IThirdParty.RequiresVCard(VirtualCardInfo info) => RequiresVCard(info);

        #endregion

        #region PreBook

        // We don't have a prebook in their interface so just calculate the costs so that this works in the xml gateway
        public bool PreBook(PropertyDetails PropertyDetails)
        {
            // Get Cancelation Policy
            bool bSuccess = CalculateCancellationPolicy(PropertyDetails);

            PropertyDetails.LocalCost = PropertyDetails.Rooms.Sum(r => r.LocalCost);

            return bSuccess;

        }

        #endregion

        #region Book

        public string Book(PropertyDetails PropertyDetails)
        {

            var oRequest = new XmlDocument();
            var oResponse = new XmlDocument();
            string sReference = "";

            PropertyDetails.LocalCost = PropertyDetails.Rooms.Sum(r => r.LocalCost);

            try
            {

                var oBaseHelper = new TPReference(PropertyDetails.Rooms[0].ThirdPartyReference);

                var sb = new StringBuilder();

                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<reservationRequest returnCompeleteBookingDetails=\"Y\">");
                sb.Append("<control>");
                sb.AppendFormat("<userName>{0}</userName>", _settings.get_Username(PropertyDetails));
                sb.AppendFormat("<passWord>{0}</passWord>", _settings.get_Password(PropertyDetails));
                sb.Append("</control>");

                sb.AppendFormat("<reservationDetails timeStamp=\"{0}\">", DateTime.Now.ToString("yyyymmddThh:mm:ss"));

                sb.AppendFormat("<confirmationType>CON</confirmationType>");
                sb.AppendFormat("<tourOperatorOrderNumber>{0}</tourOperatorOrderNumber>", DateTime.Now.ToString("yyyymmddThh:mm:ss"));
                sb.AppendFormat("<checkIn>{0}</checkIn>", PropertyDetails.ArrivalDate.ToString("dd-MMM-yyyy"));
                sb.AppendFormat("<checkOut>{0}</checkOut>", PropertyDetails.DepartureDate.ToString("dd-MMM-yyyy"));
                sb.AppendFormat("<noOfRooms>{0}</noOfRooms>", PropertyDetails.Rooms.Count);
                sb.AppendFormat("<noOfNights>{0}</noOfNights>", PropertyDetails.Duration);
                sb.AppendFormat("<hotelCode>{0}</hotelCode>", PropertyDetails.TPKey);
                sb.AppendFormat("<total currency=\"{0}\">{1}</total>", oBaseHelper.CurrencyCode, PropertyDetails.LocalCost);
                sb.AppendFormat("<totalTax currency=\"{0}\">{1}</totalTax>", oBaseHelper.CurrencyCode, oBaseHelper.TotalTax);

                int iRoomNumber = 1;
                foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
                {

                    oBaseHelper = new TPReference(oRoomDetails.ThirdPartyReference);

                    // room
                    sb.Append("<roomData>");
                    sb.AppendFormat("<roomNo>{0}</roomNo>", iRoomNumber);
                    sb.AppendFormat("<roomCode>{0}</roomCode>", oBaseHelper.RoomCode);
                    sb.AppendFormat("<roomTypeCode>{0}</roomTypeCode>", oBaseHelper.RoomTypeCode);
                    sb.AppendFormat("<bedTypeCode>{0}</bedTypeCode>", oBaseHelper.BedTypeCode);
                    sb.AppendFormat("<ratePlanCode>{0}</ratePlanCode>", oBaseHelper.MealBasis);
                    sb.AppendFormat("<noOfAdults>{0}</noOfAdults>", oRoomDetails.Passengers.TotalAdults);
                    sb.AppendFormat("<noOfChildren>{0}</noOfChildren>", oRoomDetails.Passengers.TotalChildren + oRoomDetails.Passengers.TotalInfants);
                    sb.Append("<occupancy>");

                    // guest
                    foreach (Passenger oPassenger in oRoomDetails.Passengers)
                    {

                        sb.Append("<guest>");
                        sb.AppendFormat("<title>{0}</title>", oPassenger.Title);
                        sb.AppendFormat("<firstName>{0}</firstName>", oPassenger.FirstName);
                        sb.AppendFormat("<lastName>{0}</lastName>", oPassenger.LastName);
                        if (oPassenger.PassengerType == PassengerType.Child || oPassenger.PassengerType == PassengerType.Infant)
                        {
                            sb.AppendFormat("<age>{0}</age>", oPassenger.Age);
                        }
                        sb.Append("</guest>");

                    }

                    sb.Append("</occupancy>");
                    sb.Append("</roomData>");

                    iRoomNumber += 1;

                }

                sb.Append("<comment>");

                // any comments for the hotel
                foreach (BookingComment oComment in PropertyDetails.BookingComments)
                    sb.AppendFormat("<hotel>{0}</hotel>", oComment.Text);

                // any other comments from the customer
                sb.Append("<customer></customer>");
                sb.Append("</comment>");
                sb.Append("</reservationDetails>");
                sb.Append("</reservationRequest>");


                oRequest.LoadXml(sb.ToString());

                // send the request to Bonotel
                // Dim sLoggingType As String = ThirdPartyData.LoggingType(ThirdParties.BONOTEL, PropertyDetails.SalesChannelID, PropertyDetails.BrandID)
                // Dim bCreateLogs As Boolean = sLoggingType <> "None"
                // Dim bCreateLogs As Boolean = sLoggingType <> "None"

                // #If DEBUG Then
                // bCreateLogs = True
                // #End If

                var oWebRequest = new Request();
                oWebRequest.EndPoint = _settings.get_URL(PropertyDetails) + "GetReservation.do";
                oWebRequest.Method = eRequestMethod.POST;
                oWebRequest.SetRequest(oRequest);
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.Source = ThirdParties.BONOTEL;
                oWebRequest.TimeoutInSeconds = _settings.get_BookTimeout(PropertyDetails);
                oWebRequest.Send(_httpclient, _logger);

                oResponse = oWebRequest.ResponseXML;


                if (!string.IsNullOrEmpty(oResponse.InnerText))
                {
                    oResponse.LoadXml(oResponse.InnerXml);
                }
                else
                {
                    sReference = "failed";
                }

                // Get the reference
                if (oResponse.SelectSingleNode("/reservationresponse/referenceno") is not null)
                {
                    sReference = oResponse.SelectSingleNode("/reservationresponse/referenceno").InnerText;
                }
                else if (oResponse.SelectSingleNode("/reservationResponse/referenceNo") is not null)
                {
                    sReference = oResponse.SelectSingleNode("/reservationResponse/referenceNo").InnerText;
                }
                else
                {
                    sReference = "failed";
                }
            }

            catch (Exception ex)
            {

                PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString());

                sReference = "failed";
            }

            finally
            {

                // store the request and response xml on the property booking
                if (oRequest is not null)
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Book Request", oRequest);
                }

                if (oResponse is not null)
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Book Response", oResponse);
                }

            }

            return sReference;

        }

        #endregion

        #region Cancellation

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails PropertyDetails)
        {

            var oThirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            var oRequest = new XmlDocument();
            var oResponse = new XmlDocument();

            try
            {

                // Create XML for cancellation request
                string sRequest;
                sRequest = BuildCancellationRequest(PropertyDetails.SourceReference, PropertyDetails);

                // Log request
                oRequest.LoadXml(sRequest);


                // 'Get the logging type
                // Dim sLoggingType As String = ThirdPartyData.LoggingType(ThirdParties.BONOTEL, PropertyDetails.SalesChannelID, PropertyDetails.BrandID)
                // Dim bCreateLogs As Boolean = sLoggingType <> "None"

                // #If DEBUG Then
                // bCreateLogs = True
                // #End If


                // Send the request
                var oWebRequest = new Request();
                oWebRequest.EndPoint = _settings.get_URL(PropertyDetails) + "GetCancellation.do";
                oWebRequest.Method = eRequestMethod.POST;
                oWebRequest.SetRequest(oRequest);
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.Source = ThirdParties.BONOTEL;
                oWebRequest.CreateLog = PropertyDetails.CreateLogs;
                oWebRequest.LogFileName = "Cancellation";
                oWebRequest.Send(_httpclient, _logger);

                oResponse = oWebRequest.ResponseXML;


                // Get the reference
                if (oResponse.SelectSingleNode("cancellationResponse") is not null)
                {
                    if (oResponse.SelectSingleNode("cancellationResponse").Attributes["status"].Value == "Y")
                    {

                        oThirdPartyCancellationResponse.TPCancellationReference = oResponse.SelectSingleNode("cancellationResponse/cancellationNo").InnerText;
                        oThirdPartyCancellationResponse.Success = true;

                    }
                }
            }

            catch (Exception ex)
            {
                PropertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
            }

            finally
            {

                // Store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(oRequest.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Cancellation Request", oRequest);
                }

                if (oResponse is not null)
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Cancellation Response", oResponse);
                }

            }

            return oThirdPartyCancellationResponse;

        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails PropertyDetails)
        {
            return new ThirdPartyCancellationFeeResult();
        }

        private string BuildCancellationRequest(string BookingReference, IThirdPartyAttributeSearch SearchDetails)
        {

            var sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.Append("<cancellationRequest>");
            sb.Append("<control>");
            sb.AppendFormat("<userName>{0}</userName>", _settings.get_Username(SearchDetails));
            sb.AppendFormat("<passWord>{0}</passWord>", _settings.get_Password(SearchDetails));
            sb.Append("</control>");
            sb.AppendFormat("<supplierReferenceNo>{0}</supplierReferenceNo>", BookingReference);
            sb.Append("<cancellationReason/>");
            sb.Append("<cancellationNotes/>");
            sb.Append("</cancellationRequest>");

            return sb.ToString();

        }

        #endregion

        #region Booking Search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails oBookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public string CreateReconciliationReference(string sInputReference)
        {
            return "";
        }

        #endregion

        #region Booking Status Update

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails oPropertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        #endregion
        public void EndSession(PropertyDetails oPropertyDetails)
        {

        }


        #region Third Party Reference Helper

        private class TPReference
        {
            public string RoomCode;
            public string RoomTypeCode;
            public string BedTypeCode;
            public string CurrencyCode;
            public string MealBasis;
            public string TotalTax;

            public TPReference(string TPReference)
            {
                var aParts = TPReference.Split('|');
                RoomCode = aParts[0];
                RoomTypeCode = aParts[1];
                BedTypeCode = aParts[2];
                CurrencyCode = aParts[3];
                MealBasis = aParts[4];
                TotalTax = aParts[5];
            }
        }

        #endregion

        #region Search Hotel Again for Cancellation Charges

        public bool CalculateCancellationPolicy(PropertyDetails PropertyDetails)
        {

            bool bSuccess = true;
            var oRequest = new XmlDocument();
            var oResponse = new XmlDocument();

            try
            {

                // Send the request to Bonotel
                // Dim sLoggingType As String = ThirdPartyData.LoggingType(ThirdParties.BONOTEL, PropertyDetails.BrandID, PropertyDetails.SalesChannelID)
                // Dim bCreateLogs As Boolean = sLoggingType <> "None"

                // #If DEBUG Then
                // bCreateLogs = True
                // #End If

                oRequest.LoadXml(GetAvailabilityRequest(PropertyDetails, PropertyDetails));

                var oWebRequest = new Request();
                oWebRequest.EndPoint = _settings.get_URL(PropertyDetails) + "GetAvailability.do";
                oWebRequest.Method = eRequestMethod.POST;
                oWebRequest.Source = ThirdParties.BONOTEL;
                oWebRequest.LogFileName = "CancellationCharges";
                oWebRequest.CreateLog = PropertyDetails.CreateLogs;
                oWebRequest.SetRequest(oRequest);
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.Send(_httpclient, _logger);

                oResponse = oWebRequest.ResponseXML;
                foreach (XmlNode oRoomNode in oResponse.SelectNodes("availabilityResponse/hotelList/hotel/roomInformation"))
                {
                    string sFeeType = "";
                    string sFeeIndicator = "";
                    string sErrataTitle = "";
                    string sErrataDescription = "";
                    if (oRoomNode.SelectNodes("rateInformation/hotelFees/hotelFee").Count > 0)
                    {
                        foreach (XmlNode oHotelFeeNode in oRoomNode.SelectNodes("rateInformation/hotelFees/hotelFee"))
                        {
                            sFeeType = oHotelFeeNode.SelectSingleNode("feeType").InnerText;
                            string sRequiredFee = oHotelFeeNode.SelectSingleNode("requiredFee").InnerText;
                            string sFeeMethod = oHotelFeeNode.SelectSingleNode("feeMethod").InnerText;
                            sFeeIndicator = GetFeeIndicator(sRequiredFee, sFeeMethod);
                            sErrataTitle = string.Format("{0} Fee - {1}", sFeeType, sFeeIndicator);
                            string sAmount = oHotelFeeNode.SelectSingleNode("feeTotal").InnerText;
                            string sConditions = oHotelFeeNode.SelectSingleNode("conditions").InnerText;
                            sErrataDescription = string.Format("{0}{1}", sAmount, sConditions);
                            PropertyDetails.Errata.AddNew(sErrataTitle.ToNiceName(), sErrataDescription);
                        }
                    }
                }

                var oResult = _serializer.DeSerialize<AvailabilityResponse>(oResponse);

                // Take all the policies out of the xsl
                var oPolicies = new List<CancellationPolicy>();

                foreach (Hotel o in oResult.hotelList.hotel)
                {
                    foreach (RoomInformation t in o.roomInformation)
                    {
                        foreach (RoomBookingPolicy p in t.roomBookingPolicy)
                        {
                            var oPolicyLoad = new CancellationPolicy();
                            oPolicyLoad.AmendmentType = p.amendmentType;
                            oPolicyLoad.PolicyBasedOn = p.policyBasedOn;
                            oPolicyLoad.PolicyBasedOnValue = p.policyBasedOnValue;
                            oPolicyLoad.CancellationType = p.cancellationType;
                            oPolicyLoad.StayDateRequirement = p.stayDateRequirement;
                            oPolicyLoad.ArrivalRange = p.arrivalRange;
                            oPolicyLoad.ArrivalRangeValue = p.arrivalRangeValue;
                            oPolicyLoad.PolicyFee = p.policyFee.Replace("$", "").ToSafeDecimal();
                            oPolicyLoad.NoShowBasedOn = p.noShowBasedOn;
                            oPolicyLoad.NoShowBasedOnValue = p.noShowBasedOnValue;
                            oPolicyLoad.NoShowPolicyFee = p.noShowPolicyFee.Replace("$", "").ToSafeDecimal();

                            oPolicies.Add(oPolicyLoad);
                        }
                    }
                }

                // Loads The Policies into the OverrideSupplierCancellations Class
                var oCancellations = new Cancellations();

                foreach (CancellationPolicy oPolicy in oPolicies)
                {
                    bool bSpecialFlag = false;

                    if (oPolicy.AmendmentType == "Cancel")
                    {

                        // Checks if there is a special policy that overlaps the normal policy
                        if (oPolicy.CancellationType == "Special")
                        {

                            bSpecialFlag = true;

                            if (oPolicy.ArrivalRange == "Less Than")
                            {
                                int iDays = oPolicy.ArrivalRangeValue.ToSafeInt();

                                oCancellations.AddNew(PropertyDetails.ArrivalDate.AddDays(-iDays), PropertyDetails.ArrivalDate.AddDays(-1), oPolicy.PolicyFee);

                                oCancellations.AddNew(PropertyDetails.ArrivalDate, new DateTime(2099, 1, 1), oPolicy.NoShowPolicyFee);
                            }

                            else if (oPolicy.ArrivalRange == "Greater Than")
                            {

                                oCancellations.AddNew(PropertyDetails.ArrivalDate, new DateTime(2099, 1, 1), oPolicy.NoShowPolicyFee);
                            }

                            else if (oPolicy.ArrivalRange == "Any")
                            {

                                oCancellations.AddNew(DateTime.Now, PropertyDetails.ArrivalDate.AddDays(-1), oPolicy.PolicyFee);

                                oCancellations.AddNew(PropertyDetails.ArrivalDate, new DateTime(2099, 1, 1), oPolicy.NoShowPolicyFee);

                            }
                        }

                        // Normal Policy
                        else if (oPolicy.CancellationType == "Normal" & bSpecialFlag == false)
                        {

                            if (oPolicy.ArrivalRange == "Less Than")
                            {
                                int iDays = oPolicy.ArrivalRangeValue.ToSafeInt();

                                oCancellations.AddNew(PropertyDetails.ArrivalDate.AddDays(-iDays), PropertyDetails.ArrivalDate.AddDays(-1), oPolicy.PolicyFee);

                                oCancellations.AddNew(PropertyDetails.ArrivalDate, new DateTime(2099, 1, 1), oPolicy.NoShowPolicyFee);
                            }

                            else if (oPolicy.ArrivalRange == "Greater Than")
                            {

                                oCancellations.AddNew(PropertyDetails.ArrivalDate, new DateTime(2099, 1, 1), oPolicy.NoShowPolicyFee);
                            }

                            else if (oPolicy.ArrivalRange == "Any")
                            {

                                oCancellations.AddNew(DateTime.Now, PropertyDetails.ArrivalDate.AddDays(-1), oPolicy.PolicyFee);

                                oCancellations.AddNew(PropertyDetails.ArrivalDate, new DateTime(2099, 1, 1), oPolicy.NoShowPolicyFee);

                            }

                        }

                    }

                    bSpecialFlag = false;

                }

                oCancellations.Solidify(SolidifyType.Sum);
                PropertyDetails.Cancellations = oCancellations;

                // check for price changes here
                foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
                {
                    var oBaseHelper = new TPReference(oRoomDetails.ThirdPartyReference);

                    foreach (XmlNode oRoomNode in oWebRequest.ResponseXML.SelectNodes("availabilityResponse/hotelList/hotel/roomInformation"))
                    {
                        // find the correct node via room code
                        if ((oRoomNode.SafeNodeValue("roomCode") ?? "") == (oBaseHelper.RoomCode ?? "") & (oRoomNode.SafeNodeValue("rateInformation/ratePlanCode") ?? "") == (oBaseHelper.MealBasis ?? ""))
                        {
                            decimal nNewPrice = oRoomNode.SafeNodeValue("rateInformation/totalRate").ToSafeMoney();
                            if (nNewPrice != oRoomDetails.LocalCost)
                            {
                                oRoomDetails.GrossCost = nNewPrice;
                                oRoomDetails.LocalCost = nNewPrice;
                            }
                        }

                    }
                }
            }

            catch (Exception ex)
            {

                PropertyDetails.Warnings.AddNew("Cancellation Costs Exception", ex.ToString());
                bSuccess = false;
            }

            finally
            {

                if (!string.IsNullOrEmpty(oRequest.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Cancellation Cost Request", oRequest);
                }

                if (!string.IsNullOrEmpty(oResponse.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.BONOTEL, "Cancellation Cost Response", oResponse);
                }

            }

            return bSuccess;

        }


        public string GetAvailabilityRequest(IThirdPartyAttributeSearch SearchDetails, PropertyDetails PropertyDetails)
        {
            var sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<availabilityRequest cancelpolicy=\"Y\" hotelfees=\"Y\">");


            sb.Append("<control>");
            sb.AppendFormat("<userName>{0}</userName>", _settings.get_Username(SearchDetails));
            sb.AppendFormat("<passWord>{0}</passWord>", _settings.get_Password(SearchDetails));
            sb.Append("</control>");


            sb.AppendFormat("<checkIn>{0}</checkIn>", PropertyDetails.ArrivalDate.ToString("dd-MMM-yyyy"));
            sb.AppendFormat("<checkOut>{0}</checkOut>", PropertyDetails.DepartureDate.ToString("dd-MMM-yyyy"));
            sb.AppendFormat("<noOfRooms>{0}</noOfRooms>", PropertyDetails.Rooms.Count);
            sb.AppendFormat("<noOfNights>{0}</noOfNights>", PropertyDetails.Duration);
            sb.AppendFormat("<hotelCodes>");
            sb.AppendFormat("<hotelCode>{0}</hotelCode>", PropertyDetails.TPKey);
            sb.AppendFormat("</hotelCodes>");


            sb.AppendFormat("<roomsInformation>");

            int iRoomNumber = 1;
            foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
            {

                var oBaseHelper = new TPReference(oRoomDetails.ThirdPartyReference);

                sb.AppendFormat("<roomInfo>");
                sb.AppendFormat("<roomTypeId>{0}</roomTypeId>", oBaseHelper.RoomTypeCode);
                sb.AppendFormat("<bedTypeId>{0}</bedTypeId> ", oBaseHelper.BedTypeCode);
                sb.AppendFormat("<adultsNum>{0}</adultsNum>", oRoomDetails.Passengers.TotalAdults);
                sb.AppendFormat("<childNum>{0}</childNum>", oRoomDetails.Passengers.TotalChildren + oRoomDetails.Passengers.TotalInfants);

                if (oRoomDetails.Passengers.TotalChildren + oRoomDetails.Passengers.TotalInfants > 0)
                {

                    sb.AppendFormat("<childAges>");

                    foreach (Passenger oPassenger in oRoomDetails.Passengers)
                    {

                        if (oPassenger.PassengerType == PassengerType.Child || oPassenger.PassengerType == PassengerType.Infant)
                        {
                            sb.AppendFormat("<childAge>{0}</childAge>", oPassenger.Age);
                        }
                    }

                    sb.AppendFormat("</childAges>");

                }

                sb.AppendFormat("</roomInfo>");

            }

            sb.AppendFormat("</roomsInformation>");
            sb.Append("</availabilityRequest>");

            return sb.ToString();

        }

        public string GetFeeIndicator(string FeeIndicator, string FeeMethod)
        {
            string sIndicator = "";
            if (FeeIndicator == "No")
            {
                sIndicator = "Optional";
            }
            else if (FeeMethod == "Exclusive")
            {
                sIndicator = "Payable Locally";
            }
            else if (FeeMethod == "Inclusive")
            {
                sIndicator = "Included";
            }
            return sIndicator;
        }

        public class CancellationPolicy
        {

            public string AmendmentType;
            public string PolicyBasedOn;
            public string PolicyBasedOnValue;
            public string CancellationType;
            public string StayDateRequirement;
            public string ArrivalRange;
            public string ArrivalRangeValue;
            public decimal PolicyFee;

            public string NoShowBasedOn;
            public string NoShowBasedOnValue;
            public decimal NoShowPolicyFee;

        }

        #endregion

    }
}