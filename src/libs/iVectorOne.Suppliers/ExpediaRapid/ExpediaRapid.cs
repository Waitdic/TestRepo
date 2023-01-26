﻿namespace iVectorOne.Suppliers.ExpediaRapid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using MoreLinq;
    using Newtonsoft.Json;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.ExpediaRapid.RequestConstants;
    using iVectorOne.Suppliers.ExpediaRapid.SerializableClasses;
    using iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Book;
    using iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.BookingItinerary;
    using iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Prebook;
    using iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Search;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Models.Property;

    public class ExpediaRapid : IThirdParty, ISingleSource
    {
        private readonly IExpediaRapidAPI _api;

        private readonly IExpediaRapidSettings _settings;

        private readonly ITPSupport _support;

        private const string ErrataTitle = "Pay at Hotel";

        public ExpediaRapid(IExpediaRapidAPI api, IExpediaRapidSettings settings, ITPSupport support)
        {
            _api = Ensure.IsNotNull(api, nameof(api));
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        public bool SupportsRemarks => true;
        public bool SupportsBookingSearch => false;

        public string Source => ThirdParties.EXPEDIARAPID;

        public bool RequiresVCard(VirtualCardInfo info, string source)
            => throw new NotImplementedException();

        public void EndSession(PropertyDetails propertyDetails)
        {
            return;
        }

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            try
            {
                // retry search to get cancellation and errata
                var searchResponse = await GetPrebookSearchRedoResponseAsync(propertyDetails);

                if (searchResponse is null)
                {
                    throw new Exception("Errors in second search response.");
                }

                var responseRooms = GetPrebookSpecificRoomRates(propertyDetails, searchResponse);

                if (!responseRooms.Any())
                {
                    throw new Exception("Room(s) no longer available. Retry search.");
                }

                propertyDetails.Errata = await GetErrataFromAllRoomsAsync(propertyDetails, responseRooms);

                //Mandatory to display terms and conditions link
                propertyDetails.Errata.AddNew("Terms and Conditions - link", _settings.TermsAndConditionsLink(propertyDetails));

                propertyDetails.Cancellations = GetCancellationsFromAllRooms(propertyDetails, responseRooms);
                propertyDetails.Cancellations.Solidify(SolidifyType.Sum);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook RedoSearch Error", ex.ToString());
                return false;
            }

            var roomPrebookResponses = new List<PrebookResponse>();

            // prebook to get the costs and book link
            try
            {
                var firstRoom = propertyDetails.Rooms.First();

                var prebookResponse = await GetResponseAsync<PrebookResponse>(propertyDetails, firstRoom.ThirdPartyReference.Split('|')[1], RequestMethod.GET, "Prebook PriceCheck ", false);

                if (prebookResponse is null)
                {
                    throw new Exception("Room price check failed.");
                }

                UpdateCostsAtRoomLevel(propertyDetails, prebookResponse);
                GetBookingLink(propertyDetails, firstRoom, prebookResponse);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Price Check Error", ex.ToString());
                return false;
            }

            propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);
            propertyDetails.GrossCost = propertyDetails.Rooms.Sum(r => r.GrossCost);

            if (propertyDetails.Rooms.Any(o => !string.IsNullOrEmpty(o.RateBasis))) 
            {
                propertyDetails.OverrideRateBasis = propertyDetails.Rooms.Where(o => !string.IsNullOrEmpty(o.RateBasis)).First().RateBasis;
                decimal nCommissionPercentage = decimal.Round(propertyDetails.Rooms.Max(o => o.CommissionPercentage), 6);
                if (!decimal.Equals(nCommissionPercentage, propertyDetails.CommissionPercentage)) 
                {
                    propertyDetails.CommissionPercentage = nCommissionPercentage;
                    propertyDetails.CommissionPercentageChange = true;
                }
            }

            return true;
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            try
            {
                var firstRoom = propertyDetails.Rooms.First();
                string bookRequestBody = await BuildBookRequestBodyAsync(propertyDetails);

                var bookResponse = GetResponseAsync<BookResponse>(propertyDetails, propertyDetails.TPRef2, RequestMethod.POST, "Book ", true, bookRequestBody);
                if (await Task.WhenAny(bookResponse, Task.Delay(90000)) == bookResponse)
                {
                    if (bookResponse.Result is null)
                    {
                        throw new ArgumentNullException("bookResponse", "Invalid book response recieved for room.");
                    }

                    propertyDetails.SourceSecondaryReference = bookResponse.Result.Links["retrieve"].HRef;
                    return bookResponse.Result.ItineraryID;
                }
                else
                {
                    var retrieveResponse = await GetResponseAsync<RetrieveResponse>(propertyDetails, propertyDetails.TPRef2, RequestMethod.GET, "Retrieve Book ", true, bookRequestBody);
                    if (retrieveResponse.Message == "Itinerary was not found with provided request.")
                    {
                        await bookResponse;

                        if (bookResponse.Result is null)
                        {
                            throw new ArgumentNullException("bookResponse", "Invalid book response recieved for room.");
                        }

                        propertyDetails.SourceSecondaryReference = bookResponse.Result.Links["retrieve"].HRef;
                        return bookResponse.Result.ItineraryID;
                    }
                    else if (retrieveResponse.Message == "An itinerary already exists with this affiliate reference id.")
                    {
                        throw new ArgumentException("Book response still processing.", "Retrieve Response");
                    }
                    else
                    {
                        propertyDetails.SourceSecondaryReference = retrieveResponse.Links["retrieve"].HRef;
                        return retrieveResponse.ItineraryID;
                    }
                }
            }

            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Error", ex.ToString());
                return "failed";
            }
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            decimal amount = 0m;
            var cancelReferences = new List<string>();
            bool success = true;

            try
            {
                var bookingItineraryResponse1 = await GetResponseAsync<BookingItineraryResponse>(propertyDetails, propertyDetails.SourceSecondaryReference, RequestMethod.GET, "Booking Itinerary - Before Cancel ", true);

                if (bookingItineraryResponse1 is null)
                {
                    throw new ArgumentNullException("bookingItineraryResponse1", "Unable to find booking itinerary for room.");
                }

                cancelReferences = await CancelRoomsAsync(propertyDetails, bookingItineraryResponse1);

                if (propertyDetails.Warnings.Any())
                {
                    throw new Exception("Unable to cancel a room.");
                }

                var bookingItineraryResponse2 = await GetResponseAsync<BookingItineraryResponse>(propertyDetails, propertyDetails.SourceSecondaryReference, RequestMethod.GET, "Booking Itinerary - After Cancel ", true);

                if (bookingItineraryResponse2 is null)
                {
                    throw new ArgumentNullException("bookingItineraryResponse2", "Unable to find booking itinerary for room.");
                }

                if (bookingItineraryResponse2.Rooms.Any(r => r.Status != "canceled"))
                {
                    throw new Exception("Unable to cancel room.");
                }

                var cancelPenalties = bookingItineraryResponse2.Rooms.SelectMany(r => r.Rate.CancelPenalities);

                if (cancelPenalties.Any())
                {
                    amount += cancelPenalties.Where(cp => cp.CancelStartDate < DateTime.Now && cp.CancelEndDate > DateTime.Now).Sum(cp => cp.Amount);

                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancel Error", ex.ToString());
                success = false;
            }

            return new ThirdPartyCancellationResponse()
            {
                Amount = amount,
                CurrencyCode = await _support.TPCurrencyCodeLookupAsync(propertyDetails.Source, propertyDetails.ISOCurrencyCode),
                Success = success,
                CostRecievedFromThirdParty = amount > 0m,
                TPCancellationReference = string.Join("|", cancelReferences)
            };
        }

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            decimal amount = 0m;
            string? currencyCode = null;
            bool success = true;

            try
            {
                var precancelResponse = await GetResponseAsync<BookingItineraryResponse>(propertyDetails, propertyDetails.SourceSecondaryReference, RequestMethod.GET, "Booking Itinerary ", true);

                if (precancelResponse is null)
                {
                    throw new ArgumentNullException("precancelResponse", "There was an error in the precancel response.");
                }

                if ((precancelResponse.AffiliateReferenceID ?? "") != (propertyDetails.BookingReference.Trim() ?? "") && _settings.ValidateAffiliateID(propertyDetails))
                {
                    throw new Exception("Unrecognised precancel response booking reference.");
                }

                var cancelPenalties = precancelResponse.Rooms.SelectMany(r => r.Rate.CancelPenalities);

                if (cancelPenalties.Any())
                {
                    amount += cancelPenalties.Where(cp => cp.CancelStartDate < DateTime.Now && cp.CancelEndDate > DateTime.Now).Sum(cp => cp.Amount);
                    currencyCode = cancelPenalties.Where(cp => cp.CurrencyCode != null).Select(cp => cp.CurrencyCode).First();
                }
            }

            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Precancel Error", ex.ToString());
                success = false;
            }

            return new ThirdPartyCancellationFeeResult()
            {
                Amount = amount,
                CurrencyCode = currencyCode ?? await _support.TPCurrencyCodeLookupAsync(propertyDetails.Source, propertyDetails.ISOCurrencyCode),
                Success = success
            };
        }

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            throw new NotImplementedException();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        private async Task<List<string>> CancelRoomsAsync(PropertyDetails propertyDetails, BookingItineraryResponse bookingItineraryResponse1)
        {
            var cancelReferences = new List<string>();

            foreach (BookingItineraryResponseRoom room in bookingItineraryResponse1.Rooms)
            {
                try
                {
                    string cancelLink = room.Links["cancel"].HRef;

                    string url = BuildDefaultURL(propertyDetails, cancelLink);
                    var request = BuildRequest(propertyDetails, url, "Cancel", RequestMethod.DELETE, true);

                    string cancelResponseString = await _api.GetResponseAsync(propertyDetails, request);
                    int statusCode = (int)request.ResponseStatusCode;

                    if (!string.IsNullOrWhiteSpace(cancelResponseString) && (statusCode != 202 || statusCode != 204))
                    {

                        throw new ArgumentNullException("cancelResponse", "There was an error in the cancel response.");
                    }

                    cancelReferences.Add(propertyDetails.SourceReference);
                }

                catch (Exception ex)
                {
                    propertyDetails.Warnings.AddNew("Cancel Error", ex.ToString());
                    cancelReferences.Add("[Failed]");
                }
            }

            return cancelReferences;
        }

        private void GetBookingLink(PropertyDetails propertyDetails, RoomDetails firstRoom, PrebookResponse prebookResponse)
        {
            if (!prebookResponse.Links.TryGetValue("book", out Link bookLink))
            {
                throw new Exception("Couldn't find booklink for room in prebook response");
            }

            string tpSessionID = firstRoom.ThirdPartyReference.Split('|')[0];
            propertyDetails.TPRef1 = tpSessionID;
            propertyDetails.TPRef2 = bookLink.HRef;
        }

        private void UpdateCostsAtRoomLevel(PropertyDetails propertyDetails, PrebookResponse prebookResponse)
        {
            foreach (RoomDetails room in propertyDetails.Rooms)
            {
                var occupancy = new ExpediaRapidOccupancy(room.Adults, room.ChildAges, room.Infants);

                if (!prebookResponse.OccupancyRoomRates.TryGetValue(occupancy.GetExpediaRapidOccupancy(), out OccupancyRoomRate occupancyRoomRate))
                {
                    throw new Exception("Couldn't find room in prebook response");
                }

                var inclusiveTotal = occupancyRoomRate.OccupancyRateTotals["inclusive"];

                OccupancyRateTotal? marketingFee = null;

                if (occupancyRoomRate.OccupancyRateTotals.ContainsKey("marketing_fee")) 
                {
                    marketingFee = occupancyRoomRate.OccupancyRateTotals["marketing_fee"];
                }

                if (marketingFee != null && marketingFee.TotalInBillableCurrency.Amount > 0)
                {
                    room.LocalCost = inclusiveTotal.TotalInBillableCurrency.Amount;
                    room.GrossCost = inclusiveTotal.TotalInBillableCurrency.Amount;
                    room.RateBasis = "Gross Margin";
                    room.CommissionPercentage = marketingFee.TotalInBillableCurrency.Amount /
                                                    inclusiveTotal.TotalInBillableCurrency.Amount * 100;
                }
                else 
                {
                    room.LocalCost = inclusiveTotal.TotalInRequestCurrency.Amount;
                    room.GrossCost = inclusiveTotal.TotalInRequestCurrency.Amount;
                }
            }
        }

        private async Task<TResponse> GetResponseAsync<TResponse>(
            PropertyDetails propertyDetails,
            string link,
            RequestMethod method,
            string logName,
            bool addCustomerIPHeader,
            string requestBody = null!) where TResponse : IExpediaRapidResponse<TResponse>, new()
        {
            string url = BuildDefaultURL(propertyDetails, link);
            var request = BuildRequest(propertyDetails, url, logName, method, addCustomerIPHeader, requestBody);
            var response = await _api.GetDeserializedResponseAsync<TResponse>(propertyDetails, request);
            return response;
        }

        private async Task<SearchResponse> GetPrebookSearchRedoResponseAsync(PropertyDetails propertyDetails)
        {
            string searchURL = await BuildPrebookSearchURLAsync(propertyDetails);
            var searchRequest = BuildRequest(propertyDetails, searchURL, "Prebook - Redo Search", RequestMethod.GET, false);
            var searchResponse = await _api.GetDeserializedResponseAsync<SearchResponse>(propertyDetails, searchRequest);
            return searchResponse;
        }

        private Cancellations GetCancellationsFromAllRooms(PropertyDetails propertyDetails, List<SearchResponseRoom> responseRooms)
        {
            var cancellations = new Cancellations();

            foreach (RoomDetails room in propertyDetails.Rooms)
            {
                string roomID = room.RoomTypeCode.Split('|')[0];
                string rateID = room.RoomTypeCode.Split('|')[1];
                var roomrate = GetExactRoomRate(responseRooms, roomID, rateID);

                var occuapncy = new ExpediaRapidOccupancy(room.Adults, room.ChildAges, room.Infants);
                var occupancyRoomRate = GetExactOccupancyRoomRate(responseRooms, occuapncy, roomID, rateID);

                if (!roomrate.IsRefundable && !roomrate.CancelPenalities.Any())
                {
                    decimal roomAmount = occupancyRoomRate.OccupancyRateTotals["inclusive"].TotalInRequestCurrency.Amount;
                    cancellations.Add(new Cancellation(DateTime.Now.Date, propertyDetails.ArrivalDate, roomAmount));
                }

                cancellations.AddRange(roomrate.CancelPenalities.Select(cp => BuildCancellation(occupancyRoomRate, cp)));
            }

            return cancellations;
        }

        private Cancellation BuildCancellation(OccupancyRoomRate occupancyRoomRate, CancelPenalty cancelPenalty)
        {
            decimal amount = 0m;

            if (cancelPenalty.Amount != 0m)
            {
                amount += cancelPenalty.Amount;
            }

            if (!string.IsNullOrEmpty(cancelPenalty.Percent))
            {
                decimal percent = cancelPenalty.Percent.Replace("%", "").ToSafeDecimal();
                decimal roomAmount = occupancyRoomRate.OccupancyRateTotals["inclusive"].TotalInRequestCurrency.Amount;
                amount += Math.Round(roomAmount / 100m * percent, 2, MidpointRounding.AwayFromZero);
            }

            if (cancelPenalty.Nights > 0)
            {
                var nightCancellations = occupancyRoomRate.NightlyRates.Take(cancelPenalty.Nights).ToList();
                foreach (List<Rate> night in nightCancellations)
                {
                    foreach (Rate rate in night)
                        amount += rate.Amount;
                }
            }

            var cancellation = new Cancellation(cancelPenalty.CancelStartDate, cancelPenalty.CancelEndDate, amount);

            return cancellation;
        }

        private async Task<Errata> GetErrataFromAllRoomsAsync(PropertyDetails propertyDetails, List<SearchResponseRoom> responseRooms)
        {
            var errata = new Errata();
            string currencyCode = await _support.TPCurrencyCodeLookupAsync(Source, propertyDetails.ISOCurrencyCode);

            var mandatoryFees = new List<OccupancyRateFee>();
            var resortFees = new List<OccupancyRateFee>();
            var mandatoryTaxes = new List<OccupancyRateFee>();
            decimal taxAndServiceFee = 0m;
            decimal salesTax = 0m;
            decimal extraPersonFee = 0m;

            foreach (var room in propertyDetails.Rooms)
            {
                var occuapncy = new ExpediaRapidOccupancy(room.Adults, room.ChildAges, room.Infants);
                string roomID = room.RoomTypeCode.Split('|')[0];
                string rateID = room.RoomTypeCode.Split('|')[1];

                var occupancyRoomRate = GetExactOccupancyRoomRate(responseRooms, occuapncy, roomID, rateID);

                if (occupancyRoomRate.OccupancyRateFees.Any())
                {
                    if (occupancyRoomRate.OccupancyRateFees.TryGetValue("mandatory_fee", out OccupancyRateFee mandatoryFee))
                    {
                        mandatoryFees.Add(mandatoryFee);
                    }

                    if (occupancyRoomRate.OccupancyRateFees.TryGetValue("resort_fee", out OccupancyRateFee resortFee))
                    {
                        resortFees.Add(resortFee);
                    }

                    if (occupancyRoomRate.OccupancyRateFees.TryGetValue("mandatory_tax", out OccupancyRateFee mandatoryTax))
                    {
                        mandatoryTaxes.Add(mandatoryTax);
                    }
                }

                taxAndServiceFee += ExpediaRapidSearch.GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.TaxAndServiceFee);
                salesTax += ExpediaRapidSearch.GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.SalesTax);
                extraPersonFee += ExpediaRapidSearch.GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.ExtraPersonFee);

                if (occupancyRoomRate.StayRates.Any())
                {
                    taxAndServiceFee += ExpediaRapidSearch.GetStayRateFromType(occupancyRoomRate, RateTypes.TaxAndServiceFee);
                    salesTax += ExpediaRapidSearch.GetStayRateFromType(occupancyRoomRate, RateTypes.SalesTax);
                    extraPersonFee += ExpediaRapidSearch.GetStayRateFromType(occupancyRoomRate, RateTypes.ExtraPersonFee);
                }
            }

            static string GetCurrency(IEnumerable<OccupancyRateFee> items) => items.First().TotalInBillableCurrency.CurrencyCode;

            if (mandatoryFees.Any())
                errata.Add(BuildErrata("Mandatory Fee", mandatoryFees.Sum(f => f.TotalInBillableCurrency.Amount), GetCurrency(mandatoryFees)));
            if (resortFees.Any())
                errata.Add(BuildErrata("Resort Fee", resortFees.Sum(f => f.TotalInBillableCurrency.Amount), GetCurrency(resortFees)));
            if (mandatoryTaxes.Any())
                errata.Add(BuildErrata("Mandatory Tax", mandatoryTaxes.Sum(f => f.TotalInBillableCurrency.Amount), GetCurrency(mandatoryTaxes)));
            if (taxAndServiceFee > 0m)
                errata.Add(BuildErrata("Tax and Service Fee", taxAndServiceFee, currencyCode, "Included"));
            if (salesTax > 0m)
                errata.Add(BuildErrata("Sales Tax", salesTax, currencyCode));
            if (extraPersonFee > 0m)
                errata.Add(BuildErrata("Extra Person", extraPersonFee, currencyCode));

            return errata;
        }

        private OccupancyRoomRate GetExactOccupancyRoomRate(List<SearchResponseRoom> responseRooms, ExpediaRapidOccupancy occuapncy, string roomID, string rateID)
        {
            var roomRate = GetExactRoomRate(responseRooms, roomID, rateID);
            var occupancyRoomRate = roomRate.OccupancyRoomRates[occuapncy.GetExpediaRapidOccupancy()];

            return occupancyRoomRate;
        }

        private RoomRate GetExactRoomRate(List<SearchResponseRoom> responseRooms, string roomID, string rateID)
        {
            var responseRoom = responseRooms.First(r => (r.RoomID ?? "") == (roomID ?? ""));
            var roomRate = responseRoom.Rates.First(r => (r.RateID ?? "") == (rateID ?? ""));
            return roomRate;
        }

        private Erratum BuildErrata(string feeName, decimal amount, string currencyCode, string overrideTitle = "")
        {
            var title = ErrataTitle;
            if (!string.IsNullOrEmpty(overrideTitle))
            {
                title = overrideTitle;
            }

            return new Erratum(title, $"{feeName}: {currencyCode}/{amount}");
        }

        private List<SearchResponseRoom> GetPrebookSpecificRoomRates(PropertyDetails propertyDetails, SearchResponse searchResponse)
        {
            var propertyAvail = searchResponse.First(sr => (sr.PropertyID ?? "") == (propertyDetails.TPKey ?? ""));

            if (propertyAvail is null || !propertyAvail.Rooms.Any())
            {
                return null;
            }

            var sameRoomRates = new List<SearchResponseRoom>();

            foreach (var room in propertyDetails.Rooms)
            {
                string roomID = room.RoomTypeCode.Split('|')[0];
                string rateID = room.RoomTypeCode.Split('|')[1];
                string bedGroupID = room.RoomTypeCode.Split('|')[2];

                var matchedRoom = propertyAvail.Rooms.First(r => (r.RoomID ?? "") == (roomID ?? ""));

                if (matchedRoom is null || !matchedRoom.Rates.Any())
                {
                    return null;
                }

                var matchedRate = matchedRoom.Rates.First(r => (r.RateID ?? "") == (rateID ?? ""));

                if (matchedRate is null || matchedRate.BedGroupAvailabilities is null || !matchedRate.BedGroupAvailabilities.ContainsKey(bedGroupID))
                {
                    return null;
                }

                var occupancy = new ExpediaRapidOccupancy(room.Adults, room.ChildAges, room.Infants);

                if (matchedRate.OccupancyRoomRates[occupancy.GetExpediaRapidOccupancy()] is null)
                {
                    return null;
                }

                sameRoomRates.Add(matchedRoom);
            }

            return sameRoomRates;
        }

        private async Task<string> BuildPrebookSearchURLAsync(PropertyDetails propertyDetails)
        {
            var tpKeys = new List<string>() { propertyDetails.TPKey };
            string currencyCode = await _support.TPCurrencyCodeLookupAsync(Source, propertyDetails.ISOCurrencyCode);
            var countryCode = _settings.SourceMarket(propertyDetails);

            if (propertyDetails.SellingCountry != string.Empty)
            {
                countryCode = await _support.TPCountryCodeLookupAsync(Source, propertyDetails.SellingCountry, propertyDetails.AccountID);
            }
            
            var occupancies = propertyDetails.Rooms.Select(r => new ExpediaRapidOccupancy(r.Adults, r.ChildAges, r.Infants));

            return ExpediaRapidSearch.BuildSearchURL(tpKeys, _settings, propertyDetails, propertyDetails.ArrivalDate, propertyDetails.DepartureDate, currencyCode, countryCode, occupancies);
        }

        private Request BuildRequest(
            PropertyDetails propertyDetails,
            string url,
            string logName,
            RequestMethod method,
            bool addCustomerIPHeader,
            string requestBody = "")
        {
            bool useGzip = _settings.UseGZip(propertyDetails);
            string apiKey = _settings.APIKey(propertyDetails);
            string secret = _settings.Secret(propertyDetails);
            string userAgent = _settings.UserAgent(propertyDetails);
            string customerIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].MapToIPv4().ToString();
            string tpRef = propertyDetails.TPRef1;
            string tpSessionID = !string.IsNullOrWhiteSpace(tpRef) ? tpRef.Split('|')[0] : Guid.NewGuid().ToString();
            var headers = new RequestHeaders();

            headers.AddNew(SearchHeaderKeys.CustomerSessionID, tpSessionID);
            headers.Add(ExpediaRapidSearch.CreateAuthorizationHeader(apiKey, secret));

            if (addCustomerIPHeader)
            {
                headers.AddNew(SearchHeaderKeys.CustomerIP, customerIp);
            }

            var request = ExpediaRapidSearch.BuildDefaultRequest(url, method, headers, useGzip, userAgent, requestBody, logName);

            return request;
        }

        private async Task<string> BuildBookRequestBodyAsync(PropertyDetails propertyDetails)
        {
            var bookRequest = new BookRequest()
            {
                AffiliateReferenceId = propertyDetails.BookingReference.Trim(),
                Hold = false,
                Email = propertyDetails.LeadGuestEmail,
                LeadGuestPhone = new Phone(propertyDetails.LeadGuestPhone),
                Rooms = propertyDetails.Rooms.Select((r, i) => CreateBookRequestRoom(propertyDetails, r.Passengers.First(), r,i)).ToList(),
                Payments = new List<Payment>()
                {
                    new Payment()
                    {
                        Type = "affiliate_collect",
                        BillingContact = new BillingContact()
                        {
                            GivenName = propertyDetails.LeadGuestFirstName,
                            FamilyName = propertyDetails.LeadGuestLastName,
                            Address = new SerializableClasses.Book.Address()
                            {
                                Line1 = propertyDetails.LeadGuestAddress1,
                                Line2 = propertyDetails.LeadGuestAddress2,
                                City = propertyDetails.LeadGuestTownCity,
                                StateProvinceCode = propertyDetails.LeadGuestCounty,
                                PostalCode = propertyDetails.LeadGuestPostcode,
                                CountryCode = await _support.TPCountryCodeLookupAsync(Source, propertyDetails.LeadGuestCountryCode, propertyDetails.AccountID)
                            }
                        }
                    }
                }
            };

            return JsonConvert.SerializeObject(bookRequest);
        }

        private bool ValidateBookRequest(PropertyDetails propertyDetails)
        {
            char[] specialChars = new char[] { '<', '>', '(', ')', '&' };
            string bookingReference = propertyDetails.BookingReference.Trim();

            // booking reference should not contain special characters and should not exceed 28 chars
            return !bookingReference.Any(specialChars.Contains) | bookingReference.Count() <= 28;
        }

        private BookRequestRoom CreateBookRequestRoom(PropertyDetails propertyDetails, Passenger firstPasseneger, RoomDetails room,int index)
        {
            if (!ValidateBookRequest(propertyDetails))
            {
                propertyDetails.Warnings.AddNew("Invalid booking reference", "Booking reference should not be more than 28 characters and it should not contain special characters such as following: <, >, (, ) or &");
                throw new Exception("Invalid booking reference");
            }

            return new BookRequestRoom()
            {
                SpecialRequest = room.SpecialRequest,
                GivenName = index == 0 ? propertyDetails.LeadGuestFirstName : firstPasseneger.FirstName,
                FamilyName = index == 0 ? propertyDetails.LeadGuestLastName : firstPasseneger.LastName
            };
        }

        private string BuildDefaultURL(PropertyDetails propertyDetails, string path)
        {
            var uriBuilder = new UriBuilder(_settings.Scheme(propertyDetails), _settings.Host(propertyDetails), -1);

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path", "No link supplied to append to URL.");

            return uriBuilder.Uri.AbsoluteUri.TrimEnd('/') + path;
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            throw new NotImplementedException();
        }
    }
}