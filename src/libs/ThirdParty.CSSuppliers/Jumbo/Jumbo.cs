namespace ThirdParty.CSSuppliers.Jumbo
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
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class Jumbo : IThirdParty, ISingleSource
    {
        #region Constructor

        private readonly IJumboSettings _settings;

        private readonly ISerializer _serializer;

        private readonly HttpClient _httpClient;

        private readonly ILogger<Jumbo> _logger;

        public Jumbo(IJumboSettings settings, ISerializer serializer, HttpClient HttpClient, ILogger<Jumbo> Logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(HttpClient, nameof(HttpClient));
            _logger = Ensure.IsNotNull(Logger, nameof(Logger));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.JUMBO;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
            => _settings.AllowCancellations(searchDetails);

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
            => _settings.OffsetCancellationDays(searchDetails, false);

        public bool RequiresVCard(VirtualCardInfo info, string source) => false;

        #endregion

        #region Prebook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            var webRequest = new Request();
            bool success = false;

            try
            {
                string request = BuildPreBookRequest(propertyDetails);

                // get the response
                webRequest = new Request()
                {
                    EndPoint = _settings.BookingURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.JUMBO,
                    ContentType = ContentTypes.Text_xml,
                    LogFileName = "PreBook",
                    CreateLog = true,
                };
                webRequest.SetRequest(request);
                await webRequest.Send(_httpClient, _logger);

                var response = _serializer.CleanXmlNamespaces(webRequest.ResponseXML);

                if (response.SelectSingleNode("Envelope/Body/Fault") is not null)
                {
                    throw new Exception(response.SafeNodeValue("Envelope/Body/Fault/faultstring"));
                }

                // store the Errata if we have any
                var errata = response.SelectNodes("Envelope/Body/valuateExtendsV13Response/result/remarks[type = 'ERRATA']");
                foreach (XmlNode erratum in errata)
                {
                    propertyDetails.Errata.AddNew(
                        "Important Information",
                        erratum.SelectSingleNode("text").InnerText);
                }

                // get the costs from the response and match up with the room bookings
                decimal cost = response.SafeNodeValue("Envelope/Body/valuateExtendsV13Response/result/amount/value").ToSafeMoney();

                if (cost == 0m)
                {
                    return false;
                }

                if (propertyDetails.LocalCost.ToSafeMoney() != cost)
                {
                    propertyDetails.LocalCost = cost;
                    propertyDetails.Rooms.First().LocalCost = cost;
                    propertyDetails.Warnings.AddNew("Third Party / Prebook Price Changed", "Price Changed");
                }

                // get the cancellation costs
                GetCancellations(ref propertyDetails, response);

                success = true;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                success = false;
            }
            finally
            {
                propertyDetails.AddLog("Prebook", webRequest);
            }

            return success;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var webRequest = new Request();
            string reference = "";

            if (await CheckAvailabilityAsync(propertyDetails))
            {
                try
                {
                    string sRequest = BuildBookRequest(propertyDetails);

                    // send the request
                    webRequest = new Request()
                    {
                        EndPoint = _settings.BookingURL(propertyDetails),
                        Source = ThirdParties.JUMBO,
                        SOAP = true,
                        LogFileName = "Book",
                        CreateLog = true,
                    };
                    webRequest.SetRequest(sRequest);
                    await webRequest.Send(_httpClient, _logger);

                    var response = _serializer.CleanXmlNamespaces(webRequest.ResponseXML);

                    // jumbo throws an internal error if anything is wrong with the request so webrequest returns a blank string
                    if (string.IsNullOrEmpty(response.InnerText))
                    {
                        reference = "failed";
                    }

                    if (response.SelectSingleNode("Envelope/Body/confirmExtendsV13Response/result/basket/basketId") is not null)
                    {
                        reference = response.SelectSingleNode("Envelope/Body/confirmExtendsV13Response/result/basket/basketId").InnerText;
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
            }
            else
            {
                reference = "failed";
                propertyDetails.Warnings.AddNew("Availability Issue", "Selected room is no longer available at the selected price");
            }

            return reference;
        }

        public async Task<bool> CheckAvailabilityAsync(PropertyDetails propertyDetails)
        {
            var webRequest = new Request();
            bool available = false;

            try
            {
                string request = BuildPreBookRequest(propertyDetails);

                // get the response
                webRequest = new Request()
                {
                    EndPoint = _settings.BookingURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.JUMBO,
                    ContentType = ContentTypes.Text_xml,
                    LogFileName = "BookAvailability",
                    CreateLog = true,
                };
                webRequest.SetRequest(request);
                await webRequest.Send(_httpClient, _logger);

                var response = _serializer.CleanXmlNamespaces(webRequest.ResponseXML);

                if (response.SelectSingleNode("Envelope/Body/Fault") is not null)
                {
                    throw new Exception(response.SafeNodeValue("Envelope/Body/Fault/faultstring"));
                }

                // get the costs from the response and match up with the room bookings
                decimal cost = response.SafeNodeValue("Envelope/Body/valuateExtendsV13Response/result/amount/value").ToSafeMoney();

                if (propertyDetails.LocalCost.ToSafeMoney() == cost)
                {
                    available = true;
                }
                else
                {
                    throw new Exception("Change in room price");
                }
            }
            catch (Exception ex)
            {
                available = false;
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Book Availability", webRequest);
            }

            return available;
        }

        #endregion

        #region Cancellations

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var requestXml = new XmlDocument();
            var responseXml = new XmlDocument();
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            var webRequest = new Request();

            try
            {
                // build up the cancellation request
                var sbCancellationRequest = new StringBuilder();

                sbCancellationRequest.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sbCancellationRequest.Append("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"");
                sbCancellationRequest.Append(" xmlns:tns=\"http://xtravelsystem.com/v1_0rc1/basket/types\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\">");
                sbCancellationRequest.Append("<soap:Body>");
                sbCancellationRequest.Append("<tns:cancel>");
                sbCancellationRequest.Append("<CancelRQ_1>");
                sbCancellationRequest.AppendFormat("<agencyCode>{0}</agencyCode>", GetCredentials(propertyDetails, propertyDetails.NationalityCode, "AgencyCode", _settings));
                sbCancellationRequest.AppendFormat("<brandCode>{0}</brandCode>", GetCredentials(propertyDetails, propertyDetails.NationalityCode, "BrandCode", _settings));
                sbCancellationRequest.AppendFormat("<pointOfSaleId>{0}</pointOfSaleId>", GetCredentials(propertyDetails, propertyDetails.NationalityCode, "POS", _settings));
                sbCancellationRequest.AppendFormat("<basketId>{0}</basketId>", propertyDetails.SourceReference);
                sbCancellationRequest.Append("<comment/>");
                sbCancellationRequest.Append("<language>en</language>");
                sbCancellationRequest.Append("<userId/>");
                sbCancellationRequest.Append("</CancelRQ_1>");
                sbCancellationRequest.Append("</tns:cancel>");
                sbCancellationRequest.Append("</soap:Body>");

                sbCancellationRequest.Append("</soap:Envelope>");

                requestXml.LoadXml(sbCancellationRequest.ToString());

                // get the response
                webRequest = new Request()
                {
                    EndPoint = _settings.CancellationURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.JUMBO,
                    ContentType = ContentTypes.Text_xml,
                    LogFileName = "Cancellation",
                    CreateLog = true,
                };
                webRequest.SetRequest(sbCancellationRequest.ToString());
                await webRequest.Send(_httpClient, _logger);

                // send the request
                responseXml = _serializer.CleanXmlNamespaces(webRequest.ResponseXML);

                if (responseXml.SelectSingleNode("Envelope/Body/Error") is null && responseXml.SelectSingleNode("Envelope/Body/cancelResponse/result/status").InnerText == "CANCELLED")
                {
                    // populate the cancellation response
                    thirdPartyCancellationResponse.CostRecievedFromThirdParty = true;
                    thirdPartyCancellationResponse.Success = true;
                    thirdPartyCancellationResponse.TPCancellationReference = responseXml.SelectSingleNode("Envelope/Body/cancelResponse/result/basketId").InnerText;
                    thirdPartyCancellationResponse.Amount = responseXml.SelectSingleNode("Envelope/Body/cancelResponse/result/total/value").InnerText.ToSafeMoney();
                    thirdPartyCancellationResponse.CurrencyCode = responseXml.SelectSingleNode("Envelope/Body/cancelResponse/result/total/currencyCode").InnerText;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
                thirdPartyCancellationResponse.Success = false;
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

        #endregion

        #region Booking Search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
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

        public void EndSession(PropertyDetails propertyDetails)
        {

        }

        #region Helpers

        #region TPReference Helper

        private class TPReference
        {
            public string RoomTypeCode;
            public string MealBasisCode;

            public TPReference(string tpReference)
            {
                var parts = tpReference.Split('|');
                RoomTypeCode = parts[0];
                MealBasisCode = parts[1];
            }
        }

        #endregion

        #region GetCancellationCosts

        public void GetCancellations(ref PropertyDetails propertyDetails, XmlDocument preBookResponse)
        {
            var cancellations = new Cancellations();

            try
            {
                // get the cancellation costs
                foreach (XmlNode roomNode in preBookResponse.SelectNodes("Envelope/Body/valuateExtendsV13Response/result/occupations"))
                {
                    var tempCancellations = new Cancellations();
                    decimal roomRate = roomNode.SelectSingleNode("amount/value").InnerText.ToSafeMoney();

                    foreach (XmlNode cancellationNode in roomNode.SelectNodes("cancellationComments[type='Cancellation term']"))
                    {
                        int cancellationStart = cancellationNode.SelectSingleNode("text").InnerText.Split('-')[0].ToSafeInt();
                        int cancellationPercentage = cancellationNode.SelectSingleNode("text").InnerText.Split('-')[1].Replace("%", "").ToSafeInt();

                        var cancellation = new Cancellation
                        {
                            Amount = roomRate * cancellationPercentage / 100m,
                            StartDate = propertyDetails.ArrivalDate.AddDays(-cancellationStart),
                            EndDate = propertyDetails.ArrivalDate
                        };

                        tempCancellations.Add(cancellation);
                    }

                    tempCancellations.Sort((c1, c2) => c1.StartDate.CompareTo(c2.StartDate));

                    // get the end date for each of the cancellations
                    foreach (var cancellation in tempCancellations)
                    {
                        if (tempCancellations.IndexOf(cancellation) == tempCancellations.Count - 1)
                        {
                            cancellations.AddNew(cancellation.StartDate, cancellation.EndDate, cancellation.Amount);
                        }
                        else
                        {
                            cancellations.AddNew(cancellation.StartDate, tempCancellations[tempCancellations.IndexOf(cancellation) + 1].StartDate.AddDays(-1), cancellation.Amount);
                        }
                    }
                }

                cancellations.Solidify(SolidifyType.Sum);
            }
            catch
            {
                // no need to do anything here - we'll just return the empty class if it fails
            }

            propertyDetails.Cancellations = cancellations;
        }

        #endregion

        #endregion

        #region Request Builders

        public string BuildPreBookRequest(PropertyDetails propertyDetails)
        {
            var sbPreBookRequest = new StringBuilder();

            // header
            sbPreBookRequest.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:typ=\"http://xtravelsystem.com/v1_0rc1/hotel/types\">");
            sbPreBookRequest.Append("<soapenv:Header/>");
            sbPreBookRequest.Append("<soapenv:Body>");
            sbPreBookRequest.Append("<typ:valuateExtendsV13>");

            // body
            sbPreBookRequest.Append("<ValuationRQ_1>");
            sbPreBookRequest.AppendFormat("<agencyCode>{0}</agencyCode>", GetCredentials(propertyDetails, propertyDetails.NationalityCode, "AgencyCode", _settings));
            sbPreBookRequest.AppendFormat("<brandCode>{0}</brandCode>", GetCredentials(propertyDetails, propertyDetails.NationalityCode, "BrandCode", _settings));
            sbPreBookRequest.AppendFormat("<pointOfSaleId>{0}</pointOfSaleId>", GetCredentials(propertyDetails, propertyDetails.NationalityCode, "POS", _settings));
            sbPreBookRequest.AppendFormat("<checkin>{0}</checkin>", propertyDetails.ArrivalDate.ToString("yyyy-MM-ddThh:mm:ss"));
            sbPreBookRequest.AppendFormat("<checkout>{0}</checkout>", propertyDetails.ArrivalDate.AddDays(propertyDetails.Duration).ToString("yyyy-MM-ddThh:mm:ss"));
            sbPreBookRequest.AppendFormat("<establishmentId>{0}</establishmentId>", propertyDetails.TPKey);
            sbPreBookRequest.AppendFormat("<language>{0}</language>", "en");

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                var baseHelper = new TPReference(roomDetails.ThirdPartyReference);

                sbPreBookRequest.Append("<occupancies>");
                sbPreBookRequest.AppendFormat("<adults>{0}</adults>", roomDetails.Adults);
                sbPreBookRequest.AppendFormat("<boardTypeCode>{0}</boardTypeCode>", baseHelper.MealBasisCode);
                sbPreBookRequest.AppendFormat("<children>{0}</children>", roomDetails.Children + roomDetails.Infants);

                foreach (var passenger in roomDetails.Passengers)
                {
                    if (passenger.PassengerType == PassengerType.Child || passenger.PassengerType == PassengerType.Infant)
                    {
                        sbPreBookRequest.AppendFormat("<childrenAges>{0}</childrenAges>", passenger.Age);
                    }
                }

                sbPreBookRequest.AppendFormat("<numberOfRooms>{0}</numberOfRooms>", "1");
                sbPreBookRequest.AppendFormat("<roomTypeCode>{0}</roomTypeCode>", baseHelper.RoomTypeCode);
                sbPreBookRequest.Append("</occupancies>");
            }

            sbPreBookRequest.Append("<onlyOnline>true</onlyOnline>");
            sbPreBookRequest.Append("</ValuationRQ_1>");

            // footer
            sbPreBookRequest.Append("</typ:valuateExtendsV13>");
            sbPreBookRequest.Append("</soapenv:Body>");
            sbPreBookRequest.Append("</soapenv:Envelope>");

            return sbPreBookRequest.ToString();
        }

        public string BuildBookRequest(PropertyDetails propertyDetails)
        {
            var sbBookRequest = new StringBuilder();

            // header
            sbBookRequest.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:typ=\"http://xtravelsystem.com/v1_0rc1/hotel/types\">");
            sbBookRequest.Append("<soapenv:Header/>");
            sbBookRequest.Append("<soapenv:Body>");
            sbBookRequest.Append("<typ:confirmExtendsV13>");

            // body
            sbBookRequest.Append("<ConfirmRQ_1>");
            sbBookRequest.AppendFormat("<agencyCode>{0}</agencyCode>", GetCredentials(propertyDetails, propertyDetails.NationalityCode, "AgencyCode", _settings));
            sbBookRequest.AppendFormat("<brandCode>{0}</brandCode>", GetCredentials(propertyDetails, propertyDetails.NationalityCode, "BrandCode", _settings));
            sbBookRequest.AppendFormat("<pointOfSaleId>{0}</pointOfSaleId>", GetCredentials(propertyDetails, propertyDetails.NationalityCode, "POS", _settings));
            sbBookRequest.AppendFormat("<checkin>{0}</checkin>", propertyDetails.ArrivalDate.ToString("yyyy-MM-ddThh:mm:ss"));
            sbBookRequest.AppendFormat("<checkout>{0}</checkout>", propertyDetails.ArrivalDate.AddDays(propertyDetails.Duration).ToString("yyyy-MM-ddThh:mm:ss"));
            sbBookRequest.AppendFormat("<establishmentId>{0}</establishmentId>", propertyDetails.TPKey);
            sbBookRequest.AppendFormat("<language>{0}</language>", "en");

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                var baseHelper = new TPReference(roomDetails.ThirdPartyReference);

                sbBookRequest.Append("<occupancies>");
                sbBookRequest.AppendFormat("<adults>{0}</adults>", roomDetails.Adults);
                sbBookRequest.AppendFormat("<boardTypeCode>{0}</boardTypeCode>", baseHelper.MealBasisCode);
                sbBookRequest.AppendFormat("<children>{0}</children>", roomDetails.Children + roomDetails.Infants);

                foreach (var passenger in roomDetails.Passengers)
                {
                    if (passenger.PassengerType == PassengerType.Child || passenger.PassengerType == PassengerType.Infant)
                    {
                        sbBookRequest.AppendFormat("<childrenAges>{0}</childrenAges>", passenger.Age);
                    }
                }

                sbBookRequest.AppendFormat("<numberOfRooms>{0}</numberOfRooms>", "1");
                sbBookRequest.AppendFormat("<roomTypeCode>{0}</roomTypeCode>", baseHelper.RoomTypeCode);
                sbBookRequest.Append("</occupancies>");
            }

            sbBookRequest.Append("<onlyOnline>true</onlyOnline>");
            sbBookRequest.Append("<basketId/>");
            sbBookRequest.Append("<closeBasket>true</closeBasket>");

            if (propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any())
            {
                string bookingComment = propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any() ?
                         string.Join("\n", propertyDetails.Rooms.Select(x => x.SpecialRequest)) :
                         "";

                sbBookRequest.Append("<comments>");
                sbBookRequest.AppendFormat("<text>{0}</text>", bookingComment.Trim());
                sbBookRequest.Append("<type>Guest requests</type>");
                sbBookRequest.Append("</comments>");
            }

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                sbBookRequest.Append("<paxList>");

                foreach (var passenger in roomDetails.Passengers)
                {
                    sbBookRequest.AppendFormat("<paxNames>{0}</paxNames>", passenger.FirstName + " " + passenger.LastName);
                }

                var baseHelper = new TPReference(roomDetails.ThirdPartyReference);

                sbBookRequest.AppendFormat("<roomTypeCode>{0}</roomTypeCode>", baseHelper.RoomTypeCode);
                sbBookRequest.Append("</paxList>");
            }

            // reset this to the first person on the booking
            string titular = propertyDetails.Rooms[0].Passengers[0].FirstName + " " + propertyDetails.Rooms[0].Passengers[0].LastName;

            sbBookRequest.AppendFormat("<titular>{0}</titular>", titular);
            sbBookRequest.Append("<userId />");
            sbBookRequest.Append("</ConfirmRQ_1>");

            // footer
            sbBookRequest.Append("</typ:confirmExtendsV13>");
            sbBookRequest.Append("</soapenv:Body>");
            sbBookRequest.Append("</soapenv:Envelope>");

            return sbBookRequest.ToString();
        }

        #endregion

        #region Credentials

        public static string GetCredentials(IThirdPartyAttributeSearch searchDetails, string nationalityCode, string type, IJumboSettings settings)
        {
            // credentials
            string agencyCode = settings.AgencyID(searchDetails);
            string brandCode = settings.BrandCode(searchDetails);
            string pos = settings.POS(searchDetails);

            string nationalityBasedCredentials = settings.NationalityBasedCredentials(searchDetails);
            if (!string.IsNullOrEmpty(nationalityBasedCredentials) && nationalityBasedCredentials.Split('#').Count() > 0)
            {
                // find the credentials with the correct nationality
                foreach (string nationalityBasedCredential in nationalityBasedCredentials.Split('#'))
                {
                    if (nationalityBasedCredential.Split('|').Count() == 4)
                    {
                        string nationalities = nationalityBasedCredential.Split('|')[3];

                        if (nationalities.Split(',').Contains(nationalityCode.ToString()))
                        {
                            brandCode = nationalityBasedCredential.Split('|')[0];
                            pos = nationalityBasedCredential.Split('|')[1];
                            agencyCode = nationalityBasedCredential.Split('|')[2];

                            break;
                        }
                    }
                }
            }

            switch (type.ToLower() ?? "")
            {
                case "agencycode":
                    {
                        return agencyCode;
                    }
                case "brandcode":
                    {
                        return brandCode;
                    }
                case "pos":
                    {
                        return pos;
                    }
                default:
                    {
                        return "";
                    }
            }
        }

        #endregion
    }
}