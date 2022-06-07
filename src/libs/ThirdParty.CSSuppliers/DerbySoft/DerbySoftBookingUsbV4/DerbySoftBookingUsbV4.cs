namespace ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using ThirdParty;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Lookups;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4.Models;
    using ThirdParty.CSSuppliers.DerbySoft.Models;

    public abstract class DerbySoftBookingUsbV4 : IThirdParty
    {
        #region Properties

        private readonly IDerbySoftSettings _settings;
        private readonly ITPSupport _support;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public virtual string Source { get; set; }
        public bool SupportsRemarks => true;
        public bool SupportsBookingSearch => false;
        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source) => true;
        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails) => 0;
        public bool RequiresVCard(VirtualCardInfo oInfo) => false;

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails oBookingSearchDetails) => new ThirdPartyBookingSearchResults();
        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails oPropertyDetails) => new ThirdPartyBookingStatusUpdateResult();
        public void EndSession(PropertyDetails oPropertyDetails) { }
        public string CreateReconciliationReference(string sInputReference) => throw new NotImplementedException();

        #endregion

        #region "Constructor"

        public DerbySoftBookingUsbV4(IDerbySoftSettings settings, ITPSupport support, HttpClient httpClient, ILogger logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region PreBook

        public bool PreBook(PropertyDetails propertyDetails)
        {
            bool preBookSuccess = true;

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                preBookSuccess = CheckAvailability(propertyDetails, roomDetails);

                if (!preBookSuccess)
                {
                    break;
                }

                preBookSuccess = PreBookRoom(propertyDetails, roomDetails, true) && preBookSuccess;
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

        private bool CheckAvailability(PropertyDetails propertyDetails, RoomDetails roomDetails)
        {
            bool availabilitySuccess = true;
            var availabilityRequest = new Request();
            try
            {
                PreBookHelper preBookHelper = PreBookHelper.DeserializePreBookHelper(roomDetails.ThirdPartyReference);

                var availabilityDeserialisedRequest = BuildAvailabilityRequest(propertyDetails, roomDetails, preBookHelper);

                var availabilityResponse =
                    GetResponse<DerbySoftBookingUsbV4AvailabilityRequest, DerbySoftBookingUsbV4AvailabilityResponse>(
                        propertyDetails,
                        availabilityRequest,
                        availabilityDeserialisedRequest,
                        _settings.SearchURL(propertyDetails),
                        "Prebook - Availability");

                var roomRate = availabilityResponse?.RoomRates?.FirstOrDefault(r =>
                    !string.IsNullOrWhiteSpace(r.RoomId) && r.RoomId == roomDetails.RoomTypeCode &&
                    !string.IsNullOrWhiteSpace(r.MealPlan) && (r.MealPlan == roomDetails.MealBasisCode || (r.MealPlan == "RO" && roomDetails.MealBasisCode == "NA")) &&
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

        private bool PreBookRoom(PropertyDetails propertyDetails, RoomDetails roomDetails, bool addCancellations)
        {
            var preBookSuccess = true;
            var prebookRequest = new Request();
            try
            {
                //Data stored from search
                PreBookHelper preBookHelper = PreBookHelper.DeserializePreBookHelper(roomDetails.ThirdPartyReference);

                var prebookDeserialisedRequest = BuildPreBookRequest(propertyDetails, preBookHelper, roomDetails);

                var prebookResponse =
                    GetResponse<DerbySoftBookingUsbV4PreBookRequest, DerbySoftBookingUsbV4PreBookResponse>(
                        propertyDetails,
                        prebookRequest,
                        prebookDeserialisedRequest,
                        _settings.PreBookURL(propertyDetails),
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
                SupplierId = _settings.SupplierID(propertyDetails),
                DistributorId = _settings.User(propertyDetails),
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
                SupplierId = _settings.SupplierID(propertyDetails),
                DistributorId = _settings.User(propertyDetails),
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

        #region "Book"

        public string Book(PropertyDetails propertyDetails)
        {
            var references = new List<string>();
            var supplierSourceReferences = new List<string>();
            var refForCancellations = new List<string>();

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                var webRequest = new Request();

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
                    var preBook = PreBookRoom(propertyDetails, roomDetails, false);
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
                        GetResponse<DerbySoftBookingUsbV4BookRequest, DerbySoftBookingUsbV4BookResponse>(
                            propertyDetails,
                            webRequest,
                            bookRequest,
                            _settings.BookURL(propertyDetails),
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
                SupplierId = _settings.SupplierID(propertyDetails),
                DistributorId = _settings.User(propertyDetails),
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
            comments.AddRange(propertyDetails.BookingComments.Select(c => c.Text));

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

        #region "Cancel"

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails)
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
                var request = new Request();

                var roomIndex = roomSourceReferences.IndexOf(sourceReference);

                try
                {
                    var cancelRequest = BuildCancelRequest(propertyDetails, sourceReference, supplierSourceReferences[roomIndex], refForCancellations[roomIndex]);

                    var response =
                        GetResponse<DerbySoftBookingUsbV4CancelRequest, DerbySoftBookingUsbV4CancelResponse>(
                            propertyDetails,
                            request,
                            cancelRequest,
                            _settings.CancelURL(propertyDetails),
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

        private DerbySoftBookingUsbV4CancelRequest BuildCancelRequest(IThirdPartyAttributeSearch searchDetails, string sourceReference, string supplierSourceReference, string RefForCancellation)
        {
            var header = new Header
            {
                SupplierId = _settings.SupplierID(searchDetails),
                DistributorId = _settings.User(searchDetails),
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


        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails oPropertyDetails)
        {
            return new ThirdPartyCancellationFeeResult();
        }

        #endregion

        #region "Helpers"

        private TResponse GetResponse<TRequest, TResponse>(
            PropertyDetails propertyDetails,
            Request request,
            TRequest deserialisedRequest,
            string endPoint,
            string logFileName)
        {
            request.EndPoint = endPoint;
            request.Method = eRequestMethod.POST;
            request.Source = Source;
            request.ContentType = ContentTypes.Application_json;
            request.UseGZip = _settings.UseGZip(propertyDetails);
            request.CreateLog = propertyDetails.CreateLogs;
            request.LogFileName = logFileName;
            request.Accept = "application/json";

            string availabilityRequestString = JsonConvert.SerializeObject(deserialisedRequest, DerbySoftSupport.GetJsonSerializerSettings());
            request.SetRequest(availabilityRequestString);
            request.Headers.AddNew("Authorization", "Bearer " + _settings.Password(propertyDetails));
            request.Send(_httpClient, _logger).RunSynchronously();

            if (!string.IsNullOrWhiteSpace(request.RequestString) && !string.IsNullOrWhiteSpace(request.ResponseString))
            {
                propertyDetails.Logs.AddNew(Source, $"{Source} {logFileName}", request.RequestString, request.ResponseString);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(request.RequestString))
                {
                    propertyDetails.Logs.AddNew(Source, $"{Source} {logFileName} Request", request.RequestString);
                }

                if (!string.IsNullOrWhiteSpace(request.ResponseString))
                {
                    propertyDetails.Logs.AddNew(Source, $"{Source} {logFileName} Response", request.ResponseString);
                }
            }

            return JsonConvert.DeserializeObject<TResponse>(request.ResponseString);
        }

        private DerbySoftBookingUsbV4PreBookRequest.ContactPerson BuildPreBookContactPerson(PropertyDetails propertyDetails, Passengers passengers)
            => new DerbySoftBookingUsbV4PreBookRequest.ContactPerson
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
                age = propertyDetails.DateOfBirth.GetAgeFromDateOfBirth(),
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