namespace iVectorOne.Suppliers.TourPlanTransfers
{
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Transfer;
    using System.Threading.Tasks;
    using iVectorOne.Models.Transfer;
    using Intuitive;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using iVectorOne.SDK.V2;
    using System;
    using System.Collections.Generic;

    public abstract class TourPlanTransfersBase : IThirdParty, ISingleSource
    {
        public abstract string Source { get; }

        private ITourPlanTransfersSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TourPlanTransfersSearchBase> _logger;

        public TourPlanTransfersBase(
            ITourPlanTransfersSettings settings,
            HttpClient httpClient,
            ILogger<TourPlanTransfersSearchBase> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public Task<bool> PreBookAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> BookAsync(TransferDetails transferDetails)
        {
            try
            {
                var supplierReferenceValues = SplitSupplierReference(transferDetails);
                return Task.FromResult("failed");
            }
            catch (ArgumentException ex)
            {
                transferDetails.Warnings.AddNew("ArgumentException", ex.Message);
                return Task.FromResult("failed");
            }
        }

        public Task<ThirdPartyCancellationResponse> CancelBookingAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            throw new System.NotImplementedException();
        }
        private class SupplierReferenceValues
        {
            public string Opt { get; set; }
            public string RateId { get; set; }
        }
        private List<SupplierReferenceValues> SplitSupplierReference(TransferDetails transferDetails)
        {
            List<SupplierReferenceValues> lstSupplierReferenceValues = new List<SupplierReferenceValues>();

            if (transferDetails.OneWay)
            {
                string[] supplierReferenceValues = transferDetails.SupplierReference.Split("-");
                if (supplierReferenceValues.Length != 2)
                {
                    throw new ArgumentException(WarningMessages.InvalidSupplierReference);
                }
                lstSupplierReferenceValues.Add(new SupplierReferenceValues { Opt = supplierReferenceValues[0], RateId = supplierReferenceValues[1] });
            }
            else
            {
                string[] supplierReferenceValues = transferDetails.SupplierReference.Split("|");
                if (supplierReferenceValues.Length != 2)
                {
                    throw new ArgumentException(WarningMessages.InvalidSupplierReference);
                }
                foreach (string sr in supplierReferenceValues)
                {
                    string[] srValues = sr.Split("-");

                    if (srValues.Length != 2)
                    {
                        throw new ArgumentException(WarningMessages.InvalidSupplierReference);
                    }
                    lstSupplierReferenceValues.Add(new SupplierReferenceValues { Opt = srValues[0], RateId = srValues[1] });
                }

            }
            return lstSupplierReferenceValues;
        }
    }
}
