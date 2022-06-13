using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Intuitive;
using Intuitive.Helpers.Serialization;
using ThirdParty.Constants;
using ThirdParty.Models;
using ThirdParty.Models.Property.Booking;
using ThirdParty.CSSuppliers.Models.W2M;
using ThirdParty.CSSuppliers.Xml.W2M;
using Log = ThirdParty.Models.Log;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ThirdParty.CSSuppliers.Helpers.W2M
{
    public class W2MHelper
    {
        private readonly SearchRequestBuilder _searchRequestBuilder;
        private readonly SoapRequestXmlBuilder _soapRequestXmlBuilder;
        private readonly IW2MSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public W2MHelper(IW2MSettings settings, ISerializer serializer, HttpClient httpClient, ILogger logger)
        {
            _settings = settings;
            _serializer = serializer;
            _httpClient = httpClient;
            _logger = logger;
            _searchRequestBuilder = new SearchRequestBuilder(settings, serializer, httpClient, logger);
            _soapRequestXmlBuilder = new SoapRequestXmlBuilder(serializer);
        }

        public static decimal GetCancellationCost(decimal price, int duration, PolicyRule policyRule) =>
            policyRule.FixedPrice > 0
                ? policyRule.FixedPrice
                : policyRule.PercentPrice > 0
                    ? policyRule.PercentPrice * price / 100
                    : policyRule.Nights > 0
                        ? price * policyRule.Nights / duration
                        : 0;

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(BaseRequestParameters parameters, List<string> cancellationRefs)
        {
            var cancellationResponse = new ThirdPartyCancellationResponse();

            foreach (var cancellationRef in cancellationRefs)
            {
                var cancellationRequest = _soapRequestXmlBuilder.BuildCancellation(parameters, cancellationRef);
                var webRequest = _searchRequestBuilder.BuildJuniperWebRequest(cancellationRequest, Constants.RequestNames.CancelBooking, parameters, Constants.SoapActions.CancelBooking);

                try
                {
                    await webRequest.Send(_httpClient, _logger);
                    var response = _serializer.DeSerialize<CancelBookingResponse>(_serializer.CleanSoapDocument(webRequest.ResponseString));

                    var error = response.BookingRS.Errors?.Error;
                    var warning = response.BookingRS?.Warnings?.Warning;

                    if (error?.Code == Constants.CancellationResponse.ReservationAlreadyCancelledCode)
                    {
                        cancellationResponse.Success = true;
                    }
                    else if (warning?.Code == Constants.CancellationResponse.CancelledAndCancellationCostRetrieved)
                    {
                        cancellationResponse.Success = true;
                        var cancelInfo = response.BookingRS?.Warnings?.CancelInfo;
                        if (cancelInfo == null) throw new NullReferenceException();
                        cancellationResponse.Amount = cancelInfo.BookingCancelCost;
                        cancellationResponse.CurrencyCode = cancelInfo.BookingCancelCostCurrency;
                    }
                    else
                    {
                        var warningOrError = (error != null ? error.Text : warning?.Text) ?? string.Empty;

                        cancellationResponse.Logs.AddNew(ThirdParties.W2M, "Cancel booking failed", warningOrError);
                        cancellationResponse.Success = false;
                    }
                }
                catch (Exception ex)
                {
                    cancellationResponse.Logs.AddNew(ThirdParties.W2M, "Cancel booking exception", ex.ToString());
                    cancellationResponse.Success = false;
                }
                finally
                {
                    cancellationResponse.Logs.AddNew(ThirdParties.W2M, "Cancel booking request", cancellationRequest);
                    cancellationResponse.Logs.AddNew(ThirdParties.W2M, "Cancel booking response", webRequest?.ResponseString ?? string.Empty);
                }
            }

            return cancellationResponse;
        }

        public async Task<PreBookResult> PreBookAsync(PreBookRequestParameters parameters, List<RoomDetails> rooms)
        {
            var preBookResult = new PreBookResult();

            try
            {
                foreach (var room in rooms)
                {
                    var ratePlanCode = room.ThirdPartyReference;

                    var availabilityRequestString = _soapRequestXmlBuilder.BuildAvailabilityCheck(parameters, ratePlanCode);
                    preBookResult.Logs.Add(AvailabilityRequestLog(availabilityRequestString));

                    var responseString = await _searchRequestBuilder.GetAvailabilityCheckResponseAsync(
                        availabilityRequestString,
                        parameters.BaseRequestParameters);

                    preBookResult.Logs.Add(AvailabilityResponseLog(responseString));

                    var response = _serializer.DeSerialize<CheckAvailabilityResponse>(_serializer.CleanSoapDocument(responseString));
                    if (response?.CheckAvailRS == null)
                    {
                        preBookResult.Warnings.Add($"Error: RatePlanCode {ratePlanCode})!", "Failed to deserialise response");
                        preBookResult.Success = false;
                        return preBookResult;
                    }

                    if (response.CheckAvailRS.Errors != null)
                    {
                        preBookResult.Warnings.Add($"Error: RatePlanCode {ratePlanCode}!", response.CheckAvailRS.Errors.Error.Text);
                        preBookResult.Success = false;
                        return preBookResult;
                    }

                    UpdateRoom(room, response, ratePlanCode);

                    if (response.CheckAvailRS.Warnings?.Warning?.Code == Constants.HotelAvailCheck.PriceChangedWarning)
                    {
                        ratePlanCode = response.CheckAvailRS.Results.HotelResult.HotelOptions.First().RatePlanCode;
                        preBookResult.Warnings.Add($"Warning: RatePlanCode {ratePlanCode}!", "Price changed");
                    }

                    var bookingRulesRequestString = _soapRequestXmlBuilder.BuildBookingRules(parameters, ratePlanCode);

                    var bookingRulesRequest = _searchRequestBuilder.BuildJuniperWebRequest(bookingRulesRequestString, Constants.RequestNames.HotelBookingRules,
                        parameters.BaseRequestParameters, Constants.SoapActions.HotelBookingRules);

                    preBookResult.Logs.Add(BookingRulesRequestLog(bookingRulesRequestString));

                    await bookingRulesRequest.Send(_httpClient, _logger);
                    var bookingRulesResponseString = bookingRulesRequest.ResponseString;

                    preBookResult.Logs.Add(BookingRulesResponseLog(bookingRulesResponseString));

                    var bookingRulesResponse = _serializer.DeSerialize<HotelBookingRulesResponse>(_serializer.CleanSoapDocument(bookingRulesResponseString));

                    if (bookingRulesResponse.BookingRulesRS.Errors != null)
                    {
                        preBookResult.Warnings.Add($"Error: RatePlanCode {ratePlanCode}!", bookingRulesResponse.BookingRulesRS.Errors.Error.Text);
                        preBookResult.Success = false;
                        return preBookResult;
                    }

                    AddCancellations(preBookResult, bookingRulesResponse, room.LocalCost, parameters.Duration, parameters.ArrivalDate);

                    var result = bookingRulesResponse.BookingRulesRS?.Results?.HotelResult;
                    if (result == null)
                    {
                        preBookResult.Warnings.Add($"Booking Rules Error: RatePlanCode {ratePlanCode}!", "No Hotel Results");
                        preBookResult.Success = false;
                        return preBookResult;
                    }

                    AddErrata(result, preBookResult);

                    string bookingCodeText = result.HotelOptions?.First()?.BookingCode.Text ?? string.Empty;
                    if (!string.IsNullOrEmpty(bookingCodeText))
                        preBookResult.BookingCodes.Add(bookingCodeText);
                }

                preBookResult.Success = true;
            }
            catch (Exception ex)
            {
                preBookResult.Logs.Add(PreBookExceptionLog(ex));
                preBookResult.Success = false;
            }

            return preBookResult;
        }

        private static void AddErrata(HotelResult result, PreBookResult preBookResult)
        {
            if (result.HotelOptions?.First()?.OptionalElements.Comments.Comment == null)
            {
                return;
            }

            foreach (var comment in result.HotelOptions.First().OptionalElements.Comments.Comment)
            {
                preBookResult.Errata.Add(new PreBookErrata
                {
                    Title = Constants.ErrataTitle,
                    Text = comment.Text
                });
            }
        }

        private static void UpdateRoom(RoomDetails room, CheckAvailabilityResponse response, string ratePlanCode)
        {
            var totalFixAmounts = response.CheckAvailRS.Results.HotelResult.HotelOptions.First().Prices.Price.TotalFixAmounts;

            room.LocalCost = totalFixAmounts.Gross;
            room.GrossCost = totalFixAmounts.Gross;
            room.ThirdPartyReference = ratePlanCode;
        }

        public async Task<BookResult> BookAsync(PropertyDetails propertyDetails)
        {
            var startDate = propertyDetails.ArrivalDate.ToString(Constants.DateTimeFormat);
            var endDate = propertyDetails.DepartureDate.ToString(Constants.DateTimeFormat);
            var bookingCodes = propertyDetails.TPRef1.Split(Constants.BookingCodesSeparator).ToList();

            var leadGuest = new LeadGuest
            {
                Address = propertyDetails.LeadGuestAddress1,
                City = propertyDetails.LeadGuestTownCity,
                Country = propertyDetails.LeadGuestBookingCountry,
                Nationality = _settings.DefaultNationality(propertyDetails),
                PassportNumber = propertyDetails.PassportNumber,
                Email = propertyDetails.LeadGuestEmail,
                Phone = propertyDetails.LeadGuestPhone,
                PostCode = propertyDetails.LeadGuestPostcode
            };

            var parameters = new BaseRequestParameters
            {
                Username = _settings.Username(propertyDetails),
                Password = _settings.Password(propertyDetails),
                Language = _settings.LangID(propertyDetails),
                Endpoint = _settings.BookUrl(propertyDetails),
                SoapPrefix = _settings.SoapActionPrefix(propertyDetails),
                CreateLogs = propertyDetails.CreateLogs
            };

            var hotelRequest = propertyDetails.BookingComments.FirstOrDefault()?.Text ?? string.Empty;

            var bookRequestParameters = new BookRequestParameters(parameters,
                propertyDetails.BookingReference,
                startDate, endDate, hotelRequest, propertyDetails.TPKey,
                propertyDetails.CurrencyCode, bookingCodes, leadGuest);

            var rooms = propertyDetails.Rooms;

            //BookRequestParameters parameters, List<RoomDetails> rooms
            var bookResult = new BookResult();
            var bookFailure = false;

            for (int i = 0; i < rooms.Count; i++)
            {
                var room = rooms[i];
                var bookingCode = bookRequestParameters.BookingCodes[i];

                var searchString = _soapRequestXmlBuilder.BuildHotelBookingRequest(bookRequestParameters, bookingCode, room);
                var request = _searchRequestBuilder.BuildJuniperWebRequest(searchString, Constants.RequestNames.HotelBooking, bookRequestParameters.BaseParameters, Constants.SoapActions.HotelBooking);

                bookResult.Logs.Add(BookingRequestLog(request.RequestString));

                await request.Send(_httpClient, _logger);

                bookResult.Logs.Add(BookingResponseLog(request.ResponseString));

                var hotelBookingResponse = _serializer.DeSerialize<HotelBookingResponse>(_serializer.CleanSoapDocument(request.ResponseString));

                if (hotelBookingResponse.BookingRS.Errors != null)
                {
                    var errorText = hotelBookingResponse.BookingRS.Errors.Error.Text;
                    propertyDetails.Logs.AddNew(ThirdParties.W2M, $"Third Party / Book Failed", errorText);
                    bookResult.Warnings.Add("Book error", hotelBookingResponse.BookingRS.Errors.Error.Text);
                    bookResult.Success = false;

                    bookFailure = true;
                    bookResult.References.Add($"Room {i + 1} Failed");

                    continue;
                }

                var reservation = hotelBookingResponse.BookingRS.Reservations.Reservation;
                bookResult.References.Add(reservation.Locator);
            }

            bookResult.Success = !bookFailure;
            return bookResult;
        }

        internal static void AddCancellations(PreBookResult preBookResult, HotelBookingRulesResponse response,
            decimal roomCost, int duration, DateTime arrivalDate)
        {
            foreach (var rule in response.BookingRulesRS.Results.HotelResult.HotelOptions.First().CancellationPolicy.PolicyRules.RuleList)
            {
                preBookResult.Cancellations.Add(new PreBookCancellation
                {
                    StartDate = rule.DateFrom != null
                        ? DateTime.ParseExact(rule.DateFrom, Constants.DateTimeFormat, CultureInfo.InvariantCulture)
                        : DateTime.Now.Date,
                    RuleEndDate = rule.DateTo != null
                        ? DateTime.ParseExact(rule.DateTo, Constants.DateTimeFormat, CultureInfo.InvariantCulture).AddDays(-1)
                        : arrivalDate,
                    Amount = GetCancellationCost(roomCost, duration, rule)
                });
            }
        }

        #region Logs
        private static BookLog AvailabilityRequestLog(string request) =>
            new BookLog { Source = ThirdParties.W2M, Title = "W2M Hotel price check Request", Log = request };

        private static BookLog AvailabilityResponseLog(string response) =>
            new BookLog { Source = ThirdParties.W2M, Title = "W2M Hotel price check Response", Log = response };

        private static BookLog PreBookExceptionLog(Exception ex) =>
            new BookLog { Source = ThirdParties.W2M, Title = "W2M Prebook Exception", Log = ex.ToString() };

        private static BookLog BookingRulesResponseLog(string bookingRulesResponseString) =>
            new BookLog { Source = ThirdParties.W2M, Title = "W2M Hotel Booking Rules Response", Log = bookingRulesResponseString };

        private static BookLog BookingRulesRequestLog(string bookingRulesString) =>
            new BookLog { Source = ThirdParties.W2M, Title = "W2M Hotel Booking Rules Request", Log = bookingRulesString };

        private static BookLog BookingResponseLog(string responseString) =>
            new BookLog { Source = ThirdParties.W2M, Title = "W2M Hotel Booking Response", Log = responseString };

        private static BookLog BookingRequestLog(string searchString) =>
            new BookLog { Source = ThirdParties.W2M, Title = "W2M Hotel Booking Request", Log = searchString };

        #endregion*/
    }

    internal static class LogListExtension
    {
        public static void AddNew(this List<Log> logs, string source, string title, string text)
        {
            logs.Add(new Log { Source = source, Title = title, Text = text });
        }
    }
}
