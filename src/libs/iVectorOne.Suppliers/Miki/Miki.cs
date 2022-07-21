namespace iVectorOne.Suppliers.Miki
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
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Models;
    using Models.Common;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;

    public class Miki : IThirdParty, ISingleSource
    {
        private readonly IMikiSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Miki> _logger;

        public Miki(
            IMikiSettings settings,
            ITPSupport support,
            ISerializer serializer,
            IMemoryCache cache,
            HttpClient httpClient,
            ILogger<Miki> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _cache = Ensure.IsNotNull(cache, nameof(cache));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public bool SupportsRemarks => false;
        public bool SupportsBookingSearch => false;
        public string Source => ThirdParties.MIKI;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails, false);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails, false);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            bool success = true;

            // added 27/7/11 JW
            // the cancellation charges were only being picked up after the rooms were booked so the charges displayed and given to the customer were wrong
            // before the booking
            // the cancellation charges can be found in the search response so do another search for just the hotel they are booking and match the rooms
            var request = new Request();

            try
            {
                var searchRequest = await HotelSearchAsync(propertyDetails);

                request = new Request
                {
                    EndPoint = _settings.BaseURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = Source,
                    ContentType = "application/soap+xml;charset=UTF-8;action=\"hotelSearch\"",
                    LogFileName = "PreBook",
                    CreateLog = true,
                    UseGZip = true,
                    SoapAction = "hotelSearch"
                };
                request.SetRequest(searchRequest);
                await request.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<Envelope<HotelSearchResponse>>(request.ResponseXML);

                // check to see if the costs have changed
                UpdateCostsAndReferences(propertyDetails, response);

                // need to match the rooms and get the cancellation details out
                propertyDetails.Cancellations = CreatePreBookCancellations(response, propertyDetails);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                success = false;
            }
            finally
            {
                propertyDetails.AddLog("Prebook Availability Check", request);
            }
            
            return success;
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var bookingRequest = new Request();
            string reference;

            try
            {
                var requestXml = await GetXmlAsync(propertyDetails);
                bookingRequest = new Request
                {
                    EndPoint = _settings.BaseURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = Source,
                    UseGZip = true,
                    ContentType = "application/soap+xml;charset=UTF-8;action=\"hotelBooking\"",
                    SoapAction = "hotelBooking",
                    LogFileName = "Book",
                    CreateLog = true,
                };

                bookingRequest.SetRequest(requestXml);
                await bookingRequest.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<Envelope<HotelBookingResponse>>(bookingRequest.ResponseXML);
                var bookingReferenceNode = response.Body.Content.Booking;

                if (!string.IsNullOrEmpty(bookingReferenceNode.BookingReference))
                {
                    if (propertyDetails.Rooms[0].ThirdPartyReference.Split('~').Length > 2)
                    {
                        propertyDetails.SourceSecondaryReference =
                            propertyDetails.Rooms[0].ThirdPartyReference.Split('~')[1];
                    }

                    propertyDetails.SourceSecondaryReference += "~";

                    foreach (var item in bookingReferenceNode.Items)
                    {
                        propertyDetails.SourceSecondaryReference += $"{item.TourReference}#";
                    }

                    propertyDetails.SourceSecondaryReference = propertyDetails.SourceSecondaryReference.Chop();

                    // cancellation
                    propertyDetails.Cancellations = CreateBookCancellations(response);
                    reference = bookingReferenceNode.BookingReference;
                }
                else
                {
                    // if a booking fails - do not continue
                    reference = "failed";
                    propertyDetails.Warnings.AddNew("Book Fail", "There is no reference for this node");
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
                reference = "failed";
            }
            finally
            {
                propertyDetails.AddLog("Book", bookingRequest);
            }

            return reference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            var cancellationReferences = new List<string>();

            try
            {
                // agent code stored in sourcecancellation reference as needs to be consistent with booking agent code and there are three different possibilities
                string agentCode = propertyDetails.SourceSecondaryReference.Split('~')[0];
                string references;
                char delimiter = '#';

                if (propertyDetails.SourceSecondaryReference.Split('~').Length > 1)
                {
                    // using new version where main source reference is the group clientBookingreference
                    // we need to use the mikiTourReference
                    references = propertyDetails.SourceSecondaryReference.Split('~')[1];
                }
                else
                {
                    references = propertyDetails.SourceReference;
                    delimiter = '~';
                }

                foreach (string reference in references.Split(delimiter))
                {
                    var request = await BuildCancellationRequestAsync(reference, propertyDetails, agentCode);
                    var webRequest = new Request
                    {
                        EndPoint = _settings.BaseURL(propertyDetails),
                        Method = RequestMethod.POST,
                        Source = Source,
                        ContentType = "application/soap+xml;charset=UTF-8;action=\"cancellation\"",
                        LogFileName = "Cancel",
                        CreateLog = true,
                        UseGZip = true,
                        SoapAction = "cancellation"
                    };
                    webRequest.SetRequest(request);

                    try
                    {
                        await webRequest.Send(_httpClient, _logger);
                    }
                    finally
                    {
                        propertyDetails.AddLog("Cancellation", webRequest);
                    }

                    var response = _serializer.DeSerialize<Envelope<CancellationResponse>>(webRequest.ResponseXML);
                    string node = response.Body.Content.CancelledTours.Select(x => x.Status).First();
                    if (!string.IsNullOrEmpty(node) && node == "Cancelled")
                    {
                        cancellationReferences.Add(response.Body.Content.CancelledTours
                            .Select(x => x.CancellationReference).First());
                        thirdPartyCancellationResponse.Success = true;
                    }
                    else
                    {
                        // failed cancellation - assume all rooms failed so that it can be handled manually
                        thirdPartyCancellationResponse.TPCancellationReference = string.Empty;
                        thirdPartyCancellationResponse.Success = false;
                        break;
                    }
                }

                if (cancellationReferences.Count > 0 && thirdPartyCancellationResponse.Success)
                {
                    thirdPartyCancellationResponse.TPCancellationReference = string.Join("~", cancellationReferences);
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString());
                thirdPartyCancellationResponse.Success = false;
            }

            return thirdPartyCancellationResponse;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
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

        public void EndSession(PropertyDetails propertyDetails) { }

        private async Task<XmlDocument> HotelSearchAsync(PropertyDetails propertyDetails)
        {
            string currencyCode = await _support.TPCurrencyCodeLookupAsync(propertyDetails.Source, propertyDetails.ISOCurrencyCode);
            string agentCode = MikiSupport.GetAgentCode(currencyCode, propertyDetails, _settings);
            string password = await MikiSupport.GetPasswordAsync(propertyDetails, _settings, _serializer, _cache);

            string paxNationality = string.Empty;
            string nationalityId = propertyDetails.ISONationalityCode;

            if (!string.IsNullOrEmpty(nationalityId))
            {
                paxNationality = await _support.TPNationalityLookupAsync(Source, nationalityId);
            }

            if (string.IsNullOrEmpty(paxNationality))
            {
                paxNationality = _settings.BookingCountryCode(propertyDetails);
            }

            var rooms = propertyDetails.Rooms.Select((roomDetails, roomCount) =>
            {
                roomCount++;
                var childAgesUnder12 = new List<int>();
                if (roomDetails.ChildAges.Count > 0)
                {
                    childAgesUnder12.AddRange(roomDetails.ChildAges.Where(age => age <= 12));
                }

                var guests = new List<Guest>();
                for (int i = 1; i <= roomDetails.Adults + roomDetails.ChildAges.Count - childAgesUnder12.Count; i++)
                {
                    guests.Add(new Guest { Type = GuestCountType.ADT });
                }

                if (childAgesUnder12.Count + roomDetails.Infants > 0)
                {
                    for (int i = 1; i <= childAgesUnder12.Count + roomDetails.Infants; i++)
                    {
                        int childAge = childAgesUnder12.Count > i - 1
                            ? Math.Max(childAgesUnder12[i - 1], 3)
                            : 3;

                        guests.Add(new Guest
                        {
                            Type = GuestCountType.CHD,
                            Age = childAge
                        });
                    }
                }

                return new Room
                {
                    RoomNo = roomCount,
                    Guests = guests.ToArray(),
                };
            }).ToArray();

            var request = new Envelope<HotelSearchRequest>
            {
                Body =
                {
                    Content =
                    {
                        VersionNumber = "7.0",
                        RequestAuditInfo = MikiSupport.BuildRequestAuditInfo(agentCode, password),
                        HotelSearchCriteria =
                        {
                            CurrencyCode = propertyDetails.ISOCurrencyCode,
                            PaxNationality = paxNationality,
                            LanguageCode = _settings.Language(propertyDetails).ToLower(),
                            Destination =
                            {
                                HotelRefs =
                                {
                                    ProductCodes = new []{ propertyDetails.TPKey }
                                }
                            },
                            StayPeriod =
                            {
                                CheckinDate = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                                NumberOfNights = propertyDetails.Duration
                            },
                            Rooms = rooms
                        }
                    }
                }
            };

            return _serializer.Serialize(request);
        }

        private void UpdateCostsAndReferences(PropertyDetails propertyDetails, Envelope<HotelSearchResponse> response)
        {
            foreach (var roomDetails in propertyDetails.Rooms)
            {
                string roomTypeCode = roomDetails.ThirdPartyReference.Split('~')[0];
                string agentCode = roomDetails.ThirdPartyReference.Split('~')[1];
                string roomDescription = roomDetails.RoomType.ToLower();
                string mealBasisCode = roomDetails.MealBasisCode.ToLower();

                bool validRoom = false;
                var nodes = response.Body.Content.Hotels
                    .Where(x => x.ProductCode == propertyDetails.TPKey)
                    .SelectMany(r => r.RoomOptions)
                    .Where(r => r.RoomTypeCode == roomTypeCode);

                foreach (var node in nodes)
                {
                    if (node.RoomDescription.ToLower() == roomDescription
                        && node.MealBasis.MealBasisCode.ToLower() == mealBasisCode)
                    {
                        roomDetails.ThirdPartyReference = $"{roomTypeCode}~{agentCode}~{node.RateIdentifier}";

                        decimal newCost = node.RoomTotalPrice.Price;
                        bool available = node.AvailabilityStatus;

                        if (newCost == 0 || !available)
                        {
                            throw new Exception($"Room description {roomDescription} no longer available");
                        }

                        if (newCost != roomDetails.GrossCost)
                        {
                            roomDetails.LocalCost = newCost;
                            roomDetails.GrossCost = newCost;
                        }

                        validRoom = true;
                    }

                    if (!validRoom)
                    {
                        throw new Exception($"Room description {roomDescription} no longer available");
                    }
                }
            }
        }

        private async Task<XmlDocument> GetXmlAsync(PropertyDetails propertyDetails)
        {
            string password = await MikiSupport.GetPasswordAsync(propertyDetails, _settings, _serializer, _cache);
            string currencyCode = propertyDetails.ISOCurrencyCode;
            string agentCode = MikiSupport.GetAgentCode(currencyCode, propertyDetails, _settings);
            string ivectorReference = propertyDetails.BookingReference.Trim();
            string paxNationality = string.Empty;

            if (!string.IsNullOrEmpty(propertyDetails.ISONationalityCode))
            {
                paxNationality = await _support.TPNationalityLookupAsync(Source, propertyDetails.ISONationalityCode);
            }

            if (string.IsNullOrEmpty(paxNationality))
            {
                paxNationality = _settings.BookingCountryCode(propertyDetails);
            }

            var rooms = new List<Room>();
            int roomCount = 1;
            foreach (var roomDetails in propertyDetails.Rooms)
            {
                string roomTypeCode = roomDetails.ThirdPartyReference.Split('~')[0];
                string rateIdentifier = roomDetails.ThirdPartyReference.Split('~')[2];

                var room = new Room
                {
                    RoomTypeCode = roomTypeCode,
                    RoomNo = roomCount,
                    RateIdentifier = rateIdentifier,
                    RoomTotalPrice = roomDetails.GrossCost,
                };
                
                // add adults names, roomtype will indicate if children / infants included
                var guests = new List<Guest>();
                foreach (var passenger in roomDetails.Passengers)
                {
                    var guest = new Guest
                    {
                        PaxName = new PaxName
                        {
                            Title = passenger.Title != "TBA" ? passenger.Title : "Mr",
                            FirstName = passenger.FirstName,
                            LastName = passenger.LastName
                        }
                    };

                    var type = passenger.PassengerType;
                    if (type == PassengerType.Adult || (type == PassengerType.Child && passenger.Age > 11))
                    {
                        guest.Type = GuestCountType.ADT;
                    }
                    else if(type is PassengerType.Child or PassengerType.Infant)
                    {
                        guest.Type = GuestCountType.CHD;
                        guest.Age = type == PassengerType.Infant ? 3 : passenger.Age;
                    }

                    guests.Add(guest);
                }

                room.Guests = guests.ToArray();
                rooms.Add(room);
                roomCount++;
            }

            var request = new Envelope<HotelBookingRequest> { 
                Body = 
                {
                    Content =
                    {
                        VersionNumber = "7.0",
                        RequestAuditInfo = MikiSupport.BuildRequestAuditInfo(agentCode, password),
                        Booking =
                        {
                            CurrencyCode = currencyCode,
                            PaxNationality = paxNationality,
                            ClientRef = ivectorReference,
                            Items = new [] { new Item 
                                { 
                                    ItemNumber = 1,
                                    ImmediateConfirmationRequired = true,
                                    ProductCode = propertyDetails.TPKey,
                                    LeadPaxName =
                                    {
                                        Title = propertyDetails.Rooms[0].Passengers[0].Title != "TBA" 
                                            ? propertyDetails.Rooms[0].Passengers[0].Title 
                                            : "Mr",
                                        FirstName = propertyDetails.Rooms[0].Passengers[0].FirstName,
                                        LastName = propertyDetails.Rooms[0].Passengers[0].LastName
                                    },
                                    Hotel =
                                    {
                                        StayPeriod =
                                        {
                                            CheckinDate = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                                            NumberOfNights = propertyDetails.Duration
                                        },
                                        Rooms = rooms.ToArray()
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return _serializer.Serialize(request);
        }

        private async Task<XmlDocument> BuildCancellationRequestAsync(string reference, IThirdPartyAttributeSearch searchDetails, string agentCode)
        {
            string password = await MikiSupport.GetPasswordAsync(searchDetails, _settings, _serializer, _cache);
            var request = new Envelope<CancellationRequest>
            {
                Body =
                {
                    Content =
                    {
                        VersionNumber = "7.0",
                        RequestAuditInfo = MikiSupport.BuildRequestAuditInfo(agentCode, password),
                        TourReference = reference
                    }
                }
            };

            return _serializer.Serialize(request);
        }

        private Cancellations CreatePreBookCancellations(Envelope<HotelSearchResponse> response, PropertyDetails propertyDetails)
        {
            var mikiCancellations = new CustomCancellations();

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                var cancellationPolicies = response.Body.Content.Hotels
                    .Where(x => x.ProductCode == propertyDetails.TPKey)
                    .SelectMany(r => r.RoomOptions)
                    .Where(r => r.RoomTypeCode == roomDetails.ThirdPartyReference.Split('~')[0]);

                foreach (var cancellationPolicy in cancellationPolicies)
                {
                    decimal roomPrice = cancellationPolicy.RoomTotalPrice.Price;
                    foreach (var policies in cancellationPolicy.CancellationPolicies)
                    {
                        decimal price = policies.FullStay ? roomPrice : roomPrice/propertyDetails.Duration;
                        mikiCancellations.AddNew(
                            policies.AppliesFrom,
                            price * policies.CancellationCharge.Percentage / 100);
                    }
                }
            }

            return mikiCancellations.ConvertToCancellations();
        }

        private Cancellations CreateBookCancellations(Envelope<HotelBookingResponse> response)
        {
            var mikiCancellations = new CustomCancellations();

            foreach (var node in response.Body.Content.Booking.Items.SelectMany(x => x.Hotel.Rooms))
            {
                decimal roomPrice = node.RoomTotalPrice;
                int stayLength = response.Body.Content.Booking.Items
                    .Select(x => x.Hotel.StayPeriod.NumberOfNights)
                    .First();

                foreach (var cancellationNode in node.CancellationPolicies)
                {
                    decimal price = cancellationNode.FullStay 
                        ? roomPrice 
                        : roomPrice / stayLength;

                    mikiCancellations.AddNew(
                        cancellationNode.AppliesFrom,
                        price * cancellationNode.CancellationCharge.Percentage / 100);
                }
            }

            return mikiCancellations.ConvertToCancellations();
        }

        private class CustomCancellations : List<Cancellation>
        {
            public void AddNew(DateTime dateFrom, decimal amount)
            {
                // first up jink the date, if time=23:59:59 just add a day
                if (dateFrom.ToLongTimeString() == "23:59:59")
                    dateFrom = dateFrom.AddSeconds(1);

                bool bDone = false;
                foreach (var cancellation in this)
                {
                    if (dateFrom != cancellation.DateFrom) continue;

                    cancellation.Amount += amount;
                    bDone = true;
                    break;
                }

                if (!bDone)
                    Add(new Cancellation(dateFrom, amount));
            }

            public Cancellations ConvertToCancellations()
            {
                var cancellations = new Cancellations();

                // order the cancellations first
                Sort(new CancellationComparer());

                // loop through and add the datebands and amount
                for (int i = 0; i <= Count-1; i++)
                {
                    var cancellation = this[i];

                    var lastDate = i < Count - 1 
                        ? this[i + 1].DateFrom.AddDays(-1) 
                        : new DateTime(2099, 12, 31);

                    cancellations.AddNew(cancellation.DateFrom, lastDate, cancellation.Amount);
                }

                // second loop now we need to go through and cumulatively add the previous amounts
                decimal amount = 0;
                foreach (var cancellation in cancellations)
                {
                    decimal useAmount = amount;
                    amount += cancellation.Amount;
                    cancellation.Amount += useAmount;
                }

                return cancellations;
            }
        }

        private class Cancellation
        {
            public DateTime DateFrom;
            public decimal Amount;

            public Cancellation(DateTime dateFrom, decimal amount)
            {
                this.DateFrom = dateFrom;
                this.Amount = amount;
            }
        }

        private class CancellationComparer : IComparer<Cancellation>
        {
            public int Compare(Cancellation x, Cancellation y)
            {
                if (x.DateFrom < y.DateFrom)
                    return -1;
                
                return x.DateFrom > y.DateFrom ? 1 : 0;
            }
        }
    }
}
