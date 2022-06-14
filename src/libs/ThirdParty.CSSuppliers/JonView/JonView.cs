namespace ThirdParty.CSSuppliers.JonView
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class JonView : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IJonViewSettings _settings;

        private readonly HttpClient _httpClient;

        private readonly IMemoryCache _cache;

        private readonly ILogger<JonView> _logger;

        public bool SupportsRemarks => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.get_AllowCancellations(searchDetails);
        }

        public bool SupportsBookingSearch => false;

        public string Source => ThirdParties.JONVIEW;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.get_OffsetCancellationDays(searchDetails, false);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        #endregion

        #region Constructor

        public JonView(IJonViewSettings settings, HttpClient httpClient, IMemoryCache cache, ILogger<JonView> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _cache= Ensure.IsNotNull(cache, nameof(cache));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            try
            {
                await GetCancellationPolicyAsync(propertyDetails);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                return false;
            }

            return true;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var response = new XmlDocument();
            string request = string.Empty;
            string bookingReference = "";

            try
            {
                // build request
                request = BuildBookingURL(propertyDetails);

                // send the request
                var bookingRequest = new Request
                {
                    EndPoint = _settings.get_URL(propertyDetails) + GetRequestHeader(propertyDetails) + request,
                    Source = ThirdParties.JONVIEW,
                    ContentType = ContentTypes.Text_xml,
                    LogFileName = "Book",
                    CreateLog = true
                };
                await bookingRequest.Send(_httpClient, _logger);

                response = bookingRequest.ResponseXML;

                // get booking reference
                var bookingStatus = response.SelectSingleNode("/message/actionseg/status");
                if (bookingStatus is not null && bookingStatus.InnerText == "C")
                {
                    bookingReference = response.SelectSingleNode("/message/actionseg/resnumber").InnerText;
                }

                // return reference or failed
                if (string.IsNullOrEmpty(bookingReference) || bookingReference.ToLower() == "error")
                {
                    bookingReference = "failed";
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
                bookingReference = "failed";
            }
            finally
            {
                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(request))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Book Request", request);
                }

                if (!string.IsNullOrEmpty(response.InnerXml))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Book Response", response);
                }
            }

            return bookingReference;
        }

        #endregion

        #region Get Cancellation Policy

        public async Task GetCancellationPolicyAsync(PropertyDetails propertyDetails)
        {
            // create an array variable to hold the policy for each room
            var policies = new Cancellations[propertyDetails.Rooms.Count];

            // we'll need this regular expression too (declared here for efficiency)
            var dailyRegEx = new System.Text.RegularExpressions.Regex(@"on (?<type>\S+) (?<numberofdays>[0-9]+) day(\(s\))?");

            // loop through the rooms
            int loop = 0;
            foreach (var roomDetails in propertyDetails.Rooms)
            {
                // build request
                string cancellationURL = BuildCancellationURL(propertyDetails, roomDetails);

                // get response
                var response = new XmlDocument();

                var webRequest = new Request
                {
                    EndPoint = _settings.get_URL(propertyDetails) + GetRequestHeader(propertyDetails) + cancellationURL,
                    ContentType = ContentTypes.Text_xml,
                    Source = ThirdParties.JONVIEW,
                    LogFileName = "CancellationPolicy",
                    CreateLog = true
                };
                await webRequest.Send(_httpClient, _logger);

                response = webRequest.ResponseXML;

                // make sure we initialize the final policy for this room
                policies[loop] = new Cancellations();

                // check the status
                if (response.SafeNodeValue("/message/actionseg/status") == "C")
                {
                    // we need to get the end date and amount for each cancellation item, and add them to the final cancellation policy for this room
                    foreach (XmlNode cancellationNode in response.SelectNodes("/message/productlistseg/listrecord/cancellation/canitem"))
                    {
                        int fromDaysBeforeArrival = cancellationNode.SelectSingleNode("fromdays").InnerText.ToSafeInt();
                        int toDaysBeforeArrival = cancellationNode.SelectSingleNode("todays").InnerText.ToSafeInt();
                        string chargeType = cancellationNode.SelectSingleNode("chargetype").InnerText;
                        string cancellationRateType = cancellationNode.SelectSingleNode("ratetype").InnerText;
                        double cancellationRate = (double)cancellationNode.SelectSingleNode("canrate").InnerText.ToSafeDecimal();

                        var noteNode = cancellationNode.SelectSingleNode("cannote");
                        string note = noteNode is not null ? noteNode.InnerText : "";

                        // get start date
                        var startDate = propertyDetails.ArrivalDate.AddDays(-fromDaysBeforeArrival);
                        if (toDaysBeforeArrival < 0)
                        {
                            startDate = propertyDetails.ArrivalDate;
                        }
                        else
                        {
                            startDate = propertyDetails.ArrivalDate.AddDays(-fromDaysBeforeArrival);
                        }

                        // get end date
                        DateTime endDate;
                        if (toDaysBeforeArrival < 0)
                        {
                            endDate = propertyDetails.ArrivalDate;
                        }
                        else
                        {
                            endDate = propertyDetails.ArrivalDate.AddDays(-toDaysBeforeArrival);
                        }

                        // calculate the base amounts (the amounts we're going to use to get the final amount from)
                        var baseAmounts = new List<decimal>();

                        switch (chargeType ?? "")
                        {
                            case "EI":
                            case "ENTIRE ITEM":
                                {
                                    baseAmounts.Add(roomDetails.LocalCost);
                                    break;
                                }

                            case "P":
                            case "PER PERSON": // unfortunately we have to guess these as we are using the wrong search request (CT instead of CU)
                                {
                                    for (int i = 1; i <= roomDetails.Adults + roomDetails.Children; i++)
                                    {
                                        baseAmounts.Add(roomDetails.LocalCost / roomDetails.Adults + roomDetails.Children);
                                    }
                                    break;
                                }

                            case "DAILY":
                                {
                                    var dailyRegMatch = dailyRegEx.Match(note);
                                    string type = dailyRegMatch.Groups["type"].Value.ToLower();
                                    int numberOfDays = dailyRegMatch.Groups["numberofdays"].Value.ToSafeInt();

                                    // make sure we don't get zero rates (we have to be careful of this because if there is a 'stay X pay Y' special offer,
                                    // often the first night will be zero - and the cancellation fee should be based on the second night instead)
                                    var rates = Array.FindAll(roomDetails.ThirdPartyReference.Split('_')[1].Split('/'), (sRate) => sRate.ToSafeDecimal() != 0m);

                                    var ratesWeWant = new string[numberOfDays];
                                    var sourceIndex = default(int);

                                    // I'm not sure the type is ever anything other than 'first' but I thought I'd check here just in case
                                    if (type == "first")
                                    {
                                        sourceIndex = 0;
                                    }
                                    else if (type == "last")
                                    {
                                        sourceIndex = rates.Length - numberOfDays;
                                    }

                                    if (rates.Length > sourceIndex)
                                    {
                                        Array.ConstrainedCopy(rates, sourceIndex, ratesWeWant, 0, numberOfDays);
                                    }

                                    foreach (string rate in ratesWeWant)
                                    {
                                        baseAmounts.Add(rate.ToSafeDecimal());
                                    }
                                    break;
                                }
                        }

                        // now, for each base amount, we're going to either add a value or a percentage to the final amount
                        double finalAmountForThisRule = 0d;

                        foreach (decimal amount in baseAmounts)
                        {
                            switch (cancellationRateType ?? "")
                            {
                                case "D": // fixed amount in dollars
                                    {
                                        finalAmountForThisRule += (double)amount;
                                        break;
                                    }

                                case "P": // percentage of base amount
                                    {
                                        finalAmountForThisRule += (double)amount * (cancellationRate / 100.0d);
                                        break;
                                    }
                            }
                        }

                        // we've got everything we need (finally) - now lets add it to the policy
                        policies[loop].AddNew(startDate, endDate, finalAmountForThisRule.ToSafeDecimal());
                    }

                    // solidify the policy (turns our random collection of rules into a proper (continuous) policy ready for merging)
                    policies[loop].Solidify(SolidifyType.Max, new DateTime(2099, 12, 31), roomDetails.LocalCost);
                }

                // increment the loop counter 
                loop++;

                // Add the Logs to the booking
                if (!string.IsNullOrEmpty(cancellationURL))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Cancellation Costs Request", cancellationURL);
                }

                if (cancellationURL is not null)
                {
                    propertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "JonView Cancellation Costs Response", cancellationURL);
                }
            }

            // merge the policies and add it to the booking
            propertyDetails.Cancellations = Cancellations.MergeMultipleCancellationPolicies(policies);
        }

        #endregion

        #region Cancellation

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            string request = "";
            var response = new XmlDocument();

            try
            {
                // build request
                request = BuildReservationCancellationURL(propertyDetails.SourceReference);

                // Send the request
                var webRequest = new Request
                {
                    EndPoint = _settings.get_URL(propertyDetails) + GetRequestHeader(propertyDetails) + request,
                    ContentType = ContentTypes.Text_xml,
                    Source = ThirdParties.JONVIEW,
                    LogFileName = "Cancel",
                    CreateLog = true
                };
                await webRequest.Send(_httpClient, _logger);

                response = webRequest.ResponseXML;

                // get reference
                if (response.SelectSingleNode("message/actionseg/status").InnerText == "D")
                {
                    thirdPartyCancellationResponse.TPCancellationReference = DateTime.Now.ToString("yyyyMMddhhmm");
                    thirdPartyCancellationResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString());
                thirdPartyCancellationResponse.Success = false;
            }
            finally
            {
                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(request))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "Cancellation Request", request);
                }

                if (!string.IsNullOrEmpty(response.InnerXml))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.JONVIEW, "Cancellation Response", response);
                }
            }

            return thirdPartyCancellationResponse;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails PropertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
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
            string sComment = propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any() ?
                     string.Join("\n", propertyDetails.Rooms.Select(x => x.SpecialRequest)) :
                     "";

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