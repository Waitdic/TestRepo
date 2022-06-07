namespace ThirdParty.CSSuppliers.BedsWithEase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.CSSuppliers.BedsWithEase.Models;
    using ThirdParty.CSSuppliers.BedsWithEase.Models.Common;

    public class BedsWithEase : IThirdParty
    {
        private readonly IBedsWithEaseSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BedsWithEase> _logger;

        public BedsWithEase(IBedsWithEaseSettings settings, ISerializer serializer, HttpClient httpClient, ILogger<BedsWithEase> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public string Source => ThirdParties.BEDSWITHEASE;

        public bool SupportsRemarks => false;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        public bool PreBook(PropertyDetails propertyDetails)
        {
            bool bSuccess = true;
            try
            {
                // SessionID has been passed into the room tp reference field via XSL 
                propertyDetails.TPRef1 = propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[3];
                var reservedBookCodes = new List<string>();
                var sbErrata = new StringBuilder();

                // Send reservation requests and store returned errata and reservation book codes
                foreach (var xmlRequest in BuildReservationRequests(propertyDetails))
                {
                    var reserveResult = BedsWithEaseHelper.SendRequest(propertyDetails, xmlRequest, _settings.SOAPHotelReservation(propertyDetails), _settings, _httpClient, _logger);

                    var response = _serializer.DeSerialize<Envelope<HotelReservationResponse>>(reserveResult.Response).Body.Content;

                    string bookCode = response.RsReservation.BookCode;
                    reservedBookCodes.Add(bookCode);

                    sbErrata.AppendLine(response.RsReservation.Erratum);
                    sbErrata.AppendLine();

                    // Send CancellationInfoRequest - one per reservation request
                    GetPrebookCancellationDetails(propertyDetails, bookCode);

                    if (reserveResult.Request != null)
                    {
                        propertyDetails.Logs.AddNew(ThirdParties.BEDSWITHEASE, "BedsWithEase Pre-Book Request", reserveResult.Request);
                    }

                    if (reserveResult.Response != null)
                    {
                        propertyDetails.Logs.AddNew(ThirdParties.BEDSWITHEASE, "BedsWithEase Pre-Book Response", reserveResult.Response);
                    }
                }

                // Store reserved book codes in TPRef2 Property of property details for use in buildreservationconfirmation
                propertyDetails.TPRef2 = string.Join("|", reservedBookCodes);
                string sErrata = sbErrata.ToString();
                if (!ReferenceEquals(sErrata, ""))
                {
                    propertyDetails.Errata.AddNew("Important Information", sErrata);
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBookExceptionRS", ex.ToString());
                bSuccess = false;
            }

            return bSuccess;
        }

        private void GetPrebookCancellationDetails(PropertyDetails propertyDetails, string bookCode)
        {
            Result? result = null;
            try
            {
                result = BedsWithEaseHelper.SendRequest(
                    propertyDetails,
                    BuildPrebookCancellationInfoRequest(propertyDetails, bookCode),
                    _settings.SOAPCancellationInfo(propertyDetails),
                    _settings,
                    _httpClient,
                    _logger);

                var response = _serializer.DeSerialize<Envelope<CancellationInfoResponse>>(result.Response).Body.Content;

                if (result.Response != null)
                {
                    AddPrebookCancellationCosts(propertyDetails, response);
                    AddPrebookCancellationErrata(propertyDetails, response);
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("CancellationCost Exception", ex.ToString());
            }

            // Add the xml to the booking
            if (result?.Request != null)
            {
                propertyDetails.Logs.AddNew(ThirdParties.BEDSWITHEASE, "Prebook CancellationInfo Request", result.Request);
            }

            if (result?.Response != null)
            {
                propertyDetails.Logs.AddNew(ThirdParties.BEDSWITHEASE, "Prebook CancellationInfo Response", result.Response);
            }
        }

        private static void AddPrebookCancellationErrata(PropertyDetails propertyDetails, CancellationInfoResponse response)
        {
            foreach (var cancellationNote in response.CancellationInfo.CancellationNotes)
            {
                propertyDetails.Errata.AddNew(cancellationNote.Title, cancellationNote.Text);
            }
        }

        private static void AddPrebookCancellationCosts(PropertyDetails propertyDetails, CancellationInfoResponse response)
        {
            if (response.Errors.Any() || !response.CancellationInfo.Supported)
                return;

            string cancellationFeeAmount = response.CancellationInfo.CancellationFeeAmount;

            if (response.CancellationInfo.CancellationPolicies.Any())
            {
                foreach (var cancellationPolicy in response.CancellationInfo.CancellationPolicies)
                {
                    string costingRule = cancellationPolicy.HowRule.ToUpper();
                    decimal amount = cancellationPolicy.Amount.ToSafeDecimal();
                    int fromDays = cancellationPolicy.FromDays.ToSafeInt();
                    int toDays = cancellationPolicy.ToDays.ToSafeInt();

                    var startDate = propertyDetails.ArrivalDate.AddDays(-toDays);
                    var endDate = propertyDetails.ArrivalDate.AddDays(-fromDays);

                    if (costingRule == "PERCENT")
                    {
                        amount = amount / 100 * propertyDetails.GrossCost;
                        // Else "FLAT" Rate
                    }

                    propertyDetails.Cancellations.Add(new Cancellation(startDate, endDate, Math.Round(amount, 2)));
                }
            }
            else if (!string.IsNullOrEmpty(cancellationFeeAmount))
            {
                propertyDetails.Cancellations.Add(new Cancellation(DateTime.Now, DateTime.Now,
                    cancellationFeeAmount.ToSafeDecimal()));
            }
        }

        private XmlDocument BuildPrebookCancellationInfoRequest(PropertyDetails propertyDetails, string bookCode)
        {
            var envelope = new Envelope<CancellationInfoRequest>
            {
                Body =
                {
                    Content =
                    {
                        SessionId = propertyDetails.TPRef1,
                        OperatorCode = _settings.OperatorCode(propertyDetails),
                        BookCodes = new[] { bookCode }
                    }
                }
            };

            return _serializer.Serialize(envelope);
        }

        private IEnumerable<XmlDocument> BuildReservationRequests(PropertyDetails propertyDetails)
        {
            return propertyDetails.Rooms.Select(
                    roomDetails => new Envelope<HotelReservationRequest>
                    {
                        Body =
                        {
                            Content =
                            {
                                SessionId = propertyDetails.TPRef1,
                                BookCode = roomDetails.ThirdPartyReference.Split('|')[0]
                            }
                        }
                    })
                .Select(envelope => _serializer.Serialize(envelope)).ToList();
        }

        public string Book(PropertyDetails propertyDetails)
        {
            string sReference = string.Empty;
            Result? oResult = null;

            try
            {
                oResult = BedsWithEaseHelper.SendRequest(
                    propertyDetails,
                    BuildReservationConfirmation(propertyDetails),
                    _settings.SOAPReservationConfirmation(propertyDetails),
                    _settings,
                    _httpClient,
                    _logger);

                var confirmationResponse = _serializer.DeSerialize<Envelope<ConfirmationResponse>>(oResult.Response).Body.Content;

                if (confirmationResponse.Errors.Any())
                {
                    sReference = "failed";
                }
                else
                {
                    foreach (var confirmation in confirmationResponse.Confirmations)
                    {
                        if (!string.IsNullOrEmpty(confirmation.Failed))
                        {
                            sReference = "failed";
                            break;
                        }

                        sReference += $"{confirmation.Confirmed.BookingReference}|";
                    }
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("BookException", ex.ToString());
                sReference = "failed";
            }

            sReference = sReference.TrimEnd('|');

            if (oResult?.Request != null)
            {
                propertyDetails.Logs.AddNew(ThirdParties.BEDSWITHEASE, "Book Request", oResult.Request);
            }

            if (oResult?.Response != null)
            {
                propertyDetails.Logs.AddNew(ThirdParties.BEDSWITHEASE, "Book Response", oResult.Response);
            }

            BedsWithEaseHelper.EndSession(propertyDetails, _settings, _serializer, _httpClient, _logger);

            return sReference;
        }

        private XmlDocument BuildReservationConfirmation(PropertyDetails propertyDetails)
        {
            // bookcodes were concatenated with separators into TPRef2 in prebook
            var oReservedBookCodes = new List<string>();
            oReservedBookCodes.AddRange(propertyDetails.TPRef2.Split('|'));

            var envelope = 
                new Envelope<ConfirmationRequest>
                {
                    Body =
                    {
                        Content =
                        {
                            SessionId = propertyDetails.TPRef1,
                        }
                    }
                };

            var confirmationRequest = envelope.Body.Content;

            foreach (string bookCode in oReservedBookCodes)
            {
                var reserveGroup = new ReservedGroup
                {
                    BookCodes = new[] {bookCode},
                    ErrataAccepted =
                    {
                        Accepted = { Value = string.IsNullOrEmpty(propertyDetails.TPRef2) ? "false" : "true" }
                    }
                };

                var roomDetails = propertyDetails.Rooms[oReservedBookCodes.IndexOf(bookCode)];

                if (roomDetails.Passengers.Count > 0)
                {
                    int passengerRecordNumber = 1;

                    foreach (var passenger in roomDetails.Passengers)
                    {
                        if (DateTime.Compare(passenger.DateOfBirth, new DateTime(1900, 1, 1)) == -1)
                        {
                            if (passenger.Age != default && passenger.Age > 0)
                            {
                                var dateOfBirth = propertyDetails.ArrivalDate.AddYears(-passenger.Age);
                                passenger.DateOfBirth = dateOfBirth;
                            }
                            else
                            {
                                var dateOfBirth = propertyDetails.ArrivalDate.AddYears(-40);
                                passenger.DateOfBirth = dateOfBirth;
                            }
                        }

                        reserveGroup.Passengers.Add(
                            new Models.Common.Passenger
                            {
                                PassengerRecordNumber = passengerRecordNumber,
                                PersonAgeCode = passenger.PassengerType switch
                                {
                                    PassengerType.Adult => "A",
                                    PassengerType.Child => "C",
                                    PassengerType.Infant => "I",
                                    _ => throw new ArgumentOutOfRangeException()
                                },
                                Title = BedsWithEaseHelper.ConvertTitleFormat(passenger.Title),
                                FirstName = passenger.FirstName,
                                LastName = passenger.LastName,
                                BirthDate = passenger.DateOfBirth.ToString("yyyy-MM-dd")
                            }
                        );

                        ++passengerRecordNumber;
                    }
                }

                confirmationRequest.BookCodesToConfirm.ReservedGroup.Add(reserveGroup);
            }

            confirmationRequest.AgencyAddress.AddressLine1 = _settings.AgencyAddressLine1(propertyDetails);
            confirmationRequest.AgencyAddress.AddressLine2 = _settings.AgencyAddressLine2(propertyDetails);

            confirmationRequest.AgencyReference = propertyDetails.BookingReference;

            return _serializer.Serialize(envelope);
        }

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails)
        {
            var response = new ThirdPartyCancellationResponse();
            bool success = false;
            Result? result = null;

            try
            {
                propertyDetails.TPRef1 = BedsWithEaseHelper.GetSessionId(propertyDetails, _settings, _serializer, _httpClient, _logger);
                result = BedsWithEaseHelper.SendRequest(
                    propertyDetails,
                    BuildCancelBookingRequest(propertyDetails),
                    _settings.SOAPCancelBooking(propertyDetails),
                    _settings,
                    _httpClient,
                    _logger);

                var cancellationResponse = _serializer.DeSerialize<Envelope<CancellationResponse>>(result.Response).Body.Content;

                if (!cancellationResponse.Errors.Any())
                {
                    if (cancellationResponse.Cancelled.SupplierCclStatus == "Confirmed")
                    {
                        success = true;
                        var cancelled = cancellationResponse.Cancelled;
                        response.Amount = cancelled.Price.ToSafeDecimal();
                        response.CurrencyCode = cancelled.CurrencyCode;
                        response.TPCancellationReference = propertyDetails.SourceReference;
                    }
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
            }
            finally
            {
                if (result?.Request != null)
                {
                    propertyDetails.Logs.AddNew(ThirdParties.BEDSWITHEASE, "Cancel Booking Request", result.Request);
                }

                if (result?.Response != null)
                {
                    propertyDetails.Logs.AddNew(ThirdParties.BEDSWITHEASE, "Cancel Booking Response", result.Response);
                }

                BedsWithEaseHelper.EndSession(propertyDetails, _settings, _serializer, _httpClient, _logger);
            }

            response.Success = success;
            return response;
        }

        private XmlDocument BuildCancelBookingRequest(PropertyDetails propertyDetails)
        {
            var envelope = new Envelope<CancellationRequest>
            {
                Body =
                {
                    Content =
                    {
                        SessionId = propertyDetails.TPRef1,
                        OperatorCode = _settings.OperatorCode(propertyDetails),
                        BookingReference = propertyDetails.SourceReference
                    }
                }
            };

            return _serializer.Serialize(envelope);
        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails)
        {
            Result? result = null;
            var feeResult = new ThirdPartyCancellationFeeResult();
            feeResult.Success = false;
            try
            {
                // Store sessionid on the propertybooking for the book
                propertyDetails.TPRef1 = BedsWithEaseHelper.GetSessionId(propertyDetails, _settings, _serializer, _httpClient, _logger);
                result = BedsWithEaseHelper.SendRequest(
                    propertyDetails,
                    BuildCancelationInfoRequest(propertyDetails),
                    _settings.SOAPCancellationInfo(propertyDetails),
                    _settings,
                    _httpClient,
                    _logger);

                var cancellationInfoResponse = _serializer.DeSerialize<Envelope<CancellationInfoResponse>>(result.Response).Body.Content;

                if (!cancellationInfoResponse.Errors.Any())
                {
                    var cancellationInfo = cancellationInfoResponse.CancellationInfo;
                    if (cancellationInfo.Supported.ToSafeBoolean())
                    {
                        feeResult.Success = true;
                        feeResult.Amount = string.IsNullOrEmpty(cancellationInfo.CancellationFeeAmount) ? 0 : cancellationInfo.CancellationFeeAmount.ToSafeDecimal();
                        feeResult.CurrencyCode = cancellationInfo.CurrencyCode;
                    }
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("CancellationCost Exception", ex.ToString());
            }
            finally
            {
                // Add the xml to the booking
                if (result?.Request != null)
                {
                    propertyDetails.Logs.AddNew(ThirdParties.BEDSWITHEASE, "CancellationInfo Request", result.Request);
                }

                if (result?.Response != null)
                {
                    propertyDetails.Logs.AddNew(ThirdParties.BEDSWITHEASE, "CancellationInfo Response", result.Response);
                }

                BedsWithEaseHelper.EndSession(propertyDetails, _settings, _serializer, _httpClient, _logger);
            }

            return feeResult;
        }

        private XmlDocument BuildCancelationInfoRequest(PropertyDetails propertyDetails)
        {
            var envelope = new Envelope<CancellationInfoRequest>
            {
                Body =
                {
                    Content =
                    {
                        SessionId = propertyDetails.TPRef1,
                        OperatorCode = _settings.OperatorCode(propertyDetails),
                        BookingReference = propertyDetails.SourceReference
                    }
                }
            };

            return _serializer.Serialize(envelope);
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

        public void EndSession(PropertyDetails propertyDetails)
        {

        }
    }
}