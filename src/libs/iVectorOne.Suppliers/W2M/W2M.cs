namespace iVectorOne.Suppliers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.Helpers.W2M;
    using iVectorOne.Suppliers.Models.W2M;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Models.Property;

    public class W2M : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IW2MSettings _settings;
        private readonly W2MHelper _w2mHelper;

        public string Source => ThirdParties.W2M;
        public bool SupportsRemarks => false;
        public bool SupportsBookingSearch => false;

        #endregion

        #region Constructors

        public W2M(IW2MSettings settings, ISerializer serializer, HttpClient httpClient, ILogger<W2M> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _w2mHelper = new W2MHelper(_settings, serializer, httpClient, logger);
        }

        #endregion

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var bookResult = await _w2mHelper.BookAsync(propertyDetails);

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

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var referencesForCancellation = (propertyDetails.SourceReference == Constants.FailedReference ?
                    propertyDetails.SourceSecondaryReference :
                    propertyDetails.SourceReference)
                    .Split(Constants.BookingCodesSeparator)
                .Where(x => x != Constants.FailedReference)
                .ToList();

            var baseParams = new BaseRequestParameters
            {
                Username = _settings.User(propertyDetails),
                Password = _settings.Password(propertyDetails),
                Language = _settings.LanguageCode(propertyDetails),
                Endpoint = _settings.CancellationURL(propertyDetails),
                SoapPrefix = _settings.SoapActionPrefix(propertyDetails),
                CreateLogs = true
            };

            var cancellationResponse = await _w2mHelper.CancelBookingAsync(baseParams, referencesForCancellation);

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

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
            => Task.FromResult(
                new ThirdPartyCancellationFeeResult
                    {
                        Success = true,
                        CurrencyCode = propertyDetails.ISOCurrencyCode
                    });

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            try
            {
                var baseParameters = new BaseRequestParameters
                {
                    Username = _settings.User(propertyDetails),
                    Password = _settings.Password(propertyDetails),
                    Language = _settings.LanguageCode(propertyDetails),
                    Endpoint = _settings.PrebookURL(propertyDetails),
                    SoapPrefix = _settings.SoapActionPrefix(propertyDetails),
                    CreateLogs = true
                };

                var parameters = new PreBookRequestParameters(baseParameters,
                    propertyDetails.ArrivalDate, propertyDetails.DepartureDate,
                    propertyDetails.Duration,
                    propertyDetails.TPKey,
                    propertyDetails.Source,
                    propertyDetails.LeadGuestCountryCode);

                var preBookResult = await _w2mHelper.PreBookAsync(parameters, propertyDetails.Rooms);
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

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            throw new NotImplementedException();
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return true;
        }
    }
}