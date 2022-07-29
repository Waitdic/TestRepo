namespace iVectorOne.Suppliers.Bonotel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;

    public class Bonotel : IThirdParty, ISingleSource
    {
        private readonly IBonotelSettings _settings;

        private readonly HttpClient _httpclient;

        private readonly ISerializer _serializer;

        private readonly ILogger<Bonotel> _logger;

        #region Constructor

        public Bonotel(IBonotelSettings settings, HttpClient httpClient, ISerializer serializer, ILogger<Bonotel> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpclient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region Properties

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
            => _settings.AllowCancellations(searchDetails, false);

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public string Source => ThirdParties.BONOTEL;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
            => _settings.OffsetCancellationDays(searchDetails, false);

        public bool RequiresVCard(VirtualCardInfo info, string source) => false;

        #endregion

        #region PreBook

        // We don't have a prebook in their interface so just calculate the costs so that this works in the xml gateway
        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            // Get Cancelation Policy
            bool success = await CalculateCancellationPolicyAsync(propertyDetails);

            propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);

            return success;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var request = new XmlDocument();
            var response = new XmlDocument();
            string reference = "";
            var webRequest = new Request();

            propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);

            try
            {
                var baseHelper = new TPReference(propertyDetails.Rooms[0].ThirdPartyReference);

                var sb = new StringBuilder();

                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<reservationRequest returnCompeleteBookingDetails=\"Y\">");
                sb.Append("<control>");
                sb.AppendFormat("<userName>{0}</userName>", _settings.User(propertyDetails));
                sb.AppendFormat("<passWord>{0}</passWord>", _settings.Password(propertyDetails));
                sb.Append("</control>");

                sb.AppendFormat("<reservationDetails timeStamp=\"{0}\">", DateTime.Now.ToString("yyyymmddThh:mm:ss"));

                sb.AppendFormat("<confirmationType>CON</confirmationType>");
                sb.AppendFormat("<tourOperatorOrderNumber>{0}</tourOperatorOrderNumber>", DateTime.Now.ToString("yyyymmddThh:mm:ss"));
                sb.AppendFormat("<checkIn>{0}</checkIn>", propertyDetails.ArrivalDate.ToString("dd-MMM-yyyy"));
                sb.AppendFormat("<checkOut>{0}</checkOut>", propertyDetails.DepartureDate.ToString("dd-MMM-yyyy"));
                sb.AppendFormat("<noOfRooms>{0}</noOfRooms>", propertyDetails.Rooms.Count);
                sb.AppendFormat("<noOfNights>{0}</noOfNights>", propertyDetails.Duration);
                sb.AppendFormat("<hotelCode>{0}</hotelCode>", propertyDetails.TPKey);
                sb.AppendFormat("<total currency=\"{0}\">{1}</total>", baseHelper.CurrencyCode, propertyDetails.LocalCost);
                sb.AppendFormat("<totalTax currency=\"{0}\">{1}</totalTax>", baseHelper.CurrencyCode, baseHelper.TotalTax);

                int roomNumber = 1;
                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    baseHelper = new TPReference(roomDetails.ThirdPartyReference);

                    // room
                    sb.Append("<roomData>");
                    sb.AppendFormat("<roomNo>{0}</roomNo>", roomNumber);
                    sb.AppendFormat("<roomCode>{0}</roomCode>", baseHelper.RoomCode);
                    sb.AppendFormat("<roomTypeCode>{0}</roomTypeCode>", baseHelper.RoomTypeCode);
                    sb.AppendFormat("<bedTypeCode>{0}</bedTypeCode>", baseHelper.BedTypeCode);
                    sb.AppendFormat("<ratePlanCode>{0}</ratePlanCode>", baseHelper.MealBasis);
                    sb.AppendFormat("<noOfAdults>{0}</noOfAdults>", roomDetails.Passengers.TotalAdults);
                    sb.AppendFormat("<noOfChildren>{0}</noOfChildren>", roomDetails.Passengers.TotalChildren + roomDetails.Passengers.TotalInfants);
                    sb.Append("<occupancy>");

                    // guest
                    foreach (var passenger in roomDetails.Passengers)
                    {
                        sb.Append("<guest>");
                        sb.AppendFormat("<title>{0}</title>", passenger.Title);
                        sb.AppendFormat("<firstName>{0}</firstName>", passenger.FirstName);
                        sb.AppendFormat("<lastName>{0}</lastName>", passenger.LastName);

                        if (passenger.PassengerType == PassengerType.Child || passenger.PassengerType == PassengerType.Infant)
                        {
                            sb.AppendFormat("<age>{0}</age>", passenger.Age);
                        }

                        sb.Append("</guest>");
                    }

                    sb.Append("</occupancy>");
                    sb.Append("</roomData>");

                    roomNumber += 1;
                }

                sb.Append("<comment>");

                // any comments for the hotel
                if (propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any())
                {
                    sb.AppendFormat("<hotel>{0}</hotel>", string.Join("\n", propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Select(x => x.SpecialRequest)));
                }

                // any other comments from the customer
                sb.Append("<customer></customer>");
                sb.Append("</comment>");
                sb.Append("</reservationDetails>");
                sb.Append("</reservationRequest>");

                request.LoadXml(sb.ToString());

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails) + "GetReservation.do",
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Text_xml,
                    Source = ThirdParties.BONOTEL,
                    TimeoutInSeconds = _settings.BookTimeout(propertyDetails)
                };
                webRequest.SetRequest(request);
                await webRequest.Send(_httpclient, _logger);

                response = webRequest.ResponseXML;

                if (!string.IsNullOrEmpty(response.InnerText))
                {
                    response.LoadXml(response.InnerXml);
                }
                else
                {
                    reference = "failed";
                }

                // Get the reference
                if (response.SelectSingleNode("/reservationresponse/referenceno") is not null)
                {
                    reference = response.SelectSingleNode("/reservationresponse/referenceno").InnerText;
                }
                else if (response.SelectSingleNode("/reservationResponse/referenceNo") is not null)
                {
                    reference = response.SelectSingleNode("/reservationResponse/referenceNo").InnerText;
                }
                else
                {
                    reference = "failed";
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());

                reference = "failed";
            }
            finally
            {
                propertyDetails.AddLog("Book", webRequest);
            }

            return reference;
        }

        #endregion

        #region Cancellation

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            var request = new XmlDocument();
            var response = new XmlDocument();
            var webRequest = new Request();

            try
            {
                // Create XML for cancellation request
                string requestBody = BuildCancellationRequest(propertyDetails.SourceReference, propertyDetails);

                // Log request
                request.LoadXml(requestBody);

                // Send the request
                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails) + "GetCancellation.do",
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Text_xml,
                    Source = ThirdParties.BONOTEL,
                    CreateLog = true,
                    LogFileName = "Cancellation"
                };
                webRequest.SetRequest(request);
                await webRequest.Send(_httpclient, _logger);

                response = webRequest.ResponseXML;

                // Get the reference
                if (response.SelectSingleNode("cancellationResponse") is not null)
                {
                    if (response.SelectSingleNode("cancellationResponse").Attributes["status"].Value == "Y")
                    {
                        thirdPartyCancellationResponse.TPCancellationReference = response.SelectSingleNode("cancellationResponse/cancellationNo").InnerText;
                        thirdPartyCancellationResponse.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Cancellation", webRequest);
            }

            return thirdPartyCancellationResponse;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails PropertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        private string BuildCancellationRequest(string BookingReference, IThirdPartyAttributeSearch SearchDetails)
        {

            var sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.Append("<cancellationRequest>");
            sb.Append("<control>");
            sb.AppendFormat("<userName>{0}</userName>", _settings.User(SearchDetails));
            sb.AppendFormat("<passWord>{0}</passWord>", _settings.Password(SearchDetails));
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

        public void EndSession(PropertyDetails propertyDetails)
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

        public async Task<bool> CalculateCancellationPolicyAsync(PropertyDetails propertyDetails)
        {
            bool success = true;
            var request = new XmlDocument();
            var response = new XmlDocument();
            var webRequest = new Request();

            try
            {
                request.LoadXml(GetAvailabilityRequest(propertyDetails));

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails) + "GetAvailability.do",
                    Method = RequestMethod.POST,
                    Source = ThirdParties.BONOTEL,
                    LogFileName = "CancellationCharges",
                    CreateLog = true,
                    ContentType = ContentTypes.Text_xml
                };
                webRequest.SetRequest(request);
                await webRequest.Send(_httpclient, _logger);

                response = webRequest.ResponseXML;
                foreach (XmlNode roomNode in response.SelectNodes("availabilityResponse/hotelList/hotel/roomInformation"))
                {
                    string feeType = "";
                    string feeIndicator = "";
                    string errataTitle = "";
                    string errataDescription = "";
                    if (roomNode.SelectNodes("rateInformation/hotelFees/hotelFee").Count > 0)
                    {
                        foreach (XmlNode hotelFeeNode in roomNode.SelectNodes("rateInformation/hotelFees/hotelFee"))
                        {
                            feeType = hotelFeeNode.SelectSingleNode("feeType").InnerText;
                            string requiredFee = hotelFeeNode.SelectSingleNode("requiredFee").InnerText;
                            string feeMethod = hotelFeeNode.SelectSingleNode("feeMethod").InnerText;
                            feeIndicator = GetFeeIndicator(requiredFee, feeMethod);
                            errataTitle = string.Format("{0} Fee - {1}", feeType, feeIndicator);
                            string amount = hotelFeeNode.SelectSingleNode("feeTotal").InnerText;
                            string conditions = hotelFeeNode.SelectSingleNode("conditions").InnerText;
                            errataDescription = string.Format("{0}{1}", amount, conditions);
                            propertyDetails.Errata.AddNew(errataTitle.ToNiceName(), errataDescription);
                        }
                    }
                }

                var result = _serializer.DeSerialize<AvailabilityResponse>(response);

                // Take all the policies out of the xsl
                var policies = new List<CancellationPolicy>();

                foreach (Hotel hotel in result.hotelList.hotel)
                {
                    foreach (RoomInformation room in hotel.roomInformation)
                    {
                        foreach (RoomBookingPolicy policy in room.roomBookingPolicy)
                        {
                            var oPolicyLoad = new CancellationPolicy
                            {
                                AmendmentType = policy.amendmentType,
                                PolicyBasedOn = policy.policyBasedOn,
                                PolicyBasedOnValue = policy.policyBasedOnValue,
                                CancellationType = policy.cancellationType,
                                StayDateRequirement = policy.stayDateRequirement,
                                ArrivalRange = policy.arrivalRange,
                                ArrivalRangeValue = policy.arrivalRangeValue,
                                PolicyFee = policy.policyFee.Replace("$", "").ToSafeDecimal(),
                                NoShowBasedOn = policy.noShowBasedOn,
                                NoShowBasedOnValue = policy.noShowBasedOnValue,
                                NoShowPolicyFee = policy.noShowPolicyFee.Replace("$", "").ToSafeDecimal()
                            };

                            policies.Add(oPolicyLoad);
                        }
                    }
                }

                // Loads The Policies into the OverrideSupplierCancellations Class
                var cancellations = new Cancellations();

                foreach (var policy in policies)
                {
                    bool specialFlag = false;

                    if (policy.AmendmentType == "Cancel")
                    {
                        // Checks if there is a special policy that overlaps the normal policy
                        if (policy.CancellationType == "Special")
                        {
                            specialFlag = true;

                            if (policy.ArrivalRange == "Less Than")
                            {
                                int days = policy.ArrivalRangeValue.ToSafeInt();

                                cancellations.AddNew(propertyDetails.ArrivalDate.AddDays(-days), propertyDetails.ArrivalDate.AddDays(-1), policy.PolicyFee);

                                cancellations.AddNew(propertyDetails.ArrivalDate, new DateTime(2099, 1, 1), policy.NoShowPolicyFee);
                            }
                            else if (policy.ArrivalRange == "Greater Than")
                            {
                                cancellations.AddNew(propertyDetails.ArrivalDate, new DateTime(2099, 1, 1), policy.NoShowPolicyFee);
                            }
                            else if (policy.ArrivalRange == "Any")
                            {
                                cancellations.AddNew(DateTime.Now, propertyDetails.ArrivalDate.AddDays(-1), policy.PolicyFee);

                                cancellations.AddNew(propertyDetails.ArrivalDate, new DateTime(2099, 1, 1), policy.NoShowPolicyFee);
                            }
                        }

                        // Normal Policy
                        else if (policy.CancellationType == "Normal" & specialFlag == false)
                        {
                            if (policy.ArrivalRange == "Less Than")
                            {
                                int days = policy.ArrivalRangeValue.ToSafeInt();

                                cancellations.AddNew(propertyDetails.ArrivalDate.AddDays(-days), propertyDetails.ArrivalDate.AddDays(-1), policy.PolicyFee);

                                cancellations.AddNew(propertyDetails.ArrivalDate, new DateTime(2099, 1, 1), policy.NoShowPolicyFee);
                            }
                            else if (policy.ArrivalRange == "Greater Than")
                            {
                                cancellations.AddNew(propertyDetails.ArrivalDate, new DateTime(2099, 1, 1), policy.NoShowPolicyFee);
                            }
                            else if (policy.ArrivalRange == "Any")
                            {
                                cancellations.AddNew(DateTime.Now, propertyDetails.ArrivalDate.AddDays(-1), policy.PolicyFee);

                                cancellations.AddNew(propertyDetails.ArrivalDate, new DateTime(2099, 1, 1), policy.NoShowPolicyFee);
                            }
                        }
                    }

                    specialFlag = false;
                }

                cancellations.Solidify(SolidifyType.Sum);
                propertyDetails.Cancellations = cancellations;

                // check for price changes here
                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    var baseHelper = new TPReference(roomDetails.ThirdPartyReference);

                    foreach (XmlNode roomNode in webRequest.ResponseXML.SelectNodes("availabilityResponse/hotelList/hotel/roomInformation"))
                    {
                        // find the correct node via room code
                        if ((roomNode.SafeNodeValue("roomCode") ?? "") == (baseHelper.RoomCode ?? "") & (roomNode.SafeNodeValue("rateInformation/ratePlanCode") ?? "") == (baseHelper.MealBasis ?? ""))
                        {
                            decimal newPrice = roomNode.SafeNodeValue("rateInformation/totalRate").ToSafeMoney();
                            if (newPrice != roomDetails.LocalCost)
                            {
                                roomDetails.GrossCost = newPrice;
                                roomDetails.LocalCost = newPrice;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancellation Costs Exception", ex.ToString());
                success = false;
            }
            finally
            {
                propertyDetails.AddLog("Cancellation Cost", webRequest);
            }

            return success;
        }

        public string GetAvailabilityRequest(PropertyDetails propertyDetails)
        {
            var sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<availabilityRequest cancelpolicy=\"Y\" hotelfees=\"Y\">");

            sb.Append("<control>");
            sb.AppendFormat("<userName>{0}</userName>", _settings.User(propertyDetails));
            sb.AppendFormat("<passWord>{0}</passWord>", _settings.Password(propertyDetails));
            sb.Append("</control>");

            sb.AppendFormat("<checkIn>{0}</checkIn>", propertyDetails.ArrivalDate.ToString("dd-MMM-yyyy"));
            sb.AppendFormat("<checkOut>{0}</checkOut>", propertyDetails.DepartureDate.ToString("dd-MMM-yyyy"));
            sb.AppendFormat("<noOfRooms>{0}</noOfRooms>", propertyDetails.Rooms.Count);
            sb.AppendFormat("<noOfNights>{0}</noOfNights>", propertyDetails.Duration);
            sb.AppendFormat("<hotelCodes>");
            sb.AppendFormat("<hotelCode>{0}</hotelCode>", propertyDetails.TPKey);
            sb.AppendFormat("</hotelCodes>");


            sb.AppendFormat("<roomsInformation>");

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                var baseHelper = new TPReference(roomDetails.ThirdPartyReference);

                sb.AppendFormat("<roomInfo>");
                sb.AppendFormat("<roomTypeId>{0}</roomTypeId>", baseHelper.RoomTypeCode);
                sb.AppendFormat("<bedTypeId>{0}</bedTypeId> ", baseHelper.BedTypeCode);
                sb.AppendFormat("<adultsNum>{0}</adultsNum>", roomDetails.Passengers.TotalAdults);
                sb.AppendFormat("<childNum>{0}</childNum>", roomDetails.Passengers.TotalChildren + roomDetails.Passengers.TotalInfants);

                if (roomDetails.Passengers.TotalChildren + roomDetails.Passengers.TotalInfants > 0)
                {
                    sb.AppendFormat("<childAges>");

                    foreach (var passenger in roomDetails.Passengers)
                    {
                        if (passenger.PassengerType == PassengerType.Child || passenger.PassengerType == PassengerType.Infant)
                        {
                            sb.AppendFormat("<childAge>{0}</childAge>", passenger.Age);
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

        public string GetFeeIndicator(string feeIndicator, string feeMethod)
        {
            string indicator = "";
            if (feeIndicator == "No")
            {
                indicator = "Optional";
            }
            else if (feeMethod == "Exclusive")
            {
                indicator = "Payable Locally";
            }
            else if (feeMethod == "Inclusive")
            {
                indicator = "Included";
            }
            return indicator;
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