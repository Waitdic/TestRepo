namespace ThirdParty.CSSuppliers.SunHotels
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Search.Models;

    public class SunHotels : IThirdParty, ISingleSource
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

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public string Source => ThirdParties.SUNHOTELS;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails, false);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source) => false;

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            // build the search url
            var sbPrebookURL = new StringBuilder();
            var prebookResponseXml = new XmlDocument();

            try
            {
                sbPrebookURL.Append(_settings.PrebookURL(propertyDetails));
                sbPrebookURL.AppendFormat("userName={0}", _settings.User(propertyDetails));
                sbPrebookURL.AppendFormat("&password={0}", _settings.Password(propertyDetails));
                sbPrebookURL.AppendFormat("&language={0}", _settings.LanguageCode(propertyDetails));
                sbPrebookURL.AppendFormat("&currency={0}", _settings.Currency(propertyDetails));
                sbPrebookURL.AppendFormat("&checkInDate={0}", GetSunHotelsDate(propertyDetails.ArrivalDate));
                sbPrebookURL.AppendFormat("&checkOutDate={0}", GetSunHotelsDate(propertyDetails.DepartureDate));
                sbPrebookURL.AppendFormat("&roomId={0}", propertyDetails.Rooms[0].ThirdPartyReference.Split('_')[0]);
                sbPrebookURL.AppendFormat("&rooms={0}", propertyDetails.Rooms.Count);
                sbPrebookURL.AppendFormat("&adults={0}", propertyDetails.Rooms[0].Adults);
                sbPrebookURL.AppendFormat("&children={0}", propertyDetails.Rooms[0].Children);

                if (propertyDetails.Rooms[0].Children > 0)
                {
                    sbPrebookURL.AppendFormat("&childrenAges={0}", propertyDetails.Rooms[0].GetChildAgeCsv());
                }
                else
                {
                    sbPrebookURL.AppendFormat("&childrenAges=");
                }

                sbPrebookURL.AppendFormat("&infant={0}", IsInfantIncluded(propertyDetails.Rooms[0]));
                sbPrebookURL.AppendFormat("&mealId={0}", propertyDetails.Rooms[0].ThirdPartyReference.Split('_')[1]);
                sbPrebookURL.AppendFormat("&customerCountry={0}", _settings.CustomerCountryCode(propertyDetails));
                sbPrebookURL.Append("&B2C=");
                sbPrebookURL.Append("&searchPrice=");
                sbPrebookURL.AppendFormat("&showPriceBreakdown=0");

                sbPrebookURL.Append("&blockSuperDeal=0");

                // send the request to SunHotels
                var webRequest = new Request
                {
                    EndPoint = sbPrebookURL.ToString(),
                    Method = eRequestMethod.GET,
                    Source = ThirdParties.SUNHOTELS,
                    ContentType = ContentTypes.Text_xml,
                    LogFileName = "Prebook",
                    CreateLog = true,
                    TimeoutInSeconds = 100
                };
                await webRequest.Send(_httpClient, _logger);

                prebookResponseXml = webRequest.ResponseXML;

                // strip out anything we don't need
                prebookResponseXml = _serializer.CleanXmlNamespaces(prebookResponseXml);

                if (prebookResponseXml.SelectSingleNode("preBookResult/Error") is not null)
                {
                    propertyDetails.Warnings.AddNew("Prebook Failed", prebookResponseXml.SafeNodeValue("preBookResult/Error/Message"));
                    return false;
                }

                // store the Errata if we have any
                var errata = prebookResponseXml.SelectNodes("preBookResult/Notes/Note");
                foreach (XmlNode erratum in errata)
                {
                    propertyDetails.Errata.AddNew("Important Information", erratum.SelectSingleNode("text").InnerText);
                }

                // recheck the price in case it has changed
                // ** needs to be changed if we implement multi-rooms in the future**
                decimal price = 0m;
                var cancellations = new Cancellations();
                price = prebookResponseXml.SafeNodeValue("preBookResult/Price").ToSafeMoney();
                propertyDetails.TPRef1 = prebookResponseXml.SafeNodeValue("preBookResult/PreBookCode");

                // override the cancellations
                int cancellationPolicyCount = 0;

                foreach (XmlNode cancellationPolicy in prebookResponseXml.SelectNodes("preBookResult/CancellationPolicies/CancellationPolicy"))
                {
                    cancellationPolicyCount += 1;

                    var hours = new TimeSpan(cancellationPolicy.SelectSingleNode("deadline").InnerText.ToSafeInt(), 0, 0);
                    DateTime startDate;

                    // for 100% cancel;lations we don't get an hours before
                    // so force the start date to be from now
                    if (hours.TotalHours == 0d)
                    {
                        startDate = DateTime.Now.Date;
                    }
                    else
                    {
                        startDate = propertyDetails.ArrivalDate.Subtract(hours);
                    }

                    decimal cancellationCost = (price * cancellationPolicy.SelectSingleNode("percentage").InnerText.ToSafeDecimal() / 100m).ToSafeDecimal();

                    // take the end date of the next cancellation policy otherwise set it a long way off
                    DateTime endDate;
                    if (prebookResponseXml.SelectSingleNode(string.Format("preBookResult/CancellationPolicies/CancellationPolicy[{0}]", cancellationPolicyCount + 1)) is not null)
                    {
                        // the hours of the end dates need to be rounded to the nearest 24 hours to stop them overlapping with the start of the next cancellation policy and making
                        // the charges add together
                        var endHours = new TimeSpan(RoundHoursUpToTheNearest24Hours(prebookResponseXml.SelectSingleNode(string.Format("preBookResult/CancellationPolicies/CancellationPolicy[{0}]/deadline", cancellationPolicyCount + 1)).InnerText.ToSafeInt()), 0, 0);

                        endDate = propertyDetails.ArrivalDate.Subtract(endHours);
                        endDate = endDate.AddDays(-1);
                    }
                    else
                    {
                        endDate = new DateTime(2099, 1, 1);
                    }

                    cancellations.AddNew(startDate, endDate, cancellationCost);
                }

                propertyDetails.Cancellations = cancellations;

                if (price > 0m && price != propertyDetails.LocalCost)
                {
                    propertyDetails.Rooms[0].GrossCost = price;
                    propertyDetails.Rooms[0].LocalCost = price;
                    propertyDetails.GrossCost = price;
                    propertyDetails.LocalCost = price;
                    propertyDetails.AddLog(ThirdParties.SUNHOTELS, "Third Party / Prebook Price Changed");
                }

                return true;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("SunHotels Exception", ex.ToString());
                return false;
            }
            finally
            {
                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(sbPrebookURL.ToString()))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels PreBook Request", sbPrebookURL.ToString());
                }

                if (!string.IsNullOrEmpty(prebookResponseXml.InnerXml))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Prebook Response", prebookResponseXml);
                }
            }
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            // **NOTE - THIS ONLY WORKS FOR SINGLE ROOM BOOKINGS**
            string requestURL = "";
            var response = new XmlDocument();
            string bookingReference = "";

            try
            {
                // split out the room reference and the meal type
                string roomId = propertyDetails.Rooms[0].ThirdPartyReference.Split('_')[0];

                // Grab the BookingReference If we have it
                string iVectorReference = "";
                if (string.IsNullOrEmpty(propertyDetails.BookingReference))
                {
                    iVectorReference = _settings.SupplierReference(propertyDetails);
                }
                else
                {
                    iVectorReference = propertyDetails.BookingReference;
                }

                // build the book request url
                var sbBookingRequestURL = new StringBuilder();

                sbBookingRequestURL.Append(_settings.BookingURL(propertyDetails));
                sbBookingRequestURL.AppendFormat("userName={0}", _settings.User(propertyDetails));
                sbBookingRequestURL.AppendFormat("&password={0}", _settings.Password(propertyDetails));
                sbBookingRequestURL.AppendFormat("&currency={0}", _settings.Currency(propertyDetails));
                sbBookingRequestURL.AppendFormat("&language={0}", _settings.LanguageCode(propertyDetails));
                sbBookingRequestURL.AppendFormat("&email={0}", _settings.ContactEmail(propertyDetails));
                sbBookingRequestURL.AppendFormat("&checkInDate={0}", GetSunHotelsDate(propertyDetails.ArrivalDate));
                sbBookingRequestURL.AppendFormat("&checkOutDate={0}", GetSunHotelsDate(propertyDetails.DepartureDate));
                sbBookingRequestURL.AppendFormat("&roomId={0}", roomId);
                sbBookingRequestURL.AppendFormat("&rooms={0}", propertyDetails.Rooms.Count);
                sbBookingRequestURL.AppendFormat("&adults={0}", propertyDetails.Adults);
                sbBookingRequestURL.AppendFormat("&children={0}", propertyDetails.Children);
                sbBookingRequestURL.AppendFormat("&infant={0}", IsInfantIncluded(propertyDetails.Rooms[0]));
                sbBookingRequestURL.AppendFormat("&yourRef={0}", iVectorReference);

                if (propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any())
                {
                    sbBookingRequestURL.AppendFormat("&specialrequest={0}", string.Join("\n", propertyDetails.Rooms.Select(x => x.SpecialRequest)));
                }
                else
                {
                    sbBookingRequestURL.Append("&specialrequest=");
                }

                sbBookingRequestURL.AppendFormat("&mealId={0}", propertyDetails.Rooms[0].ThirdPartyReference.Split('_')[1]);

                int adults = 1;
                int children = 1;

                // add the adults that in the booking
                foreach (var passenger in propertyDetails.Rooms[0].Passengers)
                {
                    if (passenger.PassengerType == PassengerType.Adult)
                    {
                        sbBookingRequestURL.AppendFormat("&adultGuest{0}FirstName={1}", adults, passenger.FirstName);
                        sbBookingRequestURL.AppendFormat("&adultGuest{0}LastName={1}", adults, passenger.LastName);
                        adults += 1;
                    }
                }

                // add empty elements for all the other adults up to 9
                for (int i = 1; i <= 9; i++)
                {
                    if (i >= adults)
                    {
                        sbBookingRequestURL.AppendFormat("&adultGuest{0}FirstName=", i);
                        sbBookingRequestURL.AppendFormat("&adultGuest{0}LastName=", i);
                    }
                }

                // add the children
                foreach (var passenger in propertyDetails.Rooms[0].Passengers)
                {
                    if (passenger.PassengerType == PassengerType.Child && passenger.Age <= 17)
                    {
                        sbBookingRequestURL.AppendFormat("&childrenGuest{0}FirstName={1}", children, passenger.FirstName);
                        sbBookingRequestURL.AppendFormat("&childrenGuest{0}LastName={1}", children, passenger.LastName);
                        sbBookingRequestURL.AppendFormat("&childrenGuestAge{0}={1}", children, passenger.Age);
                        children += 1;
                    }
                }

                // add empty elements for all the other adults up to 9
                for (int i = 1; i <= 9; i++)
                {
                    if (i >= children)
                    {
                        sbBookingRequestURL.AppendFormat("&childrenGuest{0}FirstName=", i);
                        sbBookingRequestURL.AppendFormat("&childrenGuest{0}LastName=", i);
                        sbBookingRequestURL.AppendFormat("&childrenGuestAge{0}=", i);
                    }
                }

                sbBookingRequestURL.AppendFormat("&paymentMethodId={0}", propertyDetails.Rooms[0].ThirdPartyReference.Split('_')[3]);
                sbBookingRequestURL.AppendFormat(GetCardDetails(propertyDetails.Rooms[0].ThirdPartyReference.Split('_')[3], propertyDetails));
                sbBookingRequestURL.Append("&customerEmail=");
                sbBookingRequestURL.Append("&invoiceRef=");
                sbBookingRequestURL.Append("&commissionAmountInHotelCurrency=");
                sbBookingRequestURL.AppendFormat("&customerCountry={0}", _settings.CustomerCountryCode(propertyDetails));
                sbBookingRequestURL.Append("&B2C=");

                sbBookingRequestURL.AppendFormat("&PreBookCode={0}", propertyDetails.TPRef1);

                requestURL = sbBookingRequestURL.ToString();

                var webRequest = new Request
                {
                    Source = ThirdParties.SUNHOTELS,
                    EndPoint = requestURL,
                    Method = eRequestMethod.GET,
                    ContentType = ContentTypes.Text_xml,
                    LogFileName = "Prebook",
                    CreateLog = true,
                };
                await webRequest.Send(_httpClient, _logger);

                response = _serializer.CleanXmlNamespaces(webRequest.ResponseXML);

                if (response.SelectSingleNode("bookResult/booking/Error") is null)
                {
                    // grab the booking reference
                    bookingReference = response.SelectSingleNode("bookResult/booking/bookingnumber").InnerText;
                }
                else
                {
                    bookingReference = "failed";
                }
            }
            catch (Exception ex)
            {
                bookingReference = "failed";
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
            }
            finally
            {
                // store the request and response xml on the property booking
                if (!string.IsNullOrEmpty(requestURL))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Book Request", requestURL);
                }

                if (!string.IsNullOrEmpty(response.InnerXml))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Book Response", response);
                }
            }

            return bookingReference;
        }

        #endregion

        #region Cancellations

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();

            var sbCancellationRequestURL = new StringBuilder();
            var cancellationResponseXML = new XmlDocument();

            try
            {
                // build the cancellation url
                sbCancellationRequestURL.Append(_settings.CancellationURL(propertyDetails));
                sbCancellationRequestURL.AppendFormat("userName={0}", _settings.User(propertyDetails));
                sbCancellationRequestURL.AppendFormat("&password={0}", _settings.Password(propertyDetails));
                sbCancellationRequestURL.AppendFormat("&bookingID={0}", propertyDetails.SourceReference.ToString());
                sbCancellationRequestURL.AppendFormat("&language={0}", _settings.LanguageCode(propertyDetails));

                // Send the request
                var webRequest = new Request
                {
                    EndPoint = sbCancellationRequestURL.ToString(),
                    Method = eRequestMethod.GET,
                    Source = ThirdParties.SUNHOTELS,
                    ContentType = ContentTypes.Text_xml,
                    LogFileName = "Cancel",
                    CreateLog = true,
                    TimeoutInSeconds = 100
                };
                await webRequest.Send(_httpClient, _logger);

                cancellationResponseXML = webRequest.ResponseXML;

                // tidy up the response
                cancellationResponseXML = _serializer.CleanXmlNamespaces(cancellationResponseXML);

                // Check for success
                var resultCodeNode = cancellationResponseXML.SelectSingleNode("result/Code");
                if (resultCodeNode is not null && resultCodeNode.InnerText.ToSafeInt() != -1)
                {
                    thirdPartyCancellationResponse.TPCancellationReference = DateTime.Now.ToString("dd MMM/HH:mm");
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
                if (!string.IsNullOrEmpty(sbCancellationRequestURL.ToString()))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Cancellation Request", sbCancellationRequestURL.ToString());
                }

                if (!string.IsNullOrEmpty(cancellationResponseXML.InnerXml))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.SUNHOTELS, "SunHotels Cancellation Response", cancellationResponseXML);
                }
            }

            return thirdPartyCancellationResponse;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
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

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails oPropertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        #endregion

        public void EndSession(PropertyDetails propertyDetails)
        {

        }

        #region Helpers

        public static string GetSunHotelsDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static int IsInfantIncluded(SearchDetails searchDetails)
        {
            int infantIncluded = 0;

            if (searchDetails.TotalInfants > 0)
            {
                infantIncluded = 1;
            }

            return infantIncluded;
        }

        public static int IsInfantIncluded(ThirdParty.Models.Property.Booking.RoomDetails roomDetails)
        {
            int infantIncluded = 0;

            if (roomDetails.Infants > 0)
            {
                infantIncluded = 1;
            }

            return infantIncluded;
        }

        public static string GetAdultsFromSearchDetails(SearchDetails searchDetails)
        {
            int adultCount = 0;

            foreach (var room in searchDetails.RoomDetails)
            {
                adultCount += room.Adults;

                foreach (int age in room.ChildAges)
                {
                    if (age > 17)
                    {
                        adultCount += 1;
                    }
                }
            }

            return adultCount.ToString();
        }

        public static string GetChildrenFromSearchDetails(SearchDetails searchDetails)
        {
            int childCount = 0;

            foreach (var room in searchDetails.RoomDetails)
            {
                foreach (int age in room.ChildAges)
                {
                    if (age <= 17)
                    {
                        childCount += 1;
                    }
                }
            }

            return childCount.ToString();
        }

        public static string GetChildrenAges(SearchDetails SearchDetails)
        {
            var sb = new StringBuilder();

            foreach (var room in SearchDetails.RoomDetails)
            {
                sb.Append(room.ChildAgeCSV);
            }

            return string.Join(",", sb.ToString());
        }

        public static int RoundHoursUpToTheNearest24Hours(int hours)
        {
            int i = Math.Round((hours / 24d).ToSafeDecimal(), 0, MidpointRounding.AwayFromZero).ToSafeInt();

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