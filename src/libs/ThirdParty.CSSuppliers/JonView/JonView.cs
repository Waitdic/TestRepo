namespace ThirdParty.CSSuppliers.JonView
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class JonView : IThirdParty
    {
        #region Properties

        private readonly IJonViewSettings _settings;

        private readonly ITPSupport _support;

        private readonly HttpClient _httpClient;

        private readonly IMemoryCache _cache;

        private readonly ILogger<JonView> _logger;

        public bool SupportsRemarks
        {
            get
            {
                return false;
            }
        }

        private bool IThirdParty_SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.get_AllowCancellations(searchDetails);
        }

        bool IThirdParty.SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source) => IThirdParty_SupportsLiveCancellation(searchDetails, source);

        public bool SupportsBookingSearch
        {
            get
            {
                return false;
            }
        }

        public string Source => ThirdParties.JONVIEW;

        private bool IThirdParty_TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails)
        {
            return false;
        }

        bool IThirdParty.TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails) => IThirdParty_TakeSavingFromCommissionMargin(searchDetails);

        private int IThirdParty_OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.get_OffsetCancellationDays(searchDetails, false);
        }

        int IThirdParty.OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails) => IThirdParty_OffsetCancellationDays(searchDetails);

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        #endregion

        #region Constructor

        public JonView(IJonViewSettings settings, ITPSupport support, HttpClient httpClient, IMemoryCache cache, ILogger<JonView> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _cache= Ensure.IsNotNull(cache, nameof(cache));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region PreBook

        public bool PreBook(PropertyDetails PropertyDetails)
        {

            try
            {
                GetCancellationPolicy(PropertyDetails);
            }

            catch (Exception ex)
            {

                PropertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                return false;

            }

            return true;

        }

        #endregion

        #region Book

        public string Book(PropertyDetails PropertyDetails)
        {

            var oResponse = new XmlDocument();
            string oRequest = string.Empty;
            string sBookingReference = "";

            try
            {

                // build request
                oRequest = BuildBookingURL(PropertyDetails);

                // send the request
                var oBookingRequest = new Request();
                oBookingRequest.EndPoint = _settings.get_URL(PropertyDetails) + GetRequestHeader(PropertyDetails) + oRequest;
                oBookingRequest.Source = ThirdParties.JONVIEW;
                oBookingRequest.ContentType = ContentTypes.Text_xml;
                oBookingRequest.Send(_httpClient, _logger);

                oResponse = oBookingRequest.ResponseXML;


                // get booking reference
                var oBookingStatus = oResponse.SelectSingleNode("/message/actionseg/status");
                if (oBookingStatus is not null && oBookingStatus.InnerText == "C")
                {
                    sBookingReference = oResponse.SelectSingleNode("/message/actionseg/resnumber").InnerText;
                }


                // return reference or failed
                if (string.IsNullOrEmpty(sBookingReference) || sBookingReference.ToLower() == "error")
                {
                    sBookingReference = "failed";
                }
            }

            catch (Exception ex)
            {
                PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
                sBookingReference = "failed";
            }

            finally
            {

                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(oRequest))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Book Request", oRequest);
                }

                if (!string.IsNullOrEmpty(oResponse.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Book Response", oResponse);
                }

            }

            return sBookingReference;

        }

        #endregion

        #region Get Cancellation Policy

        public void GetCancellationPolicy(PropertyDetails PropertyDetails)
        {

            // create an array variable to hold the policy for each room
            var aPolicies = new Cancellations[PropertyDetails.Rooms.Count];

            // we'll need this regular expression too (declared here for efficiency)
            var oDailyRegEx = new System.Text.RegularExpressions.Regex(@"on (?<type>\S+) (?<numberofdays>[0-9]+) day(\(s\))?");

            // loop through the rooms
            int iLoop = 0;
            foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
            {

                // build request
                string cancellationURL = BuildCancellationURL(PropertyDetails, oRoomDetails);

                // get response
                var oResponse = new XmlDocument();

                var oWebRequest = new Request();
                oWebRequest.EndPoint = _settings.get_URL(PropertyDetails) + GetRequestHeader(PropertyDetails) + cancellationURL;
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.Source = ThirdParties.JONVIEW;
                oWebRequest.LogFileName = "CancellationPolicy";
                oWebRequest.CreateLog = true;
                oWebRequest.Send(_httpClient, _logger);

                oResponse = oWebRequest.ResponseXML;


                // make sure we initialize the final policy for this room
                aPolicies[iLoop] = new Cancellations();


                // check the status
                if (oResponse.SafeNodeValue("/message/actionseg/status") == "C")
                {

                    // we need to get the end date and amount for each cancellation item, and add them to the final cancellation policy for this room
                    foreach (XmlNode oCancellationNode in oResponse.SelectNodes("/message/productlistseg/listrecord/cancellation/canitem"))
                    {

                        int iFromDaysBeforeArrival = oCancellationNode.SelectSingleNode("fromdays").InnerText.ToSafeInt();
                        int iToDaysBeforeArrival = oCancellationNode.SelectSingleNode("todays").InnerText.ToSafeInt();
                        string sChargeType = oCancellationNode.SelectSingleNode("chargetype").InnerText;
                        string sCancellationRateType = oCancellationNode.SelectSingleNode("ratetype").InnerText;
                        double nCancellationRate = (double)oCancellationNode.SelectSingleNode("canrate").InnerText.ToSafeDecimal();

                        var oNoteNode = oCancellationNode.SelectSingleNode("cannote");
                        string sNote = oNoteNode is not null ? oNoteNode.InnerText : "";


                        // get start date
                        var dStartDate = PropertyDetails.ArrivalDate.AddDays(-iFromDaysBeforeArrival);
                        if (iToDaysBeforeArrival < 0)
                        {
                            dStartDate = PropertyDetails.ArrivalDate;
                        }
                        else
                        {
                            dStartDate = PropertyDetails.ArrivalDate.AddDays(-iFromDaysBeforeArrival);
                        }

                        // get end date
                        DateTime dEndDate;
                        if (iToDaysBeforeArrival < 0)
                        {
                            dEndDate = PropertyDetails.ArrivalDate;
                        }
                        else
                        {
                            dEndDate = PropertyDetails.ArrivalDate.AddDays(-iToDaysBeforeArrival);
                        }


                        // calculate the base amounts (the amounts we're going to use to get the final amount from)
                        var aBaseAmounts = new List<decimal>();

                        switch (sChargeType ?? "")
                        {

                            case "EI":
                            case "ENTIRE ITEM":
                                {
                                    aBaseAmounts.Add(oRoomDetails.LocalCost);
                                    break;
                                }

                            case "P":
                            case "PER PERSON": // unfortunately we have to guess these as we are using the wrong search request (CT instead of CU)
                                {

                                    for (int i = 1, loopTo = oRoomDetails.Adults + oRoomDetails.Children; i <= loopTo; i++)
                                        aBaseAmounts.Add(oRoomDetails.LocalCost / oRoomDetails.Adults + oRoomDetails.Children);
                                    break;
                                }

                            case "DAILY":
                                {

                                    var oDailyRegMatch = oDailyRegEx.Match(sNote);
                                    string sType = oDailyRegMatch.Groups["type"].Value.ToLower();
                                    int iNumberOfDays = oDailyRegMatch.Groups["numberofdays"].Value.ToSafeInt();

                                    // make sure we don't get zero rates (we have to be careful of this because if there is a 'stay X pay Y' special offer,
                                    // often the first night will be zero - and the cancellation fee should be based on the second night instead)
                                    var aRates = Array.FindAll(oRoomDetails.ThirdPartyReference.Split('_')[1].Split('/'), (sRate) => sRate.ToSafeDecimal() != 0m);

                                    var aRatesWeWant = new string[iNumberOfDays];
                                    var iSourceIndex = default(int);

                                    // I'm not sure the type is ever anything other than 'first' but I thought I'd check here just in case
                                    if (sType == "first")
                                    {
                                        iSourceIndex = 0;
                                    }
                                    else if (sType == "last")
                                    {
                                        iSourceIndex = aRates.Length - iNumberOfDays;
                                    }

                                    if (aRates.Length > iSourceIndex)
                                        Array.ConstrainedCopy(aRates, iSourceIndex, aRatesWeWant, 0, iNumberOfDays);

                                    foreach (string sRate in aRatesWeWant)
                                        aBaseAmounts.Add(sRate.ToSafeDecimal());
                                    break;
                                }

                        }


                        // now, for each base amount, we're going to either add a value or a percentage to the final amount
                        double nFinalAmountForThisRule = 0d;

                        foreach (decimal nAmount in aBaseAmounts)
                        {

                            switch (sCancellationRateType ?? "")
                            {

                                case "D": // fixed amount in dollars
                                    {
                                        nFinalAmountForThisRule += (double)nAmount;
                                        break;
                                    }

                                case "P": // percentage of base amount
                                    {
                                        nFinalAmountForThisRule += (double)nAmount * (nCancellationRate / 100.0d);
                                        break;
                                    }

                            }

                        }

                        // we've got everything we need (finally) - now lets add it to the policy
                        aPolicies[iLoop].AddNew(dStartDate, dEndDate, nFinalAmountForThisRule.ToSafeDecimal());

                    }


                    // solidify the policy (turns our random collection of rules into a proper (continuous) policy ready for merging)
                    aPolicies[iLoop].Solidify(SolidifyType.Max, new DateTime(2099, 12, 31), oRoomDetails.LocalCost);

                }


                // increment the loop counter 
                iLoop += 1;

                // Add the Logs to the booking
                if (!string.IsNullOrEmpty(cancellationURL))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Cancellation Costs Request", cancellationURL);
                }

                if (cancellationURL is not null)
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Cancellation Costs Response", cancellationURL);
                }

            }

            // merge the policies and add it to the booking
            PropertyDetails.Cancellations = Cancellations.MergeMultipleCancellationPolicies(aPolicies);

        }

        #endregion

        #region Cancellation

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails PropertyDetails)
        {

            var oThirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            string sRequest = "";
            var oResponse = new XmlDocument();

            try
            {

                // build request
                sRequest = BuildReservationCancellationURL(PropertyDetails.SourceReference);

                // Send the request
                var oWebRequest = new Request();
                oWebRequest.EndPoint = _settings.get_URL(PropertyDetails) + GetRequestHeader(PropertyDetails) + sRequest;
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.Source = ThirdParties.JONVIEW;
                oWebRequest.LogFileName = "Cancel";
                oWebRequest.CreateLog = true;
                oWebRequest.Send(_httpClient, _logger);


                oResponse = oWebRequest.ResponseXML;


                // get reference
                if (oResponse.SelectSingleNode("message/actionseg/status").InnerText == "D")
                {
                    oThirdPartyCancellationResponse.TPCancellationReference = DateTime.Now.ToString("yyyyMMddhhmm");
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
                if (!string.IsNullOrEmpty(sRequest))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "Cancellation Request", sRequest);
                }

                if (!string.IsNullOrEmpty(oResponse.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "Cancellation Response", oResponse);
                }

            }

            return oThirdPartyCancellationResponse;

        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails PropertyDetails)
        {
            return new ThirdPartyCancellationFeeResult();
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

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        #endregion

        #region Support

        private string BuildBookingURL(PropertyDetails propertyDetails)
        {
            var message = new StringBuilder();

            message.Append("<message>");
            message.Append("<actionseg>AR</actionseg>");
            message.Append("<commitlevelseg>1</commitlevelseg>");
            message.Append("<resinfoseg>");
            message.AppendFormat("<refitem>{0}</refitem>", DateTime.Now.ToString("yyyyMMddhhmm"));
            message.Append("<attitem>host</attitem>");
            message.Append("<resitem></resitem>");
            message.Append("</resinfoseg>");
            message.Append("<paxseg>");

            // for each guest in each room
            int iGuestIndex = 0;
            foreach (RoomDetails oRoomDetails in propertyDetails.Rooms)
            {
                foreach (Passenger oPassenger in oRoomDetails.Passengers)
                {

                    iGuestIndex += 1;

                    message.Append("<paxrecord>");
                    message.AppendFormat("<paxnum>{0}</paxnum>", iGuestIndex);
                    message.Append("<paxseq></paxseq>");
                    message.AppendFormat("<titlecode>{0}</titlecode>", GetTitle(oPassenger.Title.ToUpper()));
                    message.AppendFormat("<fname>{0}</fname>", oPassenger.FirstName);
                    message.AppendFormat("<lname>{0}</lname>", oPassenger.LastName);

                    string sAge = "";
                    if (oPassenger.PassengerType == PassengerType.Child)
                    {
                        sAge = oPassenger.Age.ToString();
                    }
                    else if (oPassenger.PassengerType == PassengerType.Infant)
                    {
                        sAge = "1";
                    }

                    message.AppendFormat("<age>{0}</age>", sAge);
                    message.Append("<language>EN</language>");
                    message.Append("</paxrecord>");

                }
            }

            message.Append("</paxseg>");
            message.Append("<bookseg>");


            // get hotel request if it exists
            string sComment = "";
            foreach (BookingComment oComment in propertyDetails.BookingComments)
                sComment += oComment.Text + " ";


            // for each room
            int iRoomIndex = 0;
            int incRoomGuestCount = 0;
            foreach (RoomDetails oRoomDetails in propertyDetails.Rooms)
            {

                iRoomIndex += 1;

                message.Append("<bookrecord>");
                message.AppendFormat("<booknum>{0}</booknum>", iRoomIndex);
                message.Append("<bookseq></bookseq>");
                message.AppendFormat("<prodcode>{0}</prodcode>", oRoomDetails.ThirdPartyReference.Split('_')[0]);
                message.AppendFormat("<startdate>{0}</startdate>", propertyDetails.ArrivalDate.ToString("dd-MMM-yyyy"));
                message.AppendFormat("<duration>{0}</duration>", propertyDetails.Duration);
                message.AppendFormat("<note>{0}</note>", sComment);

                var sbPaxArray = new StringBuilder();

                int iRoomGuestCount = oRoomDetails.Passengers.Count;
                for (int i = 1, loopTo = propertyDetails.Adults + propertyDetails.Children + propertyDetails.Infants; i <= loopTo; i++)
                {

                    if (i >= incRoomGuestCount + 1 && i < incRoomGuestCount + 1 + iRoomGuestCount)
                    {
                        sbPaxArray.Append("Y");
                    }
                    else
                    {
                        sbPaxArray.Append("N");
                    }

                }

                incRoomGuestCount += iRoomGuestCount;

                message.AppendFormat("<paxarray>{0}</paxarray>", sbPaxArray.ToString());
                message.Append("</bookrecord>");

            }

            message.Append("</bookseg>");
            message.Append("</message>");

            return message.ToString();

        }

        private string BuildReservationCancellationURL(string bookingReference)
        {
            var message = new StringBuilder();

            message.Append("<message>");
            message.Append("<actionseg>CR</actionseg>");
            message.Append("<resinfoseg>");
            message.AppendFormat("<resitem>{0}</resitem>", bookingReference);
            message.Append("</resinfoseg>");
            message.Append("</message>");

            return message.ToString();
        }

        public string GetTitle(string sTitle)
        {
            var oTitle = _cache.GetOrCreate("JonViewTitle", () =>
                                                {
                                                    var oTitles = new List<string>();
                                                    var sTitles = new StringReader(JonViewRes.Title);
                                                    string sTitlesLine = sTitles.ReadLine();
                                                    while (sTitlesLine is not null)
                                                    {

                                                        if (!oTitles.Contains(sTitlesLine))
                                                        {
                                                            oTitles.Add(sTitlesLine);
                                                        }
                                                        sTitlesLine = sTitles.ReadLine();
                                                    }
                                                    return oTitles;
                                                }, 9999);

            if (oTitle.Contains(sTitle))
            {
                return sTitle;
            }
            else
            {
                return "MR";
            }
        }

        private string BuildCancellationURL(PropertyDetails propertyDetails, RoomDetails roomDetails)
        {
            var message = new StringBuilder();

            message.Append("<message>");
            message.Append("<actionseg>DP</actionseg>");
            message.Append("<searchseg>");
            message.Append("<changesince></changesince>");
            message.AppendFormat("<fromdate>{0}</fromdate>", propertyDetails.ArrivalDate.ToString("dd-MMM-yyyy"));
            message.AppendFormat("<todate>{0}</todate>", propertyDetails.DepartureDate.ToString("dd-MMM-yyyy"));
            message.Append("<prodtypecode>ALL</prodtypecode>");
            message.Append("<searchtype>PROD</searchtype>");
            message.Append("<productlistseg>");
            message.Append("<codeitem>");
            message.AppendFormat("<productcode>{0}</productcode>", roomDetails.ThirdPartyReference.Split('_')[0]);
            message.Append("</codeitem>");
            message.Append("</productlistseg>");
            message.Append("<displayrestriction>N</displayrestriction>");
            message.Append("<displaypolicy>Y</displaypolicy>");
            message.Append("<displayschdate>N</displayschdate>");
            message.Append("</searchseg>");
            message.Append("</message>");
            message.Append("<displaynamedetails>Y</displaynamedetails>");
            message.Append("<displayusage>Y</displayusage>");
            message.Append("<displaygeocode>Y</displaygeocode>");
            message.Append("<displaydynamicrates>Y</displaydynamicrates>");

            return message.ToString();
        }

        private string GetRequestHeader(IThirdPartyAttributeSearch searchDetails)
        {
            var header = new StringBuilder();
            header.AppendFormat("?actioncode=HOSTXML&clientlocseq={0}&userid={1}&" + "password={2}&message=<?xml version=\"1.0\" encoding=\"UTF-8\"?>", _settings.get_ClientLoc(searchDetails), _settings.get_UserID(searchDetails), _settings.get_Password(searchDetails));

            return header.ToSafeString();
        }

        #endregion

        #region End session
        public void EndSession(PropertyDetails oPropertyDetails)
        {

        }

        #endregion

    }
}