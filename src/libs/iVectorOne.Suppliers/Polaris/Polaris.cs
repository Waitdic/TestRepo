namespace iVectorOne.Suppliers.Polaris
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using System.Threading.Tasks;
    using iVectorOne.Suppliers.Polaris.Models;
    using System.Text;
    using Newtonsoft.Json;

    public class Polaris : IThirdParty, ISingleSource
    {
        #region "Properties"
        public string Source => ThirdParties.POLARIS;

        private readonly IPolarisSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ISecretKeeper _secretKeeper;
        private readonly ILogger<Polaris> _logger;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        #endregion

        #region "Constructors"
        public Polaris(
            IPolarisSettings settings,
            HttpClient httpClient,
            ISecretKeeper secretKeeper,
            ILogger<Polaris> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region "PreBook"
        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            if (_settings.SplitMultiRoom(propertyDetails)) return await PreBookSplitAsync(propertyDetails);

            var preBookSuccess = false;
            var request = new Request();

            try
            {
                var prebookResponses = new List<PrebookResponse>();

                var tpRef = PolarisTpRef.Decrypt(_secretKeeper, propertyDetails.Rooms.First().ThirdPartyReference);

                var preBookRequest = await CreateRequestAsync(propertyDetails, _settings.PrebookURL(propertyDetails), 
                    new PrebookRequest 
                    { 
                        BookToken = tpRef.BookToken 
                    });

                var prebookResponse = JsonConvert.DeserializeObject<PrebookResponse>(preBookRequest.ResponseString);
                var roomRate = prebookResponse.Hotel.RoomRates.First();

                //check status
                if (!string.Equals(prebookResponse.Status, Constant.Status.Confirmed)) 
                {
                    throw new Exception($"PreBook status is {prebookResponse.Status}");
                }

                //verify price
                var isPriceChanged = false;
                var roomIdx = 0;
                foreach (var room in roomRate.Rooms.OrderBy(x => x.Index)) 
                {
                    if (!decimal.Equals(room.Pricing.Net.Price, propertyDetails.Rooms[roomIdx].LocalCost))
                    {
                        propertyDetails.Rooms[roomIdx].LocalCost = room.Pricing.Net.Price;
                        propertyDetails.Rooms[roomIdx].GrossCost = room.Pricing.Net.Price;
                    }
                    roomIdx++;
                }
                if (isPriceChanged) 
                {
                    propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);
                    propertyDetails.GrossCost = propertyDetails.Rooms.Sum(r => r.GrossCost);
                }

                //add cancellations with solidify
                var canx = new Cancellations();
                canx.AddRange(TransformCancellations(roomRate.CancellationPolicies));

                propertyDetails.Cancellations = canx;

                //add erratum
                foreach (var observation in roomRate.Observations) 
                {
                    propertyDetails.Errata.Add(new Erratum 
                    {
                        Title = "Important information",
                        Text = $"{observation.Type} {observation.Txt}"
                    });
                }

                preBookSuccess = true;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBook Failed", ex.Message);
                preBookSuccess = false;
            }
            finally 
            {
                propertyDetails.AddLog("Prebook", request);
            }

            return preBookSuccess;
        }



        public async Task<bool> PreBookSplitAsync(PropertyDetails propertyDetails) 
        {
            var preBookSuccess = false;
            var requests = new List<Request>();
            var prebookResponses = new List<PrebookResponse>();

            try
            {
                var isPriceChanged = false;
                var canx = new Cancellations();
                var observations = new List<Observation>();

                foreach (var room in propertyDetails.Rooms) 
                {
                    var polarisTpRef = PolarisTpRef.Decrypt(_secretKeeper, room.ThirdPartyReference);
                    var prebookContent = new PrebookRequest
                    {
                        BookToken = polarisTpRef.BookToken
                    };

                    var preBookRequest = await CreateRequestAsync(propertyDetails, _settings.PrebookURL(propertyDetails), prebookContent);
                    var prebookResponse = JsonConvert.DeserializeObject<PrebookResponse>(preBookRequest.ResponseString);
                    var roomRate = prebookResponse.Hotel.RoomRates.First();

                    if (!string.Equals(prebookResponse.Status, Constant.Status.Confirmed)) 
                    {
                        throw new Exception($"PreBook status is {prebookResponse.Status}");
                    }

                    room.ThirdPartyReference = new PolarisTpRef 
                    {
                        BookToken = roomRate.BookToken,
                        RoomIndex = polarisTpRef.RoomIndex
                    }.Encrypt(_secretKeeper);

                    var roomPrice = roomRate.Pricing.Net.Price;
                    if (!Equals(room.LocalCost, roomPrice)) 
                    {
                        isPriceChanged = true;
                        room.LocalCost = roomPrice;
                        room.GrossCost = roomPrice;
                    }

                    var cancellations = TransformCancellations(roomRate.CancellationPolicies);
                    canx.AddRange(cancellations);

                    observations.AddRange(roomRate.Observations);
                }
                if (isPriceChanged)
                {
                    propertyDetails.LocalCost = propertyDetails.Rooms.Sum(x => x.LocalCost);
                    propertyDetails.GrossCost = propertyDetails.Rooms.Sum(x => x.GrossCost);
                }

                propertyDetails.Cancellations = Cancellations.MergeMultipleCancellationPolicies(canx);

                //add erratum
                var information = observations.Select(x => $"{x.Type} {x.Txt}").Distinct();
                foreach (var observation in information)
                {
                    propertyDetails.Errata.Add(new Erratum
                    {
                        Title = "Important Information",
                        Text = observation
                    });
                }

                preBookSuccess = true;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBook Failed", ex.Message);
                preBookSuccess = false;
            }
            finally 
            {
                foreach (var request in requests)
                {
                    propertyDetails.AddLog("Prebook", request);
                }
            }

            return preBookSuccess;
        }

        #endregion

        #region "Book"
        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            if (_settings.SplitMultiRoom(propertyDetails)) return await BookSplitAsync(propertyDetails);            
            
            string reference;
            var bookRequest = new Request();

            try
            {
                var bookRequestContent = BuildBookRequest(propertyDetails.Rooms, propertyDetails.BookingReference, propertyDetails.ArrivalDate);
                bookRequest = await CreateRequestAsync(propertyDetails, _settings.BookingURL(propertyDetails), bookRequestContent);
                var bookResponse = JsonConvert.DeserializeObject<BookResponse>(bookRequest.ResponseString);
                if (!string.Equals(bookResponse.Status, Constant.Status.Confirmed)) 
                {
                    var errors = string.Join(";", bookResponse.Notes.Err.Select(err => $"desc={err.Desc} type={err.Type}"));
                    throw new Exception($"Response status is [{bookResponse.Status}], Errors: {errors}");
                }
                
                reference = bookResponse.BookingReference.BookingReferenceId;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBook Failed", ex.Message);
                reference = Constant.Failed;
            }
            finally 
            {
                propertyDetails.AddLog("Book", bookRequest);
            }

            return reference;
        }

        public BookRequest BuildBookRequest(List<RoomDetails> roomDetails, string bookingReference, DateTime arrivalDate)
        {
            var encryptedTpRef = roomDetails.First().ThirdPartyReference;
            var polarisTpRef = PolarisTpRef.Decrypt(_secretKeeper, encryptedTpRef);

            return new BookRequest
            {
                BookToken = polarisTpRef.BookToken,
                BookingReference =
                {
                    RequestReferenceId = bookingReference,
                },
                Remarks = string.Join(", ", roomDetails.Select((room, roomIdx) => 
                        string.IsNullOrEmpty(room.SpecialRequest)
                            ? string.Empty
                            : $"Room {roomIdx + 1}: {room.SpecialRequest}")),
                Travellers = roomDetails.SelectMany((room, roomIdx) => room.Passengers
                                            .Select(pax => new Traveller 
                                            { 
                                                Age = CalculateAge(pax.DateOfBirth, arrivalDate),
                                                Index = roomIdx + 1,
                                                Person = 
                                                {
                                                    LastName = pax.LastName,
                                                    Name = pax.FirstName
                                                }
                                            })).ToList()
            };
        }

        public async Task<string> BookSplitAsync(PropertyDetails propertyDetails) 
        {
            var requests = new List<Request>();
            var references = new List<string>();

            try
            {
                foreach (var room in propertyDetails.Rooms) 
                {
                    var bookingReference = $"{propertyDetails.BookingReference}{room.PropertyRoomBookingID}"; 
                    var bookRequestContent = BuildBookRequest(new List<RoomDetails> { room }, bookingReference, propertyDetails.ArrivalDate);
                    var bookRequest = await CreateRequestAsync(propertyDetails, _settings.BookingURL(propertyDetails), bookRequestContent);
                    var bookResponse = JsonConvert.DeserializeObject<BookResponse>(bookRequest.ResponseString);

                    if (!string.Equals(bookResponse.Status, Constant.Status.Confirmed))
                    {
                        var errors = string.Join(";", bookResponse.Notes.Err.Select(err => $"desc={err.Desc} type={err.Type}"));
                        throw new Exception($"Response status is [{bookResponse.Status}], Errors: {errors}");
                    }
                    var reference = bookResponse.BookingReference.BookingReferenceId;
                    requests.Add(bookRequest);
                    references.Add(reference);
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBook Failed", ex.Message);
                references.Add(Constant.Failed);
            }
            finally 
            {
                foreach (var request in requests)
                {
                    propertyDetails.AddLog("Prebook", request);
                }
            }

            return string.Join(Constant.ReferenceSeparator, references);
        }

        #endregion

        #region "Cancellations"
        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var success = true;
            var cancelReferences = new List<string>();
            var currencyCode = "";
            decimal amountSum = 0.00M;

            var references = propertyDetails.SourceReference.Split(Constant.ReferenceSeparator)
                .Where(x => !string.Equals(x, Constant.Failed)).ToList();

            try
            {
                foreach (var reference in references) 
                {
                    try
                    {
                        var cancelContent = new CancelRequest
                        {
                            BookingReference =
                            {
                                BookingReferenceId = reference
                            }
                        };
                        request = await CreateRequestAsync(propertyDetails, _settings.CancellationURL(propertyDetails), cancelContent);
                        var cancelResponse = JsonConvert.DeserializeObject<CancelResponse>(request.ResponseString);

                        if (!new List<string> { 
                                Constant.Status.Cancel, 
                                Constant.Status.AlreadyBookCancel 
                            }.Contains(cancelResponse.Status))
                        {
                            throw new Exception($"Cancellation failed, response status is {cancelResponse.Status}");
                        }

                        if (cancelResponse.Hotel != null) 
                        {
                            var roomRate = cancelResponse.Hotel.RoomRates.First();
                            var cancellations = TransformCancellations(roomRate.CancellationPolicies);
                            var now = Now();
                            var activeCancelPeriod = cancellations.FirstOrDefault(cx => cx.StartDate <= now && cx.EndDate >= now);
                            if (activeCancelPeriod != null)
                            {
                                amountSum += activeCancelPeriod.Amount;
                                currencyCode = roomRate.Pricing.Currency;
                            }
                        }
                        cancelReferences.Add(reference);
                    }
                    catch (Exception ex) 
                    {
                        cancelReferences.Add(Constant.Failed);
                        success = false;
                        propertyDetails.Warnings.AddNew("Cancellation Exception", ex.Message);
                    }
                }
            }
            finally
            {
                propertyDetails.AddLog("Cancellation", request);
            }
            return new ThirdPartyCancellationResponse
            {
                Success = success,
                TPCancellationReference = string.Join(Constant.ReferenceSeparator, cancelReferences),
                CurrencyCode = currencyCode,
                Amount = amountSum
            };
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }
        #endregion

        #region "Other Funcitons"
        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }
        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }
        #endregion

        #region "Helpers"

        public async Task<Request> CreateRequestAsync<T>(PropertyDetails propertyDetail, string endpoint, T requestContent)
        {
            var requestString = JsonConvert.SerializeObject(requestContent);
            var request = new Request()
            {
                EndPoint = endpoint,
                Method = RequestMethod.POST,
                Source = Source,
                ContentType = ContentTypes.Application_json,
                UseGZip = _settings.UseGZip(propertyDetail)
            };
            request.SetRequest(requestString);
            request.Headers.AddNew("Authorization", "Basic " + BasicToken(propertyDetail, _settings));

            await request.Send(_httpClient, _logger);

            return request;
        }

        public static List<Cancellation> TransformCancellations(List<CancellationPolicy> cancellations)
        {
            var canxs = cancellations.Select(cancellation => new Cancellation
            {
                StartDate = cancellation.From.ToSafeDate(),
                EndDate = cancellation.To.ToSafeDate(),
                Amount = (cancellation.Pricing.Net != null)
                            ? cancellation.Pricing.Net.Price
                            : cancellation.Pricing.Sell.Price
            }).ToList();
            return canxs;
        }

        internal static string BasicToken(IThirdPartyAttributeSearch tpAttributeSearch, IPolarisSettings settings)
        {
            return Convert.ToBase64String(new UTF8Encoding().GetBytes(
                $"{settings.User(tpAttributeSearch)}:{settings.Password(tpAttributeSearch)}"));
        }

        internal static int CalculateAge(DateTime dateOfBirth, DateTime arrivalDate) 
        {
            var age = arrivalDate.Year - dateOfBirth.Year
                    + (dateOfBirth.Month <= arrivalDate.Month
                        && dateOfBirth.Day <= arrivalDate.Day 
                        ? 0 : -1);

            // should match with search request pax ages            
            if (age < 2) age = Constant.DefaultInfantAge;
            if (age >= 18) age = Constant.DefaultAdultAge;

            return age;
        }

        internal static DateTime Now() 
        {
            var now = DateTime.UtcNow; 
            return now;
        }
        #endregion
    }
}
