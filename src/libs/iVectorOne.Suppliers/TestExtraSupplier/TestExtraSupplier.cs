namespace iVectorOne.Suppliers
{
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Extra;
    using iVectorOne.Extra;
    using System;
    using System.Threading.Tasks;

    public partial class TestExtraSupplier : IThirdParty, ISingleSource
    {
        public string Source => ThirdParties.TESTEXTRASUPPLIER;

        async Task<bool> IThirdParty.PreBookAsync(ExtraDetails extraDetails)
        {
            bool prebookSuccess = true;

            await Task.Delay(50);

            if (string.IsNullOrEmpty(extraDetails.SupplierReference))
            {
                return false;
            }

            extraDetails.SupplierReference += "_prebooked";
            extraDetails.LocalCost = 200M;
            extraDetails.Cancellations.AddNew(DateTime.Now, extraDetails.DepartureDate, extraDetails.LocalCost);
            extraDetails.Errata.AddNew("Errata", "Notes");

            return prebookSuccess;
        }

        async Task<string> IThirdParty.BookAsync(ExtraDetails extraDetails)
        {
            string reference = string.Empty;

            await Task.Delay(50);

            if (!extraDetails.SupplierReference.Contains("prebooked"))
            {
                extraDetails.Warnings.AddNew("Booking Failed!", "Invalid Supplier Reference");
                return "failed";
            }

            extraDetails.LocalCost = 200M;
            extraDetails.Cancellations.AddNew(DateTime.Now, extraDetails.DepartureDate, extraDetails.LocalCost);
            extraDetails.ConfirmationReference= "confirmed_ref";
            reference = "booked_ref";

            return reference;
        }

        Task<ThirdPartyCancellationResponse> IThirdParty.CancelBookingAsync(ExtraDetails extraDetails)
        {
            var cancellationResponse = new ThirdPartyCancellationResponse();

            try
            {
                cancellationResponse.Success = true;
                cancellationResponse.CurrencyCode = extraDetails.ISOCurrencyCode;
                cancellationResponse.Amount = extraDetails.LocalCost;
                cancellationResponse.TPCancellationReference = "cancel_ref";
            }
            catch (Exception exception)
            {
                cancellationResponse.Success = false;
                cancellationResponse.TPCancellationReference = "failed";

                extraDetails.Warnings.AddNew("Cancellation Exception", exception.ToString());
            }

            return Task.FromResult(cancellationResponse);
        }

        Task<ThirdPartyCancellationFeeResult> IThirdParty.GetCancellationCostAsync(ExtraDetails extraDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        bool IThirdParty.SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            throw new System.NotImplementedException();
        }

        public bool ValidateSettings(ExtraDetails extraDetails)
        {
            return true;
        }
    }
}
