namespace ThirdParty.CSSuppliers.Netstorming
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty;
    using ThirdParty.CSSuppliers.Netstorming.Models;
    using ThirdParty.CSSuppliers.Netstorming.Models.Common;
    using ThirdParty.Interfaces;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class Netstorming : IThirdParty, IMultiSource
    {
        private readonly INetstormingSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Netstorming> _logger;

        public Netstorming(
            INetstormingSettings settings,
            ITPSupport support,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<Netstorming> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public List<string> Sources => NetstormingSupport.NetstormingSources;

        public bool SupportsRemarks => false;

        public bool SupportsBookingSearch => false;

        public bool RequiresVCard(VirtualCardInfo info, string source) => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails, source);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails, source);
        }

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            XmlDocument? xmlRequest = null;
            XmlDocument? xmlResponse = null;
            XmlDocument? xmlCancellationCostRequest = null;
            XmlDocument? xmlCancellationCostResponse = null;

            try
            {
                // Netstorming send us fixed room combinations but we want to display everything. If rooms have different contract agreements, 
                // fail the prebook, otherwise the incorrect rooms will be booked.
                string[] firstRoomDetailsSplit = propertyDetails.Rooms[0].ThirdPartyReference.Split('_');
                string agreementId = firstRoomDetailsSplit[8];
                string searchNumber = firstRoomDetailsSplit[14];
                foreach (string[] referenceItems in propertyDetails.Rooms.Select(roomDetails => roomDetails.ThirdPartyReference.Split('_')))
                {
                    if (referenceItems[8] != agreementId)
                    {
                        propertyDetails.Warnings.AddNew("Prebook Exception",
                            "Only rooms from the same contract agreement can be booked together.");
                        return false;
                    }

                    if (referenceItems[14] != searchNumber)
                    {
                        propertyDetails.Warnings.AddNew("Prebook Exception",
                            @"Only rooms from the same Netstorming search response can be booked together. " +
                            "Using the Netstorming response logs, verify the rooms you have selected have the same Search Number.");
                        return false;
                    }
                }

                string hotelCode = firstRoomDetailsSplit[4];
                string cityCode = firstRoomDetailsSplit[6];
                string nationality = await _support.TPNationalityLookupAsync(propertyDetails.Source, propertyDetails.NationalityCode);

                // build and log the request
                xmlRequest = BuildSearchRequest(
                    propertyDetails,
                    propertyDetails.Rooms,
                    firstRoomDetailsSplit[14],
                    agreementId,
                    propertyDetails.TotalPrice,
                    cityCode,
                    propertyDetails.ArrivalDate,
                    propertyDetails.DepartureDate,
                    _settings.Actor(propertyDetails, propertyDetails.Source),
                    _settings.User(propertyDetails, propertyDetails.Source),
                    _settings.Password(propertyDetails, propertyDetails.Source),
                    _settings.Version(propertyDetails, propertyDetails.Source),
                    nationality,
                    hotelCode);

                // send the request 
                var availabilityRequest = new Request
                {
                    Source = propertyDetails.Source,
                    CreateLog = true,
                    LogFileName = "Prebook",
                    EndPoint = _settings.GenericURL(propertyDetails, propertyDetails.Source),
                    Param = string.Empty
                };
                availabilityRequest.SetRequest(xmlRequest);
                await availabilityRequest.Send(_httpClient, _logger);

                xmlResponse = availabilityRequest.ResponseXML;

                if (xmlResponse == null || xmlResponse.SelectSingleNode("envelope/response")?.InnerText == "No hotel available")
                {
                    return false;
                }

                var response = NetstormingSupport.DeSerialize<NetstormingAvailabilityResponse>(xmlResponse, _serializer);
                var hotelAgreements = GetHotelAgreements(response, propertyDetails.Rooms);

                if (!hotelAgreements.Any())
                {
                    return false;
                }

                // This uses the first room as this was how it was done in previous code
                var hotelAgreement = hotelAgreements.First();

                propertyDetails.TPRef1 = hotelAgreement.Id;
                propertyDetails.TPRef2 = $"{response.Response.Search.Number}_{cityCode}_{hotelCode}";
                var roomTypeCodes = GetRoomTypeCodes(propertyDetails.Rooms);
                decimal cost = GetCost(hotelAgreements, roomTypeCodes);

                // Check for price change
                if (cost != propertyDetails.LocalCost)
                {
                    propertyDetails.LocalCost = cost;
                    decimal roomCost = cost / propertyDetails.Rooms.Count;

                    foreach (var oRoomDetails in propertyDetails.Rooms)
                    {
                        oRoomDetails.LocalCost = roomCost;
                        oRoomDetails.GrossCost = roomCost;
                    }
                }

                // do we have any remarks?
                var remarks = from hotel in response.Response.Hotels.Hotel
                    from agreement in hotel.Agreement
                    from remark in agreement.Remarks
                    where agreement.Id == propertyDetails.TPRef1
                    select remark;

                foreach (var remark in remarks.Where(r => !string.IsNullOrWhiteSpace(r.Text)))
                {
                    propertyDetails.Errata.AddNew("Important Information", remark.Text);
                }

                // build the request to get the cancellation details
                var cancellationCostRequest = new NetstormingCancellationCostRequest
                {
                    Header = NetstormingSupport.Header(
                        _settings.Actor(propertyDetails, propertyDetails.Source),
                        _settings.User(propertyDetails, propertyDetails.Source),
                        _settings.Password(propertyDetails, propertyDetails.Source),
                        _settings.Version(propertyDetails, propertyDetails.Source)),
                    Query =
                    {
                        Type = "get_deadline",
                        Product = "hotel",
                        Availability = {Id = propertyDetails.TPRef2.Split('_')[0]},
                        Hotel = {Id = hotelCode},
                        Agreement = {Code = propertyDetails.TPRef1}
                    }
                };

                // build and log and send the request
                xmlCancellationCostRequest = NetstormingSupport.Serialize(cancellationCostRequest, _serializer);
                var cancellationRequest = new Request
                {
                    Source = propertyDetails.Source,
                    CreateLog = true,
                    LogFileName = "Prebook Cancellation Cost",
                    EndPoint = _settings.GenericURL(propertyDetails, propertyDetails.Source),
                    Param = string.Empty
                };
                cancellationRequest.SetRequest(xmlCancellationCostRequest);
                await cancellationRequest.Send(_httpClient, _logger);

                xmlCancellationCostResponse = cancellationRequest.ResponseXML;

                // set the cancellations
                var cancellations = new Cancellations();

                // the deadline node always has a date, the policy and remarks can be empty. 
                // if the policy node is empty then the date in the deadline node is from when the full cost applies
                var cancellationCostResponse = NetstormingSupport.DeSerialize<NetstormingCancellationCostResponse>(xmlCancellationCostResponse, _serializer);

                if (cancellationCostResponse.Response.Policies.Policy.Any())
                {
                    // get the start date of the next cancellation policy if there is one otherwise its the last one and use a date in the future
                    int policyCount = 1;
                    foreach (var cancellationPolicy in cancellationCostResponse.Response.Policies.Policy)
                    {
                        policyCount += 1;
                        var cancellation = new Cancellation {StartDate = cancellationPolicy.From.ToSafeDate()};

                        if (cancellation.StartDate < DateTime.Now.Date)
                            cancellation.StartDate = DateTime.Now.Date;

                        cancellation.Amount = propertyDetails.LocalCost / 100 * cancellationPolicy.Percentage.ToSafeDecimal();

                        cancellation.EndDate = cancellationCostResponse.Response.Policies.Policy.Length > policyCount
                            ? cancellationCostResponse.Response.Policies.Policy[policyCount].From.ToSafeDate()
                            : new DateTime(2099, 12, 30);

                        cancellations.Add(cancellation);
                    }

                    cancellations.Solidify(SolidifyType.LatestStartDate);
                }
                else
                {
                    var cancellation = new Cancellation
                    {
                        StartDate = cancellationCostResponse.Response.Deadline.Value.Split(' ')[0].ToSafeDate()
                    };

                    if (cancellation.StartDate < DateTime.Now.Date)
                        cancellation.StartDate = DateTime.Now.Date;

                    cancellation.EndDate = new DateTime(2099, 12, 30);
                    cancellation.Amount = propertyDetails.LocalCost;
                    cancellations.Add(cancellation);
                }

                propertyDetails.Cancellations = cancellations;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                return false;
            }
            finally
            {
                // store the request and response xml on the property booking
                if (xmlRequest != null && !string.IsNullOrEmpty(xmlRequest.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, $"{propertyDetails.Source} Prebook Request", xmlRequest);
                }

                if (xmlResponse != null && !string.IsNullOrEmpty(xmlResponse.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, $"{propertyDetails.Source} Prebook Response", xmlResponse);
                }

                if (xmlCancellationCostRequest != null && !string.IsNullOrEmpty(xmlCancellationCostRequest.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, $"{propertyDetails.Source} Cancellation Cost Request", xmlCancellationCostRequest);
                }

                if (xmlCancellationCostResponse != null && !string.IsNullOrEmpty(xmlCancellationCostResponse.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, $"{propertyDetails.Source} Cancellation Cost Response", xmlCancellationCostResponse);
                }
            }

            return true;
        }

        private static IReadOnlyList<Agreement> GetHotelAgreements(NetstormingAvailabilityResponse response, IEnumerable<RoomDetails> rooms)
        {
            var result = new List<Agreement>();

            foreach (var agreement in rooms.Select(room => GetHotelAgreement(response, room.ThirdPartyReference)))
            {
                if (agreement == null)
                {
                    return result;
                }

                result.Add(agreement);
            }

            return result;
        }

        private static Agreement? GetHotelAgreement(NetstormingAvailabilityResponse response, string firstRoomThirdPartyReference)
        {
            string[] items = firstRoomThirdPartyReference.Split('_');
            return GetHotelAgreementById(response, items) ??
                   GetHotelAgreementByType(response, items);
        }

        private static Agreement? GetHotelAgreementById(NetstormingAvailabilityResponse response, IReadOnlyList<string> referenceItems)
        {
            var matchedAgreements =
                from hotel in response.Response.Hotels.Hotel
                from agreement in hotel.Agreement
                where agreement.Id == referenceItems[8]
                select agreement;

            return matchedAgreements.FirstOrDefault();
        }

        private static Agreement? GetHotelAgreementByType(NetstormingAvailabilityResponse response, IReadOnlyList<string> referenceItems)
        {
            string selectedRoomBasis = referenceItems[9];
            string selectedMealBasis = referenceItems[10];
            string selectedType = referenceItems[11];
            string selectedAvailable = referenceItems[12];
            string selectedSpecial = referenceItems[13];
            string selectedCode = referenceItems[4];

            var matchedAgreements =
                from hotel in response.Response.Hotels.Hotel
                from agreement in hotel.Agreement
                where (agreement.RoomBasis ?? string.Empty) == (selectedRoomBasis ?? string.Empty) &&
                      (agreement.MealBasis ?? string.Empty) == (selectedMealBasis ?? string.Empty) &&
                      (agreement.C_type ?? string.Empty) == (selectedType ?? string.Empty) &&
                      (agreement.Available ?? string.Empty) == (selectedAvailable ?? string.Empty) &&
                      (agreement.Special ?? string.Empty) == (selectedSpecial ?? string.Empty) &&
                      (hotel.Code ?? string.Empty) == (selectedCode ?? string.Empty)
                select agreement;

            return matchedAgreements.FirstOrDefault();
        }

        private static decimal GetCost(IReadOnlyList<Agreement> hotelAgreements, IReadOnlyList<string> roomTypeCodes)
        {
            decimal cost = 0m;
            for (int i = 0, loopTo = hotelAgreements.Count - 1; i <= loopTo; i++)
            {
                foreach (var room in hotelAgreements[i].Room.Where(r => r.Type == roomTypeCodes[i]))
                {
                    cost += room.Price.Sum(r => r.RoomPrice.Nett.ToSafeDecimal());
                }
            }

            return cost;
        }

        private static IReadOnlyList<string> GetRoomTypeCodes(IEnumerable<RoomDetails> rooms)
        {
            return rooms.Select(room => room.ThirdPartyReference.Split('_')[0]).ToList();
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            XmlDocument? xmlRequest = null;
            XmlDocument? xmlResponse = null;
            string reference;

            try
            {
                string bookingRef = GenerateBookingReference(propertyDetails.BookingReference);
                string[] ref2Split = propertyDetails.TPRef2.Split('_');
                string nationality = await _support.TPNationalityLookupAsync(propertyDetails.Source, propertyDetails.NationalityCode);
                xmlRequest = BuildBookRequest(
                    propertyDetails,
                    _settings.Actor(propertyDetails, propertyDetails.Source),
                    _settings.User(propertyDetails, propertyDetails.Source),
                    _settings.Password(propertyDetails, propertyDetails.Source),
                    _settings.Version(propertyDetails, propertyDetails.Source),
                    nationality,
                    ref2Split[0],
                    propertyDetails.ArrivalDate,
                    propertyDetails.DepartureDate,
                    ref2Split[1],
                    ref2Split[2],
                    propertyDetails.TPRef1,
                    bookingRef,
                    _settings.ContactEmail(propertyDetails, propertyDetails.Source),
                    propertyDetails.Rooms);

                var webRequest = new Request
                {
                    Source = propertyDetails.Source,
                    EndPoint = _settings.GenericURL(propertyDetails, propertyDetails.Source),
                    ContentType = ContentTypes.Application_x_www_form_urlencoded
                };
                webRequest.SetRequest(xmlRequest);
                await webRequest.Send(_httpClient, _logger);

                xmlResponse = webRequest.ResponseXML;

                var bookingResponse = NetstormingSupport.DeSerialize<NetstormingBookResponse>(xmlResponse, _serializer);

                if (bookingResponse.Response.Type != "error" && bookingResponse.Response.Status.Code == "cnf")
                {
                    reference = bookingResponse.Response.Booking.Code;
                }
                else
                {
                    reference = "failed";
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
                if (xmlRequest != null && !string.IsNullOrEmpty(xmlRequest.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, $"{propertyDetails.Source} Book Request", xmlRequest);
                }

                if (xmlResponse != null && !string.IsNullOrEmpty(xmlResponse.InnerXml))
                {
                    propertyDetails.Logs.AddNew(propertyDetails.Source, $"{propertyDetails.Source} Book Response", xmlResponse);
                }
            }

            return reference;
        }

        // there is a cancellation feature but it is not instant and the cancellation status goes to pending so we will not use it
        public Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationResponse());
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        private static string GenerateBookingReference(string reference)
        {
            // the booking reference needs to be a unique number no longer than 20 characters long
            string bookingReference;

            if (!string.IsNullOrEmpty(reference))
            {
                bookingReference = reference + DateTime.Now.ToString("hhmmss");
            }
            else
            {
                bookingReference = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond;
            }

            return bookingReference;
        }

        private static string CheckTitle(string title)
        {
            // the titles can only be MR, MS, MRS or MISS so if its not just set it to MR
            string[] list = {"MR", "MS", "MRS", "MISS"};

            return list.Contains(title.ToUpper()) ? title : "MR";
        }

        public static string TrimLastName(string lastName)
        {
            return lastName.Length > 15 ? lastName.Substring(0, 15) : lastName;
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        protected virtual XmlDocument BuildSearchRequest(
            IThirdPartyAttributeSearch propertyDetails,
            IReadOnlyCollection<RoomDetails> rooms,
            string searchNumber,
            string agreement,
            decimal price,
            string resortCode,
            DateTime propertyArrivalDate,
            DateTime propertyDepartureDate,
            string actor,
            string user,
            string password,
            string version,
            string nationality,
            string hotelCode)
        {
            var request = new NetstormingAvailabilityRequest
            {
                Header = NetstormingSupport.Header(actor, user, password, version),
                Query =
                {
                    Type = "availability",
                    Product = "hotel",
                    Filters = new[] {"AVAILONLY"},
                    Search = {Number = searchNumber, Agreement = agreement, Price = price.ToSafeString()},
                    Checkin = {Date = propertyArrivalDate.ToString("yyyy-MM-dd")},
                    Checkout = {Date = propertyDepartureDate.ToString("yyyy-MM-dd")},
                    City = {Code = resortCode},
                    Hotel = {Id = hotelCode}
                }
            };

            var details = new List<Room>(rooms.Count);
            foreach (var room in rooms)
            {
                string[] reference = room.ThirdPartyReference.Split('_');

                var detail = new Room
                {
                    Type = reference[0],
                    Required = 1
                };

                if (reference[3].ToSafeBoolean())
                {
                    detail.Cot = "true";
                }

                details.Add(detail);
            }

            request.Query.Details = details.ToArray();

            return NetstormingSupport.Serialize(request, _serializer);
        }

        protected virtual XmlDocument BuildBookRequest(IThirdPartyAttributeSearch propertyDetails, string actor, string user,
            string password, string version, string nationality, string searchNumber, DateTime arrivalDate,
            DateTime departureDate, string cityCode, string hotelCode, string agreement, string bookingReference,
            string reportingEmail, IEnumerable<RoomDetails> roomDetails)
        {
            var request = new NetstormingBookRequest
            {
                Header = NetstormingSupport.Header(actor, user, password, version),
                Query =
                {
                    Type = "book",
                    Product = "hotel",
                    Search = { Number = searchNumber },
                    Synchronous = { Value = "true" },
                    Checkin = { Date = arrivalDate.ToString("yyyy-MM-dd") },
                    Checkout = { Date = departureDate.ToString("yyyy-MM-dd") },
                    City = { Code = cityCode },
                    Availonly = { Value = "true" },
                    Hotel = { Code = hotelCode, Agreement = agreement },
                    Reference = { Code = bookingReference },
                    To = new[] { new To { Url = reportingEmail } },
                    Details = roomDetails.Select(BuildRoomBook).ToArray()
                }
            };

            return NetstormingSupport.Serialize(request, _serializer);
        }

        protected Room BuildRoomBook(RoomDetails room)
        {
            string[] referenceItems = room.ThirdPartyReference.Split('_');
            string cot = referenceItems[3];
            bool hasLeadGuestMatch = room.Passengers.Any(o => o.IsLeadGuest);

            return new Room
            {
                Type = referenceItems[0],
                Cot = cot,
                Pax = room.Passengers
                    // remember that when booking for a room with cots there's no need to add the pax name of the infant as well
                    .Where(p => !(cot == "true" && p.PassengerType == PassengerType.Infant))
                    .Select((pax, i) => BuildPaxBook(pax, i, hasLeadGuestMatch)).ToArray()
            };
        }

        protected Pax BuildPaxBook(Passenger pax, int index, bool hasLeadGuestMatch)
        {
            return new Pax
            {
                Title = CheckTitle(pax.Title),
                Initial = pax.FirstName.Substring(0, 1),
                Surname = TrimLastName(pax.LastName),
                Leader = (hasLeadGuestMatch && pax.IsLeadGuest || !hasLeadGuestMatch && index == 0) ? "true" : "false",
                Age = pax.PassengerType == PassengerType.Child ? pax.Age : 0
            };
        }
    }
}