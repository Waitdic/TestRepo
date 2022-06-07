namespace ThirdParty.CSSuppliers.YouTravel
{
    using System;
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

    public class YouTravel : IThirdParty
    {

        #region Constructor

        public YouTravel(IYouTravelSettings settings, ITPSupport support, HttpClient httpClient, ISerializer serializer, ILogger<YouTravel> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region Properties

        private IYouTravelSettings _settings { get; set; }

        private ITPSupport _support { get; set; }

        private HttpClient _httpClient { get; set; }

        private ISerializer _serializer { get; set; }

        private readonly ILogger<YouTravel> _logger;

        public bool SupportsRemarks
        {
            get
            {
                return true;
            }
        }

        private bool IThirdParty_SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.get_AllowCancellations(searchDetails, false);
        }

        bool IThirdParty.SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source) => IThirdParty_SupportsLiveCancellation(searchDetails, source);

        public bool SupportsBookingSearch
        {
            get
            {
                return false;
            }
        }

        public string Source => ThirdParties.YOUTRAVEL;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.get_OffsetCancellationDays(searchDetails, false);
        }

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        #endregion

        #region PreBook

        public bool PreBook(PropertyDetails PropertyDetails)
        {
            var oResponse = new XmlDocument();
            bool bSuccess = true;
            var oWebRequest = new Request();

            try
            {
                // Get Errata details
                var sb = new StringBuilder();
                sb.AppendFormat("{0}{1}", _settings.get_PrebookURL(PropertyDetails), "?");
                sb.AppendFormat("&LangID={0}", _settings.get_LangID(PropertyDetails));
                sb.AppendFormat("&HID={0}", PropertyDetails.TPKey);
                sb.AppendFormat("&UserName={0}", _settings.get_Username(PropertyDetails));
                sb.AppendFormat("&Password={0}", _settings.get_Password(PropertyDetails));

                oWebRequest.Source = ThirdParties.YOUTRAVEL;
                oWebRequest.CreateLog = true;
                oWebRequest.LogFileName = "Prebook";
                oWebRequest.EndPoint = sb.ToString();
                oWebRequest.Method = eRequestMethod.GET;
                oWebRequest.Send(_httpClient, _logger);

                oResponse = _serializer.CleanXmlNamespaces(oWebRequest.ResponseXML);
                var oErrataNode = oResponse.SelectSingleNode("/HtSearchRq/Hotel/Erratas");
                string sErrata = oErrataNode.InnerText;
                PropertyDetails.Errata.AddNew("Important Information", sErrata);
                // Get cancellation policies
                foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
                {
                    var oCancellationPolicyWebRequest = new Request();
                    var sbCancelPolicy = new StringBuilder();
                    var oCancelPolicyResponseXML = new XmlDocument();
                    sbCancelPolicy.AppendFormat("{0}{1}", _settings.get_CancellationPolicyURL(PropertyDetails), "?");
                    sbCancelPolicy.AppendFormat("token={0}", oRoomDetails.ThirdPartyReference.Split('_')[2]);
                    oCancellationPolicyWebRequest.Source = ThirdParties.YOUTRAVEL;
                    oCancellationPolicyWebRequest.CreateLog = true;
                    oCancellationPolicyWebRequest.LogFileName = "Cancellation Cost";
                    oCancellationPolicyWebRequest.EndPoint = sbCancelPolicy.ToString();
                    oCancellationPolicyWebRequest.Method = eRequestMethod.GET;
                    oCancellationPolicyWebRequest.Send(_httpClient, _logger);

                    oCancelPolicyResponseXML = _serializer.CleanXmlNamespaces(oCancellationPolicyWebRequest.ResponseXML);
                    foreach (XmlNode oCancellationNode in oCancelPolicyResponseXML.SelectNodes("/HtSearchRq/Policies/Policy"))
                    {
                        var dFromDate = oCancellationNode.SelectSingleNode("FromDate").InnerText.ToSafeDate();
                        var dToDate = new DateTime(2099, 1, 1);
                        decimal nCancellationCost = oCancellationNode.SelectSingleNode("Fees").InnerText.ToSafeDecimal();
                        PropertyDetails.Cancellations.AddNew(dFromDate, dToDate, nCancellationCost);
                    }
                    PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel CancellationPolicy Request " + oRoomDetails.PropertyRoomBookingID, oCancellationPolicyWebRequest.RequestLog);
                    PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel CancellationPolicy Response " + oRoomDetails.PropertyRoomBookingID, oCancellationPolicyWebRequest.ResponseLog);
                }
                PropertyDetails.Cancellations.Solidify(SolidifyType.Sum);
            }
            catch (Exception ex)
            {
                bSuccess = false;
                PropertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
            }
            finally
            {
                // save the xml for the front end
                PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Prebook Request", oWebRequest.RequestLog);
                PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Prebook Response", oWebRequest.ResponseLog);

            }

            PropertyDetails.LocalCost = PropertyDetails.Rooms.Sum(r => r.LocalCost);

            return bSuccess;
        }

        #endregion

        #region Book

        public string Book(PropertyDetails PropertyDetails)
        {
            var oResponse = new XmlDocument();
            string sRequestURL = "";
            string sReference = "";

            try
            {

                // build url
                var sbURL = new StringBuilder();
                sbURL.AppendFormat("{0}{1}", _settings.get_BookingURL(PropertyDetails), "?");
                sbURL.AppendFormat("&LangID={0}", _settings.get_LangID(PropertyDetails));
                sbURL.AppendFormat("&UserName={0}", _settings.get_Username(PropertyDetails));
                sbURL.AppendFormat("&Password={0}", _settings.get_Password(PropertyDetails));
                sbURL.AppendFormat("&session_ID={0}", PropertyDetails.Rooms[0].ThirdPartyReference.Split('|')[0]);
                sbURL.AppendFormat("&Checkin_Date={0}", YouTravelSupport.FormatDate(PropertyDetails.ArrivalDate));
                sbURL.AppendFormat("&Nights={0}", PropertyDetails.Duration);
                sbURL.AppendFormat("&Rooms={0}", PropertyDetails.Rooms.Count);

                sbURL.AppendFormat("&HID={0}", PropertyDetails.TPKey);

                // adults and children
                int iRoomIndex = 0;
                foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
                {

                    iRoomIndex += 1;

                    sbURL.AppendFormat("&ADLTS_{0}={1}", iRoomIndex, oRoomDetails.Adults);

                    if (oRoomDetails.Children + oRoomDetails.Infants > 0)
                    {

                        sbURL.AppendFormat("&CHILD_{0}={1}", iRoomIndex, oRoomDetails.Children + oRoomDetails.Infants);

                        for (int i = 1, loopTo = oRoomDetails.Children + oRoomDetails.Infants; i <= loopTo; i++)
                        {

                            if (oRoomDetails.ChildAges.Count > i - 1)
                            {

                                sbURL.AppendFormat("&ChildAgeR{0}C{1}={2}", iRoomIndex, i, oRoomDetails.ChildAges[i - 1]);
                            }

                            else
                            {

                                sbURL.AppendFormat("&ChildAgeR{0}C{1}={2}", iRoomIndex, i, -1);

                            }
                        }
                    }

                    else
                    {

                        sbURL.AppendFormat("&CHILD_{0}=0", iRoomIndex);

                    }

                    if (iRoomIndex == 1)
                    {
                        sbURL.AppendFormat("&RID={0}", oRoomDetails.ThirdPartyReference.Split('|')[1]);
                    }
                    else
                    {
                        sbURL.AppendFormat("&RID_{0}={1}", iRoomIndex, oRoomDetails.ThirdPartyReference.Split('|')[1]);
                    }

                    sbURL.AppendFormat("&Room{0}_Rate={1}", iRoomIndex, oRoomDetails.GrossCost.ToSafeMoney());

                }

                sbURL.AppendFormat("&Customer_title={0}", PropertyDetails.Rooms[0].Passengers[0].Title);
                sbURL.AppendFormat("&Customer_firstname={0}", PropertyDetails.Rooms[0].Passengers[0].FirstName);
                sbURL.AppendFormat("&Customer_Lastname={0}", PropertyDetails.Rooms[0].Passengers[0].LastName);

                if (PropertyDetails.BookingComments.Count > 0)
                {
                    // max 250 characters
                    sbURL.AppendFormat("&Requests={0}", PropertyDetails.BookingComments[0].Text.Substring(0, Math.Min(PropertyDetails.BookingComments[0].Text.Length, 250)));
                }

                sRequestURL = sbURL.ToString();

                var oWebRequest = new Request();
                oWebRequest.Source = ThirdParties.YOUTRAVEL;
                oWebRequest.CreateLog = true;
                oWebRequest.LogFileName = "Book";
                oWebRequest.EndPoint = sRequestURL;
                oWebRequest.Method = eRequestMethod.GET;
                oWebRequest.Send(_httpClient, _logger);

                oResponse = oWebRequest.ResponseXML;

                // return booking reference if it exists
                var oNode = oResponse.SelectSingleNode("/HtSearchRq/Booking_ref");
                if (oNode is not null && !string.IsNullOrEmpty(oResponse.SelectSingleNode("/HtSearchRq/Booking_ref").InnerText))
                {
                    sReference = oResponse.SelectSingleNode("/HtSearchRq/Booking_ref").InnerText;
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
                if (!string.IsNullOrEmpty(sRequestURL))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Book Request", sRequestURL);
                }

                if (!string.IsNullOrEmpty(oResponse.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Book Response", oResponse);
                }

            }

            return sReference;

        }

        #endregion

        #region Cancellation

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails PropertyDetails)
        {

            var oThirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            var sbURL = new StringBuilder();
            var oResponse = new XmlDocument();

            try
            {

                // build url
                sbURL.AppendFormat("{0}{1}", _settings.get_CancellationURL(PropertyDetails), "?");
                sbURL.AppendFormat("Booking_ref={0}", PropertyDetails.SourceReference);
                sbURL.AppendFormat("&UserName={0}", _settings.get_Username(PropertyDetails));
                sbURL.AppendFormat("&Password={0}", _settings.get_Password(PropertyDetails));


                // Send the request
                var oWebRequest = new Request();
                oWebRequest.Source = ThirdParties.YOUTRAVEL;
                oWebRequest.CreateLog = true;
                oWebRequest.LogFileName = "Cancel";
                oWebRequest.EndPoint = sbURL.ToString();
                oWebRequest.Method = eRequestMethod.GET;
                oWebRequest.Send(_httpClient, _logger);

                oResponse = oWebRequest.ResponseXML;


                // check response
                if (!string.IsNullOrEmpty(oResponse.InnerXml))
                {

                    if (oResponse.SelectSingleNode("/HtSearchRq/Success").InnerText == "True")
                    {

                        oThirdPartyCancellationResponse.TPCancellationReference = oResponse.SelectSingleNode("/HtSearchRq/Booking_ref").InnerText;
                        oThirdPartyCancellationResponse.Success = true;
                    }

                    else
                    {

                        oThirdPartyCancellationResponse.Success = false;

                    }

                }
            }

            catch (Exception ex)
            {

                PropertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
                oThirdPartyCancellationResponse.Success = false;
            }

            finally
            {

                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(sbURL.ToString()))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Cancellation Request", sbURL.ToString());
                }

                if (!string.IsNullOrEmpty(oResponse.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Cancellation Response", oResponse);
                }

            }

            return oThirdPartyCancellationResponse;

        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails PropertyDetails)
        {

            var oResult = new ThirdPartyCancellationFeeResult();
            var sbURL = new StringBuilder();
            var oResponse = new XmlDocument();

            try
            {

                sbURL.AppendFormat("{0}{1}", _settings.get_CancellationFeeURL(PropertyDetails), "?");
                sbURL.AppendFormat("Booking_ref={0}", PropertyDetails.SourceReference);
                sbURL.AppendFormat("&UserName={0}", _settings.get_Username(PropertyDetails));
                sbURL.AppendFormat("&Password={0}", _settings.get_Password(PropertyDetails));


                // Send the request
                var oWebRequest = new Request();
                oWebRequest.Source = ThirdParties.YOUTRAVEL;
                oWebRequest.CreateLog = true;
                oWebRequest.LogFileName = "Cancellation Cost";
                oWebRequest.EndPoint = sbURL.ToString();
                oWebRequest.Method = eRequestMethod.GET;
                oWebRequest.Send(_httpClient, _logger);

                oResponse = oWebRequest.ResponseXML;

                // return a fees result if found
                var oNode = oResponse.SelectSingleNode("/HtSearchRq/Success");
                if (oNode is not null && oResponse.SelectSingleNode("/HtSearchRq/Success").InnerText == "True")
                {

                    oResult.Success = true;
                    oResult.Amount = oResponse.SelectSingleNode("/HtSearchRq/Fees").InnerText.ToSafeDecimal();
                    oResult.CurrencyCode = oResponse.SelectSingleNode("/HtSearchRq/Currency").InnerText.ToSafeString();
                }

                else
                {

                    oResult.Success = false;

                }
            }

            catch (Exception ex)
            {

                oResult.Success = false;
                PropertyDetails.Warnings.AddNew("Cancellation Cost Exception", ex.ToString());
            }

            finally
            {

                if (!string.IsNullOrEmpty(sbURL.ToString()))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Cancellation Cost Request", sbURL.ToString());
                }

                if (!string.IsNullOrEmpty(oResponse.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.YOUTRAVEL, "YouTravel Cancellation Cost Response", oResponse);
                }

            }

            return oResult;

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
    }
}