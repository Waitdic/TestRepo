namespace iVectorOne.Suppliers.Italcamel
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.Italcamel.Models.Cancel;
    using iVectorOne.Suppliers.Italcamel.Models.Common;
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;
    using iVectorOne.Suppliers.Italcamel.Models.Prebook;
    using Microsoft.Extensions.Logging;
    using Request = Intuitive.Helpers.Net.Request;

    public class Italcamel : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IItalcamelSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Italcamel> _logger;
        private readonly ItalcamelHelper _helper = new();

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
            Request? request = null;

            try
            {
                var preBookRequest = BuildRequest(propertyDetails, "ESTIMATE");
                var url = _settings.GenericURL(propertyDetails);
                var soapAction = url
                    .Replace("https", "http")
                    .Replace("test", "") + "/PACKAGEESTIMATE";

                // send the request
                request = _helper.CreateWebRequest(url, soapAction, true, "Prebook");
                request.SetRequest(preBookRequest);
                await request.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<Envelope<PackageEstimateResponse>>(
                    request.ResponseXML).Body.Content.PackageEstimateResult;

                // check for error
                if (response.ErrorCode.ToSafeInt() > 0 || response.Status != 0 || (response.Package.Booking.Status != 30 && response.Package.Booking.Status != 20))
                {
                    success = false;
                }

                propertyDetails.TPRef1 = response.Package.ReferenceNumber;

                for (var i = 0; i < propertyDetails.Rooms.Count; i++)
                {
                    propertyDetails.Rooms[i].LocalCost = response.Package.Booking.InternalBooking != null 
                        ? response.Package.Booking.InternalBooking.Rooms[i].TotalAmount.ToSafeDecimal() 
                        : response.Package.Booking.Rooms[i].TotalAmount.ToSafeDecimal();
                }

                // set cancellations on booking
                string remarks;
                CancellationPolicy[] cancellationPolicies;
                if (response.Package.Booking.InternalBooking != null)
                {
                    remarks = response.Package.Booking.InternalBooking.Remarks;
                    cancellationPolicies = response.Package.Booking.InternalBooking.Rooms.SelectMany(x => x.CancellationPolicies).ToArray();
                }
                else
                {
                    remarks = response.Package.Booking.Remarks;
                    cancellationPolicies = response.Package.Booking.Rooms.SelectMany(x => x.CancellationPolicies).ToArray();
                }

                propertyDetails.Errata.Add(new Erratum("REMARKS", remarks));
                SetCancellations(propertyDetails, cancellationPolicies);
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
            Request? request = null;
            var reference = string.Empty;

            try
            {
                var bookingRequest = BuildRequest(propertyDetails, "BOOKING");
                var url = _settings.GenericURL(propertyDetails);
                var soapAction = url
                    .Replace("https", "http")
                    .Replace("test", "") + "/PACKAGEESTIMATE";

                request = _helper.CreateWebRequest(url, soapAction);
                request.SetRequest(bookingRequest);
                await request.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<Envelope<PackageEstimateResponse>>(request.ResponseXML).Body.Content.PackageEstimateResult;

                // check for error
                if (response.ErrorCode.ToSafeInt() > 0 || response.Status != 0 || (response.Package.Booking.Status != 30 && response.Package.Booking.Status != 20))
                {
                    reference = "failed";
                }
                else
                {
                    // store bookinguid - required for cancellation
                    propertyDetails.SourceSecondaryReference = $"{response.Package.UID}|{response.Package.Booking.UID}";
                    reference = response.Package.Number;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
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

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            Request? request = null;

            try
            {
                // create xml for cancellation request
                var cancelRequest = BuildCancelRequest(propertyDetails);

                // send the request
                var url = _settings.GenericURL(propertyDetails);
                var soapAction = url
                    .Replace("https", "http")
                    .Replace("test", "") + "/PACKAGEDELETE";

                request = _helper.CreateWebRequest(url, soapAction, true, "Cancel");
                request.SetRequest(cancelRequest);
                await request.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<Envelope<PackageDeleteResponse>>(request.ResponseXML).Body.Content.Response;

                // check response
                if (string.IsNullOrEmpty(response.ErrorCode) && response.Status.ToSafeInt() == 0 && response.ErrorCode.ToSafeInt() == 0)
                {
                    var amount  = await GetBookingCharge(propertyDetails, propertyDetails.SourceSecondaryReference.Split('|')[1]);

                    thirdPartyCancellationResponse.TPCancellationReference = DateTime.Now.ToString("yyyyMMddhhmm");
                    thirdPartyCancellationResponse.Success = true;
                    thirdPartyCancellationResponse.Amount = amount;
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
                if (request != null)
                {
                    propertyDetails.AddLog("Cancellation", request);
                }
            }

            return thirdPartyCancellationResponse;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        #endregion

        #region Helpers

        private string BuildRequest(PropertyDetails propertyDetails, string type)
        {
            var comment = propertyDetails.BookingComments.Count > 0
                ? propertyDetails.BookingComments[0].Text
                : string.Empty;

            var request = new Envelope<PackageEstimate>
            {
                Body =
                {
                    Content =
                    {
                        Username = _settings.Login(propertyDetails),
                        Password = _settings.Password(propertyDetails),
                        LanguageuId = _settings.LanguageID(propertyDetails),
                        Request =
                        {
                            Type = type,
                            Notes = comment,
                            ReferenceNumber = !string.IsNullOrEmpty(propertyDetails.TPRef1) 
                                ? propertyDetails.TPRef1 
                                : DateTime.Now.ToString("yyyyMMddhhmmss"),
                            Booking =
                            {
                                CheckIn = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                                CheckOut = propertyDetails.DepartureDate.ToString("yyyy-MM-dd"),
                                AccomodationUID = propertyDetails.TPKey,
                                RequestPrice = propertyDetails.LocalCost,
                                Type = type,
                                DeltaPrice = _settings.DeltaPrice(propertyDetails),
                                Rooms = propertyDetails.Rooms.Select(r => new PrebookRoom
                                {
                                    MasterUID = r.ThirdPartyReference.Split('|')[0],
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
                }
            };

            return _serializer.Serialize(request).OuterXml;
        }

        private string BuildCancelRequest(PropertyDetails propertyDetails)
        {
            var request = new Envelope<PackageDelete>
            {
                Body =
                {
                    Content =
                    {
                        Username = _settings.Login(propertyDetails),
                        Password = _settings.Password(propertyDetails),
                        LanguageUID = _settings.LanguageID(propertyDetails),
                        PackageUID = propertyDetails.SourceSecondaryReference.Split('|')[0]
                    }
                }
            };

            return _serializer.Serialize(request).OuterXml;
        }

        private void SetCancellations(PropertyDetails propertyDetails, CancellationPolicy[] cancellationPolicies)
        {
            

            // get cancellation policie(s)
            var cancellations = new Cancellations();
            foreach (var room in propertyDetails.Rooms)
            {
                var cancellationNodes =
                    cancellationPolicies.Any(c => c.RoomUID == room.ThirdPartyReference.Split('|')[0])
                        ? cancellationPolicies.Where(c => c.RoomUID == room.ThirdPartyReference.Split('|')[0]).ToList()
                        : cancellationPolicies.Where(c => c.RoomUID == string.Empty).ToList();

                foreach (var cancellationPolicy in cancellationNodes)
                {
                    var date = propertyDetails.ArrivalDate.AddDays(-cancellationPolicy.DayFrom);
                    var amount = GetCancellationAmount(
                        cancellationPolicy.Type,
                        cancellationPolicy.Value,
                        room.LocalCost,
                        propertyDetails.Duration);

                    var lastDate = propertyDetails.ArrivalDate.AddDays(-cancellationPolicy.DayTo);
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

        private async Task<decimal> GetBookingCharge(PropertyDetails propertyDetails, string uid)
        {
            Request? request = null;
            decimal amount = 0;
            try
            {
                var bookingChargeRequest =
                    _helper.BuildBookingChargeRequest(_settings, _serializer, propertyDetails, uid);
                var soapAction = _settings.GenericURL(propertyDetails)
                    .Replace("https", "http")
                    .Replace("test", "") + "/GETBOOKINGCHARGE";

                request = _helper.CreateWebRequest(_settings.GenericURL(propertyDetails), soapAction);
                request.SetRequest(bookingChargeRequest);
                await request.Send(_httpClient, _logger);

                var bookingCharges = _serializer.DeSerialize<Envelope<GetBookingChargeResponse>>(request.ResponseXML)
                    .Body.Content.GetBookingChargeResult;

                if (bookingCharges.Status != null && bookingCharges.Status.ToSafeInt() != 0)
                {
                    propertyDetails.Warnings.AddNew("GetBookingCharge failed", bookingCharges.ErrorMessage);
                }
                else
                {
                    var charge = bookingCharges.BookingCharges
                        .Where(x => x.ChargeDate <= DateTime.Now).ToList();

                    amount = charge.Any() 
                        ? charge.OrderByDescending(x => x.ChargeDate).FirstOrDefault()!.ChargeAmount 
                        : 0;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("GetBookingCharge Exception", ex.Message);
            }
            finally
            {
                if (request != null)
                {
                    propertyDetails.AddLog("GetBookingCharge", request);
                }
            }

            return amount;
        }

        #endregion
    }
}
