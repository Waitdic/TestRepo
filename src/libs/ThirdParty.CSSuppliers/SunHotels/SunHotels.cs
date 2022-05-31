namespace ThirdParty.CSSuppliers.SunHotels
{
    using System;
    using System.Net.Http;
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
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Search.Models;

    public class SunHotels : IThirdParty
    {
        #region Constructor

        public SunHotels(ISunHotelsSettings settings, HttpClient httpClient, ISerializer serializer, ILogger<SunHotels> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region Properties

        private readonly ISunHotelsSettings _settings;

        private readonly HttpClient _httpClient;

        private readonly ISerializer _serializer;

        private readonly ILogger<SunHotels> _logger;

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

        public string Source => ThirdParties.SUNHOTELS;

        private bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.get_AllowCancellations(searchDetails);
        }

        bool IThirdParty.SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source) => SupportsLiveCancellation(searchDetails, source);

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


        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        #endregion

        #region PreBook

        public bool PreBook(PropertyDetails PropertyDetails)
        {

            // build the search url
            var sbPrebookURL = new StringBuilder();
            var oPrebookResponseXML = new XmlDocument();

            try
            {

                sbPrebookURL.Append(_settings.get_PreBookURL(PropertyDetails));
                sbPrebookURL.AppendFormat("userName={0}", _settings.get_Username(PropertyDetails));
                sbPrebookURL.AppendFormat("&password={0}", _settings.get_Password(PropertyDetails));
                sbPrebookURL.AppendFormat("&language={0}", _settings.get_Language(PropertyDetails));
                sbPrebookURL.AppendFormat("&currency={0}", _settings.get_Currency(PropertyDetails));
                sbPrebookURL.AppendFormat("&checkInDate={0}", GetSunHotelsDate(PropertyDetails.ArrivalDate));
                sbPrebookURL.AppendFormat("&checkOutDate={0}", GetSunHotelsDate(PropertyDetails.DepartureDate));
                sbPrebookURL.AppendFormat("&roomId={0}", PropertyDetails.Rooms[0].ThirdPartyReference.Split('_')[0]);
                sbPrebookURL.AppendFormat("&rooms={0}", PropertyDetails.Rooms.Count);
                sbPrebookURL.AppendFormat("&adults={0}", PropertyDetails.Rooms[0].Adults);
                sbPrebookURL.AppendFormat("&children={0}", PropertyDetails.Rooms[0].Children);
                if (PropertyDetails.Rooms[0].Children > 0)
                {
                    sbPrebookURL.AppendFormat("&childrenAges={0}", PropertyDetails.Rooms[0].GetChildAgeCsv());
                }
                else
                {
                    sbPrebookURL.AppendFormat("&childrenAges=");
                }
                sbPrebookURL.AppendFormat("&infant={0}", (object)IsInfantIncluded(PropertyDetails.Rooms[0]));
                sbPrebookURL.AppendFormat("&mealId={0}", PropertyDetails.Rooms[0].ThirdPartyReference.Split('_')[1]);
                sbPrebookURL.AppendFormat("&customerCountry={0}", _settings.get_Nationality(PropertyDetails));
                sbPrebookURL.Append("&B2C=");
                sbPrebookURL.Append("&searchPrice=");
                sbPrebookURL.AppendFormat("&showPriceBreakdown=0");

                sbPrebookURL.Append("&blockSuperDeal=0");

                // send the request to SunHotels
                var oWebRequest = new Request();
                oWebRequest.EndPoint = sbPrebookURL.ToString();
                oWebRequest.Method = eRequestMethod.GET;
                oWebRequest.Source = ThirdParties.SUNHOTELS;
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.LogFileName = "Prebook";
                oWebRequest.CreateLog = true;
                oWebRequest.TimeoutInSeconds = 100;
                oWebRequest.Send(_httpClient, _logger);

                oPrebookResponseXML = oWebRequest.ResponseXML;

                // strip out anything we don't need
                oPrebookResponseXML = _serializer.CleanXmlNamespaces(oPrebookResponseXML);

                if (oPrebookResponseXML.SelectSingleNode("preBookResult/Error") is not null)
                {
                    PropertyDetails.Warnings.AddNew("Prebook Failed", oPrebookResponseXML.SafeNodeValue("preBookResult/Error/Message"));
                    return false;
                }

                // store the Errata if we have any
                var Errata = oPrebookResponseXML.SelectNodes("preBookResult/Notes/Note");
                foreach (XmlNode Erratum in Errata)
                    PropertyDetails.Errata.AddNew("Important Information", Erratum.SelectSingleNode("text").InnerText);

                // recheck the price in case it has changed
                // ** needs to be changed if we implement multi-rooms in the future**
                decimal nPrice = 0m;
                var oCancellations = new Cancellations();
                nPrice = oPrebookResponseXML.SafeNodeValue("preBookResult/Price").ToSafeMoney();
                PropertyDetails.TPRef1 = oPrebookResponseXML.SafeNodeValue("preBookResult/PreBookCode");

                // override the cancellations

                int iCancellationPolicyCount = 0;

                foreach (XmlNode oCancellationPolicy in oPrebookResponseXML.SelectNodes("preBookResult/CancellationPolicies/CancellationPolicy"))
                {

                    iCancellationPolicyCount += 1;

                    var oHours = new TimeSpan(oCancellationPolicy.SelectSingleNode("deadline").InnerText.ToSafeInt(), 0, 0);
                    DateTime dStartDate;

                    // for 100% cancel;lations we don't get an hours before
                    // so force the start date to be from now
                    if (oHours.TotalHours == 0d)
                    {
                        dStartDate = DateTime.Now.Date;
                    }
                    else
                    {
                        dStartDate = PropertyDetails.ArrivalDate.Subtract(oHours);
                    }

                    decimal nCancellationCost = (nPrice * oCancellationPolicy.SelectSingleNode("percentage").InnerText.ToSafeDecimal() / 100m).ToSafeDecimal();

                    // take the end date of the next cancellation policy otherwise set it a long way off
                    DateTime dEndDate;
                    if (oPrebookResponseXML.SelectSingleNode(string.Format("preBookResult/CancellationPolicies/CancellationPolicy[{0}]", iCancellationPolicyCount + 1)) is not null)
                    {

                        // the hours of the end dates need to be rounded to the nearest 24 hours to stop them overlapping with the start of the next cancellation policy and making
                        // the charges add together
                        var oEndHours = new TimeSpan(RoundHoursUpToTheNearest24Hours(oPrebookResponseXML.SelectSingleNode(string.Format("preBookResult/CancellationPolicies/CancellationPolicy[{0}]/deadline", iCancellationPolicyCount + 1)).InnerText.ToSafeInt()), 0, 0);

                        dEndDate = PropertyDetails.ArrivalDate.Subtract(oEndHours);
                        dEndDate = dEndDate.AddDays(-1);
                    }

                    else
                    {
                        dEndDate = new DateTime(2099, 1, 1);

                    }

                    oCancellations.AddNew(dStartDate, dEndDate, nCancellationCost);

                }

                PropertyDetails.Cancellations = oCancellations;

                if (nPrice > 0m && nPrice != PropertyDetails.LocalCost)
                {
                    PropertyDetails.Rooms[0].GrossCost = nPrice;
                    PropertyDetails.Rooms[0].LocalCost = nPrice;
                    PropertyDetails.GrossCost = nPrice;
                    PropertyDetails.LocalCost = nPrice;
                    PropertyDetails.AddLog(ThirdParties.SUNHOTELS, "Third Party / Prebook Price Changed");
                }
                return true;
            }

            catch (Exception ex)
            {

                PropertyDetails.Warnings.AddNew("SunHotels Exception", ex.ToString());
                return false;
            }

            finally
            {

                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(sbPrebookURL.ToString()))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels PreBook Request", sbPrebookURL.ToString());
                }

                if (!string.IsNullOrEmpty(oPrebookResponseXML.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Prebook Response", oPrebookResponseXML);
                }

            }

        }

        #endregion

        #region Book

        public string Book(PropertyDetails PropertyDetails)
        {

            // **NOTE - THIS ONLY WORKS FOR SINGLE ROOM BOOKINGS**
            string sRequestURL = "";
            var oResponse = new XmlDocument();
            string sBookingReference = "";

            try
            {

                // split out the room reference and the meal type
                string sRoomID = PropertyDetails.Rooms[0].ThirdPartyReference.Split('_')[0];

                // Grab the BookingReference If we have it
                string siVectorReference = "";
                if (string.IsNullOrEmpty(PropertyDetails.BookingReference))
                {
                    siVectorReference = _settings.get_SupplierReference(PropertyDetails);
                }
                else
                {
                    siVectorReference = PropertyDetails.BookingReference;
                }

                // build the book request url
                var sbBookingRequestURL = new StringBuilder();


                sbBookingRequestURL.Append(_settings.get_BookURL(PropertyDetails));
                sbBookingRequestURL.AppendFormat("userName={0}", _settings.get_Username(PropertyDetails));
                sbBookingRequestURL.AppendFormat("&password={0}", _settings.get_Password(PropertyDetails));
                sbBookingRequestURL.AppendFormat("&currency={0}", _settings.get_Currency(PropertyDetails));
                sbBookingRequestURL.AppendFormat("&language={0}", _settings.get_Language(PropertyDetails));
                sbBookingRequestURL.AppendFormat("&email={0}", _settings.get_EmailAddress(PropertyDetails));
                sbBookingRequestURL.AppendFormat("&checkInDate={0}", GetSunHotelsDate(PropertyDetails.ArrivalDate));
                sbBookingRequestURL.AppendFormat("&checkOutDate={0}", GetSunHotelsDate(PropertyDetails.DepartureDate));
                sbBookingRequestURL.AppendFormat("&roomId={0}", sRoomID);
                sbBookingRequestURL.AppendFormat("&rooms={0}", PropertyDetails.Rooms.Count);
                sbBookingRequestURL.AppendFormat("&adults={0}", PropertyDetails.Adults);
                sbBookingRequestURL.AppendFormat("&children={0}", PropertyDetails.Children);
                sbBookingRequestURL.AppendFormat("&infant={0}", (object)IsInfantIncluded(PropertyDetails.Rooms[0]));
                sbBookingRequestURL.AppendFormat("&yourRef={0}", siVectorReference);

                if (PropertyDetails.BookingComments.Count != 0)
                {
                    sbBookingRequestURL.AppendFormat("&specialrequest={0}", PropertyDetails.BookingComments[0].Text);
                }
                else
                {
                    sbBookingRequestURL.Append("&specialrequest=");
                }

                sbBookingRequestURL.AppendFormat("&mealId={0}", PropertyDetails.Rooms[0].ThirdPartyReference.Split('_')[1]);

                int iAdults = 1;
                int iChildren = 1;

                // add the adults that in the booking
                foreach (Passenger oPassenger in PropertyDetails.Rooms[0].Passengers)
                {
                    if (oPassenger.PassengerType == PassengerType.Adult)
                    {

                        sbBookingRequestURL.AppendFormat("&adultGuest{0}FirstName={1}", iAdults, oPassenger.FirstName);
                        sbBookingRequestURL.AppendFormat("&adultGuest{0}LastName={1}", iAdults, oPassenger.LastName);
                        iAdults += 1;

                    }
                }

                // add empty elements for all the other adults up to 9
                for (int i = 1; i <= 9; i++)
                {
                    if (i >= iAdults)
                    {
                        sbBookingRequestURL.AppendFormat("&adultGuest{0}FirstName=", i);
                        sbBookingRequestURL.AppendFormat("&adultGuest{0}LastName=", i);
                    }
                }

                // add the children
                foreach (Passenger oPassenger in PropertyDetails.Rooms[0].Passengers)
                {
                    if (oPassenger.PassengerType == PassengerType.Child && oPassenger.Age <= 17)
                    {

                        sbBookingRequestURL.AppendFormat("&childrenGuest{0}FirstName={1}", iChildren, oPassenger.FirstName);
                        sbBookingRequestURL.AppendFormat("&childrenGuest{0}LastName={1}", iChildren, oPassenger.LastName);
                        sbBookingRequestURL.AppendFormat("&childrenGuestAge{0}={1}", iChildren, oPassenger.Age);
                        iChildren += 1;

                    }
                }

                // add empty elements for all the other adults up to 9
                for (int i = 1; i <= 9; i++)
                {
                    if (i >= iChildren)
                    {
                        sbBookingRequestURL.AppendFormat("&childrenGuest{0}FirstName=", i);
                        sbBookingRequestURL.AppendFormat("&childrenGuest{0}LastName=", i);
                        sbBookingRequestURL.AppendFormat("&childrenGuestAge{0}=", i);
                    }
                }
                sbBookingRequestURL.AppendFormat("&paymentMethodId={0}", PropertyDetails.Rooms[0].ThirdPartyReference.Split('_')[3]);
                sbBookingRequestURL.AppendFormat(GetCardDetails(PropertyDetails.Rooms[0].ThirdPartyReference.Split('_')[3], PropertyDetails));
                sbBookingRequestURL.Append("&customerEmail=");
                sbBookingRequestURL.Append("&invoiceRef=");
                sbBookingRequestURL.Append("&commissionAmountInHotelCurrency=");
                sbBookingRequestURL.AppendFormat("&customerCountry={0}", _settings.get_Nationality(PropertyDetails));
                sbBookingRequestURL.Append("&B2C=");

                sbBookingRequestURL.AppendFormat("&PreBookCode={0}", PropertyDetails.TPRef1);

                sRequestURL = sbBookingRequestURL.ToString();

                var oWebRequest = new Request();
                oWebRequest.Source = ThirdParties.SUNHOTELS;
                oWebRequest.EndPoint = sRequestURL;
                oWebRequest.Method = eRequestMethod.GET;
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.Send(_httpClient, _logger);

                oResponse = _serializer.CleanXmlNamespaces(oWebRequest.ResponseXML);

                if (oResponse.SelectSingleNode("bookResult/booking/Error") is null)
                {

                    // grab the booking reference
                    sBookingReference = oResponse.SelectSingleNode("bookResult/booking/bookingnumber").InnerText;
                }

                else
                {

                    sBookingReference = "failed";

                }
            }

            catch (Exception ex)
            {

                sBookingReference = "failed";
                PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
            }

            finally
            {

                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(sRequestURL))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Book Request", sRequestURL);
                }

                if (!string.IsNullOrEmpty(oResponse.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Book Response", oResponse);
                }

            }

            return sBookingReference;

        }

        #endregion

        #region Cancellations

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails PropertyDetails)
        {

            var oThirdPartyCancellationResponse = new ThirdPartyCancellationResponse();

            var sbCancellationRequestURL = new StringBuilder();
            var oCancellationResponseXML = new XmlDocument();

            try
            {

                // build the cancellation url
                sbCancellationRequestURL.Append(_settings.get_CancelURL(PropertyDetails));
                sbCancellationRequestURL.AppendFormat("userName={0}", _settings.get_Username(PropertyDetails));
                sbCancellationRequestURL.AppendFormat("&password={0}", _settings.get_Password(PropertyDetails));
                sbCancellationRequestURL.AppendFormat("&bookingID={0}", PropertyDetails.SourceReference.ToString());
                sbCancellationRequestURL.AppendFormat("&language={0}", _settings.get_Language(PropertyDetails));

                // Send the request
                var oWebRequest = new Request();
                oWebRequest.EndPoint = sbCancellationRequestURL.ToString();
                oWebRequest.Method = eRequestMethod.GET;
                oWebRequest.Source = ThirdParties.SUNHOTELS;
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.LogFileName = "Cancel";
                oWebRequest.CreateLog = true;
                oWebRequest.TimeoutInSeconds = 100;
                oWebRequest.Send(_httpClient, _logger);

                oCancellationResponseXML = oWebRequest.ResponseXML;

                // tidy up the response
                oCancellationResponseXML = _serializer.CleanXmlNamespaces(oCancellationResponseXML);

                // Check for success
                var oResultCodeNode = oCancellationResponseXML.SelectSingleNode("result/Code");
                if (oResultCodeNode is not null && oResultCodeNode.InnerText.ToSafeInt() != -1)
                {
                    oThirdPartyCancellationResponse.TPCancellationReference = DateTime.Now.ToString("dd MMM/HH:mm");
                    oThirdPartyCancellationResponse.Success = true;
                }
            }

            catch (Exception ex)
            {

                PropertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString());
                oThirdPartyCancellationResponse.Success = false;
            }

            finally
            {

                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(sbCancellationRequestURL.ToString()))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Cancellation Request", sbCancellationRequestURL.ToString());
                }

                if (!string.IsNullOrEmpty(oCancellationResponseXML.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Cancellation Response", oCancellationResponseXML);
                }

            }

            return oThirdPartyCancellationResponse;

        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails PropertyDetails)
        {
            return GetCancellationCost(PropertyDetails);
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


        #region Helpers

        public static string GetSunHotelsDate(DateTime dDate)
        {

            return dDate.ToString("yyyy-MM-dd");

        }

        public static int IsInfantIncluded(SearchDetails searchDetails)
        {
            int iInfantIncluded = 0;
            if (searchDetails.TotalInfants > 0)
            {
                iInfantIncluded = 1;
            }
            return iInfantIncluded;
        }

        public static int IsInfantIncluded(ThirdParty.Models.Property.Booking.RoomDetails roomDetails)
        {
            int iInfantIncluded = 0;
            if (roomDetails.Infants > 0)
            {
                iInfantIncluded = 1;
            }
            return iInfantIncluded;
        }

        public static string GetAdultsFromSearchDetails(SearchDetails SearchDetails)
        {

            int iAdultCount = 0;

            foreach (RoomDetail oRoom in SearchDetails.RoomDetails)
            {

                iAdultCount += oRoom.Adults;

                foreach (int iAge in oRoom.ChildAges)
                {
                    if (iAge > 17)
                        iAdultCount += 1;
                }

            }

            return iAdultCount.ToString();

        }

        public static string GetChildrenFromSearchDetails(SearchDetails SearchDetails)
        {

            int iChildCount = 0;

            foreach (RoomDetail oRoom in SearchDetails.RoomDetails)
            {

                foreach (int iAge in oRoom.ChildAges)
                {
                    if (iAge <= 17)
                        iChildCount += 1;
                }

            }

            return iChildCount.ToString();

        }

        public static string GetChildrenAges(SearchDetails SearchDetails)
        {

            string sChildAges = "";
            var sb = new StringBuilder();

            foreach (RoomDetail oRoom in SearchDetails.RoomDetails)
                sb.Append(oRoom.ChildAgeCSV);
            sChildAges = string.Join(",", sb.ToString());
            return sChildAges;

        }

        public static int RoundHoursUpToTheNearest24Hours(int Hours)
        {

            int i;
            i = Math.Round((Hours / 24d).ToSafeDecimal(), 0, MidpointRounding.AwayFromZero).ToSafeInt();

            return i * 24;

        }

        public string GetCardDetails(string PaymentMethodId, PropertyDetails PropertyDetails)
        {
            var sbCardDetails = new StringBuilder();
            sbCardDetails.Append("&creditCardType=");
            sbCardDetails.Append("&creditCardNumber=");
            sbCardDetails.Append("&creditCardHolder=");
            sbCardDetails.Append("&creditCardCVV2=");
            sbCardDetails.Append("&creditCardExpYear=");
            sbCardDetails.Append("&creditCardExpMonth=");
            return sbCardDetails.ToString();
        }

        #endregion

    }
}