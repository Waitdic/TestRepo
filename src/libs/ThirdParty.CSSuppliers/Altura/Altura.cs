namespace ThirdParty.CSSuppliers.Altura
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.CSSuppliers.Models.Altura;

    public class Altura : IThirdParty
    {
        #region "Properties"

        private readonly IAlturaSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Altura> _logger;

        public string Source { get; set; } = ThirdParties.ALTURA;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info)
        {
            return false;
        }

        #endregion

        #region "Constructors"

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

        #region "PreBook"

        public bool PreBook(PropertyDetails oPropertyDetails)
        {
            //'Check if prebook multirooms is allowed
            if (oPropertyDetails.Rooms.Count > 1 && !_settings.SplitMultiRoom(oPropertyDetails))
            {
                oPropertyDetails.Warnings.AddNew("Altura Exception", "Cannot proceed with Prebook as Multiroom is turned off.");
                return false;
            }

            try
            {
                //'Send a check request for each room  to see if still available 
                foreach (var oRoom in oPropertyDetails.Rooms)
                {
                    var sRateId = oRoom.ThirdPartyReference.Split('|')[1];
                    var sSessionId = oRoom.ThirdPartyReference.Split('|')[0];

                    var checkRequest = SetCheckRequest(oPropertyDetails, sRateId, sSessionId);
                    var oCheckResponseXML = SendRequest(checkRequest, oPropertyDetails, Constant.LogfilePrebook);
                    var oResponse = _serializer.DeSerialize<AlturaPrebookResponse>(oCheckResponseXML);

                    //'Fail entire booking if any room is unavailable
                    if (!string.Equals(oResponse?.Response.Result.State ?? "", Constant.StatusAvailable))
                    {
                        return false;
                    }
                    //'Retrieve booking price
                    decimal dBookingPrice = SafeTypeExtensions.ToSafeDecimal(oResponse.Response.RateDetails.TotalPrice) / 100;

                    //'Check if non-refundable
                    var bIsNoNRefundable = string.Equals(oResponse.Response.RateDetails.NoRefundable, "1");

                    if (!bIsNoNRefundable)
                    {
                        // 'set the cancellation charges
                        foreach (var oCancellationCharge in oResponse.Response.CancellationPrices)
                        {
                            TimeSpan oTimeSpan = new TimeSpan(SafeTypeExtensions.ToSafeInt(oCancellationCharge.Timeframe), 0, 0, 0);
                            DateTime dStartDate = oPropertyDetails.ArrivalDate.Subtract(oTimeSpan);
                            decimal nFee = SafeTypeExtensions.ToSafeDecimal(oCancellationCharge.TotalPrice) / 100;

                            oPropertyDetails.Cancellations.AddNew(dStartDate, Constant.DateMax, nFee);
                        }
                    }
                    else
                    {
                        oPropertyDetails.Cancellations.AddNew(DateTime.Now, Constant.DateMax, dBookingPrice);
                    }

                    //'Pick up Erratum
                    var sErrata = oResponse.Response.ContractRemarks.FirstOrDefault() ?? "";
                    if (!string.IsNullOrEmpty(sErrata))
                    {
                        oPropertyDetails.Errata.AddNew("Contract Remarks", sErrata);
                    }
                }
                //'Merge policies
                oPropertyDetails.Cancellations.Solidify(SolidifyType.Sum);
            }
            catch (Exception ex)
            {
                oPropertyDetails.Warnings.AddNew("PreBookExceptionRS", ex.Message);
                return false;
            }

            return true;
        }

        #endregion

        #region "Book"
        public string Book(PropertyDetails oPropertyDetails)
        {
            var oReferences = new List<string>();
            var oSupplierSourceReferences = new List<string>();

            //'Send a book reqeust for each room 
            foreach (var oRoom in oPropertyDetails.Rooms)
            {
                try
                {
                    //'Check if room has been already booked
                    var iRoomIndex = oPropertyDetails.Rooms.IndexOf(oRoom);
                    if (IsRoomBooked(oPropertyDetails, iRoomIndex))
                    {
                        oReferences.Add(GetSourceReference(oPropertyDetails).Split('|')[iRoomIndex]);
                        oSupplierSourceReferences.Add(oPropertyDetails.SupplierSourceReference.Split('|')[iRoomIndex]);
                        continue;
                    }

                    //'Seperate the session and rate IDs
                    var sRateId = oRoom.ThirdPartyReference.Split('|')[1];
                    var sSessionId = oRoom.ThirdPartyReference.Split('|')[0];

                    var oConfirmationRequest = SetConfirmationRequest(oPropertyDetails, sRateId, sSessionId, iRoomIndex);
                    var oConfirmationResponse = SendRequest(oConfirmationRequest, oPropertyDetails, Constant.LogFileBook);
                    var oResponse = _serializer.DeSerialize<AlturaBookResponse>(oConfirmationResponse).Response;

                    //'Save confirmationRef else set to fail 
                    if (string.Equals(oResponse.BookingConfirmation.Status, Constant.StatusConfirmed))
                    {
                        var sBookingReference = oResponse.BookingConfirmation.Id;
                        oReferences.Add(sBookingReference);
                        oSupplierSourceReferences.Add(sBookingReference);
                    }
                    else
                    {
                        oReferences.Add(Constant.FailedReference);
                        oSupplierSourceReferences.Add(Constant.FailedReference);
                    }
                }
                catch (Exception ex)
                {
                    oPropertyDetails.Warnings.AddNew("BookException", ex.Message);
                    oReferences.Add(Constant.FailedReference);
                    oSupplierSourceReferences.Add(Constant.FailedReference);
                }
            }

            oPropertyDetails.SupplierSourceReference = string.Join("|", oSupplierSourceReferences);
            var sReference = string.Join("|", oReferences);

            //'fail entire booking if one of the request failed
            if (oReferences.Contains(Constant.FailedReference))
            {
                oPropertyDetails.SourceSecondaryReference = sReference;
                return "failed";
            }

            return sReference;
        }

        #endregion

        #region "Cancellations"

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails oPropertyDetails)
        {
            GetCancellationCost(oPropertyDetails);

            var oSupplierSourceReferences = oPropertyDetails.SupplierSourceReference.Split('|');
            var oCancellationPrices = oPropertyDetails.TPRef1.Split('|');
            var oSessionIds = oPropertyDetails.TPRef2.Split('|');

            var oThirdPartyCancellationResponse = new ThirdPartyCancellationResponse
            {
                Success = true
            };

            var oCancellationReferences = new List<string>();
            decimal dAmount = 0M;
            var oCurrencyCodes = new List<string>();

            //'Send a cancellation request for each confirmed booking 
            for (int iRoomIndex = 0; iRoomIndex < oSupplierSourceReferences.Count(); iRoomIndex++)
            {
                try
                {
                    var sCancellationPrice = SafeTypeExtensions.ToSafeDecimal(oCancellationPrices[iRoomIndex]);
                    var sSessionId = oSessionIds[iRoomIndex];
                    var sSupplierSourceReference = oSupplierSourceReferences[iRoomIndex];

                    //'Check if booking was confirmed and cancellation request was sucessful
                    if (!string.Equals(sCancellationPrice, Constant.FailedReference)
                        && !string.Equals(sSessionId, Constant.FailedReference)
                        && !string.Equals(sSupplierSourceReference, Constant.FailedReference))
                    {
                        var oCancellationRequest = SetCancelConfirmationRequest(oPropertyDetails, sSessionId, sSupplierSourceReference, sCancellationPrice);
                        var oCancellationResponse = SendRequest(oCancellationRequest, oPropertyDetails, Constant.LogFileCancel);
                        var oResponse = _serializer.DeSerialize<AlturaCancellationResponse>(oCancellationResponse).Response;

                        if (string.Equals(oResponse.Result.Status, Constant.StatusCancelled))
                        {
                            dAmount += SafeTypeExtensions.ToSafeDecimal(oResponse.Result.CancellationPrice) / 100;
                            oCurrencyCodes.Add(oResponse.Result.Currency);
                            oCancellationReferences.Add(sSupplierSourceReference);
                        }
                        else
                        {
                            oCancellationReferences.Add(Constant.FailedReference);
                            oThirdPartyCancellationResponse.Success = false;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception)
                {
                    oCancellationReferences.Add(Constant.FailedReference);
                    oThirdPartyCancellationResponse.Success = false;
                }
            }

            //'Check if all currency codes returned are same
            if (oCurrencyCodes.Count() != oCurrencyCodes.Distinct().Count())
            {
                oPropertyDetails.Warnings.AddNew("Cancellation confirmation exception", $"Different currencies were returned for cancellations, the distinct currency is:: {oCurrencyCodes.Distinct().First()}");
                oThirdPartyCancellationResponse.Success = false;
            }

            oThirdPartyCancellationResponse.TPCancellationReference = string.Join("|", oCancellationReferences);
            oThirdPartyCancellationResponse.Amount = dAmount;
            oThirdPartyCancellationResponse.CurrencyCode = oCurrencyCodes.FirstOrDefault();

            return oThirdPartyCancellationResponse;
        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails oPropertyDetails)
        {
            var oCancellationFeeResult = new ThirdPartyCancellationFeeResult
            {
                Success = true
            };

            var oSupplierSourceReferences = oPropertyDetails.SupplierSourceReference.Split('|').ToList();
            var oCancellationPrices = new List<string>();

            var oSessionIds = new List<string>();
            decimal dAmount = 0.00M;
            var oCurrencyCodes = new List<string>();

            //'Send a cancellation request for each confirmed booking 
            foreach (var sSupplierSourceRef in oSupplierSourceReferences)
            {
                try
                {
                    if (!string.Equals(sSupplierSourceRef, Constant.FailedReference))
                    {
                        var oCancellationCostRequest = SetCancellationRequest(oPropertyDetails, sSupplierSourceRef);
                        var oCancellationCostResponse = SendRequest(oCancellationCostRequest, oPropertyDetails, Constant.LogFilePreCancel);

                        var preCancelResponse = _serializer.DeSerialize<AlturaCancellationResponse>(oCancellationCostResponse).Response;

                        dAmount += SafeTypeExtensions.ToSafeDecimal(preCancelResponse.Result.CancellationPrice) / 100;
                        oCancellationPrices.Add($"{dAmount}");
                        oCurrencyCodes.Add(preCancelResponse.Result.Currency);
                        oSessionIds.Add(preCancelResponse.Session.Id);
                    }
                    else
                    {
                        //'Set cacelationfee and session id to failed if booking was not sucessfull
                        oCancellationPrices.Add(Constant.FailedReference);
                        oSessionIds.Add(Constant.FailedReference);
                        oCancellationFeeResult.Success = false;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    oPropertyDetails.Warnings.AddNew("Cancellation exception", ex.Message);
                    oCancellationPrices.Add(Constant.FailedReference);
                    oSessionIds.Add(Constant.FailedReference);
                    oCancellationFeeResult.Success = false;
                }
            }
            //'Check if all currency codes returned are same
            if (oCurrencyCodes.Count() != oCurrencyCodes.Distinct().Count())
            {
                oPropertyDetails.Warnings.AddNew("Cancellation exception", $"Different currencies were returned for cancellations, the distinct currency is: {oCurrencyCodes.Distinct().First()}");
                oCancellationFeeResult.Success = false;
            }

            oCancellationFeeResult.Amount = dAmount;
            oCancellationFeeResult.CurrencyCode = oCurrencyCodes.FirstOrDefault();

            //'Store cancellation prices for each cancellation to be sent for cacellation confirmation request
            oPropertyDetails.TPRef1 = string.Join("|", oCancellationPrices);

            //'Update session ids to be sent for cancellation confirmation request
            oPropertyDetails.TPRef2 = string.Join("|", oSessionIds);

            oPropertyDetails.CancellationAmount = dAmount;

            return oCancellationFeeResult;
        }

        #endregion

        #region "Booking Search"

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return "";
        }

        #endregion

        #region "Booking Status Update"

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }

        #endregion

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        #region "Helpers"

        public AlturaPrebookRequest SetCheckRequest(PropertyDetails oPropertyDetails, string iRateId, string iSessionId)
        {
            return new AlturaPrebookRequest
            {
                Request =
                {
                    RequestType = Constant.RequestTypePrebook,
                    Version = Constant.ApiVersion,
                    Session = GetSession(oPropertyDetails, iSessionId),
                    RateId = iRateId
                }
            };
        }

        public AlturaBookRequest SetConfirmationRequest(PropertyDetails oPropertyDetails, string sRateId, string sSessionId, int iRoomIndex)
        {
            //'build the request xml
            return new AlturaBookRequest
            {
                Request =
                {
                    RequestType = Constant.RequestTypeBook,
                    Version = Constant.ApiVersion,
                    Session = GetSession(oPropertyDetails, sSessionId),
                    BookingConfirmation =
                    {
                        RateId = sRateId,
                        SpecialRequest = oPropertyDetails.BookingComments.Any()
                                ? $"{string.Join(",", oPropertyDetails.BookingComments.Select(c => c.Text))} (Room {iRoomIndex + 1}/{oPropertyDetails.Rooms.Count})"
                                : $"(Room {iRoomIndex + 1}/{oPropertyDetails.Rooms.Count})",
                    },
                    Guest =
                    {
                            Tittle = oPropertyDetails.LeadGuestTitle,
                            FirstName = oPropertyDetails.LeadGuestFirstName,
                            LastName = oPropertyDetails.LeadGuestLastName,
                            Email = oPropertyDetails.LeadGuestEmail,
                            Phone = oPropertyDetails.LeadGuestPhone
                    }
                }
            };
        }

        public AlturaCancellationRequest SetCancellationRequest(PropertyDetails oPropertyDetails, string sBookingId)
        {
            return new AlturaCancellationRequest
            {
                Request =
                {
                    Session = GetSession(oPropertyDetails),
                    Cancellation =
                    {
                        BookingId = sBookingId
                    }
                }
            };
        }

        public AlturaCancellationRequest SetCancelConfirmationRequest(PropertyDetails oPropertyDetails, string sSessionId, string sBookingId, decimal dCancelationAmount)
        {
            return new AlturaCancellationRequest
            {
                Request =
                {
                    RequestType = Constant.RequestTypeCancellation,
                    Session = GetSession(oPropertyDetails, sSessionId),
                    Cancellation =
                    {
                        BookingId = sBookingId,
                        CancellationPrice = dCancelationAmount * 100
                    }
                }
            };
        }

        private Session GetSession(PropertyDetails oPropertyDetails, string sSessionId = "")
        {
            return new Session
            {
                Id = sSessionId,
                AgencyId = _settings.AgencyId(oPropertyDetails),
                Password = _settings.Password(oPropertyDetails),
                ExternalId = _settings.ExternalId(oPropertyDetails)
            };
        }

        public bool IsRoomBooked(PropertyDetails oPropertyDetails, int iRoomIndex)
        {
            var sSourceReference = GetSourceReference(oPropertyDetails);

            //'Check whether rooms have never been booked
            if (string.IsNullOrEmpty(sSourceReference))
            {
                return false;
            }

            var oSourceReferences = sSourceReference.Split('|');

            //'Check if previous book has been sucessfull
            if (oSourceReferences.Count() == oPropertyDetails.Rooms.Count())
            {
                return !string.Equals(oSourceReferences[iRoomIndex], Constant.FailedReference);
            }

            return false;
        }

        public static string GetSourceReference(PropertyDetails oPropertyDetails)
        {
            return !string.Equals(oPropertyDetails.SourceReference, "failed")
                        ? oPropertyDetails.SourceReference
                        : oPropertyDetails.SourceSecondaryReference;
        }

        public XmlDocument SendRequest<T>(T tRequest, PropertyDetails oPropertyDetails, string sLogFilename)
        {
            var xmlRequest = _serializer.Serialize(tRequest);

            var oRequest = new Request
            {
                EndPoint = _settings.URL(oPropertyDetails),
                Method = eRequestMethod.POST,
                Source = Source,
                CreateLog = true,
                LogFileName = sLogFilename,
                UseGZip = _settings.UseGZip(oPropertyDetails),
                Param = "xml",
                ContentType = ContentTypes.Application_x_www_form_urlencoded,
            };

            oRequest.SetRequest(xmlRequest);

            oRequest.Send(_httpClient, _logger).RunSynchronously();

            //'save the xml for the front end
            if (!string.IsNullOrEmpty(xmlRequest.OuterXml))
            {
                oPropertyDetails.Logs.AddNew(Source, $"{Source} {sLogFilename} Request", xmlRequest.OuterXml);
            }
            if (!string.IsNullOrEmpty(oRequest.ResponseLog))
            {
                oPropertyDetails.Logs.AddNew(Source, $"{Source} {sLogFilename} Response", oRequest.ResponseLog);
            }

            return oRequest.ResponseXML;
        }

        #endregion
    }
}