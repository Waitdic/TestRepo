namespace ThirdParty.CSSuppliers.DOTW
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Interfaces;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Models;
    using System.Threading.Tasks;

    public class DOTW : IThirdParty, ISingleSource
    {
        #region Constructor

        private readonly IDOTWSettings _settings;

        private readonly ITPSupport _support;

        private readonly ISerializer _serializer;

        private readonly HttpClient _httpClient;

        private readonly ILogger<DOTW> _logger;

        private readonly IDOTWSupport _dotwSupport;

        public DOTW(
            IDOTWSettings settings,
            ITPSupport support,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<DOTW> logger,
            IDOTWSupport dotwSupport)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _dotwSupport = Ensure.IsNotNull(dotwSupport, nameof(dotwSupport));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.DOTW;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public bool SupportsRemarks => false;

        public bool SupportsBookingSearch => false;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
            => _settings.OffsetCancellationDays(searchDetails, false);

        public bool RequiresVCard(VirtualCardInfo info, string source) => false;

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            try
            {
                await GetAllocationReferencesAsync(propertyDetails);
                await BlockRoomsAsync(propertyDetails);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                return false;
            }

            return true;
        }

        #region Sub Methods of the pre book

        private async Task GetAllocationReferencesAsync(PropertyDetails propertyDetails)
        {
            // get the room rates so we can get the stupidly long allocationDetails code
            string requestString = await BuildPreBookRequestAsync(propertyDetails);

            var headers = new RequestHeaders();
            if (_settings.UseGZip(propertyDetails))
            {
                headers.AddNew("Accept-Encoding", "gzip");
            }

            // Get the response 
            var request = new Request
            {
                EndPoint = _settings.GenericURL(propertyDetails),
                Method = RequestMethod.POST,
                Source = ThirdParties.DOTW,
                Headers = headers,
                LogFileName = "Prebook Room",
                ContentType = ContentTypes.Text_xml,
                CreateLog = true
            };

            request.SetRequest(requestString);
            await request.Send(_httpClient, _logger);

            var response = _serializer.CleanXmlNamespaces(request.ResponseXML);

            // check for a valid response
            var successNode = response.SelectSingleNode("result/successful");
            if (successNode is null || successNode.InnerText != "TRUE")
            {
                throw new Exception("booking response does not return success");
            }

            // loop through each room and get the relevant allocationDetails string and append to TPReference
            int roomRunNo = 0;
            foreach (var roomDetails in propertyDetails.Rooms)
            {
                // get the room code and meal basis for the predicate
                string roomTypeCode = roomDetails.ThirdPartyReference.Split('|')[0];
                string mealBasis = roomDetails.ThirdPartyReference.Split('|')[1];

                // build the predicate
                string predicate = string.Format("/result/hotel/rooms/room[@runno='{0}']/roomType[@roomtypecode='{1}']/rateBases/rateBasis[@id='{2}']/", roomRunNo, roomTypeCode, mealBasis);

                // grab the allocationDetails using the predicate
                var allocationNode = response.SelectSingleNode(predicate + "allocationDetails");
                if (allocationNode is null)
                {
                    throw new Exception("Allocation Details could not be found in prebook");
                }

                // assign the allocation details to the TPReference
                roomDetails.ThirdPartyReference += "|" + allocationNode.InnerText;

                // increment for each room
                roomRunNo += 1;

                // add errata
                string tariffNotes = XmlNodeExtensions.SafeNodeValue(response, predicate + "tariffNotes");
                if (!string.IsNullOrEmpty(tariffNotes))
                {
                    propertyDetails.Errata.AddNew("Important Information", tariffNotes);
                }
            }

            // store the request and response xml on the property booking
            propertyDetails.AddLog("Availability", request);
        }

        private async Task BlockRoomsAsync(PropertyDetails propertyDetails)
        {
            // block the rooms
            string requestString = await BuildBlockRequestAsync(propertyDetails);

            var oHeaders = new RequestHeaders();
            if (_settings.UseGZip(propertyDetails))
            {
                oHeaders.AddNew("Accept-Encoding", "gzip");
            }

            // Get the response 
            var request = new Request
            {
                EndPoint = _settings.GenericURL(propertyDetails),
                Method = RequestMethod.POST,
                Source = ThirdParties.DOTW,
                Headers = oHeaders,
                LogFileName = "Prebook Block Room",
                ContentType = ContentTypes.Text_xml,
                CreateLog = true
            };

            request.SetRequest(requestString);
            await request.Send(_httpClient, _logger);

            var response = _serializer.CleanXmlNamespaces(request.ResponseXML);

            // check for a valid response
            var successNode = response.SelectSingleNode("result/successful");
            if (successNode is null || successNode.InnerText != "TRUE")
            {
                throw new Exception("block response does not return success");
            }

            int roomRunNo = 0;
            decimal newLocalCost = 0m;

            // loop through each room and get the relevant allocationDetails string and append to TPReference
            foreach (var roomDetails in propertyDetails.Rooms)
            {
                // get the room code and meal basis for the predicate
                string roomTypeCode = roomDetails.ThirdPartyReference.Split('|')[0];
                string mealBasis = roomDetails.ThirdPartyReference.Split('|')[1];

                string currentAllocationDetails = roomDetails.ThirdPartyReference.Split('|')[2];

                // We get one status = checked per room booked. This is the one we want. Use it. Please.
                string predicate = string.Format("/result/hotel/rooms/room[@runno='{0}']/roomType[@roomtypecode='{1}']/rateBases/rateBasis[@id='{2}'][status='checked']", roomRunNo, roomTypeCode, mealBasis);

                var resultNode = response.SelectSingleNode(predicate);

                if (resultNode is null)
                {
                    throw new Exception("room type could not be blocked");
                }

                // grab the allocationDetails using the predicate
                var allocationNode = resultNode.SelectSingleNode("allocationDetails");
                if (allocationNode is null)
                {
                    throw new Exception("Allocation Details could not be found in prebook");
                }

                // assign the allocation details to the TPReference
                roomDetails.ThirdPartyReference = roomDetails.ThirdPartyReference.Replace(currentAllocationDetails, allocationNode.InnerText);

                // Check for Price Changes for each room booking
                decimal roomCost = 0m;
                try
                {
                    roomCost = resultNode.SelectSingleNode("total/formatted").InnerText.ToSafeDecimal();

                    if (_settings.UseMinimumSellingPrice(propertyDetails) && resultNode.SelectSingleNode("totalMinimumSelling/formatted") is not null)
                    {
                        roomCost = resultNode.SelectSingleNode("totalMinimumSelling/formatted").InnerText.ToSafeDecimal();
                    }
                }
                catch
                {
                    // probably no minimumselling price listed, ignore exception and use the normal total
                }

                if (roomCost != roomDetails.LocalCost && roomCost != 0m)
                {
                    roomDetails.LocalCost = roomCost;
                    roomDetails.GrossCost = roomCost;
                }

                newLocalCost += roomDetails.LocalCost;

                // increment the roomrunno 
                roomRunNo++;
            }

            // have to recalculate costs after price changes or cancellations will use the wrong cost!!!
            if (propertyDetails.LocalCost != newLocalCost)
            {
                propertyDetails.LocalCost = newLocalCost;
            }

            // get the cancellation policy
            propertyDetails.Cancellations.AddRange(GetCancellationPolicy(propertyDetails, response));

            // store the request and response xml on the property booking
            propertyDetails.AddLog("Pre-Book", request);
        }

        #endregion

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string returnReference = string.Empty;
            var response = new XmlDocument();
            var request = new XmlDocument();
            var webRequest = new Request();

            try
            {
                // build request
                string requestString = await BuildBookingRequestAsync(propertyDetails);

                var headers = new RequestHeaders();
                if (_settings.UseGZip(propertyDetails))
                {
                    headers.AddNew("Accept-Encoding", "gzip");
                }

                // Get the response 
                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.DOTW,
                    Headers = headers,
                    LogFileName = "Book",
                    ContentType = ContentTypes.Text_xml,
                    CreateLog = true
                };
                webRequest.SetRequest(requestString);
                await webRequest.Send(_httpClient, _logger);

                request.LoadXml(requestString);
                response = webRequest.ResponseXML;

                // check according to documentation that there is a success node with the value TRUE in it
                var successNode = response.SelectSingleNode("result/successful");
                if (successNode is null || successNode.InnerText != "TRUE")
                {
                    throw new Exception("booking response does not return success");
                }

                // now get booking nodes
                var bookings = response.SelectSingleNode("result/returnedCode");

                if (bookings is not null)
                {
                    // concatenate the various references for each room component into a booking comment
                    var referenceNodeList = response.SelectNodes("/result/bookings/booking/bookingReferenceNumber");

                    if (referenceNodeList.Count > 1)
                    {
                        // create a booking comment on this property booking with all the room references in it.
                        var sbReferences = new StringBuilder();
                        sbReferences.Append("DOTW room booking references ");

                        for (int node = 0; node <= referenceNodeList.Count - 1; node++)
                        {
                            sbReferences.Append(referenceNodeList[node].InnerText);
                            if (node != referenceNodeList.Count - 1)
                            {
                                sbReferences.Append(", ");
                            }
                        }

                        propertyDetails.BookingComments.AddNew(sbReferences.ToString());
                    }

                    // get the reference of the booking which is displayed on their website as a reference
                    returnReference = referenceNodeList[0].InnerText;
                    propertyDetails.SourceSecondaryReference = response.SelectSingleNode("result/returnedCode").InnerText;
                }

                else
                {
                    throw new Exception("no bookings found in booking response xml");
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
                returnReference = "failed";
            }
            finally
            {
                propertyDetails.AddLog("Book", webRequest);
            }

            return returnReference;
        }

        #endregion

        #region Get Cancellation Policy

        public Cancellations GetCancellationPolicy(PropertyDetails propertyDetails, XmlDocument roomXml)
        {
            // create an array variable to hold the policy for each room
            var policies = new Cancellations[propertyDetails.Rooms.Count];

            // loop through the rooms
            int loop = 0;
            foreach (var roomDetails in propertyDetails.Rooms)
            {
                // get the room code and meal basis for the predicate
                string sRoomTypeCode = roomDetails.ThirdPartyReference.Split('|')[0];
                string sMealBasis = roomDetails.ThirdPartyReference.Split('|')[1];

                // build the predicate
                string predicate = string.Format("/result/hotel/rooms/room[@runno='{0}']/roomType[@roomtypecode='{1}']/rateBases/rateBasis[@id='{2}']", loop, sRoomTypeCode, sMealBasis);

                // get the cancellation deadline if it exists - we have to strip this out of a text string because dotw are idiots
                var cancellationDeadline = DateTimeExtensions.EmptyDate;

                var cancellationNode = roomXml.SelectSingleNode(predicate + "/cancellation");
                if (cancellationNode is not null && cancellationNode.InnerText.StartsWith("Cancellation Deadline: "))
                {
                    cancellationDeadline = cancellationNode.InnerText.Substring(23).Replace(" hrs", "").ToSafeDate().Date;
                }

                // add the rules into the policy for this room
                // in version 2 they have added a no show policy element for some of the properties which doesn't have a to or from date just a charge so we will add our or dates
                // as we go through if this is case
                policies[loop] = new Cancellations();

                foreach (XmlNode ruleNode in roomXml.SelectNodes(predicate + "/cancellationRules/rule"))
                {
                    var fromDateNode = ruleNode.SelectSingleNode("fromDate");
                    var toDateNode = ruleNode.SelectSingleNode("toDate");
                    var amountNode = ruleNode.SelectSingleNode("charge");

                    bool nonRefundable = false;
                    if (ruleNode.SelectSingleNode("cancelRestricted") is not null)
                    {
                        nonRefundable = (ruleNode.SelectSingleNode("cancelRestricted").InnerText).ToSafeBoolean();
                    }

                    bool noShowPolicy = false;
                    if (ruleNode.SelectSingleNode("noShowPolicy") is not null)
                    {
                        noShowPolicy = (ruleNode.SelectSingleNode("noShowPolicy").InnerText).ToSafeBoolean();
                    }

                    // get the start date
                    DateTime startDate;
                    if (fromDateNode is not null)
                    {
                        // these always come back with the time as HH:mm:01 but the end dates come back as HH:mm:00, so I'm taking off a second
                        startDate = fromDateNode.InnerText.ToSafeDate().AddSeconds(-1).Date;
                    }
                    else if (noShowPolicy)
                    {
                        startDate = propertyDetails.ArrivalDate.Date;
                    }
                    else
                    {
                        startDate = DateTime.Now;
                    }

                    // get the end date
                    DateTime endDate;
                    if (toDateNode is not null)
                    {
                        endDate = toDateNode.InnerText.ToSafeDate().Date.AddDays(-1); // take off a day so our date bands don't overlap
                    }
                    else
                    {
                        endDate = cancellationDeadline != DateTimeExtensions.EmptyDate ? cancellationDeadline : new DateTime(2099, 12, 31);
                    }

                    // get the amount
                    decimal amount = 0m;
                    if (amountNode is not null)
                    {
                        amount = amountNode.FirstChild.Value.ToSafeDecimal();
                    }
                    else if (nonRefundable)
                    {
                        amount = roomDetails.LocalCost;
                    }

                    // add the rule into the policy
                    policies[loop].AddNew(startDate, endDate, amount);
                }

                // call solidify on the policy
                policies[loop].Solidify(SolidifyType.Sum, new DateTime(2099, 12, 31), roomDetails.LocalCost);

                // increment the loop counter 
                loop += 1;
            }

            // merge the policies and return
            return Cancellations.MergeMultipleCancellationPolicies(policies);
        }

        #endregion

        #region Cancellation

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();

            string request = string.Empty;
            var response = new XmlDocument();
            string canxRequest = string.Empty;
            var canxResponse = new XmlDocument();
            var preCancelWebRequest = new Request();
            var cancellationWebRequest = new Request();

            try
            {
                // get costs and service numbers
                // build request
                request = this.BuildCancellationCostRequest(propertyDetails.SourceSecondaryReference, propertyDetails);

                // Get the response
                // todo - move this to precancel?
                preCancelWebRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.DOTW,
                    LogFileName = "Precancel",
                    ContentType = ContentTypes.Text_xml,
                    CreateLog = true
                };
                preCancelWebRequest.SetRequest(request);
                await preCancelWebRequest.Send(_httpClient, _logger);

                response = preCancelWebRequest.ResponseXML;

                // check according to documentation that there is a success node with the value TRUE in it
                var successNode = response.SelectSingleNode("result/successful");
                if (successNode is null || successNode.InnerText != "TRUE")
                {
                    throw new Exception("cancellation request did not return success");
                }

                // get service numbers
                var cancellationDetails = new Dictionary<string, string>();
                cancellationDetails = GetCancellationDetails(response);

                // now make the actual cancellation
                // build request
                canxRequest = this.BuildCancellationRequest(propertyDetails.SourceSecondaryReference, propertyDetails, cancellationDetails);

                // Get the response 
                cancellationWebRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.DOTW,
                    LogFileName = "Cancel",
                    ContentType = ContentTypes.Text_xml,
                    CreateLog = true
                };
                cancellationWebRequest.SetRequest(canxRequest);
                await cancellationWebRequest.Send(_httpClient, _logger);

                canxResponse = cancellationWebRequest.ResponseXML;

                // check according to documentation that there is a success node with the value TRUE in it
                successNode = canxResponse.SelectSingleNode("result/successful");
                if (successNode is null || successNode.InnerText != "TRUE")
                {
                    throw new Exception("cancellation request did not return success");
                }

                if (_settings.Version(propertyDetails) == 2)
                {
                    var productsLeft = canxResponse.SelectSingleNode("result/productsLeftOnItinerary ");
                    if (productsLeft is not null && productsLeft.InnerText.ToSafeInt() != 0)
                    {
                        throw new Exception("cancellation request did not cancel all components");
                    }
                }

                // no cancellation reference is given, so use the time stamp as others do.
                thirdPartyCancellationResponse.TPCancellationReference = DateTime.Now.ToString("yyyyMMddhhmm");
                thirdPartyCancellationResponse.Success = true;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
                thirdPartyCancellationResponse.TPCancellationReference = "";
                thirdPartyCancellationResponse.Success = false;
            }
            finally
            {
                propertyDetails.AddLog("PreCancellation", preCancelWebRequest);
                propertyDetails.AddLog("Cancellation", cancellationWebRequest);
            }

            return thirdPartyCancellationResponse;
        }

        private Dictionary<string, string> GetCancellationDetails(XmlDocument xml)
        {
            var cancellationDetails = new Dictionary<string, string>();

            foreach (XmlNode node in xml.SelectNodes("result/services/service"))
            {
                cancellationDetails.Add(
                    node.Attributes["code"].Value.ToSafeString(),
                    node.SelectSingleNode("cancellationPenalty/charge/text()").InnerText.ToSafeString());
            }

            return cancellationDetails;
        }

        #endregion

        #region Booking Search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        #endregion

        #region Booking Status Update

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        #endregion

        #region End Session

        public void EndSession(PropertyDetails propertyDetails)
        {

        }

        #endregion

        #region Cancellation Cost

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            var result = new ThirdPartyCancellationFeeResult();
            var request = new XmlDocument();
            var response = new XmlDocument();
            var webRequest = new Request();

            try
            {
                // build request
                string requestString = this.BuildCancellationCostRequest(propertyDetails.SourceSecondaryReference, propertyDetails);

                request.LoadXml(requestString);

                // Get the response 
                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.DOTW,
                    LogFileName = "Cancellation Cost",
                    ContentType = ContentTypes.Text_xml,
                    CreateLog = true
                };
                webRequest.SetRequest(requestString);
                await webRequest.Send(_httpClient, _logger);

                response = webRequest.ResponseXML;

                // check according to documentation that there is a success node with the value TRUE in it
                var successNode = response.SelectSingleNode("result/successful");
                if (successNode is null || successNode.InnerText != "TRUE")
                {
                    throw new Exception("cancellation cost response does not return success");
                }

                // get the cancellation cost from this booking
                var costNodes = response.SelectNodes("result/services/service");
                if (costNodes is null || costNodes.Count == 0)
                {
                    throw new Exception("cancellation costs request not in expected format");
                }

                decimal amount = 0m;
                foreach (XmlNode oCostNode in costNodes)
                {
                    amount += oCostNode.SelectSingleNode("cancellationPenalty/charge/text()").InnerText.ToSafeDecimal();
                }

                // grab the currency from the first node
                result.Amount = amount;
                result.CurrencyCode = response.SelectSingleNode("/result/services[1]/service/cancellationPenalty/currencyShort").InnerText.ToSafeString();
                result.Success = true;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Get Cancellation Cost Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Cancellation Cost", webRequest);
            }

            return result;
        }

        #endregion

        #region Build requests

        private async Task<string> BuildBookingRequestAsync(PropertyDetails propertyDetails)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<customer>");
            sb.AppendFormat("<username>{0}</username>", _settings.User(propertyDetails)).AppendLine();
            sb.AppendFormat("<password>{0}</password>", _dotwSupport.MD5Password(_settings.Password(propertyDetails))).AppendLine();
            sb.AppendFormat("<id>{0}</id>", _settings.CompanyCode(propertyDetails)).AppendLine();
            sb.AppendLine("<source>1</source>");
            sb.AppendLine("<product>hotel</product>");
            sb.AppendLine("<request command=\"confirmbooking\">");
            sb.AppendLine("<bookingDetails>");
            sb.AppendFormat("<fromDate>{0}</fromDate>", propertyDetails.ArrivalDate.ToString("yyyy-MM-dd")).AppendLine();
            sb.AppendFormat("<toDate>{0}</toDate>", propertyDetails.DepartureDate.ToString("yyyy-MM-dd")).AppendLine();
            sb.AppendFormat("<currency>{0}</currency>", await _dotwSupport.GetCurrencyCodeAsync(propertyDetails.ISOCurrencyCode, propertyDetails));
            sb.AppendFormat("<productId>{0}</productId>", propertyDetails.TPKey);

            string customerReference = propertyDetails.BookingReference.Trim();

            if (string.IsNullOrEmpty(customerReference))
            {
                customerReference = DateTime.Now.ToString("yyyyMMddhhmmss");
            }

            sb.AppendFormat("<customerReference>{0}</customerReference>", customerReference);
            sb.AppendFormat("<rooms no=\" {0}\">", propertyDetails.Rooms.Count);

            int roomRunNo = 0;
            foreach (var roomDetails in propertyDetails.Rooms)
            {
                string roomTypeCode = roomDetails.ThirdPartyReference.Split('|')[0];
                string mealBasis = roomDetails.ThirdPartyReference.Split('|')[1];
                string allocationDetails = roomDetails.ThirdPartyReference.Split('|')[2];

                int adults = roomDetails.Passengers.TotalAdults;
                int children = 0;

                foreach (var passenger in roomDetails.Passengers)
                {
                    if (passenger.PassengerType == PassengerType.Child || passenger.PassengerType == PassengerType.Infant)
                    {
                        if (passenger.Age > 12)
                        {
                            adults += 1;
                        }
                        else
                        {
                            children += 1;
                        }
                    }
                }

                sb.AppendFormat("<room runno=\" {0}\">", roomRunNo);
                sb.AppendFormat("<roomTypeCode>{0}</roomTypeCode>", roomTypeCode);
                sb.AppendFormat("<selectedRateBasis>{0}</selectedRateBasis>", mealBasis);

                sb.AppendFormat("<allocationDetails>{0}</allocationDetails>", allocationDetails);
                sb.AppendFormat("<adultsCode>{0}</adultsCode>", adults);
                sb.AppendFormat("<children no=\" {0}\">", children);

                int childAgeRunNo = 0;
                foreach (var passenger in roomDetails.Passengers)
                {
                    if (passenger.PassengerType == PassengerType.Child && passenger.Age <= 12 || passenger.PassengerType == PassengerType.Infant)
                    {
                        sb.AppendFormat("<child runno=\"{0}\">{1}</child>", childAgeRunNo.ToString(), passenger.Age == 0 ? 1 : passenger.Age.ToString());
                        childAgeRunNo += 1;
                    }
                }

                sb.AppendFormat("</children>");
                sb.AppendLine("<extraBed>0</extraBed>");
                if (_settings.Version(propertyDetails) == 2)
                {
                    string nationality = await GetNationalityAsync(propertyDetails.ISONationalityCode, propertyDetails, _support, _settings);
                    string countryCode = GetCountryOfResidence(nationality, propertyDetails, _settings);

                    if (!string.IsNullOrEmpty(nationality))
                    {
                        sb.AppendFormat("<passengerNationality>{0}</passengerNationality>", nationality);
                    }

                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        sb.AppendFormat("<passengerCountryOfResidence>{0}</passengerCountryOfResidence>", countryCode);
                    }
                }

                sb.AppendLine("<passengersDetails>");

                int guestRunNo = 0;
                foreach (var passenger in roomDetails.Passengers)
                {
                    // get the guest title
                    int titleId;

                    if (passenger.PassengerType == PassengerType.Child || passenger.PassengerType == PassengerType.Infant)
                    {
                        titleId = _dotwSupport.GetTitleID("Child");
                    }
                    else
                    {
                        titleId = _dotwSupport.GetTitleID(passenger.Title);
                    }

                    sb.AppendFormat("<passenger leading=\"{0}\">", guestRunNo == 0 ? "yes" : "no");
                    sb.AppendFormat("<salutation>{0}</salutation>", titleId);
                    sb.AppendFormat("<firstName>{0}</firstName>", _dotwSupport.CleanName(passenger.FirstName));
                    sb.AppendFormat("<lastName>{0}</lastName>", _dotwSupport.CleanName(passenger.LastName));
                    sb.AppendLine("</passenger>");

                    guestRunNo++;
                }

                sb.AppendLine("</passengersDetails>");
                sb.AppendLine("</room>");

                roomRunNo += 1;
            }

            sb.AppendLine("</rooms>");
            sb.AppendLine("</bookingDetails>");
            sb.AppendLine("</request>");
            sb.AppendLine("</customer>");

            return sb.ToString();
        }

        private string BuildCancellationCostRequest(string bookingReference, IThirdPartyAttributeSearch searchDetails)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<customer>");
            sb.AppendFormat("<username>{0}</username>", _settings.User(searchDetails)).AppendLine();
            sb.AppendFormat("<password>{0}</password>", _dotwSupport.MD5Password(_settings.Password(searchDetails))).AppendLine();
            sb.AppendFormat("<id>{0}</id>", _settings.CompanyCode(searchDetails)).AppendLine();
            sb.AppendLine("<source>1</source>");
            sb.AppendLine("<request command=\"deleteitinerary\">");
            sb.AppendLine("<bookingDetails>");
            sb.AppendLine("<bookingType>1</bookingType>");
            sb.AppendFormat("<bookingCode>{0}</bookingCode>", bookingReference);
            sb.AppendLine("<confirm>no</confirm> ");
            sb.AppendLine("</bookingDetails>");
            sb.AppendLine("</request>");
            sb.AppendLine("</customer>");

            return sb.ToString();
        }

        private string BuildCancellationRequest(string bookingReference, IThirdPartyAttributeSearch searchDetails, Dictionary<string, string> cancellationDetails)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<customer>");
            sb.AppendFormat("<username>{0}</username>", _settings.User(searchDetails)).AppendLine();
            sb.AppendFormat("<password>{0}</password>", _dotwSupport.MD5Password(_settings.Password(searchDetails))).AppendLine();
            sb.AppendFormat("<id>{0}</id>", _settings.CompanyCode(searchDetails)).AppendLine();
            sb.AppendLine("<source>1</source>");
            sb.AppendLine("<request command=\"deleteitinerary\">");
            sb.AppendLine("<bookingDetails>");
            sb.AppendLine("<bookingType>1</bookingType>");
            sb.AppendFormat("<bookingCode>{0}</bookingCode>", bookingReference);
            sb.AppendLine("<confirm>yes</confirm>");
            sb.AppendLine("<testPricesAndAllocation>");

            foreach (var detail in cancellationDetails)
            {
                sb.AppendFormat("<service referencenumber=\"{0}\">", detail.Key).AppendLine();
                sb.AppendFormat("<penaltyApplied>{0}</penaltyApplied>", detail.Value).AppendLine();
                sb.AppendLine("</service>");
            }

            sb.AppendLine("</testPricesAndAllocation>");
            sb.AppendLine("</bookingDetails>");
            sb.AppendLine("</request>");
            sb.AppendLine("</customer>");

            return sb.ToString();
        }

        private async Task<string> BuildPreBookRequestAsync(PropertyDetails propertyDetails)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<customer>");
            sb.AppendFormat("<username>{0}</username>", _settings.User(propertyDetails));
            sb.AppendFormat("<password>{0}</password>", _dotwSupport.MD5Password(_settings.Password(propertyDetails)));
            sb.AppendFormat("<id>{0}</id>", _settings.CompanyCode(propertyDetails));
            sb.AppendLine("<source>1</source>");
            sb.AppendLine("<product>hotel</product>");
            sb.AppendLine("<request command = \"getrooms\">");
            sb.AppendLine("<bookingDetails>");
            sb.AppendFormat("<fromDate>{0}</fromDate>", propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"));
            sb.AppendFormat("<toDate>{0}</toDate>", propertyDetails.DepartureDate.ToString("yyyy-MM-dd"));
            sb.AppendFormat("<currency>{0}</currency>", await _dotwSupport.GetCurrencyCodeAsync(propertyDetails.ISOCurrencyCode, propertyDetails));
            sb.AppendFormat("<rooms no = \"{0}\">", propertyDetails.Rooms.Count);

            int roomRunNo = 0;
            foreach (var roomDetails in propertyDetails.Rooms)
            {
                string roomTypeCode = roomDetails.ThirdPartyReference.Split('|')[0];
                string mealBasis = roomDetails.ThirdPartyReference.Split('|')[1];

                int adults = roomDetails.AdultsSetAgeOrOver(13);
                int children = roomDetails.ChildrenUnderSetAge(13);

                sb.AppendFormat("<room runno=\"{0}\">", roomRunNo);
                sb.AppendFormat("<adultsCode>{0}</adultsCode>", adults);
                sb.AppendFormat("<children no=\"{0}\">", children);

                int childAgeRunNo = 0;
                foreach (int age in roomDetails.Passengers.ChildAgesUnderSetAge(13))
                {
                    sb.AppendFormat("<child runno=\"{0}\">{1}</child>", childAgeRunNo, age);
                    childAgeRunNo++;
                }

                sb.AppendLine("</children>");
                sb.AppendLine("<extraBed>0</extraBed>");
                sb.AppendFormat("<rateBasis>{0}</rateBasis>", mealBasis);

                if (_settings.Version(propertyDetails) == 2)
                {
                    string nationality = await GetNationalityAsync(propertyDetails.ISONationalityCode, propertyDetails, _support, _settings);
                    string countryCode = GetCountryOfResidence(nationality, propertyDetails, _settings);

                    if (!string.IsNullOrEmpty(nationality))
                    {
                        sb.AppendFormat("<passengerNationality>{0}</passengerNationality>", nationality);
                    }

                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        sb.AppendFormat("<passengerCountryOfResidence>{0}</passengerCountryOfResidence>", countryCode);
                    }
                }

                sb.AppendLine("</room>");
                roomRunNo++;
            }

            sb.AppendLine("</rooms>");
            sb.AppendFormat("<productId>{0}</productId>", propertyDetails.TPKey);
            sb.AppendLine("</bookingDetails>");
            sb.AppendLine("</request>");

            sb.AppendLine("</customer>");

            return sb.ToString();
        }

        private async Task<string> BuildBlockRequestAsync(PropertyDetails propertyDetails)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<customer>");
            sb.AppendFormat("<username>{0}</username>", _settings.User(propertyDetails));
            sb.AppendFormat("<password>{0}</password>", _dotwSupport.MD5Password(_settings.Password(propertyDetails)));
            sb.AppendFormat("<id>{0}</id>", _settings.CompanyCode(propertyDetails));
            sb.AppendLine("<source>1</source>");
            sb.AppendLine("<product>hotel</product>");
            sb.AppendLine("<request command = \"getrooms\">");
            sb.AppendLine("<bookingDetails>");
            sb.AppendFormat("<fromDate>{0}</fromDate>", propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"));
            sb.AppendFormat("<toDate>{0}</toDate>", propertyDetails.DepartureDate.ToString("yyyy-MM-dd"));
            sb.AppendFormat("<currency>{0}</currency>", await _dotwSupport.GetCurrencyCodeAsync(propertyDetails.ISOCurrencyCode, propertyDetails));
            sb.AppendFormat("<rooms no = \"{0}\">", propertyDetails.Rooms.Count);

            int roomRunNo = 0;
            foreach (var roomDetails in propertyDetails.Rooms)
            {
                string roomTypeCode = roomDetails.ThirdPartyReference.Split('|')[0];
                string mealBasis = roomDetails.ThirdPartyReference.Split('|')[1];
                string allocationDetail = roomDetails.ThirdPartyReference.Split('|')[2];

                int adults = roomDetails.AdultsSetAgeOrOver(13);
                int children = roomDetails.ChildrenUnderSetAge(13);

                sb.AppendFormat("<room runno=\"{0}\">", roomRunNo);
                sb.AppendFormat("<adultsCode>{0}</adultsCode>", adults);
                sb.AppendFormat("<children no=\"{0}\">", children);

                int childAgeRunNo = 0;
                foreach (int age in roomDetails.Passengers.ChildAgesUnderSetAge(13))
                {
                    sb.AppendFormat("<child runno=\"{0}\">{1}</child>", childAgeRunNo, age);
                    childAgeRunNo += 1;
                }

                sb.AppendLine("</children>");
                sb.AppendLine("<extraBed>0</extraBed>");
                sb.AppendLine("<rateBasis>-1</rateBasis>");

                if (_settings.Version(propertyDetails) == 2)
                {
                    string nationality = await GetNationalityAsync(propertyDetails.ISONationalityCode, propertyDetails, _support, _settings);
                    string countryCode = GetCountryOfResidence(nationality, propertyDetails, _settings);

                    if (!string.IsNullOrEmpty(nationality))
                    {
                        sb.AppendFormat("<passengerNationality>{0}</passengerNationality>", nationality);
                    }

                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        sb.AppendFormat("<passengerCountryOfResidence>{0}</passengerCountryOfResidence>", countryCode);
                    }
                }

                sb.AppendLine("<roomTypeSelected>");
                sb.AppendFormat("<code>{0}</code>", roomTypeCode);
                sb.AppendFormat("<selectedRateBasis>{0}</selectedRateBasis>", mealBasis);
                sb.AppendFormat("<allocationDetails>{0}</allocationDetails>", allocationDetail);
                sb.AppendLine("</roomTypeSelected>");
                sb.AppendLine("</room>");

                roomRunNo++;
            }

            sb.AppendLine("</rooms>");
            sb.AppendFormat("<productId>{0}</productId>", propertyDetails.TPKey);
            sb.AppendLine("</bookingDetails>");
            sb.AppendLine("</request>");

            sb.AppendLine("</customer>");

            return sb.ToString();
        }

        #endregion

        #region Nationality and country of residence

        public async static Task<string> GetNationalityAsync(string nationalityISOCode, IThirdPartyAttributeSearch searchDetails, ITPSupport support, IDOTWSettings settings)
        {
            string nationality = await support.TPNationalityLookupAsync(ThirdParties.DOTW, nationalityISOCode);

            if (string.IsNullOrEmpty(nationality))
            {
                nationality = settings.LeadGuestNationality(searchDetails);
            }

            return nationality;
        }

        public static string GetCountryOfResidence(string countryCode, IThirdPartyAttributeSearch searchDetails, IDOTWSettings settings)
        {
            if (string.IsNullOrEmpty(countryCode))
            {
                countryCode = settings.CustomerCountryCode(searchDetails);
            }

            return countryCode;
        }

        #endregion
    }
}