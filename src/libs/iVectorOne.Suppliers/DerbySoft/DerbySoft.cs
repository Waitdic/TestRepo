namespace iVectorOne.Suppliers.DerbySoft
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using iVectorOne;
    using iVectorOne.Suppliers.DerbySoft.DerbySoftBookingUsbV4.Models;
    using iVectorOne.Suppliers.DerbySoft.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using System.Threading.Tasks;
    using iVectorOne.Models.Property;

    public class DerbySoft : IThirdParty, IMultiSource
    {
        #region Properties

        private readonly IDerbySoftSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<DerbySoft> _logger;


        public List<string> Sources => DerbySoftSupport.DerbysoftSources;

        public bool SupportsRemarks => true;
        public bool SupportsBookingSearch => false;
        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source) => true;
        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source) => 0;
        public bool RequiresVCard(VirtualCardInfo info, string source) => false;

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails) => new ThirdPartyBookingSearchResults();
        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails) => new ThirdPartyBookingStatusUpdateResult();
        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        public string CreateReconciliationReference(string inputReference) 
            => throw new NotImplementedException();

        #endregion

        #region Constructor

        public DerbySoft(IDerbySoftSettings settings, HttpClient httpClient, ILogger<DerbySoft> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            bool preBookSuccess = true;

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                preBookSuccess = await CheckAvailabilityAsync(propertyDetails, roomDetails);

                if (!preBookSuccess)
                {
                    break;
                }

                preBookSuccess = await PreBookRoomAsync(propertyDetails, roomDetails, true) && preBookSuccess;
            }

            //combine cancellations for multiroom
            //using custom solidifiy 
            var cancellations = new Cancellations();
            var cancellationCalculator = new CancellationCalculator();
            cancellations.AddRange(propertyDetails.Cancellations);
            propertyDetails.Cancellations.Clear();
            propertyDetails.Cancellations.AddRange(cancellationCalculator.SolidifyCancellations(cancellations));
            propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);

            return preBookSuccess;
        }

        private async Task<bool> CheckAvailabilityAsync(PropertyDetails propertyDetails, RoomDetails roomDetails)
        {
            bool availabilitySuccess = true;

            try
            {
                PreBookHelper preBookHelper = PreBookHelper.DeserializePreBookHelper(roomDetails.ThirdPartyReference);

                var availabilityDeserialisedRequest = BuildAvailabilityRequest(propertyDetails, roomDetails, preBookHelper);

                var availabilityResponse =
                    await GetResponseAsync<DerbySoftBookingUsbV4AvailabilityRequest, DerbySoftBookingUsbV4AvailabilityResponse>(
                        propertyDetails,
                        availabilityDeserialisedRequest,
                        _settings.SearchURL(propertyDetails, propertyDetails.Source),
                        "Prebook - Availability");

                var roomRate = availabilityResponse?.RoomRates?.FirstOrDefault(r =>
                    !string.IsNullOrWhiteSpace(r.RoomId) && r.RoomId == roomDetails.RoomTypeCode &&
                    !string.IsNullOrWhiteSpace(r.MealPlan) && (r.MealPlan == roomDetails.MealBasisCode || r.MealPlan == "RO" && roomDetails.MealBasisCode == "NA") &&
                    !string.IsNullOrWhiteSpace(r.RateId) && r.RateId == preBookHelper.RoomRate.RateId &&
                    (!r.Inventory.HasValue || r.Inventory.Value > 0));

                availabilitySuccess = roomRate is object;

                if (!availabilitySuccess)
                {
                    propertyDetails.Warnings.AddNew("Prebook Error", "Room not found in availability search.", WarningType.ThirdPartyError);
                    return availabilitySuccess;
                }

                // Update Cancellations 
                if ((preBookHelper.Cancellations == null || preBookHelper.Cancellations.Count <= 0) && availabilityResponse.RoomRates != null && availabilityResponse.RoomRates.Count > 0)
                {
                    var cancellationCalculator = new CancellationCalculator();

                    foreach (var room in availabilityResponse.RoomRates)
                    {
                        var dailyRateRetriever = new DailyRateRetriever();
                        var dailyRates = dailyRateRetriever.GetDailyRates(
                                            room,
                                            roomDetails.TotalPassengers,
                                            propertyDetails.ArrivalDate,
                                            propertyDetails.DepartureDate,
                                            out var feeAmount);

                        // set to amount after tax if both of the prices where returned or after tax was returned else to before tax
                        var rates = dailyRates.Item2 != null && dailyRates.Item2.Count > 0 ? dailyRates.Item2 : dailyRates.Item1;

                        preBookHelper.Cancellations.AddRange(cancellationCalculator.GetCancellations(room.CancelPolicy, rates, propertyDetails.ArrivalDate));

                    }
                }

                //Update prebook helper in case of price changes as ShoppinEngine is thier cache search
                preBookHelper = new PreBookHelper(availabilityResponse.Header.Token, roomRate, preBookHelper.Cancellations);
                roomDetails.ThirdPartyReference = PreBookHelper.SerializePreBookHelper(preBookHelper);
            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("Prebook Availability Exception", exception.InnerException.ToString(), WarningType.Exception);
                availabilitySuccess = false;
            }

            return availabilitySuccess;
        }

        private async Task<bool> PreBookRoomAsync(PropertyDetails propertyDetails, RoomDetails roomDetails, bool addCancellations)
        {
            bool preBookSuccess;

            try
            {
                //Data stored from search
                PreBookHelper preBookHelper = PreBookHelper.DeserializePreBookHelper(roomDetails.ThirdPartyReference);

                var prebookDeserialisedRequest = BuildPreBookRequest(propertyDetails, preBookHelper, roomDetails);

                var prebookResponse =
                    await GetResponseAsync<DerbySoftBookingUsbV4PreBookRequest, DerbySoftBookingUsbV4PreBookResponse>(
                        propertyDetails,
                        prebookDeserialisedRequest,
                        _settings.PrebookURL(propertyDetails, propertyDetails.Source),
                        "Prebook");

                preBookSuccess = prebookResponse?.bookingToken != null && prebookResponse.bookingToken != "";

                if (preBookSuccess)
                {
                    //store data for book
                    preBookHelper.PreBookToken = prebookDeserialisedRequest.reservationIds.distributorResId;
                    preBookHelper.PreBookBookingToken = prebookResponse.bookingToken;
                    roomDetails.ThirdPartyReference = PreBookHelper.SerializePreBookHelper(preBookHelper);

                    var dailyRateRetriever = new DailyRateRetriever();
                    var dailyRates = dailyRateRetriever.GetDailyRates(
                        preBookHelper.RoomRate,
                        roomDetails.TotalPassengers,
                        propertyDetails.ArrivalDate,
                        propertyDetails.DepartureDate,
                        out var feeAmount);

                    // set to amount after tax if both of the prices where returned or after tax was returned else to before tax
                    var totalCost = dailyRates.Item2 != null && dailyRates.Item2.Count > 0 ? dailyRates.Item2.Sum() : dailyRates.Item1.Sum();

                    totalCost += feeAmount;

                    roomDetails.LocalCost = totalCost;
                    roomDetails.GrossCost = totalCost;

                    if (addCancellations) propertyDetails.Cancellations.AddRange(preBookHelper.Cancellations);
                }

            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", exception.InnerException.ToString(), WarningType.Exception);
                preBookSuccess = false;
            }

            return preBookSuccess;
        }
        private DerbySoftBookingUsbV4AvailabilityRequest BuildAvailabilityRequest(
            PropertyDetails propertyDetails,
            RoomDetails roomDetail,
            PreBookHelper preBookHelper)
        {
            var header = new Header
            {
                SupplierId = _settings.SupplierID(propertyDetails, propertyDetails.Source),
                DistributorId = _settings.User(propertyDetails, propertyDetails.Source),
                Token = preBookHelper.SearchToken,
                Version = "v4"
            };

            var stayRange = new StayRange
            {
                CheckIn = propertyDetails.ArrivalDate,
                CheckOut = propertyDetails.ArrivalDate.AddDays(propertyDetails.Duration)
            };

            var roomCriteria = new RoomCriteria
            {
                RoomCount = 1,
                AdultCount = roomDetail.Adults,
                ChildCount = roomDetail.Children + roomDetail.Infants,
                ChildAges = roomDetail.ChildAndInfantAges.ToArray()
            };

            var productCandidate = new ProductCandidate
            {
                RoomId = roomDetail.RoomTypeCode,
                RateId = preBookHelper.RoomRate.RateId
            };


            var availabilityRequest = new DerbySoftBookingUsbV4AvailabilityRequest
            {
                Header = header,
                HotelId = propertyDetails.TPKey,
                StayRange = stayRange,
                RoomCriteria = roomCriteria,
                ProductCandidate = productCandidate
            };

            return availabilityRequest;
        }

        private DerbySoftBookingUsbV4PreBookRequest BuildPreBookRequest(PropertyDetails propertyDetails, PreBookHelper preBookHelper, RoomDetails roomDetails)
        {
            var header = new Header
            {
                SupplierId = _settings.SupplierID(propertyDetails, propertyDetails.Source),
                DistributorId = _settings.User(propertyDetails, propertyDetails.Source),
                Token = preBookHelper.SearchToken,
                Version = "v4"
            };

            var reservationIds = new DerbySoftBookingUsbV4PreBookRequest.ReservationIds
            {
                distributorResId = Guid.NewGuid().ToString()
            };

            var stayRange = new StayRange
            {
                CheckIn = propertyDetails.ArrivalDate,
                CheckOut = propertyDetails.ArrivalDate.AddDays(propertyDetails.Duration)
            };

            var roomCriteria = new RoomCriteria
            {
                RoomCount = 1,
                AdultCount = roomDetails.Adults,
                ChildCount = roomDetails.Children + roomDetails.Infants,
                ChildAges = roomDetails.ChildAges.Concat(Enumerable.Range(1, roomDetails.Infants).Select(a => 1)).ToArray()
            };

            var guests = new List<DerbySoftBookingUsbV4PreBookRequest.Guest>();

            var i = 1;
            foreach (var passenger in roomDetails.Passengers)
            {
                var guest = new DerbySoftBookingUsbV4PreBookRequest.Guest
                {
                    firstName = "AA",
                    lastName = "AA",
                    type = passenger.PassengerType.ToString(),
                    index = i.ToString()
                };
                guests.Add(guest);
                i++;
            };

            var roomRates = new List<RoomRate>();
            var roomRate = new RoomRate
            {
                RoomId = preBookHelper.RoomRate.RoomId,
                RateId = preBookHelper.RoomRate.RateId,
                Currency = preBookHelper.RoomRate.Currency,
                MealPlan = preBookHelper.RoomRate.MealPlan,
            };

            var total = new DerbySoftBookingUsbV4PreBookRequest.Total();

            var dailyRateRetriever = new DailyRateRetriever();
            var dailyRates = dailyRateRetriever.GetDailyRates(
                       preBookHelper.RoomRate,
                       roomDetails.TotalPassengers,
                       propertyDetails.ArrivalDate,
                       propertyDetails.DepartureDate,
                       out var feeAmount);

            if (dailyRates.Item2 != null && dailyRates.Item2.Count > 0)
            {
                roomRate.DailyCostAfterTax = dailyRates.Item2.ToArray();
                total.amountAfterTax = dailyRates.Item2.Sum();
            }

            if (dailyRates.Item1 != null && dailyRates.Item1.Count > 0)
            {
                roomRate.DailyCostBeforeTax = dailyRates.Item1.ToArray();
                total.amountBeforeTax = dailyRates.Item1.Sum();
            }

            roomRates.Add(roomRate);

            return new DerbySoftBookingUsbV4PreBookRequest
            {
                header = header,
                reservationIds = reservationIds,
                hotelId = propertyDetails.TPKey,
                stayRange = stayRange,
                contactPerson = BuildPreBookContactPerson(propertyDetails, roomDetails.Passengers),
                roomCriteria = roomCriteria,
                guests = guests,
                total = total,
                roomRates = roomRates
            };
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var references = new List<string>();
            var supplierSourceReferences = new List<string>();
            var refForCancellations = new List<string>();

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                try
                {
                    //check we haven't already booked this room
                    var roomIndex = propertyDetails.Rooms.IndexOf(roomDetails);
                    if (RoomAlreadyBooked(propertyDetails, roomIndex))
                    {
                        references.Add(GetSourceReference(propertyDetails).Split('|')[roomIndex]);
                        supplierSourceReferences.Add(propertyDetails.SupplierSourceReference.Split('|')[roomIndex]);
                        refForCancellations.Add(propertyDetails.TPRef1.Split('|')[roomIndex]);
                        continue;
                    }

                    //prebook token only lasts 300 seconds so run another prebook
                    var preBook = await PreBookRoomAsync(propertyDetails, roomDetails, false);
                    if (!preBook)
                    {
                        references.Add("[Failed]");
                        supplierSourceReferences.Add("[Failed]");
                        refForCancellations.Add("[Failed]");
                        continue;
                    }

                    //request
                    var bookRequest = BuildBookRequest(propertyDetails, roomDetails, roomIndex + 1);
                    var response =
                        await GetResponseAsync<DerbySoftBookingUsbV4BookRequest, DerbySoftBookingUsbV4BookResponse>(
                            propertyDetails,
                            bookRequest,
                            _settings.BookingURL(propertyDetails, propertyDetails.Source),
                            "Book");

                    var derbyResId = "";
                    var supplierResId = "";
                    var distributorResId = "";

                    if (response?.reservationIds != null)
                    {
                        derbyResId = response.reservationIds.derbyResId;
                        supplierResId = response.reservationIds.supplierResId;
                        distributorResId = response.reservationIds.distributorResId;
                    }

                    if (!string.IsNullOrEmpty(derbyResId) && !string.IsNullOrEmpty(supplierResId) && !string.IsNullOrEmpty(distributorResId))
                    {
                        references.Add(derbyResId);
                        supplierSourceReferences.Add(supplierResId);
                        refForCancellations.Add(distributorResId);
                    }
                    else
                    {
                        references.Add("[Failed]");
                        supplierSourceReferences.Add("[Failed]");
                        refForCancellations.Add("[Failed]");
                    }

                }
                catch (Exception ex)
                {
                    propertyDetails.Warnings.AddNew("Book Exception", ex.InnerException.ToString());
                    references.Add("[Failed]");
                    supplierSourceReferences.Add("[Failed]");
                    refForCancellations.Add("[Failed]");
                }
            }

            propertyDetails.SupplierSourceReference = string.Join("|", supplierSourceReferences);
            propertyDetails.TPRef1 = string.Join("|", refForCancellations);

            var reference = string.Join("|", references);

            if (references.Contains("[Failed]"))
            {
                propertyDetails.SourceSecondaryReference = reference;
                return "failed";
            }

            return reference;
        }

        private DerbySoftBookingUsbV4BookRequest BuildBookRequest(PropertyDetails propertyDetails, RoomDetails roomDetails, int roomNumber)
        {
            //Data stored from search
            PreBookHelper preBookHelper = PreBookHelper.DeserializePreBookHelper(roomDetails.ThirdPartyReference);

            var header = new Header
            {
                SupplierId = _settings.SupplierID(propertyDetails, propertyDetails.Source),
                DistributorId = _settings.User(propertyDetails, propertyDetails.Source),
                Token = preBookHelper.SearchToken,
                Version = "v4"
            };

            var reservationIds = new DerbySoftBookingUsbV4BookRequest.ReservationIds
            {
                distributorResId = string.Concat(propertyDetails.BookingReference.Trim(), "-", preBookHelper.PreBookToken)
            };

            var stayRange = new StayRange
            {
                CheckIn = propertyDetails.ArrivalDate,
                CheckOut = propertyDetails.ArrivalDate.AddDays(propertyDetails.Duration)
            };

            var roomCriteria = new RoomCriteria
            {
                RoomCount = 1,
                AdultCount = roomDetails.Adults,
                ChildCount = roomDetails.Children + roomDetails.Infants,
                ChildAges = roomDetails.ChildAges.Concat(Enumerable.Range(1, roomDetails.Infants).Select(a => 1)).ToArray()
            };

            var guests = new List<DerbySoftBookingUsbV4BookRequest.Guest>();

            var i = 1;
            foreach (var passenger in roomDetails.Passengers)
            {
                var guest = new DerbySoftBookingUsbV4BookRequest.Guest
                {
                    firstName = passenger.FirstName,
                    lastName = passenger.LastName,
                    type = passenger.PassengerType.ToString(),
                    index = roomNumber.ToString()
                };
                guests.Add(guest);
                i++;
            };

            var comments = new List<string>();
            comments.AddRange(propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Select(x => x.SpecialRequest));

            var roomRates = new List<RoomRate>();
            var roomRate = new RoomRate
            {
                RoomId = preBookHelper.RoomRate.RoomId,
                RateId = preBookHelper.RoomRate.RateId,
                Currency = preBookHelper.RoomRate.Currency,
                MealPlan = preBookHelper.RoomRate.MealPlan,
            };

            var total = new DerbySoftBookingUsbV4BookRequest.Total();

            var dailyRateRetriever = new DailyRateRetriever();
            var dailyRates = dailyRateRetriever.GetDailyRates(
            preBookHelper.RoomRate,
            roomDetails.TotalPassengers,
            propertyDetails.ArrivalDate,
            propertyDetails.DepartureDate,
            out var feeAmount);

            if (dailyRates.Item2 != null && dailyRates.Item2.Count > 0)
            {
                roomRate.DailyCostAfterTax = dailyRates.Item2.ToArray();
                total.amountAfterTax = dailyRates.Item2.Sum();
            }

            if (dailyRates.Item1 != null && dailyRates.Item1.Count > 0)
            {
                roomRate.DailyCostBeforeTax = dailyRates.Item1.ToArray();
                total.amountBeforeTax = dailyRates.Item1.Sum();
            }

            roomRates.Add(roomRate);

            return new DerbySoftBookingUsbV4BookRequest
            {
                header = header,
                reservationIds = reservationIds,
                hotelId = propertyDetails.TPKey,
                stayRange = stayRange,
                contactPerson = BuildContactPerson(propertyDetails, roomDetails.Passengers),
                roomCriteria = roomCriteria,
                guests = guests,
                comments = comments,
                total = total,
                roomRates = roomRates,
                bookingToken = preBookHelper.PreBookBookingToken
            };
        }

        #endregion

        #region Cancel

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var cancellationResponse = new ThirdPartyCancellationResponse() { Success = true };
            var cancellationReferences = new List<string>();

            var roomSourceReferences = new List<string>();
            var supplierSourceReferences = new List<string>();
            var refForCancellations = new List<string>();
            string propertySourceReference = GetSourceReference(propertyDetails);

            roomSourceReferences.AddRange(propertySourceReference.Split('|'));
            supplierSourceReferences.AddRange(propertyDetails.SupplierSourceReference.Split('|'));
            refForCancellations.AddRange(propertyDetails.TPRef1.Split('|'));

            foreach (var sourceReference in roomSourceReferences.Where(s => s != "[Failed]"))
            {
                var roomIndex = roomSourceReferences.IndexOf(sourceReference);

                try
                {
                    var cancelRequest = BuildCancelRequest(propertyDetails, sourceReference, supplierSourceReferences[roomIndex], refForCancellations[roomIndex]);

                    var response =
                        await GetResponseAsync<DerbySoftBookingUsbV4CancelRequest, DerbySoftBookingUsbV4CancelResponse>(
                            propertyDetails,
                            cancelRequest,
                            _settings.CancellationURL(propertyDetails, propertyDetails.Source),
                            "Cancel");

                    if (response != null && !string.IsNullOrEmpty(response.cancellationId))
                    {
                        cancellationReferences.Add(response.cancellationId);
                    }
                    else
                    {
                        cancellationResponse.Success = false;
                    }

                }
                catch (Exception ex)
                {
                    cancellationResponse.Success = false;
                    propertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
                }
            }

            cancellationResponse.TPCancellationReference = string.Join("|", cancellationReferences);

            return cancellationResponse;
        }

        private DerbySoftBookingUsbV4CancelRequest BuildCancelRequest(PropertyDetails propertyDetails, string sourceReference, string supplierSourceReference, string RefForCancellation)
        {
            var header = new Header
            {
                SupplierId = _settings.SupplierID(propertyDetails, propertyDetails.Source),
                DistributorId = _settings.User(propertyDetails, propertyDetails.Source),
                Token = Guid.NewGuid().ToString(),
                Version = "v4"
            };

            var reservationIds = new DerbySoftBookingUsbV4CancelRequest.ReservationIds
            {
                distributorResId = RefForCancellation,
                derbyResId = sourceReference,
                supplierResId = supplierSourceReference
            };

            return new DerbySoftBookingUsbV4CancelRequest
            {
                header = header,
                reservationIds = reservationIds
            };
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        #endregion

        #region Helpers

        private async Task<TResponse> GetResponseAsync<TRequest, TResponse>(
            PropertyDetails propertyDetails,
            TRequest deserialisedRequest,
            string endPoint,
            string logFileName)
        {
            var request = new Request
            {
                EndPoint = endPoint,
                Method = RequestMethod.POST,
                Source = propertyDetails.Source,
                ContentType = ContentTypes.Application_json,
                UseGZip = _settings.UseGZip(propertyDetails, propertyDetails.Source),
                CreateLog = true,
                LogFileName = logFileName,
                Accept = "application/json"
            };

            string availabilityRequestString = JsonConvert.SerializeObject(deserialisedRequest, DerbySoftSupport.GetJsonSerializerSettings());
            request.Headers.AddNew("Authorization", "Bearer " + _settings.Password(propertyDetails, propertyDetails.Source));
            request.SetRequest(availabilityRequestString);

            await request.Send(_httpClient, _logger);

            propertyDetails.AddLog(logFileName, request);

            return JsonConvert.DeserializeObject<TResponse>(request.ResponseString);
        }

        private DerbySoftBookingUsbV4PreBookRequest.ContactPerson BuildPreBookContactPerson(PropertyDetails propertyDetails, Passengers passengers)
            => new()
            {
                firstName = string.IsNullOrEmpty(propertyDetails.LeadGuestFirstName) ? "TBC" : propertyDetails.LeadGuestFirstName,
                lastName = string.IsNullOrEmpty(propertyDetails.LeadGuestLastName) ? "TBC" : propertyDetails.LeadGuestLastName,
                age = 0,
                type = "Adult",
                index = GetLeadPassengerIndex(passengers).ToSafeString()
            };

        private DerbySoftBookingUsbV4BookRequest.ContactPerson BuildContactPerson(PropertyDetails propertyDetails, Passengers passengers)
            => new DerbySoftBookingUsbV4BookRequest.ContactPerson
            {
                firstName = propertyDetails.LeadGuestFirstName,
                lastName = propertyDetails.LeadGuestLastName,
                age = propertyDetails.LeadGuestDateOfBirth.GetAgeFromDateOfBirth(),
                type = "Adult",
                index = GetLeadPassengerIndex(passengers).ToSafeString()
            };

        private int GetLeadPassengerIndex(Passengers passengers)
        {
            var leadPassengerIndex = 0;
            var leadPassenger = passengers.FirstOrDefault(p => p.IsLeadGuest);
            if (leadPassenger != null)
            {
                leadPassengerIndex = passengers.IndexOf(leadPassenger);
            }

            return leadPassengerIndex + 1;
        }

        private static string GetSourceReference(PropertyDetails propertyDetails)
            => propertyDetails.SourceReference != "[Failed]" && propertyDetails.SourceReference != "failed" ? propertyDetails.SourceReference : propertyDetails.SourceSecondaryReference;

        private bool RoomAlreadyBooked(PropertyDetails propertyDetails, int roomIndex)
        {
            var sourceReference = GetSourceReference(propertyDetails);

            if (string.IsNullOrEmpty(sourceReference))
            {
                return false;
            }

            var sourceReferences = new List<string>();
            sourceReferences.AddRange(sourceReference.Split('|'));

            if (sourceReferences.Count == propertyDetails.Rooms.Count)
            {
                return sourceReferences[roomIndex] != "[Failed]";
            }

            return false;
        }

        #endregion
    }
}