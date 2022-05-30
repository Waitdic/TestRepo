using System;
using System.Collections.Generic;
using System.Text;

namespace ThirdParty.CSSuppliers.DOTW
{
    using System;
    using System.Collections.Generic;
    using Intuitive;
    using Intuitive.Net.WebRequests; 
    using Newtonsoft.Json;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Models;
    using Intuitive.Helpers.Extensions;
    using ThirdParty.Search.Models;
    using Intuitive.Helpers.Security;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Lookups;
    using System.Xml;

    public class DOTW : IThirdParty
    {

        #region Constructor

        public DOTW(IDOTWSettings settings, ITPSupport support, HttpClient httpClient, ILogger<DOTW> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region Properties
        public string Source => ThirdParties.DOTW;

        private readonly IDOTWSettings _settings;

        private readonly ITPSupport _support;

        private readonly HttpClient _httpClient;

        private readonly ILogger<DOTW> _logger;

        private readonly ISecretKeeper _secretKeeper;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public bool SupportsRemarks => false;

        public bool SupportsBookingSearch => false;

        public bool TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails)
        {
            return false;
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.OffsetCancellationDays(searchDetails, false);
        }

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        #endregion

        #region PreBook

        public bool PreBook(PropertyDetails PropertyDetails)
        {

            try
            {
                GetAllocationReferences(PropertyDetails);
                BlockRooms(PropertyDetails);
            }

            catch (Exception ex)
            {

                PropertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                return false;

            }

            return true;
        }

        #region Sub Methods of the pre book

        private void GetAllocationReferences(PropertyDetails PropertyDetails)
        {

            // get the room rates so we can get the stupidly long allocationDetails code
            string sRequest = BuildPreBookRequest(PropertyDetails);
            var oRequest = new XmlDocument();
            oRequest.LoadXml(sRequest);

            var oHeaders = new Intuitive.Net.WebRequests.RequestHeaders();
            if (_settings.UseGZip(PropertyDetails))
            {
                oHeaders.AddNew("Accept-Encoding", "gzip");
            }


            // Get the response 
            var oWebRequest = new Request();
            oWebRequest.EndPoint = _settings.ServerURL(PropertyDetails);
            oWebRequest.Method = Intuitive.Net.WebRequests.eRequestMethod.POST;
            oWebRequest.Source = ThirdParties.DOTW;
            oWebRequest.Headers = oHeaders;
            oWebRequest.LogFileName = "Prebook Room";
            oWebRequest.SetRequest(sRequest);
            oWebRequest.ContentType = ContentTypes.Text_xml;
            oWebRequest.CreateLog = true;
            oWebRequest.Send(_httpClient, _logger).RunSynchronously();

            XmlDocument oResponse;
            oResponse = oWebRequest.ResponseXML;

            oResponse.InnerXml = DOTWSupport.StripNameSpaces(oResponse.InnerXml);


            // check for a valid response
            var oSuccessNode = oResponse.SelectSingleNode("result/successful");
            if (oSuccessNode is null || oSuccessNode.InnerText != "TRUE")
            {
                throw new Exception("booking response does not return success");
            }


            // loop through each room and get the relevant allocationDetails string and append to TPReference
            int iRoomRunNo = 0;
            foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
            {

                // get the room code and meal basis for the predicate
                string sRoomTypeCode = oRoomDetails.ThirdPartyReference.Split('|')[0];
                string sMealBasis = oRoomDetails.ThirdPartyReference.Split('|')[1];

                // build the predicate
                string sPredicate = string.Format("/result/hotel/rooms/room[@runno='{0}']/roomType[@roomtypecode='{1}']/rateBases/rateBasis[@id='{2}']/", iRoomRunNo, sRoomTypeCode, sMealBasis);

                // grab the allocationDetails using the predicate
                var oAllocationNode = oResponse.SelectSingleNode(sPredicate + "allocationDetails");
                if (oAllocationNode is null)
                    throw new Exception("Allocation Details could not be found in prebook");

                // assign the allocation details to the TPReference
                oRoomDetails.ThirdPartyReference += "|" + oAllocationNode.InnerText;

                // increment for each room
                iRoomRunNo += 1;

                // add errata
                string sTariffNotes = XmlNodeExtensions.SafeNodeValue(oResponse, sPredicate + "tariffNotes");
                if (!string.IsNullOrEmpty(sTariffNotes))
                {
                    PropertyDetails.Errata.AddNew("Important Information", sTariffNotes);
                }

            }

            // store the request and response xml on the property booking
            PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Availability Request", sRequest);
            PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Availability Response", oResponse);

        }

        private void BlockRooms(PropertyDetails PropertyDetails)
        {

            // block the rooms
            string sRequest = BuildBlockRequest(PropertyDetails);
            var oRequest = new XmlDocument();
            oRequest.LoadXml(sRequest);

            var oHeaders = new Intuitive.Net.WebRequests.RequestHeaders();
            if (_settings.UseGZip(PropertyDetails))
            {
                oHeaders.AddNew("Accept-Encoding", "gzip");
            }


            // Get the response 
            var oWebRequest = new Request();
            oWebRequest.EndPoint = _settings.ServerURL(PropertyDetails);
            oWebRequest.Method = Intuitive.Net.WebRequests.eRequestMethod.POST;
            oWebRequest.Source = ThirdParties.DOTW;
            oWebRequest.Headers = oHeaders;
            oWebRequest.LogFileName = "Prebook Block Room";
            oWebRequest.SetRequest(sRequest);
            oWebRequest.ContentType = ContentTypes.Text_xml;
            oWebRequest.CreateLog = true;
            oWebRequest.Send(_httpClient, _logger).RunSynchronously();

            XmlDocument oResponse;
            oResponse = oWebRequest.ResponseXML;
            oResponse.InnerXml = DOTWSupport.StripNameSpaces(oResponse.InnerXml);


            // check for a valid response
            var oSuccessNode = oResponse.SelectSingleNode("result/successful");
            if (oSuccessNode is null || oSuccessNode.InnerText != "TRUE")
            {
                throw new Exception("block response does not return success");
            }


            int iRoomRunNo = 0;
            decimal nNewLocalCost = 0m;

            // loop through each room and get the relevant allocationDetails string and append to TPReference
            foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
            {

                // get the room code and meal basis for the predicate
                string sRoomTypeCode = oRoomDetails.ThirdPartyReference.Split('|')[0];
                string sMealBasis = oRoomDetails.ThirdPartyReference.Split('|')[1];

                string sCurrentAllocationDetails = oRoomDetails.ThirdPartyReference.Split('|')[2];

                // We get one status = checked per room booked. This is the one we want. Use it. Please.
                string sPredicate = string.Format("/result/hotel/rooms/room[@runno='{0}']/roomType[@roomtypecode='{1}']/rateBases/rateBasis[@id='{2}'][status='checked']", iRoomRunNo, sRoomTypeCode, sMealBasis);

                var oResultNode = oResponse.SelectSingleNode(sPredicate);

                if (oResultNode is null)
                    throw new Exception("room type could not be blocked");

                // grab the allocationDetails using the predicate
                var oAllocationNode = oResultNode.SelectSingleNode("allocationDetails");
                if (oAllocationNode is null)
                    throw new Exception("Allocation Details could not be found in prebook");


                // assign the allocation details to the TPReference
                oRoomDetails.ThirdPartyReference = oRoomDetails.ThirdPartyReference.Replace(sCurrentAllocationDetails, oAllocationNode.InnerText);

                // Check for Price Changes for each room booking		
                decimal nRoomCost = 0m;
                try
                {
                    nRoomCost = (oResultNode.SelectSingleNode("total/formatted").InnerText).ToSafeDecimal();

                    if (_settings.UseMinimumSellingPrice(PropertyDetails) && oResultNode.SelectSingleNode("totalMinimumSelling/formatted") is not null)
                    {
                        nRoomCost = (oResultNode.SelectSingleNode("totalMinimumSelling/formatted").InnerText).ToSafeDecimal();
                    }
                }
                catch (Exception ex)
                {
                    // probably no minimumselling price listed, ignore exception and use the normal total 
                }

                if (nRoomCost != oRoomDetails.LocalCost && nRoomCost != 0m)
                {

                    oRoomDetails.LocalCost = nRoomCost;
                    oRoomDetails.GrossCost = nRoomCost;

                }

                nNewLocalCost = nNewLocalCost + oRoomDetails.LocalCost;


                // increment the roomrunno 
                iRoomRunNo += 1;

            }

            // have to recalculate costs after price changes or cancellations will use the wrong cost!!!
            if (PropertyDetails.LocalCost != nNewLocalCost)
            {
                PropertyDetails.LocalCost = nNewLocalCost;
            }

            // get the cancellation policy
            PropertyDetails.Cancellations.AddRange(GetCancellationPolicy(PropertyDetails, oResponse));

            // store the request and response xml on the property booking
            PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Pre-Book Request", sRequest);
            PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Pre-Book Response", oResponse);

        }

        #endregion

        #endregion

        #region Book

        public string Book(PropertyDetails PropertyDetails)
        {

            string sReturnReference = "";
            var oResponse = new XmlDocument();
            var oRequest = new XmlDocument();

            try
            {

                // build request
                string sRequest = BuildBookingRequest(PropertyDetails);

                var oHeaders = new Intuitive.Net.WebRequests.RequestHeaders();
                if (_settings.UseGZip(PropertyDetails))
                {
                    oHeaders.AddNew("Accept-Encoding", "gzip");
                }

                // Get the response 
                var oWebRequest = new Request();
                oWebRequest.EndPoint = _settings.ServerURL(PropertyDetails);
                oWebRequest.Method = Intuitive.Net.WebRequests.eRequestMethod.POST;
                oWebRequest.Source = ThirdParties.DOTW;
                oWebRequest.Headers = oHeaders;
                oWebRequest.LogFileName = "Book";
                oWebRequest.SetRequest(sRequest);
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.CreateLog = true;
                oWebRequest.Send(_httpClient, _logger).RunSynchronously();

                oRequest.LoadXml(sRequest);
                oResponse = oWebRequest.ResponseXML;

                // check according to documentation that there is a success node with the value TRUE in it
                var oSuccessNode = oResponse.SelectSingleNode("result/successful");
                if (oSuccessNode is null || oSuccessNode.InnerText != "TRUE")
                {
                    throw new Exception("booking response does not return success");
                }

                // now get booking nodes
                var oBookings = oResponse.SelectSingleNode("result/returnedCode");

                if (oBookings is not null)
                {

                    // concatenate the various references for each room component into a booking comment
                    var oReferenceNodeList = oResponse.SelectNodes("/result/bookings/booking/bookingReferenceNumber");

                    if (oReferenceNodeList.Count > 1)
                    {

                        // create a booking comment on this property booking with all the room references in it.
                        var oSBReferences = new StringBuilder();
                        oSBReferences.Append("DOTW room booking references ");

                        for (int iNode = 0, loopTo = oReferenceNodeList.Count - 1; iNode <= loopTo; iNode++)
                        {
                            oSBReferences.Append(oReferenceNodeList[iNode].InnerText);
                            if (iNode != oReferenceNodeList.Count - 1)
                                oSBReferences.Append(", ");
                        }

                        PropertyDetails.BookingComments.AddNew(oSBReferences.ToString());

                    }

                    // get the reference of the booking which is displayed on their website as a reference
                    sReturnReference = oReferenceNodeList[0].InnerText;
                    PropertyDetails.SourceSecondaryReference = oResponse.SelectSingleNode("result/returnedCode").InnerText;
                }

                else
                {
                    throw new Exception("no bookings found in booking response xml");
                }
            }

            catch (Exception ex)
            {

                PropertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
                sReturnReference = "failed";
            }

            finally
            {

                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(oRequest.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Book Request", oRequest);
                }

                if (!string.IsNullOrEmpty(oResponse.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Book Response", oResponse);
                }

            }

            return sReturnReference;

        }


        #endregion

        #region Get Cancellation Policy

        public Cancellations GetCancellationPolicy(PropertyDetails PropertyDetails, XmlDocument oRoomXML)
        {

            // create an array variable to hold the policy for each room
            var aPolicies = new Cancellations[PropertyDetails.Rooms.Count];


            // loop through the rooms
            int iLoop = 0;
            foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
            {

                // get the room code and meal basis for the predicate
                string sRoomTypeCode = oRoomDetails.ThirdPartyReference.Split('|')[0];
                string sMealBasis = oRoomDetails.ThirdPartyReference.Split('|')[1];

                // build the predicate
                string sPredicate = string.Format("/result/hotel/rooms/room[@runno='{0}']/roomType[@roomtypecode='{1}']/rateBases/rateBasis[@id='{2}']", iLoop, sRoomTypeCode, sMealBasis);


                // get the cancellation deadline if it exists - we have to strip this out of a text string because dotw are idiots
                DateTime dCancellationDeadline = DateTimeExtensions.EmptyDate;

                var oCancellationNode = oRoomXML.SelectSingleNode(sPredicate + "/cancellation");
                if (oCancellationNode is not null && oCancellationNode.InnerText.StartsWith("Cancellation Deadline: "))
                {
                    dCancellationDeadline = SafeTypeExtensions.ToSafeDate(oCancellationNode.InnerText.Substring(23).Replace(" hrs", "")).Date;
                }

                // Dim oTotalCostNode As XmlNode = oRoomXML.SelectSingleNode(sPredicate & "/total")

                // add the rules into the policy for this room
                // in version 2 they have added a no show policy element for some of the properties which doesn't have a to or from date just a charge so we will add our or dates
                // as we go through if this is case
                aPolicies[iLoop] = new Cancellations();

                foreach (XmlNode oRuleNode in oRoomXML.SelectNodes(sPredicate + "/cancellationRules/rule"))
                {
                    var oFromDateNode = oRuleNode.SelectSingleNode("fromDate");
                    var oToDateNode = oRuleNode.SelectSingleNode("toDate");
                    var oAmountNode = oRuleNode.SelectSingleNode("charge");

                    bool bNonRefundable = false;
                    if (oRuleNode.SelectSingleNode("cancelRestricted") is not null)
                    {
                        bNonRefundable = (oRuleNode.SelectSingleNode("cancelRestricted").InnerText).ToSafeBoolean();
                    }

                    bool bNoShowPolicy = false;
                    if (oRuleNode.SelectSingleNode("noShowPolicy") is not null)
                    {
                        bNoShowPolicy = (oRuleNode.SelectSingleNode("noShowPolicy").InnerText).ToSafeBoolean();
                    }

                    // get the start date
                    DateTime dStartDate;
                    if (oFromDateNode is not null)
                    {
                        // these always come back with the time as HH:mm:01 but the end dates come back as HH:mm:00, so I'm taking off a second
                        dStartDate = SafeTypeExtensions.ToSafeDate(oFromDateNode.InnerText).AddSeconds(-1).Date;
                    }
                    else if (bNoShowPolicy == true)
                    {
                        dStartDate = PropertyDetails.ArrivalDate.Date;
                    }
                    else
                    {
                        dStartDate = DateTime.Now;
                    }

                    // get the end date
                    DateTime dEndDate;
                    if (oToDateNode is not null)
                    {
                        dEndDate = SafeTypeExtensions.ToSafeDate(oToDateNode.InnerText).Date.AddDays(-1); // take off a day so our date bands don't overlap
                    }
                    else
                    {
                        dEndDate = dCancellationDeadline != DateTimeExtensions.EmptyDate ? dCancellationDeadline : new DateTime(2099, 12, 31);
                    }

                    // get the amount
                    decimal nAmount = 0m;
                    if (oAmountNode is not null)
                    {
                        nAmount = (oAmountNode.FirstChild.Value).ToSafeDecimal();
                    }
                    else if (bNonRefundable)
                    {
                        nAmount = oRoomDetails.LocalCost;
                    }


                    // add the rule into the policy
                    aPolicies[iLoop].AddNew(dStartDate, dEndDate, nAmount);

                }


                // call solidify on the policy
                aPolicies[iLoop].Solidify(SolidifyType.Sum, new DateTime(2099, 12, 31), oRoomDetails.LocalCost);


                // increment the loop counter 
                iLoop += 1;

            }


            // merge the policies and return
            return Cancellations.MergeMultipleCancellationPolicies(aPolicies);

        }

        #endregion

        #region Cancellation

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails PropertyDetails)
        {

            var oThirdPartyCancellationResponse = new ThirdPartyCancellationResponse();

            string sCancellationReference = "";
            string sRequest = "";
            var oResponse = new XmlDocument();
            string sCanxRequest = "";
            var oCanxResponse = new XmlDocument();

            try
            {

                // get costs and service numbers
                // build request
                sRequest = this.BuildCancellationCostRequest(PropertyDetails.SourceSecondaryReference, PropertyDetails);

                // Get the response 
                var oWebRequest = new Request();
                oWebRequest.EndPoint = _settings.ServerURL(PropertyDetails);
                oWebRequest.Method = Intuitive.Net.WebRequests.eRequestMethod.POST;
                oWebRequest.Source = ThirdParties.DOTW;
                oWebRequest.LogFileName = "Precancel";
                oWebRequest.SetRequest(sRequest);
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.CreateLog = true;
                oWebRequest.Send(_httpClient, _logger).RunSynchronously();

                oResponse = oWebRequest.ResponseXML;


                // check according to documentation that there is a success node with the value TRUE in it
                var oSuccessNode = oResponse.SelectSingleNode("result/successful");
                if (oSuccessNode is null || oSuccessNode.InnerText != "TRUE")
                {
                    throw new Exception("cancellation request did not return success");
                }


                // get service numbers
                var oCancellationDetails = new Dictionary<string, string>();
                oCancellationDetails = GetCancellationDetails(oResponse);


                // now make the actual cancellation
                // build request
                sCanxRequest = this.BuildCancellationRequest(PropertyDetails.SourceSecondaryReference, PropertyDetails.CancellationAmount, PropertyDetails, oCancellationDetails);

                // Get the response 
                var oCancellationWebRequest = new Request();
                oCancellationWebRequest.EndPoint = _settings.ServerURL(PropertyDetails);
                oCancellationWebRequest.Method = Intuitive.Net.WebRequests.eRequestMethod.POST;
                oCancellationWebRequest.Source = ThirdParties.DOTW;
                oCancellationWebRequest.LogFileName = "Cancel";
                oCancellationWebRequest.SetRequest(sCanxRequest);
                oCancellationWebRequest.ContentType = ContentTypes.Text_xml;
                oCancellationWebRequest.CreateLog = true;
                oCancellationWebRequest.Send(_httpClient, _logger).RunSynchronously();

                oCanxResponse = oCancellationWebRequest.ResponseXML;

                // check according to documentation that there is a success node with the value TRUE in it
                oSuccessNode = oCanxResponse.SelectSingleNode("result/successful");
                if (oSuccessNode is null || oSuccessNode.InnerText != "TRUE")
                {
                    throw new Exception("cancellation request did not return success");
                }

                if (SafeTypeExtensions.ToSafeInt(_settings.Version(PropertyDetails)) == 2)
                {
                    var oProductsLeft = oCanxResponse.SelectSingleNode("result/productsLeftOnItinerary ");
                    if (oProductsLeft is not null && SafeTypeExtensions.ToSafeInt(oProductsLeft.InnerText) != 0)
                    {
                        throw new Exception("cancellation request did not cancel all components");
                    }
                }

                // no cancellation reference is given, so use the time stamp as others do.
                oThirdPartyCancellationResponse.TPCancellationReference = DateTime.Now.ToString("yyyyMMddhhmm");
                oThirdPartyCancellationResponse.Success = true;
            }

            catch (Exception ex)
            {
                PropertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
                oThirdPartyCancellationResponse.TPCancellationReference = "";
                oThirdPartyCancellationResponse.Success = false;
            }

            finally
            {

                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(sRequest))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW PreCancellation Request", sRequest);
                }

                if (!string.IsNullOrEmpty(oResponse.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW PreCancellation Response", oResponse);
                }

                if (!string.IsNullOrEmpty(sCanxRequest))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Cancellation Request", sCanxRequest);
                }

                if (!string.IsNullOrEmpty(oCanxResponse.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "DOTW Cancellation Response", oCanxResponse);
                }

            }

            return oThirdPartyCancellationResponse;

        }

        private Dictionary<string, string> GetCancellationDetails(XmlDocument oXML)
        {

            var oCancellationDetails = new Dictionary<string, string>();

            foreach (XmlNode oNode in oXML.SelectNodes("result/services/service"))
                oCancellationDetails.Add((oNode.Attributes["code"].Value).ToSafeString(), (oNode.SelectSingleNode("cancellationPenalty/charge/text()").InnerText).ToSafeString());

            return oCancellationDetails;

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


        #region Cancellation Cost

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails PropertyDetails)
        {

            var oResult = new ThirdPartyCancellationFeeResult();
            var oRequest = new XmlDocument();
            var oResponse = new XmlDocument();

            try
            {

                // build request
                string sRequest = this.BuildCancellationCostRequest(PropertyDetails.SourceSecondaryReference, PropertyDetails);

                oRequest.LoadXml(sRequest);

                // Get the response 
                var oWebRequest = new Request();
                oWebRequest.EndPoint = _settings.ServerURL(PropertyDetails);
                oWebRequest.Method = Intuitive.Net.WebRequests.eRequestMethod.POST;
                oWebRequest.Source = ThirdParties.DOTW;
                oWebRequest.LogFileName = "Cancellation Cost";
                oWebRequest.SetRequest(sRequest);
                oWebRequest.ContentType = ContentTypes.Text_xml;
                oWebRequest.CreateLog = true;
                oWebRequest.Send(_httpClient, _logger).RunSynchronously();

                oResponse = oWebRequest.ResponseXML;


                // check according to documentation that there is a success node with the value TRUE in it
                var oSuccessNode = oResponse.SelectSingleNode("result/successful");
                if (oSuccessNode is null || oSuccessNode.InnerText != "TRUE")
                {
                    throw new Exception("cancellation cost response does not return success");
                }


                // get the cancellation cost from this booking
                var oCostNodes = oResponse.SelectNodes("result/services/service");
                if (oCostNodes is null || oCostNodes.Count == 0)
                {
                    throw new Exception("cancellation costs request not in expected format");
                }

                decimal nAmount = 0m;
                foreach (XmlNode oCostNode in oCostNodes)
                    nAmount += (oCostNode.SelectSingleNode("cancellationPenalty/charge/text()").InnerText).ToSafeDecimal();

                // grab the currency from the first node
                oResult.Amount = nAmount;
                oResult.CurrencyCode = (oResponse.SelectSingleNode("/result/services[1]/service/cancellationPenalty/currencyShort").InnerText).ToSafeString();
                oResult.Success = true;
            }

            catch (Exception ex)
            {
                PropertyDetails.Warnings.AddNew("Get Cancellation Cost Exception", ex.ToString());
            }

            finally
            {

                if (!string.IsNullOrEmpty(oRequest.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "Get Cancellation Cost Request", oRequest);
                }

                if (!string.IsNullOrEmpty(oResponse.InnerXml))
                {
                    PropertyDetails.Logs.AddNew(ThirdParties.DOTW, "Get Cancellation Cost Response", oResponse);
                }

            }

            return oResult;

        }

        #endregion

        #region Build requests

        private string BuildBookingRequest(PropertyDetails PropertyDetails)
        {
            var oSB = new StringBuilder();

            oSB.AppendLine("<customer>");
            oSB.AppendFormat("<username>{0}</username>", _settings.Username(PropertyDetails)).AppendLine();
            oSB.AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(PropertyDetails))).AppendLine();
            oSB.AppendFormat("<id>{0}</id>", _settings.CompanyCode(PropertyDetails)).AppendLine();
            oSB.AppendLine("<source>1</source>");
            oSB.AppendLine("<product>hotel</product>");
            oSB.AppendLine("<request command=\"confirmbooking\">");
            oSB.AppendLine("<bookingDetails>");
            oSB.AppendFormat("<fromDate>{0}</fromDate>", PropertyDetails.ArrivalDate.ToString("yyyy-MM-dd")).AppendLine();
            oSB.AppendFormat("<toDate>{0}</toDate>", PropertyDetails.DepartureDate.ToString("yyyy-MM-dd")).AppendLine();
            oSB.AppendFormat("<currency>{0}</currency>", DOTWSupport.GetCurrencyCode(PropertyDetails.CurrencyID, PropertyDetails, _settings, _support, _httpClient, _logger));
            oSB.AppendFormat("<productId>{0}</productId>", PropertyDetails.TPKey);

            string sCustomerReference = PropertyDetails.BookingReference.Trim();

            if (_settings.SendTradeReference(PropertyDetails))
            {
                sCustomerReference = PropertyDetails.TradeReference;
            }

            if (string.IsNullOrEmpty(sCustomerReference))
            {
                sCustomerReference = DateTime.Now.ToString("yyyyMMddhhmmss");
            }

            oSB.AppendFormat("<customerReference>{0}</customerReference>", sCustomerReference);
            oSB.AppendFormat("<rooms no=\" {0}\">", PropertyDetails.Rooms.Count);

            int iRoomRunNo = 0;
            foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
            {

                string sRoomTypeCode = oRoomDetails.ThirdPartyReference.Split('|')[0];
                string sMealBasis = oRoomDetails.ThirdPartyReference.Split('|')[1];
                string sAllocationDetails = oRoomDetails.ThirdPartyReference.Split('|')[2];

                int iAdults = oRoomDetails.Passengers.TotalAdults;
                int iChildren = 0;

                foreach (Passenger oPassenger in oRoomDetails.Passengers)
                {
                    if (oPassenger.PassengerType == PassengerType.Child || oPassenger.PassengerType == PassengerType.Infant)
                    {
                        if (oPassenger.Age > 12)
                        {
                            iAdults += 1;
                        }
                        else
                        {
                            iChildren += 1;
                        }
                    }
                }

                oSB.AppendFormat("<room runno=\" {0}\">", iRoomRunNo);
                oSB.AppendFormat("<roomTypeCode>{0}</roomTypeCode>", sRoomTypeCode);
                oSB.AppendFormat("<selectedRateBasis>{0}</selectedRateBasis>", sMealBasis);

                oSB.AppendFormat("<allocationDetails>{0}</allocationDetails>", sAllocationDetails);
                oSB.AppendFormat("<adultsCode>{0}</adultsCode>", iAdults);
                oSB.AppendFormat("<children no=\" {0}\">", iChildren);

                int iChildAgeRunNo = 0;
                foreach (Passenger oPassenger in oRoomDetails.Passengers)
                {
                    if (oPassenger.PassengerType == PassengerType.Child && oPassenger.Age <= 12 || oPassenger.PassengerType == PassengerType.Infant)
                    {
                        oSB.AppendFormat("<child runno=\"{0}\">{1}</child>", iChildAgeRunNo.ToString(), oPassenger.Age == 0 ? 1 : oPassenger.Age.ToString());
                        iChildAgeRunNo += 1;
                    }
                }

                oSB.AppendFormat("</children>");
                oSB.AppendLine("<extraBed>0</extraBed>");
                if (_settings.Version(PropertyDetails) == "2")
                {
                    string sNationality = GetNationality(PropertyDetails.NationalityID, PropertyDetails, _support, _settings);
                    string sCountryCode = GetCountryOfResidence(sNationality, PropertyDetails, _settings);

                    if (!string.IsNullOrEmpty(sNationality))
                    {
                        oSB.AppendFormat("<passengerNationality>{0}</passengerNationality>", sNationality);
                    }

                    if (!string.IsNullOrEmpty(sCountryCode))
                    {
                        oSB.AppendFormat("<passengerCountryOfResidence>{0}</passengerCountryOfResidence>", sCountryCode);
                    }
                }
                oSB.AppendLine("<passengersDetails>");

                int iGuestRunNo = 0;
                foreach (Passenger oPassenger in oRoomDetails.Passengers)
                {

                    // get the guest title
                    int iTitleID;

                    if (oPassenger.PassengerType == PassengerType.Child || oPassenger.PassengerType == PassengerType.Infant)
                    {
                        iTitleID = DOTWSupport.GetTitleID("Child");
                    }
                    else
                    {
                        iTitleID = DOTWSupport.GetTitleID(oPassenger.Title);
                    }

                    oSB.AppendFormat("<passenger leading=\"{0}\">", iGuestRunNo == 0 ? "yes" : "no");
                    oSB.AppendFormat("<salutation>{0}</salutation>", iTitleID);
                    oSB.AppendFormat("<firstName>{0}</firstName>", DOTWSupport.CleanName(oPassenger.FirstName, _support));
                    oSB.AppendFormat("<lastName>{0}</lastName>", DOTWSupport.CleanName(oPassenger.LastName, _support));
                    oSB.AppendLine("</passenger>");
                    iGuestRunNo += 1;

                }

                oSB.AppendLine("</passengersDetails>");
                oSB.AppendLine("</room>");

                iRoomRunNo += 1;

            }

            oSB.AppendLine("</rooms>");
            oSB.AppendLine("</bookingDetails>");
            oSB.AppendLine("</request>");
            oSB.AppendLine("</customer>");

            return oSB.ToString();

        }

        private string BuildCancellationCostRequest(string BookingReference, IThirdPartyAttributeSearch SearchDetails)
        {

            var oSB = new StringBuilder();

            oSB.AppendLine("<customer>");
            oSB.AppendFormat("<username>{0}</username>", _settings.Username(SearchDetails)).AppendLine();
            oSB.AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(SearchDetails))).AppendLine();
            oSB.AppendFormat("<id>{0}</id>", _settings.CompanyCode(SearchDetails)).AppendLine();
            oSB.AppendLine("<source>1</source>");
            oSB.AppendLine("<request command=\"deleteitinerary\">");
            oSB.AppendLine("<bookingDetails>");
            oSB.AppendLine("<bookingType>1</bookingType>");
            oSB.AppendFormat("<bookingCode>{0}</bookingCode>", BookingReference);
            oSB.AppendLine("<confirm>no</confirm> ");
            oSB.AppendLine("</bookingDetails>");
            oSB.AppendLine("</request>");
            oSB.AppendLine("</customer>");

            return oSB.ToString();

        }

        private string BuildCancellationRequest(string BookingReference, decimal CancellationAmount, IThirdPartyAttributeSearch SearchDetails, Dictionary<string, string> CancellationDetails)
        {

            var oSB = new StringBuilder();

            oSB.AppendLine("<customer>");
            oSB.AppendFormat("<username>{0}</username>", _settings.Username(SearchDetails)).AppendLine();
            oSB.AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(SearchDetails))).AppendLine();
            oSB.AppendFormat("<id>{0}</id>", _settings.CompanyCode(SearchDetails)).AppendLine();
            oSB.AppendLine("<source>1</source>");
            oSB.AppendLine("<request command=\"deleteitinerary\">");
            oSB.AppendLine("<bookingDetails>");
            oSB.AppendLine("<bookingType>1</bookingType>");
            oSB.AppendFormat("<bookingCode>{0}</bookingCode>", BookingReference);
            oSB.AppendLine("<confirm>yes</confirm>");
            oSB.AppendLine("<testPricesAndAllocation>");

            foreach (KeyValuePair<string, string> oDetail in CancellationDetails)
            {
                oSB.AppendFormat("<service referencenumber=\"{0}\">", oDetail.Key).AppendLine();
                oSB.AppendFormat("<penaltyApplied>{0}</penaltyApplied>", oDetail.Value).AppendLine();
                oSB.AppendLine("</service>");
            }

            oSB.AppendLine("</testPricesAndAllocation>");
            oSB.AppendLine("</bookingDetails>");
            oSB.AppendLine("</request>");
            oSB.AppendLine("</customer>");

            return oSB.ToString();

        }

        private string BuildPreBookRequest(PropertyDetails PropertyDetails)
        {
            var oSB = new StringBuilder();

            oSB.AppendLine("<customer>");
            oSB.AppendFormat("<username>{0}</username>", _settings.Username(PropertyDetails));
            oSB.AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(PropertyDetails)));
            oSB.AppendFormat("<id>{0}</id>", _settings.CompanyCode(PropertyDetails));
            oSB.AppendLine("<source>1</source>");
            oSB.AppendLine("<product>hotel</product>");
            oSB.AppendLine("<request command = \"getrooms\">");
            oSB.AppendLine("<bookingDetails>");
            oSB.AppendFormat("<fromDate>{0}</fromDate>", PropertyDetails.ArrivalDate.ToString("yyyy-MM-dd"));
            oSB.AppendFormat("<toDate>{0}</toDate>", PropertyDetails.DepartureDate.ToString("yyyy-MM-dd"));
            oSB.AppendFormat("<currency>{0}</currency>", DOTWSupport.GetCurrencyCode(PropertyDetails.CurrencyID, PropertyDetails, _settings, _support, _httpClient, _logger));
            oSB.AppendFormat("<rooms no = \"{0}\">", PropertyDetails.Rooms.Count);

            int iRoomRunNo = 0;
            foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
            {

                string sRoomTypeCode = oRoomDetails.ThirdPartyReference.Split('|')[0];
                string sMealBasis = oRoomDetails.ThirdPartyReference.Split('|')[1];

                int iAdults = oRoomDetails.AdultsSetAgeOrOver(13);
                int iChildren = oRoomDetails.ChildrenUnderSetAge(13);

                oSB.AppendFormat("<room runno=\"{0}\">", iRoomRunNo);
                oSB.AppendFormat("<adultsCode>{0}</adultsCode>", iAdults);
                oSB.AppendFormat("<children no=\"{0}\">", iChildren);

                int iChildAgeRunNo = 0;
                foreach (int iAge in oRoomDetails.Passengers.ChildAgesUnderSetAge(13))
                {

                    oSB.AppendFormat("<child runno=\"{0}\">{1}</child>", iChildAgeRunNo, iAge);
                    iChildAgeRunNo += 1;

                }

                oSB.AppendLine("</children>");
                oSB.AppendLine("<extraBed>0</extraBed>");
                oSB.AppendFormat("<rateBasis>{0}</rateBasis>", sMealBasis);

                if (_settings.Version(PropertyDetails) == "2")
                {
                    string sNationality = GetNationality(PropertyDetails.NationalityID, PropertyDetails, _support, _settings);
                    string sCountryCode = GetCountryOfResidence(sNationality, PropertyDetails, _settings);

                    if (!string.IsNullOrEmpty(sNationality))
                    {
                        oSB.AppendFormat("<passengerNationality>{0}</passengerNationality>", sNationality);
                    }

                    if (!string.IsNullOrEmpty(sCountryCode))
                    {
                        oSB.AppendFormat("<passengerCountryOfResidence>{0}</passengerCountryOfResidence>", sCountryCode);
                    }
                }

                oSB.AppendLine("</room>");
                iRoomRunNo += 1;

            }

            oSB.AppendLine("</rooms>");
            oSB.AppendFormat("<productId>{0}</productId>", PropertyDetails.TPKey);
            oSB.AppendLine("</bookingDetails>");
            oSB.AppendLine("</request>");

            oSB.AppendLine("</customer>");

            return oSB.ToString();

        }

        private string BuildBlockRequest(PropertyDetails PropertyDetails)
        {
            var oSB = new StringBuilder();


            oSB.AppendLine("<customer>");
            oSB.AppendFormat("<username>{0}</username>", _settings.Username(PropertyDetails));
            oSB.AppendFormat("<password>{0}</password>", DOTWSupport.MD5Password(_settings.Password(PropertyDetails)));
            oSB.AppendFormat("<id>{0}</id>", _settings.CompanyCode(PropertyDetails));
            oSB.AppendLine("<source>1</source>");
            oSB.AppendLine("<product>hotel</product>");
            oSB.AppendLine("<request command = \"getrooms\">");
            oSB.AppendLine("<bookingDetails>");
            oSB.AppendFormat("<fromDate>{0}</fromDate>", PropertyDetails.ArrivalDate.ToString("yyyy-MM-dd"));
            oSB.AppendFormat("<toDate>{0}</toDate>", PropertyDetails.DepartureDate.ToString("yyyy-MM-dd"));
            oSB.AppendFormat("<currency>{0}</currency>", DOTWSupport.GetCurrencyCode(PropertyDetails.CurrencyID, PropertyDetails, _settings, _support, _httpClient, _logger));
            oSB.AppendFormat("<rooms no = \"{0}\">", PropertyDetails.Rooms.Count);

            int iRoomRunNo = 0;
            foreach (RoomDetails oRoomDetails in PropertyDetails.Rooms)
            {

                string sRoomTypeCode = oRoomDetails.ThirdPartyReference.Split('|')[0];
                string sMealBasis = oRoomDetails.ThirdPartyReference.Split('|')[1];
                string sAllocationDetail = oRoomDetails.ThirdPartyReference.Split('|')[2];

                int iAdults = oRoomDetails.AdultsSetAgeOrOver(13);
                int iChildren = oRoomDetails.ChildrenUnderSetAge(13);

                oSB.AppendFormat("<room runno=\"{0}\">", iRoomRunNo);
                oSB.AppendFormat("<adultsCode>{0}</adultsCode>", iAdults);
                oSB.AppendFormat("<children no=\"{0}\">", iChildren);

                int iChildAgeRunNo = 0;
                foreach (int iAge in oRoomDetails.Passengers.ChildAgesUnderSetAge(13))
                {

                    oSB.AppendFormat("<child runno=\"{0}\">{1}</child>", iChildAgeRunNo, iAge);
                    iChildAgeRunNo += 1;

                }

                oSB.AppendLine("</children>");
                oSB.AppendLine("<extraBed>0</extraBed>");
                oSB.AppendLine("<rateBasis>-1</rateBasis>");

                if (_settings.Version(PropertyDetails) == "2")
                {
                    string sNationality = GetNationality(PropertyDetails.NationalityID, PropertyDetails, _support, _settings);
                    string sCountryCode = GetCountryOfResidence(sNationality, PropertyDetails, _settings);

                    if (!string.IsNullOrEmpty(sNationality))
                    {
                        oSB.AppendFormat("<passengerNationality>{0}</passengerNationality>", sNationality);
                    }

                    if (!string.IsNullOrEmpty(sCountryCode))
                    {
                        oSB.AppendFormat("<passengerCountryOfResidence>{0}</passengerCountryOfResidence>", sCountryCode);
                    }

                }

                oSB.AppendLine("<roomTypeSelected>");
                oSB.AppendFormat("<code>{0}</code>", sRoomTypeCode);
                oSB.AppendFormat("<selectedRateBasis>{0}</selectedRateBasis>", sMealBasis);
                oSB.AppendFormat("<allocationDetails>{0}</allocationDetails>", sAllocationDetail);
                oSB.AppendLine("</roomTypeSelected>");
                oSB.AppendLine("</room>");

                iRoomRunNo += 1;

            }

            oSB.AppendLine("</rooms>");
            oSB.AppendFormat("<productId>{0}</productId>", PropertyDetails.TPKey);
            oSB.AppendLine("</bookingDetails>");
            oSB.AppendLine("</request>");

            oSB.AppendLine("</customer>");

            return oSB.ToString();

        }

        #endregion

        #region Nationality and country of residence

        public static string GetNationality(string NationalityISOCode, IThirdPartyAttributeSearch SearchDetails, ITPSupport Support, IDOTWSettings Settings)
        {
            string sNationality = "";

            sNationality = Support.TPNationalityLookup(ThirdParties.DOTW, NationalityISOCode);

            if (string.IsNullOrEmpty(sNationality))
            {
                sNationality = Settings.CustomerNationality(SearchDetails);
            }

            return sNationality;
        }

        public static string GetCountryOfResidence(string sCountryCode, IThirdPartyAttributeSearch SearchDetails, IDOTWSettings Settings)
        {

            if (string.IsNullOrEmpty(sCountryCode))
            {
                sCountryCode = Settings.CustomerCountryCode(SearchDetails);
            }

            return sCountryCode;
        }

        #endregion

    }
}
