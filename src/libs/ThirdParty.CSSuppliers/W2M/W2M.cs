namespace ThirdParty.CSSuppliers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.CSSuppliers.Helpers.W2M;
    using ThirdParty.CSSuppliers.Models.W2M;
    using Microsoft.Extensions.Logging;

    public class W2M : IThirdParty
    {
        #region "Properties"

        private readonly IW2MSettings _settings;
        private readonly W2MHelper _w2mHelper;

        public string Source => ThirdParties.W2M;
        public bool SupportsRemarks => false;
        public bool SupportsBookingSearch => false;

        #endregion

        #region "Constructors"

        public W2M(IW2MSettings settings, ISerializer serializer, HttpClient httpClient, ILogger<W2M> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _w2mHelper = new W2MHelper(_settings, serializer, httpClient, logger);
        }

        #endregion

        public string Book(PropertyDetails propertyDetails)
        {
            var bookResult = _w2mHelper.Book(propertyDetails);

            foreach (var log in bookResult.Logs)
            {
                propertyDetails.Logs.AddNew(log.Source, log.Title, log.Log);
            }

            if (!bookResult.Success)
            {
                propertyDetails.SourceSecondaryReference = string.Join("|", bookResult.References);
                return Constants.FailedReference;
            }

            foreach (var kvp in bookResult.Warnings)
            {
                propertyDetails.Warnings.AddNew(kvp.Key, kvp.Value);
            }

            return string.Join("|", bookResult.References);
        }


        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            throw new NotImplementedException();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            throw new NotImplementedException();
        }

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails)
        {
            List<string> referencesForCancellation;

            if (propertyDetails.SourceReference == Constants.FailedReference)
            {
                referencesForCancellation = propertyDetails.SourceSecondaryReference
                    .Split(Constants.BookingCodesSeparator)
                    .Where(x => x != Constants.FailedReference).ToList();
            }
            else
            {
                referencesForCancellation = propertyDetails.SourceReference
                    .Split(Constants.BookingCodesSeparator)
                    .Where(x => x != Constants.FailedReference).ToList();
            }

            var baseParams = new BaseRequestParameters
            {
                Username = _settings.Username(propertyDetails),
                Password = _settings.Password(propertyDetails),
                Language = _settings.LangID(propertyDetails),
                Endpoint = _settings.CancelURL(propertyDetails),
                SoapPrefix = _settings.SoapActionPrefix(propertyDetails),
                CreateLogs = propertyDetails.CreateLogs
            };

            var cancellationResponse = _w2mHelper.CancelBooking(baseParams, referencesForCancellation);

            foreach (var log in cancellationResponse.Logs)
            {
                propertyDetails.Logs.AddNew(log.Source, log.Title, log.Text);
            }

            cancellationResponse.TPCancellationReference = propertyDetails.SourceReference;

            return cancellationResponse;
        }

        public string CreateReconciliationReference(string sInputReference)
        {
            throw new NotImplementedException();
        }

        public void EndSession(PropertyDetails oPropertyDetails)
        {
            throw new NotImplementedException();
        }

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails) =>
            new ThirdPartyCancellationFeeResult
            {
                Success = true,
                CurrencyCode = propertyDetails.CurrencyCode
            };

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails)
        {
            throw new NotImplementedException();
        }

        public bool PreBook(PropertyDetails propertyDetails)
        {
            try
            {
                var baseParameters = new BaseRequestParameters
                {
                    Username = _settings.Username(propertyDetails),
                    Password = _settings.Password(propertyDetails),
                    Language = _settings.LangID(propertyDetails),
                    Endpoint = _settings.PreBookUrl(propertyDetails),
                    SoapPrefix = _settings.SoapActionPrefix(propertyDetails),
                    CreateLogs = propertyDetails.CreateLogs
                };

                var parameters = new PreBookRequestParameters(baseParameters,
                    propertyDetails.ArrivalDate, propertyDetails.DepartureDate,
                    propertyDetails.Duration,
                    propertyDetails.TPKey,
                    propertyDetails.Source,
                    propertyDetails.LeadGuestBookingCountry);

                var preBookResult = _w2mHelper.PreBook(parameters, propertyDetails.Rooms);
                foreach (var log in preBookResult.Logs)
                {
                    propertyDetails.Logs.AddNew(log.Source, log.Title, log.Log);
                }

                if (!preBookResult.Success)
                {
                    return false;
                }

                foreach (var cancellation in preBookResult.Cancellations)
                {
                    propertyDetails.Cancellations.AddNew(cancellation.StartDate, cancellation.RuleEndDate, cancellation.Amount);
                }

                foreach (var erratum in preBookResult.Errata)
                {
                    propertyDetails.Errata.AddNew(erratum.Title, erratum.Text);
                }

                propertyDetails.Cancellations.Solidify(SolidifyType.Sum);
                propertyDetails.TPRef1 = string.Join(Constants.BookingCodesSeparator.ToString(), preBookResult.BookingCodes);

                return true;
            }
            catch (Exception ex)
            {
                propertyDetails.Logs.AddNew(ThirdParties.W2M, "W2M Prebook Exception", ex.ToString());
                return false;
            }
        }

        public bool RequiresVCard(VirtualCardInfo info)
        {
            throw new NotImplementedException();
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return true;
        }
    }
}