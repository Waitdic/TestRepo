using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Intuitive;
using Intuitive.Helpers.Extensions;
using Intuitive.Helpers.Net;
using Intuitive.Helpers.Serialization;
using iVectorOne.Constants;
using iVectorOne.Interfaces;
using iVectorOne.Models;
using iVectorOne.Models.Property.Booking;
using iVectorOne.Suppliers.Italcamel.Models.Common;
using iVectorOne.Suppliers.Italcamel.Models.Envelope;
using iVectorOne.Suppliers.Italcamel.Models.Prebook;
using iVectorOne.Suppliers.Italcamel.Models.Search;
using Microsoft.Extensions.Logging;
using Request = Intuitive.Helpers.Net.Request;

namespace iVectorOne.Suppliers.Italcamel
{
    public class Italcamel : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IItalcamelSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Italcamel> _logger;

        public string Source => ThirdParties.ITALCAMEL;

        public bool SupportsRemarks => false;
        public bool SupportsBookingSearch => false;

        public Italcamel(IItalcamelSettings settings, ISerializer serializer, HttpClient httpClient, ILogger<Italcamel> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            var success = true;
            Request request = null;

            try
            {
                var preBookRequest = BuildRequest(propertyDetails, "ESTIMATE");
                var soapAction = _settings.URL(propertyDetails).Replace("test.", "") + "/BOOKINGESTIMATE";

                // send the request
                request = new Request
                {
                    EndPoint = _settings.URL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = Source,
                    SoapAction = soapAction,
                    SOAP = true,
                    ContentType = ContentTypes.Text_Xml_charset_utf_8,
                    LogFileName = "Prebook",
                    CreateLog = true,
                };
                request.SetRequest(preBookRequest);
                await request.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<BookingEstimateResponse>(request.ResponseXML).BookingEstimateResult;

                // check for error
                if (response.ErrorCode > 0 || response.Status != 0 || response.Booking.Status != 30 || response.Booking.Status != 20)
                {
                    success = false;
                }

                var rooms = response.Booking.Rooms;
                var baseCosts = response.Booking.Rooms.Select(x => x.Amount).ToList();
                var boardCosts = response.Booking.Rooms.Select(x => x.Board).Select(a => a.Amount).ToList();
                var services = response.Booking.Rooms.SelectMany(x => x.Services).ToList();
                var servicesCost = services.Any() 
                    ? services.Where(s => s.Optional).Select(s => s.Amount).ToList()
                    : new List<decimal>();

                for (var i = 0; i < propertyDetails.Rooms.Count; i++)
                {
                    var specialOffer = rooms[i].Services.Any() ? rooms[i].Services.FirstOrDefault()!.Amount : 0;
                    var discounts = rooms[i].Extra.Discount.Any() ? rooms[i].Extra.Discount.Select(x => x.Amount).Sum() : 0;
                    var supplements = rooms[i].Extra.Supplements.Any() ? rooms[i].Extra.Supplements.Select(x => x.Amount).Sum() : 0;

                    propertyDetails.Rooms[i].LocalCost =
                        baseCosts[i] + boardCosts[i] - specialOffer - discounts - supplements;

                    propertyDetails.Rooms[i].LocalCost += servicesCost.Count > i ? servicesCost[i] : 0;
                }

                // set cancellations on booking
                SetCancellations(propertyDetails);
            }
            catch (Exception ex)
            {
                success = false;
                propertyDetails.Warnings.AddNew("PreBook Exception", ex.ToString());
            }
            finally
            {
                if (request != null)
                {
                    propertyDetails.AddLog("PreBook", request);
                }
            }

            return success;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            Request request = null;
            var reference = string.Empty;

            try
            {
                var bookingRequest = BuildRequest(propertyDetails, "BOOKING");
                request = new Request
                {
                    EndPoint = _settings.URL(propertyDetails),
                    Source = Source,
                    SOAP = true,
                    SoapAction = _settings.URL(propertyDetails).Replace("test.", "") + "/BOOKINGESTIMATE",
                    ContentType = ContentTypes.Text_Xml_charset_utf_8,
                };
                request.SetRequest(bookingRequest);
                await request.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<BookingEstimateResponse>(request.ResponseXML).BookingEstimateResult;

                // check for error
                if (response.ErrorCode > 0 || response.Status != 0 || response.Booking.Status != 30)
                {
                    reference = "failed";
                }

                // store bookinguid - required for cancellation
                propertyDetails.SourceSecondaryReference = response.Booking.UID;
                reference = response.Booking.Number;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBook Exception", ex.ToString());
                reference = "failed";
            }
            finally
            {
                // store the request and response xml on the property booking
                if (request != null)
                {
                    propertyDetails.AddLog("Book", request);
                }
            }

            return reference;
        }

        #endregion

        #region Cancel

        public Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        #endregion

        #region Helpers

        private XmlDocument BuildRequest(PropertyDetails propertyDetails, string type)
        {
            var comment = propertyDetails.BookingComments.Count > 0
                ? propertyDetails.BookingComments[0].Text
                : string.Empty;

            var request = new Envelope<BookingEstimate>
            {
                Body =
                {
                    Content =
                    {
                        Username = _settings.Username(propertyDetails),
                        Password = _settings.Username(propertyDetails),
                        LanguageuId = _settings.Username(propertyDetails),
                        Request =
                        {
                            Type = type,
                            CheckIn = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                            CheckOut = propertyDetails.DepartureDate.ToString("yyyy-MM-dd"),
                            AccomodationuId = propertyDetails.TPKey,
                            Notes = comment,
                            Rooms = propertyDetails.Rooms.Select(r => new PrebookRoom
                            {
                                UID = r.ThirdPartyReference.Split('|')[0],
                                IsWin = false,
                                IsDus = false,
                                Passengers = r.Passengers.Select(p => new Models.Common.Passenger
                                {
                                    Name = string.IsNullOrEmpty(p.FirstName) ? "TBA" : p.FirstName,
                                    Surname = string.IsNullOrEmpty(p.LastName) ? "TBA" : p.LastName,
                                    Birthdate = (p.PassengerType switch
                                    {
                                        PassengerType.Adult => DateTime.Now.AddYears(-40),
                                        PassengerType.Child => DateTime.Now.AddYears(p.Age),
                                        _ => DateTime.Now.AddYears(-30)
                                    }).ToString("yyyy-MM-dd")
                                }).ToArray(),
                                Board =
                                {
                                    UID = r.ThirdPartyReference.Split('|')[1]
                                }
                            }).ToArray()
                        }
                    }
                }
            };

            return _serializer.Serialize(request);
        }

        private async void SetCancellations(PropertyDetails propertyDetails)
        {
            // get cancellation policies
            var cancellationsRequest = ItalcamelHelper.BuildSearchRequest(
                _settings,
                _serializer,
                propertyDetails,
                propertyDetails.TPKey,
                ItalcamelHelper.SearchType.Hotel);

            // send the request
            var soapAction = _settings.URL(propertyDetails).Replace("test.", "") + "/GETAVAILABILITYSPLITTED";
            var request = new Request
            {
                EndPoint = _settings.URL(propertyDetails),
                Method = RequestMethod.POST,
                Source = Source,
                SOAP = true,
                SoapAction = soapAction,
                ContentType = ContentTypes.Text_Xml_charset_utf_8,
                LogFileName = "Get Cancellation Costs",
                CreateLog = true
            };
            request.SetRequest(cancellationsRequest);
            await request.Send(_httpClient, _logger);

            var response = _serializer.DeSerialize<GetAvailabilitySplittedResponse>(request.ResponseXML).Response;

            // get cancellation policie(s)
            var cancellations = new Cancellations();
            foreach (var room in propertyDetails.Rooms)
            {
                var cancellationPolicies = response.Accommodations.SelectMany(x => x.CancellationPolicies).ToList();
                var cancellationNodes =
                    cancellationPolicies.Any(c => c.RoomUID == room.ThirdPartyReference.Split('|')[0])
                        ? cancellationPolicies.Where(c => c.RoomUID == room.ThirdPartyReference.Split('|')[0]).ToList()
                        : cancellationPolicies.Where(c => c.RoomUID == string.Empty).ToList();

                foreach (var cancellationPolicy in cancellationNodes)
                {
                    var date = ConvertItalcamelDate(cancellationPolicy.ReleaseDate).AddDays(1);
                    var amount = GetCancellationAmount(
                        cancellationPolicy.Type,
                        cancellationPolicy.Value,
                        room.LocalCost,
                        propertyDetails.Duration);

                    var lastDate = new DateTime(2099, 12, 31);
                    var description = cancellationPolicy.Description;

                    if (Regex.IsMatch(description, "^For cancellation from [0-9]+ to [0-9]+ days before"))
                    {
                        var startIndex = description.IndexOf("to ") + 3;
                        var daysBefore = description.Substring(startIndex, description.IndexOf(" days") - startIndex).ToSafeInt();
                        lastDate = propertyDetails.ArrivalDate.AddDays(-daysBefore);
                    }

                    cancellations.AddNew(date, lastDate, amount);
                }
            }

            if (cancellations.Count > 1)
            {
                cancellations.Sort((x, y) => DateTime.Compare(x.StartDate, y.StartDate));
                for (var i = 0; i < cancellations.Count - 1; i++)
                {
                    if (cancellations[i].EndDate == new DateTime(2099, 12, 31))
                    {
                        cancellations[i].EndDate = cancellations[i + 1].StartDate.AddDays(-1);
                    }
                }
            }

            cancellations.Solidify(SolidifyType.Sum);
            propertyDetails.Cancellations = cancellations;

            propertyDetails.AddLog("ItalCamel Cancellation Costs", request);
        }

        private DateTime ConvertItalcamelDate(string date)
        {
            return new DateTime(
                date.Substring(0, 4).ToSafeInt(),
                date.Substring(5, 2).ToSafeInt(),
                date.Substring(8, 2).ToSafeInt());
        }

        private decimal GetCancellationAmount(string type, decimal value, decimal bookingCost, int duration)
        {
            return type switch
            {
                "0" =>
                    // fixed value
                    value,
                "1" =>
                    // number of nights
                    bookingCost / duration * value,
                "2" =>
                    // percentage of total cost
                    bookingCost * value / 100,
                _ => throw new Exception("Cancellation type unknown")
            };
        }

        #endregion
    }
}
