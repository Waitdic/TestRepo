namespace iVectorOne.Suppliers.GoGlobal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Suppliers.GoGlobal.Models;
    using Microsoft.Extensions.Logging;

    public class GoGlobal : IThirdParty, ISingleSource
    {
        #region "Properties"

        private readonly IGoGlobalSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoGlobal> _logger;
 
        public string Source { get; set; } = ThirdParties.GOGLOBAL;

        public bool SupportsBookingSearch => true;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public bool SupportsRemarks => true;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        #endregion

        #region "Constructors"

        public GoGlobal(IGoGlobalSettings settings, ISerializer serializer, HttpClient httpClient, ILogger<GoGlobal> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region "Prebook"

        public async Task<bool> PreBookAsync(PropertyDetails oPropertyDetails)
        {
            bool bReturn = true;
            try
            {
                string[] searchReferences = oPropertyDetails.Rooms[0].ThirdPartyReference.Split('|');
                var valuationRequest = new PreBookRq
                {
                    HotelSearchCode = searchReferences[0],
                    ArrivalDate = oPropertyDetails.ArrivalDate.ToString(Constant.DataFormat)
                };
                string sRoot = await SendRequestAsync(oPropertyDetails, valuationRequest, Constant.PreBookRequestCode,
                    Constant.PreBookRequestNumber, "Pre-Book");

                oPropertyDetails.LocalCost = oPropertyDetails.Rooms.Sum(r => r.LocalCost);

                var prebookRs = _serializer.DeSerialize<Root<PreBookRs>>(sRoot).Main;

                if (!string.IsNullOrEmpty(prebookRs.Error))
                {
                    throw new Exception($"{prebookRs.Error}, {prebookRs.DebugError}");
                }

                if (!string.IsNullOrEmpty(prebookRs.Remarks))
                {
                    oPropertyDetails.Errata.AddNew("Important Information", prebookRs.Remarks);

                    var cancellations = GetCancellationPeriods(prebookRs.Remarks, oPropertyDetails.ArrivalDate)
                            .Select(c => new Cancellation
                            {
                                StartDate = c.StartDate,
                                EndDate = c.EndDate.AddDays(-1),
                                Amount = oPropertyDetails.LocalCost * c.FeePercent / 100
                            });

                    oPropertyDetails.Cancellations.AddRange(cancellations);
                }
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("PreBook Exception", ex.Message);
                bReturn = false;
            }
            return bReturn;
        }

        #endregion

        #region "Book"

        public async Task<string> BookAsync(PropertyDetails oPropertyDetails)
        {
            string sReference;
            try
            {
                var bookReservationRequest = BookReservationRequest(oPropertyDetails);
                string xBook = await SendRequestAsync(oPropertyDetails, bookReservationRequest, Constant.BookRequestCode, Constant.BookRequestNumber, "Book");
                var bookInsertRs = _serializer.DeSerialize<Root<BookInsertRs>>(xBook).Main;

                if (!string.IsNullOrEmpty(bookInsertRs.Error))
                {
                    throw new Exception($"{bookInsertRs.Error}, {bookInsertRs.DebugError}");
                }

                sReference = bookInsertRs.GoBookingCode;

                var statusRequest = new BookStatusRq
                {
                    GoBookingCode = sReference
                };

                string xStatus = await SendRequestAsync(oPropertyDetails, statusRequest, Constant.StatusConfirmationRequestCode,
                                                            Constant.StatusConfirmationRequestNumber, "GoBookingCode");
                var bookStatusRs = _serializer.DeSerialize<Root<BookStatusRs>>(xStatus).Main;

                if (!string.IsNullOrEmpty(bookStatusRs.Error))
                {
                    throw new Exception($"{bookStatusRs.Error}, {bookStatusRs.DebugError}");
                }

                var bookingConfirmedStatuses = new List<string> { "C", "VCH" };
                if (!bookingConfirmedStatuses.Contains(bookStatusRs.GoBookingCode.Status))
                {
                    sReference = "Failed";
                }
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("Book Exception", ex.Message);
                sReference = "Failed";
            }
            return sReference;
        }

        #endregion

        #region "Cancel"

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails oPropertyDetails)
        {
            var oThirdPartyCancellationFeeResult = new ThirdPartyCancellationFeeResult
            {
                Success = false
            };

            try
            {
                var oBookingSearchRq = new BookingSearchRq
                {
                    GoBookingCode = oPropertyDetails.SourceReference
                };

                string sBookingSeaqrchRs = await SendRequestAsync(oPropertyDetails, oBookingSearchRq, Constant.BookingSearchRequestCode,
                                                        Constant.BookingSearchRequestNumber, "Cancellation Cost");

                var xBookingSeaqrchRs = _serializer.DeSerialize<Root<BookingSearchRs>>(sBookingSeaqrchRs).Main;

                if (!string.IsNullOrEmpty(xBookingSeaqrchRs.Error))
                {
                    throw new Exception($"{xBookingSeaqrchRs.Error}, {xBookingSeaqrchRs.DebugError}");
                }

                var arrivalDate = ParseDate(xBookingSeaqrchRs.ArrivalDate, "yyyy-MM-dd");
                decimal totalPrice = xBookingSeaqrchRs.TotalPrice.ToSafeDecimal();
                string currencyCode = xBookingSeaqrchRs.Currency;
                var cDeadline = ParseDate(xBookingSeaqrchRs.CancellationDeadline, "yyyy-MM-dd");
                var now = DateTime.Now;
                var cancelDeadlineIncluded = new DateTime(cDeadline.Year, cDeadline.Month, cDeadline.Day).AddDays(1);

                if (now < cancelDeadlineIncluded) return new ThirdPartyCancellationFeeResult
                {
                    Amount = 0.00M,
                    CurrencyCode = currencyCode,
                    Success = true
                };

                var cancellationPeriods = GetCancellationPeriods(xBookingSeaqrchRs.Remark, arrivalDate);
                var currentPeriod = cancellationPeriods.FirstOrDefault(cp => cp.StartDate < now && cp.EndDate > now);

                if (currentPeriod != null)
                {
                    oThirdPartyCancellationFeeResult.Success = true;
                    oThirdPartyCancellationFeeResult.CurrencyCode = currencyCode;
                    oThirdPartyCancellationFeeResult.Amount = currentPeriod.FeePercent * totalPrice;
                }
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("Cancellation Cost Exception", ex.Message);
                oThirdPartyCancellationFeeResult.Success = false;
            }

            return oThirdPartyCancellationFeeResult;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails oPropertyDetails)
        {
            var oTPCancellationResponse = new ThirdPartyCancellationResponse
            {
                Success = false,
                TPCancellationReference = "Failed"
            };

            try
            {
                var cancelRequst = new CancelRq
                {
                    GoBookingCode = oPropertyDetails.SourceReference
                };

                string xCancelRs = await SendRequestAsync(oPropertyDetails, cancelRequst, Constant.CancelRequestCode, Constant.CancelRequestNumber, "Cancel");
                var oCancelRs = _serializer.DeSerialize<Root<CancelRs>>(xCancelRs).Main;

                if (!string.IsNullOrEmpty(oCancelRs.Error))
                {
                    throw new Exception($"{oCancelRs.Error}, {oCancelRs.DebugError}");
                }

                var cancelConfirmedStatuses = new List<string> { "X", "XF" };
                if (cancelConfirmedStatuses.Contains(oCancelRs.BookingStatus))
                {
                    oTPCancellationResponse.TPCancellationReference = oCancelRs.GoBookingCode;
                    oTPCancellationResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("Cancellation Exception", ex.Message);
                oTPCancellationResponse.TPCancellationReference = "Failed";
                oTPCancellationResponse.Success = false;
            }

            return oTPCancellationResponse;
        }

        #endregion

        #region "BookingSearch"

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new();
        }

        #endregion

        #region "Other Methods"

        public string CreateReconciliationReference(string inputReference)
        {
            return "";
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        #endregion

        #region "Booking Status Update"
        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }

        #endregion

        #region "Helpers"

        internal async Task<string> SendRequestAsync<T>(PropertyDetails oPropertyDetails, T request, string operation,
                                        int operationCode, string logFileName) where T : Main, new()
        {
            var oRoot = new Root<T>
            {
                Header =
                {
                         Agency = _settings.Agency(oPropertyDetails),
                         User = _settings.User(oPropertyDetails),
                         Password = _settings.Password(oPropertyDetails),
                         Operation = operation,
                         OperationType = Constant.OperationTypeRequest,
                },
                Main = request
            };

            string sRoot = $"{_serializer.Serialize(oRoot).OuterXml}";
            var envelope = Envelope.CreateRequest(operationCode, sRoot, _serializer);

            //'Make the web request
            var oRequest = new Request
            {
                EndPoint = _settings.GenericURL(oPropertyDetails),
                Method = RequestMethod.POST,
                Source = Source,
                LogFileName = logFileName,
                CreateLog = true,
                UseGZip = true
            };
            oRequest.SetRequest(envelope);
            await oRequest.Send(_httpClient, _logger);

            oPropertyDetails.AddLog(logFileName, oRequest);

            string responseRoot = Envelope.GetResponse(oRequest.ResponseXML, _serializer);

            return responseRoot;
        }

        internal static BookInsertRq BookReservationRequest(PropertyDetails oPropertyDetails)
        {
            var insertRequest = new BookInsertRq
            {
                Version = Constant.Version_2_3,
                AgentReference = oPropertyDetails.BookingReference,
                HotelSearchCode = oPropertyDetails.Rooms[0].ThirdPartyReference.Split('|')[0],
                ArrivalDate = oPropertyDetails.ArrivalDate.ToString(Constant.DataFormat),
                Nights = oPropertyDetails.Duration,
                NoAlternativeHotel = "1",
                Remark = oPropertyDetails.Rooms[0].SpecialRequest,
                Leader =
                    {
                        LeaderPersonID = 1
                    },
                Rooms = oPropertyDetails.Rooms.Select(oRoom =>
                {
                    var adults = oRoom.Passengers.Where(p => p.PassengerType == PassengerType.Adult)
                        .Select((oGuest, idx) => new PersonName
                        {
                            FirstName = oGuest.FirstName,
                            LastName = oGuest.LastName,
                            Title = $"{oGuest.Title.ToUpper()}.",
                            PersonID = $"{idx + 1}"
                        }).ToList();

                    var childs = oRoom.Passengers.Where(p => p.PassengerType is PassengerType.Child or PassengerType.Infant)
                        .Select((oGuest, idx) => new ExtraBed
                        {
                            FirstName = oGuest.FirstName,
                            LastName = oGuest.LastName,
                            PersonID = $"{idx + 1 + adults.Count}",
                            ChildAge = oGuest.PassengerType == PassengerType.Child
                                       ? oGuest.Age
                                       : 1
                        }).ToList();

                    return new RoomType
                    {
                        Adults = adults.Count,
                        Room =
                        {
                            RoomID = oRoom.PropertyRoomBookingID,
                            Guests = adults,
                            ExtraBeds = childs
                        }
                    };
                }).ToList()
            };
            return insertRequest;
        }

        internal static List<CancelPeriod> GetCancellationPeriods(string remark, DateTime arrivalDate)
        {
            var cancellationPeriods = new List<CancelPeriod>();
            var cancellationRemark = ParseRemark(remark);
            if (cancellationRemark.Any())
            {
                int cxlsCount = cancellationRemark.Count;
                var cxls = cancellationRemark.Select(c => new { StartDate = c.Key, FeePercent = c.Value }).OrderBy(c => c.StartDate).ToList();

                cancellationPeriods = Enumerable.Range(0, cxlsCount).Select(i => new CancelPeriod
                {
                    StartDate = cxls[i].StartDate,
                    EndDate = (i + 1 < cxlsCount)
                            ? cxls[i + 1].StartDate
                            : arrivalDate,
                    FeePercent = cxls[i].FeePercent
                }).ToList();
            }

            return cancellationPeriods;
        }

        private static Dictionary<DateTime, decimal> ParseRemark(string remark)
        {
            var cancellations = new Dictionary<DateTime, decimal>();
            string template = @".*CXL charges apply as follows:(.*)";
            var matchRemark = Regex.Match(remark, template);
            bool hasCancellations = matchRemark.Success;
            if (hasCancellations)
            {
                remark = matchRemark.Groups[1].Value;
                string cancellationTemplate = @"STARTING (\d{2}/\d{2}/\d{4}) CXL-PENALTY FEE IS (\d+.?\d*)% OF BOOKING PRICE.";

                while (hasCancellations)
                {
                    var matchCancel = Regex.Match(remark, cancellationTemplate);
                    if (matchCancel.Success)
                    {
                        remark = remark.Replace(matchCancel.Groups[0].Value, "");
                        var startDate = ParseDate(matchCancel.Groups[1].Value, "dd/MM/yyyy");
                        decimal percent = matchCancel.Groups[2].Value.ToSafeDecimal();
                        cancellations.Add(startDate, percent);
                    }
                    else
                    {
                        hasCancellations = false;
                    }
                };
            }

            return cancellations;
        }

        public static DateTime ParseDate(string sDate, string format)
        {
            DateTime.TryParseExact(sDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dDate);
            return dDate;
        }

        #endregion
    }
}
