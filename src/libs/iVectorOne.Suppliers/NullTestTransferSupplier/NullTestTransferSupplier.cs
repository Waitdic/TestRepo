namespace iVectorOne.Suppliers
{
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models.Property;
    using iVectorOne.Models.Transfer;
    using iVectorOne.Transfer;
    using System;
    using System.Threading.Tasks;

    public partial class NullTestTransferSupplier : IThirdParty, ISingleSource
    {
        public string Source => ThirdParties.NULLTESTTRANSFERSUPPLIER;

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
            transferDetails.DepartureNotes = "Departure Notes";
            
            if (!transferDetails.OneWay)
            {
                transferDetails.ReturnNotes = "Return Notes";
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
            throw new System.NotImplementedException();
        }

        Task<ThirdPartyCancellationFeeResult> IThirdParty.GetCancellationCostAsync(TransferDetails transferDetails)
        {
            throw new System.NotImplementedException();
        }

        bool IThirdParty.SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            throw new System.NotImplementedException();
        }
    }
}
