﻿namespace iVectorOne.CSSuppliers.YouTravel
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
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;

    public class YouTravel : IThirdParty, ISingleSource
    {
        #region Constructor

        public YouTravel(IYouTravelSettings settings, HttpClient httpClient, ISerializer serializer, ILogger<YouTravel> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region Properties

        private readonly IYouTravelSettings _settings;

        private readonly HttpClient _httpClient;

        private readonly ISerializer _serializer;

        private readonly ILogger<YouTravel> _logger;

        public bool SupportsRemarks => true;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
            => _settings.AllowCancellations(searchDetails, false);

        public bool SupportsBookingSearch => false;

        public string Source => ThirdParties.YOUTRAVEL;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
            => _settings.OffsetCancellationDays(searchDetails, false);

        public bool RequiresVCard(VirtualCardInfo info, string source) => false;

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            var response = new XmlDocument();
            bool success = true;
            var webRequest = new Request();

            try
            {
                // Get Errata details
                var sb = new StringBuilder();

                sb.AppendFormat("{0}{1}", _settings.PrebookURL(propertyDetails), "?");
                sb.AppendFormat("&LangID={0}", _settings.LanguageCode(propertyDetails));
                sb.AppendFormat("&HID={0}", propertyDetails.TPKey);
                sb.AppendFormat("&UserName={0}", _settings.User(propertyDetails));
                sb.AppendFormat("&Password={0}", _settings.Password(propertyDetails));

                webRequest.Source = ThirdParties.YOUTRAVEL;
                webRequest.CreateLog = true;
                webRequest.LogFileName = "Prebook";
                webRequest.EndPoint = sb.ToString();
                webRequest.Method = RequestMethod.GET;
                await webRequest.Send(_httpClient, _logger);

                response = _serializer.CleanXmlNamespaces(webRequest.ResponseXML);
                var errataNode = response.SelectSingleNode("/HtSearchRq/Hotel/Erratas");
                string errata = errataNode.InnerText;
                propertyDetails.Errata.AddNew("Important Information", errata);

                // Get cancellation policies
                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    var sbCancelPolicy = new StringBuilder();
                    var cancelPolicyResponseXml = new XmlDocument();
                    sbCancelPolicy.AppendFormat("{0}{1}", _settings.CancellationPolicyURL(propertyDetails), "?");
                    sbCancelPolicy.AppendFormat("token={0}", roomDetails.ThirdPartyReference.Split('_')[2]);

                    var cancellationPolicyWebRequest = new Request
                    {
                        Source = ThirdParties.YOUTRAVEL,
                        CreateLog = true,
                        LogFileName = "Cancellation Cost",
                        EndPoint = sbCancelPolicy.ToString(),
                        Method = RequestMethod.GET
                    };

                    try
                    {
                    await cancellationPolicyWebRequest.Send(_httpClient, _logger);
                    }
                    finally
                    {
                        propertyDetails.AddLog("CancellationPolicy", cancellationPolicyWebRequest);
                    }

                    cancelPolicyResponseXml = _serializer.CleanXmlNamespaces(cancellationPolicyWebRequest.ResponseXML);
                    
                    foreach (XmlNode cancellationNode in cancelPolicyResponseXml.SelectNodes("/HtSearchRq/Policies/Policy"))
                    {
                        var fromDate = cancellationNode.SelectSingleNode("FromDate").InnerText.ToSafeDate();
                        var toDate = new DateTime(2099, 1, 1);
                        decimal cancellationCost = cancellationNode.SelectSingleNode("Fees").InnerText.ToSafeDecimal();
                        propertyDetails.Cancellations.AddNew(fromDate, toDate, cancellationCost);
                    }
                }

                propertyDetails.Cancellations.Solidify(SolidifyType.Sum);
            }
            catch (Exception ex)
            {
                success = false;
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Prebook", webRequest);
            }

            propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);

            return success;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var response = new XmlDocument();
            string requestURL = "";
            string reference = "";
            var webRequest = new Request();

            try
            {
                // build url
                var sbURL = new StringBuilder();
                sbURL.AppendFormat("{0}{1}", _settings.BookingURL(propertyDetails), "?");
                sbURL.AppendFormat("&LangID={0}", _settings.LanguageCode(propertyDetails));
                sbURL.AppendFormat("&UserName={0}", _settings.User(propertyDetails));
                sbURL.AppendFormat("&Password={0}", _settings.Password(propertyDetails));
                sbURL.AppendFormat("&session_ID={0}", propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[0]);
                sbURL.AppendFormat("&Checkin_Date={0}", YouTravelSupport.FormatDate(propertyDetails.ArrivalDate));
                sbURL.AppendFormat("&Nights={0}", propertyDetails.Duration);
                sbURL.AppendFormat("&Rooms={0}", propertyDetails.Rooms.Count);

                sbURL.AppendFormat("&HID={0}", propertyDetails.TPKey);

                // adults and children
                int roomIndex = 0;
                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    roomIndex += 1;

                    sbURL.AppendFormat("&ADLTS_{0}={1}", roomIndex, roomDetails.Adults);

                    if (roomDetails.Children + roomDetails.Infants > 0)
                    {
                        sbURL.AppendFormat("&CHILD_{0}={1}", roomIndex, roomDetails.Children + roomDetails.Infants);

                        for (int i = 1; i <= roomDetails.Children + roomDetails.Infants; i++)
                        {
                            if (roomDetails.ChildAges.Count > i - 1)
                            {
                                sbURL.AppendFormat("&ChildAgeR{0}C{1}={2}", roomIndex, i, roomDetails.ChildAges[i - 1]);
                            }
                            else
                            {
                                sbURL.AppendFormat("&ChildAgeR{0}C{1}={2}", roomIndex, i, -1);
                            }
                        }
                    }
                    else
                    {
                        sbURL.AppendFormat("&CHILD_{0}=0", roomIndex);
                    }

                    if (roomIndex == 1)
                    {
                        sbURL.AppendFormat("&RID={0}", roomDetails.ThirdPartyReference.Split('|')[1]);
                    }
                    else
                    {
                        sbURL.AppendFormat("&RID_{0}={1}", roomIndex, roomDetails.ThirdPartyReference.Split('|')[1]);
                    }

                    sbURL.AppendFormat("&Room{0}_Rate={1}", roomIndex, roomDetails.GrossCost.ToSafeMoney());
                }

                sbURL.AppendFormat("&Customer_title={0}", propertyDetails.Rooms[0].Passengers[0].Title);
                sbURL.AppendFormat("&Customer_firstname={0}", propertyDetails.Rooms[0].Passengers[0].FirstName);
                sbURL.AppendFormat("&Customer_Lastname={0}", propertyDetails.Rooms[0].Passengers[0].LastName);

                if (propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any())
                {
                    var request = string.Join(",", propertyDetails.Rooms.Select(x => x.SpecialRequest)); 

                    // max 250 characters
                    sbURL.AppendFormat("&Requests={0}", request.Substring(0, Math.Min(request.Length, 250)));
                }

                requestURL = sbURL.ToString();

                webRequest = new Request
                {
                    Source = ThirdParties.YOUTRAVEL,
                    CreateLog = true,
                    LogFileName = "Book",
                    EndPoint = requestURL,
                    Method = RequestMethod.GET
                };
                await webRequest.Send(_httpClient, _logger);

                response = webRequest.ResponseXML;

                // return booking reference if it exists
                var node = response.SelectSingleNode("/HtSearchRq/Booking_ref");
                if (node is not null && !string.IsNullOrEmpty(response.SelectSingleNode("/HtSearchRq/Booking_ref").InnerText))
                {
                    reference = response.SelectSingleNode("/HtSearchRq/Booking_ref").InnerText;
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
            var sbURL = new StringBuilder();
            var response = new XmlDocument();
            var webRequest = new Request();

            try
            {
                // build url
                sbURL.AppendFormat("{0}{1}", _settings.CancellationURL(propertyDetails), "?");
                sbURL.AppendFormat("Booking_ref={0}", propertyDetails.SourceReference);
                sbURL.AppendFormat("&UserName={0}", _settings.User(propertyDetails));
                sbURL.AppendFormat("&Password={0}", _settings.Password(propertyDetails));

                // Send the request
                webRequest = new Request
                {
                    Source = ThirdParties.YOUTRAVEL,
                    CreateLog = true,
                    LogFileName = "Cancel",
                    EndPoint = sbURL.ToString(),
                    Method = RequestMethod.GET
                };
                await webRequest.Send(_httpClient, _logger);

                response = webRequest.ResponseXML;

                // check response
                if (!string.IsNullOrEmpty(response.InnerXml))
                {
                    if (response.SelectSingleNode("/HtSearchRq/Success").InnerText == "True")
                    {
                        thirdPartyCancellationResponse.TPCancellationReference = response.SelectSingleNode("/HtSearchRq/Booking_ref").InnerText;
                        thirdPartyCancellationResponse.Success = true;
                    }
                    else
                    {
                        thirdPartyCancellationResponse.Success = false;
                    }
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

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            var result = new ThirdPartyCancellationFeeResult();
            var webRequest = new Request();

            try
            {
                var sbURL = new StringBuilder();
                sbURL.AppendFormat("{0}{1}", _settings.CancellationFeeURL(propertyDetails), "?");
                sbURL.AppendFormat("Booking_ref={0}", propertyDetails.SourceReference);
                sbURL.AppendFormat("&UserName={0}", _settings.User(propertyDetails));
                sbURL.AppendFormat("&Password={0}", _settings.Password(propertyDetails));

                // Send the request
                webRequest = new Request
                {
                    Source = ThirdParties.YOUTRAVEL,
                    CreateLog = true,
                    LogFileName = "Cancellation Cost",
                    EndPoint = sbURL.ToString(),
                    Method = RequestMethod.GET
                };
                await webRequest.Send(_httpClient, _logger);

                var response = webRequest.ResponseXML;

                // return a fees result if found
                var node = response.SelectSingleNode("/HtSearchRq/Success");
                if (node is not null && response.SelectSingleNode("/HtSearchRq/Success").InnerText == "True")
                {
                    result.Success = true;
                    result.Amount = response.SelectSingleNode("/HtSearchRq/Fees").InnerText.ToSafeDecimal();
                    result.CurrencyCode = response.SelectSingleNode("/HtSearchRq/Currency").InnerText.ToSafeString();
                }
                else
                {
                    result.Success = false;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                propertyDetails.Warnings.AddNew("Cancellation Cost Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Cancellation Cost", webRequest);
            }

            return result;
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
    }
}