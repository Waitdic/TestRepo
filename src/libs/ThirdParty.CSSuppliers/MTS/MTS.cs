namespace ThirdParty.CSSuppliers.MTS
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
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public partial class MTS : IThirdParty, ISingleSource
    {
        #region Constructor

        private readonly IMTSSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MTS> _logger;

        public MTS(IMTSSettings settings, HttpClient httpClient, ILogger<MTS> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.MTS;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails, false);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        private readonly List<string> overrideCountriesList = new List<string>();

        private List<string> GetOverrideCountries(PropertyDetails oPropertyDetails)
        {
            if (overrideCountriesList.Count == 0)
            {
                string overrideCountries = _settings.OverrideCountries(oPropertyDetails);

                if (!string.IsNullOrWhiteSpace(overrideCountries))
                {
                    // split the string and add each one to _overrideCountries
                    foreach (string country in overrideCountries.Split('|'))
                    {
                        overrideCountriesList.Add(country);
                    }
                }
                else
                {
                    overrideCountriesList.Add("United Arab Emirates");
                    overrideCountriesList.Add("Turkey");
                    overrideCountriesList.Add("Egypt");
                }
            }
            return overrideCountriesList;
        }
        #endregion

        #region Prebook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            bool success = true;

            var sbVerifyCart = new StringBuilder();
            var response = new XmlDocument();

            try
            {
                sbVerifyCart.Append("<OTA_HotelResRQ xmlns=\"http://www.opentravel.org/OTA/2003/05\" EchoToken=\"12866195988106211282233751\" ResStatus=\"Quote\" Version=\"0.1\" schemaLocation=\"OTA_HotelResRQ.xsd\">");
                sbVerifyCart.Append(GeneratePosTag(propertyDetails));
                sbVerifyCart.Append("<HotelReservations>");
                sbVerifyCart.Append("<HotelReservation>");
                sbVerifyCart.Append("<RoomStays>");

                int count = 0;
                int roomCount = 1;
                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    sbVerifyCart.Append("<RoomStay>");
                    sbVerifyCart.Append("<RoomTypes>");
                    sbVerifyCart.AppendFormat("<RoomType RoomTypeCode = \"{0}\"></RoomType>", roomDetails.ThirdPartyReference.Split('|')[0]);
                    sbVerifyCart.Append("</RoomTypes>");
                    sbVerifyCart.AppendFormat("<TimeSpan End = \"{0}\" Start = \"{1}\"></TimeSpan>", propertyDetails.DepartureDate.ToString("yyyy-MM-dd"), propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"));
                    sbVerifyCart.AppendFormat("<BasicPropertyInfo HotelCode = \"{0}\"></BasicPropertyInfo>", propertyDetails.TPKey);
                    sbVerifyCart.Append("<ResGuestRPHs>");

                    // need a new RPH for each guest; loop to add 1 for each new guest
                    foreach (var passenger in roomDetails.Passengers)
                    {
                        count++;
                        sbVerifyCart.AppendFormat("<ResGuestRPH RPH = \"{0}\"></ResGuestRPH>", count);
                    }

                    sbVerifyCart.Append("</ResGuestRPHs>");
                    sbVerifyCart.Append("<ServiceRPHs>");
                    sbVerifyCart.AppendFormat("<ServiceRPH RPH = \"{0}\"></ServiceRPH>", roomCount);
                    sbVerifyCart.Append("</ServiceRPHs>");
                    sbVerifyCart.Append("</RoomStay>");
                    roomCount += 1;
                }

                roomCount = 1;
                sbVerifyCart.Append("</RoomStays>");
                sbVerifyCart.Append("<Services>");

                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    sbVerifyCart.AppendFormat("<Service ServiceInventoryCode=\"{0}\" ServiceRPH=\"{1}\"></Service>", roomDetails.ThirdPartyReference.Split('|')[1], roomCount);
                    roomCount += 1;
                }

                sbVerifyCart.Append("</Services>");
                sbVerifyCart.Append("<ResGuests>");

                // need to loop for each person
                int guestCounter = 0;
                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    foreach (var passenger in roomDetails.Passengers)
                    {
                        guestCounter += 1;

                        var ageQualifyingCode = default(int);
                        if (passenger.PassengerType == PassengerType.Adult)
                        {
                            ageQualifyingCode = 10;
                        }
                        if (passenger.PassengerType == PassengerType.Child)
                        {
                            ageQualifyingCode = 8;
                        }
                        if (passenger.PassengerType == PassengerType.Infant)
                        {
                            ageQualifyingCode = 7;
                        }

                        sbVerifyCart.AppendFormat("<ResGuest AgeQualifyingCode = \"{0}\" ResGuestRPH = \"{1}\">", ageQualifyingCode, guestCounter);

                        if (ageQualifyingCode == 8)
                        {
                            sbVerifyCart.AppendFormat("<GuestCounts><GuestCount Age=\"{0}\"/></GuestCounts>", passenger.Age);
                        }

                        if (ageQualifyingCode == 7)
                        {
                            sbVerifyCart.AppendFormat("<GuestCounts><GuestCount Age=\"1\"/></GuestCounts>");
                        }

                        sbVerifyCart.Append("</ResGuest>");
                    }
                }

                sbVerifyCart.Append("</ResGuests>");
                sbVerifyCart.Append("</HotelReservation>");
                sbVerifyCart.Append("</HotelReservations>");

                sbVerifyCart.Append("</OTA_HotelResRQ>");

                // get the add response 
                var webRequest = new Request
                {
                    EndPoint = _settings.BaseURL(propertyDetails),
                    Method = eRequestMethod.POST,
                    Source = ThirdParties.MTS,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    LogFileName = "PreBook",
                    CreateLog = true
                };
                webRequest.SetRequest(sbVerifyCart.ToString());
                await webRequest.Send(_httpClient, _logger);

                response.LoadXml(webRequest.ResponseXML.InnerXml.Replace(" xmlns=\"http://www.opentravel.org/OTA/2003/05\"", ""));

                // get the costs from the response
                var costs = response.SelectNodes("OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/Total/@AmountAfterTax");

                if (costs[0].InnerText.ToSafeMoney() != propertyDetails.TotalCost.ToSafeMoney())
                {
                    // Only returns total cost so divide by number of rooms
                    decimal cost = costs[0].InnerText.ToSafeMoney() / propertyDetails.Rooms.Count;

                    foreach (var room in propertyDetails.Rooms)
                    {
                        room.LocalCost = cost;
                        room.GrossCost = cost;
                    }
                }

                // cancellation charges
                var cancellations = GetCancellations(propertyDetails, response);

                foreach (var cancellation in cancellations)
                {
                    propertyDetails.Cancellations.Add(cancellation);
                }

                // Grab the Errata
                foreach (XmlNode errataNode in response.SelectNodes("/OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/BasicPropertyInfo/VendorMessages/VendorMessage"))
                {
                    propertyDetails.Errata.AddNew(errataNode.SelectSingleNode("@Title").InnerText, errataNode.SelectSingleNode("SubSection/Paragraph/Text").InnerText);
                }
            }
            catch (Exception ex)
            {
                success = false;
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
            }
            finally
            {
                // store the request and response xml on the property pre-booking
                if (!string.IsNullOrEmpty(sbVerifyCart.ToString()))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Pre-Book Request", sbVerifyCart.ToString());
                }

                if (!string.IsNullOrEmpty(response.InnerXml))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Pre-Book Response", response);
                }
            }

            return success;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var sbRequest = new StringBuilder();
            var response = new XmlDocument();
            string reference = "";
            var overrideCountries = GetOverrideCountries(propertyDetails);

            try
            {
                sbRequest.Append("<OTA_HotelResRQ xmlns=\"http://www.opentravel.org/OTA/2003/05\" EchoToken=\"12866195988106211282233751\" ResStatus=\"Commit\" Version=\"0.1\" schemaLocation=\"OTA_HotelResRQ.xsd\">");
                sbRequest.Append(GeneratePosTag(propertyDetails));
                sbRequest.Append("<HotelReservations>");
                sbRequest.Append("<HotelReservation>");
                sbRequest.Append("<RoomStays>");

                int count = 0;
                int roomCount = 1;

                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    sbRequest.Append("<RoomStay>");
                    sbRequest.Append("<RoomTypes>");
                    sbRequest.AppendFormat("<RoomType RoomTypeCode = \"{0}\"></RoomType>", roomDetails.ThirdPartyReference.Split('|')[0]);
                    sbRequest.Append("</RoomTypes>");
                    sbRequest.AppendFormat("<TimeSpan End = \"{0}\" Start = \"{1}\"></TimeSpan>", propertyDetails.DepartureDate.ToString("yyyy-MM-dd"), propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"));
                    sbRequest.AppendFormat("<BasicPropertyInfo HotelCode = \"{0}\"></BasicPropertyInfo>", propertyDetails.TPKey);
                    sbRequest.Append("<ResGuestRPHs>");

                    // need a new RPH for each guest; loop to add 1 for each new guest
                    foreach (var passenger in roomDetails.Passengers)
                    {
                        count++;
                        sbRequest.AppendFormat("<ResGuestRPH RPH = \"{0}\"></ResGuestRPH>", count);
                    }

                    sbRequest.Append("</ResGuestRPHs>");
                    sbRequest.Append("<ServiceRPHs>");
                    sbRequest.AppendFormat("<ServiceRPH RPH = \"{0}\"></ServiceRPH>", roomCount);
                    sbRequest.Append("</ServiceRPHs>");
                    sbRequest.Append("</RoomStay>");
                    roomCount++;
                }

                sbRequest.Append("</RoomStays>");
                sbRequest.Append("<Services>");
                roomCount = 1;

                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    sbRequest.AppendFormat("<Service ServiceInventoryCode=\"{0}\" ServiceRPH=\"{1}\"></Service>", roomDetails.ThirdPartyReference.Split('|')[1], roomCount);
                    roomCount += 1;
                }

                sbRequest.Append("</Services>");
                sbRequest.Append("<ResGuests>");

                // need to loop for each person
                int guestCounter = 0;
                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    foreach (var passenger in roomDetails.Passengers)
                    {
                        guestCounter += 1;

                        var ageQualifyingCode = default(int);
                        if (passenger.PassengerType == PassengerType.Adult)
                        {
                            ageQualifyingCode = 10;
                        }
                        if (passenger.PassengerType == PassengerType.Child)
                        {
                            ageQualifyingCode = 8;
                        }
                        if (passenger.PassengerType == PassengerType.Infant)
                        {
                            ageQualifyingCode = 7;
                        }

                        sbRequest.AppendFormat("<ResGuest AgeQualifyingCode = \"{0}\" ResGuestRPH = \"{1}\">", ageQualifyingCode, guestCounter);

                        sbRequest.Append("<Profiles><ProfileInfo><Profile><Customer><PersonName>");
                        sbRequest.AppendFormat("<NamePrefix>{0}</NamePrefix>", passenger.Title);
                        sbRequest.AppendFormat("<GivenName>{0}</GivenName>", passenger.FirstName);
                        sbRequest.AppendFormat("<Surname>{0}</Surname>", passenger.LastName);
                        sbRequest.Append("</PersonName></Customer></Profile></ProfileInfo></Profiles>");

                        if (ageQualifyingCode == 8)
                        {
                            sbRequest.AppendFormat("<GuestCounts><GuestCount Age=\"{0}\"/></GuestCounts>", passenger.Age);
                        }

                        if (ageQualifyingCode == 7)
                        {
                            sbRequest.AppendFormat("<GuestCounts><GuestCount Age=\"1\"/></GuestCounts>");
                        }

                        sbRequest.Append("</ResGuest>");
                    }
                }

                sbRequest.Append("</ResGuests>");

                if (propertyDetails.BookingReference != "" || propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any())
                {
                    sbRequest.Append("<ResGlobalInfo>");

                    if (propertyDetails.BookingReference != "")
                    {
                        if (overrideCountries.Contains(propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[2]))
                        {
                            string sID = _settings.OverRideID(propertyDetails);
                            sbRequest.Append("<HotelReservationIDs>");
                            sbRequest.AppendFormat("<HotelReservationID ResID_SourceContext=\"Client\" ResID_Source=\"{0}\" ResID_Value=\"{1}\" />", sID, propertyDetails.BookingReference.Replace(" ", ""));
                            sbRequest.Append("</HotelReservationIDs>");
                        }
                    }

                    if (propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any())
                    {
                        sbRequest.Append("<Comments>");

                        foreach (var room in propertyDetails.Rooms)
                        {
                            sbRequest.Append("<Comment Name = \"Applicant Notice\">");
                            sbRequest.AppendFormat("<Text>{0}</Text>", room.SpecialRequest);
                            sbRequest.Append("</Comment>");
                        }

                        sbRequest.Append("</Comments>");
                    }

                    sbRequest.Append("</ResGlobalInfo>");
                }

                sbRequest.Append("</HotelReservation>");
                sbRequest.Append("</HotelReservations>");
                sbRequest.Append("</OTA_HotelResRQ>");

                // get the response 
                var webRequest = new Request
                {
                    EndPoint = _settings.BaseURL(propertyDetails),
                    Method = eRequestMethod.POST,
                    Source = ThirdParties.MTS,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    LogFileName = "Book",
                    CreateLog = true
                };
                webRequest.SetRequest(sbRequest.ToString());
                await webRequest.Send(_httpClient, _logger);

                response = webRequest.ResponseXML;
                response.LoadXml(response.InnerXml.Replace(" xmlns=\"http://www.opentravel.org/OTA/2003/05\"", ""));

                // check for any errors and save the booking code
                // p63 of documentation
                if (response.SelectSingleNode("OTA_HotelResRS/Errors/Error") is not null)
                {
                    reference = "failed";
                }
                else
                {
                    reference = response.SelectSingleNode("OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/HotelReservationIDs/HotelReservationID[@ResID_Source=\"OTS\"]/@ResID_Value").InnerText;
                }
            }
            catch (Exception ex)
            {
                reference = "failed";
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
            }
            finally
            {
                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(sbRequest.ToString()))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Book Request", sbRequest.ToString());
                }

                if (!string.IsNullOrEmpty(response.InnerXml))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Book Response", response);
                }
            }

            return reference;
        }

        #endregion

        #region Cancellations

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var sbRequest = new StringBuilder();
            var cancellationRequest = new XmlDocument();
            var cancellationResponse = new XmlDocument();
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();

            string sourceReference;
            if (propertyDetails.SourceReference is not null)
            {
                sourceReference = propertyDetails.SourceReference;
            }
            else
            {
                return thirdPartyCancellationResponse;
            }

            try
            {
                // build the cancellation request
                sbRequest.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sbRequest.Append("<OTA_CancelRQ xmlns=\"http://www.opentravel.org/OTA/2003/05\" Version=\"0.1\" CancelType=\"Commit\">");
                sbRequest.Append(GeneratePosTag(propertyDetails));
                sbRequest.AppendFormat("<UniqueID Type=\"14\" ID=\"{0}\" ID_Context=\"Internal\"/>", sourceReference);
                sbRequest.Append("</OTA_CancelRQ>");

                // send the request
                cancellationRequest.LoadXml(sbRequest.ToString());

                // get the response
                var webRequest = new Request
                {
                    EndPoint = _settings.BaseURL(propertyDetails),
                    Method = eRequestMethod.POST,
                    Source = ThirdParties.MTS,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    LogFileName = "Cancel",
                    CreateLog = true
                };
                webRequest.SetRequest(cancellationRequest);
                await webRequest.Send(_httpClient, _logger);

                cancellationResponse = webRequest.ResponseXML;
                cancellationResponse.LoadXml(cancellationResponse.InnerXml.Replace(" xmlns=\"http://www.opentravel.org/OTA/2003/05\"", ""));

                if (cancellationResponse.SelectNodes("Errors").Count > 0)
                {
                    throw new Exception("Cancellation request did not return success");
                }
                else if (!(cancellationResponse.SelectSingleNode("/OTA_CancelRS/@Status").InnerText == "Committed"))
                {
                    throw new Exception("Cancellation request did not return success");
                }
                else
                {
                    // Get a reference
                    thirdPartyCancellationResponse.TPCancellationReference = cancellationResponse.SelectSingleNode("/OTA_CancelRS/UniqueID/@ID").InnerText;
                    thirdPartyCancellationResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                thirdPartyCancellationResponse.TPCancellationReference = "Failed";
                thirdPartyCancellationResponse.Success = false;

                propertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString());
            }
            finally
            {
                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(sbRequest.ToString()))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Cancellation Request", sbRequest.ToString());
                }

                if (!string.IsNullOrEmpty(cancellationResponse.InnerXml))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Cancellation Response", cancellationResponse);
                }
            }

            return thirdPartyCancellationResponse;
        }

        public Cancellations GetCancellations(PropertyDetails propertyDetails, XmlDocument xml)
        {
            var cancellations = new Cancellations();

            try
            {
                if (xml.SelectSingleNode("OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/CancelPenalties/CancelPenalty/AmountPercent") is not null)
                {
                    // get cancellations in list so can sort them by release period
                    foreach (XmlNode xmlNode in xml.SelectNodes("OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/CancelPenalties/CancelPenalty"))
                    {
                        var cancellation = new Cancellation();

                        decimal totalAmount = (xml.SelectSingleNode("OTA_HotelResRS/HotelReservations/HotelReservation/ResGlobalInfo/Total/@AmountAfterTax").InnerText).ToSafeMoney();
                        decimal dailyAmount = totalAmount / propertyDetails.Duration;
                        int percentage = xmlNode.SelectSingleNode("AmountPercent/@Percent").InnerText.ToSafeInt();
                        int numberOfNights = xmlNode.SafeNodeValue("AmountPercent/@NmbrOfNights").ToSafeInt();

                        if (xmlNode.SelectSingleNode("Deadline/@OffsetDropTime").InnerText == "BeforeArrival" == true)
                        {
                            // Get amount
                            if (xmlNode.SelectSingleNode("AmountPercent/@Percent") is not null)
                            {
                                if (numberOfNights > 0)
                                {
                                    cancellation.Amount = (0.01d * percentage * (double)dailyAmount * numberOfNights).ToSafeMoney();
                                }
                                else
                                {
                                    // full basis
                                    cancellation.Amount = (0.01d * percentage * (double)totalAmount).ToSafeMoney();
                                }
                            }
                            else if (xmlNode.SelectSingleNode("AmountPercent/@Amount") is not null)
                            {
                                cancellation.Amount = xmlNode.SelectSingleNode("AmountPercent/@Amount").InnerText.ToSafeMoney();
                            }

                            // get end date of cancelpenalty
                            cancellation.EndDate = propertyDetails.ArrivalDate;

                            // get start date of cancelpenalty
                            if (xmlNode.SelectSingleNode("Deadline/@OffsetTimeUnit").InnerText == "Day")
                            {
                                int days = xmlNode.SelectSingleNode("Deadline/@OffsetUnitMultiplier").InnerText.ToSafeInt();

                                cancellation.StartDate = cancellation.EndDate.AddDays(-days);

                                // If 'afterbooking' overlaps 'beforearrival', start beforearrival day after afterbooking finishes
                                if (xmlNode.SelectSingleNode("Deadline/@OffsetDropTime[AfterBooking]") is not null)
                                {
                                    var afterBookingEndDate = DateTime.Now.AddDays(xmlNode.SelectSingleNode("Deadline/@OffsetMultiplier").InnerText.ToSafeInt());

                                    if (afterBookingEndDate >= cancellation.StartDate)
                                    {
                                        cancellation.StartDate = afterBookingEndDate.AddDays(1d);
                                    }
                                }
                            }
                        }

                        cancellations.Add(cancellation);
                    }

                    // need to make it so do not overlap; if they do, put end date as being start date of next
                    for (int i = 0; i <= cancellations.Count - 1; i++)
                    {
                        if (i != 0)
                        {
                            if (cancellations[i - 1].EndDate >= cancellations[i].StartDate)
                            {
                                cancellations[i - 1].EndDate = cancellations[i].StartDate.AddDays(-1);
                            }
                        }
                    }
                }

                cancellations.Solidify(SolidifyType.Sum);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Get Cancellations Exception", ex.ToString());
            }

            return cancellations;
        }

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            var result = new ThirdPartyCancellationFeeResult();
            var cancelCostRequest = new XmlDocument();
            var cancelCostResponse = new XmlDocument();

            try
            {
                // build the request
                var sbGetFees = new StringBuilder();

                // Build cancellationcost XML
                sbGetFees.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sbGetFees.Append("<OTA_CancelRQ xmlns=\"http://www.opentravel.org/OTA/2003/05\" Version=\"0.1\" CancelType=\"Quote\">");
                sbGetFees.Append(GeneratePosTag(propertyDetails));

                if (propertyDetails.SourceSecondaryReference != "")
                {
                    sbGetFees.AppendFormat("<UniqueID Type=\"14\" ID=\"{0}\" ID_Context=\"Internal\"/>", propertyDetails.SourceSecondaryReference);
                }
                else
                {
                    sbGetFees.AppendFormat("<UniqueID Type=\"14\" ID=\"{0}\" ID_Context=\"Internal\"/>", propertyDetails.SourceReference);
                }

                sbGetFees.Append("</OTA_CancelRQ>");

                // Send cancellation request to MTS and log
                cancelCostRequest.LoadXml(sbGetFees.ToString());

                // get the add response 
                var webRequest = new Request
                {
                    EndPoint = _settings.BaseURL(propertyDetails),
                    Method = eRequestMethod.POST,
                    Source = ThirdParties.MTS,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    LogFileName = "Cancellation Costs",
                    CreateLog = true
                };
                webRequest.SetRequest(cancelCostRequest);
                await webRequest.Send(_httpClient, _logger);

                cancelCostResponse = webRequest.ResponseXML;
                cancelCostResponse.LoadXml(cancelCostResponse.InnerXml.Replace(" xmlns=\"http://www.opentravel.org/OTA/2003/05\"", ""));

                if (cancelCostResponse.SelectSingleNode("OTA_CancelRS/CancelInfoRS/CancelRules/CancelRule/@Amount") is not null)
                {
                    // get the cancellation cost
                    result.Amount = cancelCostResponse.SelectSingleNode("OTA_CancelRS/CancelInfoRS/CancelRules/CancelRule/@Amount").InnerText.ToSafeMoney();
                    result.CurrencyCode = cancelCostResponse.SelectSingleNode("OTA_CancelRS/CancelInfoRS/CancelRules/CancelRule/@CurrencyCode").InnerText.ToSafeString();
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                propertyDetails.Warnings.AddNew("Cancellation Cost Exception", ex.ToString());
            }
            finally
            {
                if (!string.IsNullOrEmpty(cancelCostRequest.InnerXml))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Pre-Cancel Request", cancelCostRequest);
                }

                if (!string.IsNullOrEmpty(cancelCostResponse.InnerXml))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.MTS, "MTS Pre-Cancel Response", cancelCostResponse);
                }
            }

            return result;
        }

        #endregion

        #region Booking Search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public string CreateReconciliationReference(string inputReference)
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

        public void EndSession(PropertyDetails propertyDetails)
        {

        }

        #region Request Components

        private string GeneratePosTag(PropertyDetails propertyDetails, string country = "")
        {
            var overrideCountries = GetOverrideCountries(propertyDetails);
            var sbPosTag = new StringBuilder();
            string id;

            if (string.IsNullOrEmpty(country))
            {
                if (!string.IsNullOrEmpty(propertyDetails.Rooms[0].ThirdPartyReference))
                {
                    country = propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[2];
                }
                else if (!string.IsNullOrEmpty(propertyDetails.ResortCode))
                {
                    country = propertyDetails.ResortCode.Split('|')[0];
                }
            }

            if (overrideCountries.Contains(country) && !string.IsNullOrEmpty(country))
            {
                id = _settings.OverRideID(propertyDetails);
            }
            else
            {
                id = _settings.ID(propertyDetails);
            }

            sbPosTag.Append("<POS>");
            sbPosTag.Append("<Source>");
            sbPosTag.AppendFormat("<RequestorID ID_Context = \"{0}\" ID = \"{1}\" Type = \"{2}\"/>", _settings.ID_Context(propertyDetails), id, _settings.Type(propertyDetails));
            sbPosTag.Append("<BookingChannel Type = \"2\"/>");
            sbPosTag.Append("</Source>");
            sbPosTag.Append("<Source>");
            sbPosTag.AppendFormat("<RequestorID Type=\"{0}\" ID=\"{1}\" MessagePassword=\"{2}\"/>", _settings.AuthenticationType(propertyDetails), _settings.AuthenticationID(propertyDetails), _settings.MessagePassword(propertyDetails));
            sbPosTag.Append("</Source>");
            sbPosTag.Append("</POS>");

            return sbPosTag.ToString();
        }

        #endregion
    }
}