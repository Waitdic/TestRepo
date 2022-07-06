namespace ThirdParty.CSSuppliers.Altura
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
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.Models.Altura;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class Altura : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IAlturaSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Altura> _logger;

        public string Source => ThirdParties.ALTURA;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        #endregion

        #region Constructors

        public Altura(
            IAlturaSettings settings,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<Altura> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            //'Check if prebook multirooms is allowed
            if (propertyDetails.Rooms.Count > 1 && !_settings.EnableMultiRoomSearch(propertyDetails))
            {
                propertyDetails.Warnings.AddNew("Altura Exception", "Cannot proceed with Prebook as Multiroom is turned off.");
                return false;
            }

            try
            {
                //'Send a check request for each room  to see if still available 
                foreach (var room in propertyDetails.Rooms)
                {
                    var rateId = room.ThirdPartyReference.Split('|')[1];
                    var sessionId = room.ThirdPartyReference.Split('|')[0];

                    var checkRequest = SetCheckRequest(propertyDetails, rateId, sessionId);
                    var checkResponseXml = await SendRequestAsync(checkRequest, propertyDetails, Constant.LogfilePrebook);
                    var response = _serializer.DeSerialize<AlturaPrebookResponse>(checkResponseXml);

                    //'Fail entire booking if any room is unavailable
                    if (!string.Equals(response?.Response.Result.State ?? "", Constant.StatusAvailable))
                    {
                        return false;
                    }

                    //'Retrieve booking price
                    decimal bookingPrice = response.Response.RateDetails.TotalPrice.ToSafeDecimal() / 100;

                    //'Check if non-refundable
                    var isNonRefundable = string.Equals(response.Response.RateDetails.NoRefundable, "1");

                    if (!isNonRefundable)
                    {
                        // 'set the cancellation charges
                        foreach (var cancellationCharge in response.Response.CancellationPrices)
                        {
                            var timeSpan = new TimeSpan(cancellationCharge.Timeframe.ToSafeInt(), 0, 0, 0);
                            DateTime startDate = propertyDetails.ArrivalDate.Subtract(timeSpan);
                            decimal fee = cancellationCharge.TotalPrice.ToSafeDecimal() / 100;

                            propertyDetails.Cancellations.AddNew(startDate, Constant.DateMax, fee);
                        }
                    }
                    else
                    {
                        propertyDetails.Cancellations.AddNew(DateTime.Now, Constant.DateMax, bookingPrice);
                    }

                    //'Pick up Erratum
                    var errata = response.Response.ContractRemarks.FirstOrDefault() ?? "";
                    if (!string.IsNullOrEmpty(errata))
                    {
                        propertyDetails.Errata.AddNew("Contract Remarks", errata);
                    }
                }

                //'Merge policies
                propertyDetails.Cancellations.Solidify(SolidifyType.Sum);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("PreBookExceptionRS", ex.Message);
                return false;
            }

            return true;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var references = new List<string>();
            var supplierSourceReferences = new List<string>();

            //'Send a book reqeust for each room 
            foreach (var room in propertyDetails.Rooms)
            {
                try
                {
                    //'Check if room has been already booked
                    var roomIndex = propertyDetails.Rooms.IndexOf(room);
                    if (IsRoomBooked(propertyDetails, roomIndex))
                    {
                        references.Add(GetSourceReference(propertyDetails).Split('|')[roomIndex]);
                        supplierSourceReferences.Add(propertyDetails.SupplierSourceReference.Split('|')[roomIndex]);
                        continue;
                    }

                    //'Seperate the session and rate IDs
                    var rateId = room.ThirdPartyReference.Split('|')[1];
                    var sessionId = room.ThirdPartyReference.Split('|')[0];

                    var confirmationRequest = SetConfirmationRequest(propertyDetails, rateId, sessionId, roomIndex);
                    var confirmationResponse = await SendRequestAsync(confirmationRequest, propertyDetails, Constant.LogFileBook);
                    var response = _serializer.DeSerialize<AlturaBookResponse>(confirmationResponse).Response;

                    //'Save confirmationRef else set to fail 
                    if (string.Equals(response.BookingConfirmation.Status, Constant.StatusConfirmed))
                    {
                        var bookingReference = response.BookingConfirmation.Id;
                        references.Add(bookingReference);
                        supplierSourceReferences.Add(bookingReference);
                    }
                    else
                    {
                        references.Add(Constant.FailedReference);
                        supplierSourceReferences.Add(Constant.FailedReference);
                    }
                }
                catch (Exception ex)
                {
                    propertyDetails.Warnings.AddNew("BookException", ex.Message);
                    references.Add(Constant.FailedReference);
                    supplierSourceReferences.Add(Constant.FailedReference);
                }
            }

            propertyDetails.SupplierSourceReference = string.Join("|", supplierSourceReferences);
            var reference = string.Join("|", references);

            //'fail entire booking if one of the request failed
            if (references.Contains(Constant.FailedReference))
            {
                propertyDetails.SourceSecondaryReference = reference;
                return "failed";
            }

            return reference;
        }

        #endregion

        #region Cancellations

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            await GetCancellationCostAsync(propertyDetails);

            var supplierSourceReferences = propertyDetails.SupplierSourceReference.Split('|');
            var cancellationPrices = propertyDetails.TPRef1.Split('|');
            var sessionIds = propertyDetails.TPRef2.Split('|');

            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse
            {
                Success = true
            };

            var cancellationReferences = new List<string>();
            decimal amount = 0M;
            var currencyCodes = new List<string>();

            //'Send a cancellation request for each confirmed booking 
            for (int roomIndex = 0; roomIndex < supplierSourceReferences.Count(); roomIndex++)
            {
                try
                {
                    var cancellationPrice = cancellationPrices[roomIndex].ToSafeDecimal();
                    var sessionId = sessionIds[roomIndex];
                    var supplierSourceReference = supplierSourceReferences[roomIndex];

                    //'Check if booking was confirmed and cancellation request was sucessful
                    if (!Equals(cancellationPrice, Constant.FailedReference)
                        && !string.Equals(sessionId, Constant.FailedReference)
                        && !string.Equals(supplierSourceReference, Constant.FailedReference))
                    {
                        var cancellationRequest = SetCancelConfirmationRequest(propertyDetails, sessionId, supplierSourceReference, cancellationPrice);
                        var cancellationResponse = await SendRequestAsync(cancellationRequest, propertyDetails, Constant.LogFileCancel);
                        var response = _serializer.DeSerialize<AlturaCancellationResponse>(cancellationResponse).Response;

                        if (string.Equals(response.Result.Status, Constant.StatusCancelled))
                        {
                            amount += response.Result.CancellationPrice.ToSafeDecimal() / 100;
                            currencyCodes.Add(response.Result.Currency);
                            cancellationReferences.Add(supplierSourceReference);
                        }
                        else
                        {
                            cancellationReferences.Add(Constant.FailedReference);
                            thirdPartyCancellationResponse.Success = false;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception)
                {
                    cancellationReferences.Add(Constant.FailedReference);
                    thirdPartyCancellationResponse.Success = false;
                }
            }

            //'Check if all currency codes returned are same
            if (currencyCodes.Count() != currencyCodes.Distinct().Count())
            {
                propertyDetails.Warnings.AddNew("Cancellation confirmation exception", $"Different currencies were returned for cancellations, the distinct currency is:: {currencyCodes.Distinct().First()}");
                thirdPartyCancellationResponse.Success = false;
            }

            thirdPartyCancellationResponse.TPCancellationReference = string.Join("|", cancellationReferences);
            thirdPartyCancellationResponse.Amount = amount;
            thirdPartyCancellationResponse.CurrencyCode = currencyCodes.FirstOrDefault();

            return thirdPartyCancellationResponse;
        }

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            var cancellationFeeResult = new ThirdPartyCancellationFeeResult
            {
                Success = true
            };

            var supplierSourceReferences = propertyDetails.SupplierSourceReference.Split('|').ToList();
            var cancellationPrices = new List<string>();

            var sessionIds = new List<string>();
            decimal amount = 0.00M;
            var currencyCodes = new List<string>();

            //'Send a cancellation request for each confirmed booking 
            foreach (string supplierSourceRef in supplierSourceReferences)
            {
                try
                {
                    if (!string.Equals(supplierSourceRef, Constant.FailedReference))
                    {
                        var cancellationCostRequest = SetCancellationRequest(propertyDetails, supplierSourceRef);
                        var cancellationCostResponse = await SendRequestAsync(cancellationCostRequest, propertyDetails, Constant.LogFilePreCancel);

                        var preCancelResponse = _serializer.DeSerialize<AlturaCancellationResponse>(cancellationCostResponse).Response;

                        amount += preCancelResponse.Result.CancellationPrice.ToSafeDecimal() / 100;
                        cancellationPrices.Add($"{amount}");
                        currencyCodes.Add(preCancelResponse.Result.Currency);
                        sessionIds.Add(preCancelResponse.Session.Id);
                    }
                    else
                    {
                        //'Set cacelationfee and session id to failed if booking was not sucessfull
                        cancellationPrices.Add(Constant.FailedReference);
                        sessionIds.Add(Constant.FailedReference);
                        cancellationFeeResult.Success = false;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    propertyDetails.Warnings.AddNew("Cancellation exception", ex.Message);
                    cancellationPrices.Add(Constant.FailedReference);
                    sessionIds.Add(Constant.FailedReference);
                    cancellationFeeResult.Success = false;
                }
            }
            //'Check if all currency codes returned are same
            if (currencyCodes.Count() != currencyCodes.Distinct().Count())
            {
                propertyDetails.Warnings.AddNew("Cancellation exception", $"Different currencies were returned for cancellations, the distinct currency is: {currencyCodes.Distinct().First()}");
                cancellationFeeResult.Success = false;
            }

            cancellationFeeResult.Amount = amount;
            cancellationFeeResult.CurrencyCode = currencyCodes.FirstOrDefault();

            //'Store cancellation prices for each cancellation to be sent for cacellation confirmation request
            propertyDetails.TPRef1 = string.Join("|", cancellationPrices);

            //'Update session ids to be sent for cancellation confirmation request
            propertyDetails.TPRef2 = string.Join("|", sessionIds);

            propertyDetails.CancellationAmount = amount;

            return cancellationFeeResult;
        }

        #endregion

        #region Booking Search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return "";
        }

        #endregion

        #region Booking Status Update

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }

        #endregion

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        #region Helpers

        public AlturaPrebookRequest SetCheckRequest(PropertyDetails propertyDetails, string rateId, string sessionId)
        {
            return new AlturaPrebookRequest
            {
                Request =
                {
                    RequestType = Constant.RequestTypePrebook,
                    Version = Constant.ApiVersion,
                    Session = GetSession(propertyDetails, sessionId),
                    RateId = rateId
                }
            };
        }

        public AlturaBookRequest SetConfirmationRequest(PropertyDetails propertyDetails, string rateId, string sessionId, int roomIndex)
        {
            //'build the request xml
            return new AlturaBookRequest
            {
                Request =
                {
                    RequestType = Constant.RequestTypeBook,
                    Version = Constant.ApiVersion,
                    Session = GetSession(propertyDetails, sessionId),
                    BookingConfirmation =
                    {
                        RateId = rateId,
                        SpecialRequest = propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any() ?
                                $"{string.Join(",", propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Select(x => x.SpecialRequest))} (Room {roomIndex + 1}/{propertyDetails.Rooms.Count})" 
                                : $"(Room {roomIndex + 1}/{propertyDetails.Rooms.Count})"
                    },
                    Guest =
                    {
                            Tittle = propertyDetails.LeadGuestTitle,
                            FirstName = propertyDetails.LeadGuestFirstName,
                            LastName = propertyDetails.LeadGuestLastName,
                            Email = propertyDetails.LeadGuestEmail,
                            Phone = propertyDetails.LeadGuestPhone
                    }
                }
            };
        }

        public AlturaCancellationRequest SetCancellationRequest(PropertyDetails propertyDetails, string bookingId)
        {
            return new AlturaCancellationRequest
            {
                Request =
                {
                    Session = GetSession(propertyDetails),
                    Cancellation =
                    {
                        BookingId = bookingId
                    }
                }
            };
        }

        public AlturaCancellationRequest SetCancelConfirmationRequest(PropertyDetails propertyDetails, string sessionId, string bookingId, decimal cancelationAmount)
        {
            return new AlturaCancellationRequest
            {
                Request =
                {
                    RequestType = Constant.RequestTypeCancellation,
                    Session = GetSession(propertyDetails, sessionId),
                    Cancellation =
                    {
                        BookingId = bookingId,
                        CancellationPrice = cancelationAmount * 100
                    }
                }
            };
        }

        private Session GetSession(PropertyDetails propertyDetails, string sessionId = "")
        {
            return new Session
            {
                Id = sessionId,
                AgencyId = _settings.AgencyId(propertyDetails),
                Password = _settings.Password(propertyDetails),
                ExternalId = _settings.ExternalId(propertyDetails)
            };
        }

        public bool IsRoomBooked(PropertyDetails propertyDetails, int roomIndex)
        {
            var sSourceReference = GetSourceReference(propertyDetails);

            //'Check whether rooms have never been booked
            if (string.IsNullOrEmpty(sSourceReference))
            {
                return false;
            }

            var oSourceReferences = sSourceReference.Split('|');

            //'Check if previous book has been sucessfull
            if (oSourceReferences.Count() == propertyDetails.Rooms.Count())
            {
                return !string.Equals(oSourceReferences[roomIndex], Constant.FailedReference);
            }

            return false;
        }

        public static string GetSourceReference(PropertyDetails propertyDetails)
        {
            return !string.Equals(propertyDetails.SourceReference, "failed")
                        ? propertyDetails.SourceReference
                        : propertyDetails.SourceSecondaryReference;
        }

        public async Task<XmlDocument> SendRequestAsync<T>(T request, PropertyDetails propertyDetails, string logFilename)
        {
            var xmlRequest = _serializer.Serialize(request);

            var webRequest = new Request
            {
                EndPoint = _settings.GenericURL(propertyDetails),
                Method = eRequestMethod.POST,
                Source = Source,
                CreateLog = true,
                LogFileName = logFilename,
                UseGZip = _settings.UseGZip(propertyDetails),
                Param = "xml",
                ContentType = ContentTypes.Application_x_www_form_urlencoded,
            };

            webRequest.SetRequest(xmlRequest);

            await webRequest.Send(_httpClient, _logger);

            //'save the xml for the front end
            if (!string.IsNullOrEmpty(xmlRequest.OuterXml))
            {
                propertyDetails.Logs.AddNew(Source, $"{Source} {logFilename} Request", xmlRequest.OuterXml);
            }
            if (!string.IsNullOrEmpty(webRequest.ResponseLog))
            {
                propertyDetails.Logs.AddNew(Source, $"{Source} {logFilename} Response", webRequest.ResponseLog);
            }

            return webRequest.ResponseXML;
        }

        #endregion
    }
}