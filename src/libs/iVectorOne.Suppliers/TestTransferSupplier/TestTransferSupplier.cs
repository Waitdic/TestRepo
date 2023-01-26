﻿namespace iVectorOne.Suppliers
{
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Transfer;
    using System;
    using System.Threading.Tasks;

    public partial class TestTransferSupplier : IThirdParty, ISingleSource
    {
        public string Source => ThirdParties.TESTTRANSFERSUPPLIER;

        async Task<bool> IThirdParty.PreBookAsync(TransferDetails transferDetails)
        {
            bool prebookSuccess = true;

            await Task.Delay(50);

            if (string.IsNullOrEmpty(transferDetails.SupplierReference))
            {
                return false;
            }

            transferDetails.SupplierReference += "_prebooked";
            transferDetails.LocalCost = 200M;
            transferDetails.Cancellations.AddNew(DateTime.Now, transferDetails.DepartureDate, transferDetails.LocalCost);
            transferDetails.DepartureErrata.AddNew("Departure", "Departure Notes");
            
            if (!transferDetails.OneWay)
            {
                transferDetails.ReturnErrata.AddNew("Return", "Return Notes");
            }

            return prebookSuccess;
        }

        async Task<string> IThirdParty.BookAsync(TransferDetails transferDetails)
        {
            string reference = string.Empty;

            await Task.Delay(50);

            if (!transferDetails.SupplierReference.Contains("prebooked"))
            {
                transferDetails.Warnings.AddNew("Booking Failed!", "Invalid Supplier Reference");
                return "failed";
            }

            transferDetails.LocalCost = 200M;
            transferDetails.Cancellations.AddNew(DateTime.Now, transferDetails.DepartureDate, transferDetails.LocalCost);
            transferDetails.ConfirmationReference= "confirmed_ref";
            reference = "booked_ref";

            return reference;
        }

        Task<ThirdPartyCancellationResponse> IThirdParty.CancelBookingAsync(TransferDetails transferDetails)
        {
            var cancellationResponse = new ThirdPartyCancellationResponse();

            try
            {
                cancellationResponse.Success = true;
                cancellationResponse.CurrencyCode = transferDetails.ISOCurrencyCode;
                cancellationResponse.Amount = transferDetails.LocalCost;
                cancellationResponse.TPCancellationReference = "cancel_ref";
            }
            catch (Exception exception)
            {
                cancellationResponse.Success = false;
                cancellationResponse.TPCancellationReference = "failed";

                transferDetails.Warnings.AddNew("Cancellation Exception", exception.ToString());
            }

            return Task.FromResult(cancellationResponse);
        }

        Task<ThirdPartyCancellationFeeResult> IThirdParty.GetCancellationCostAsync(TransferDetails transferDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        bool IThirdParty.SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            throw new System.NotImplementedException();
        }

        public bool ValidateSettings(TransferDetails transferDetails)
        {
            return true;
        }
    }
}
