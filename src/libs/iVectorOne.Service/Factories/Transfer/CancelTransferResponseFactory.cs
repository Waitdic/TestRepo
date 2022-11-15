namespace iVectorOne.Factories
{
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Transfer;
    using Precancel = SDK.V2.TransferPrecancel;
    using Cancel = SDK.V2.TransferCancel;
    using iVectorOne.Models.Property;

    /// <summary>
    /// A factory that creates transfer cancellation responses using the provided transfer details
    /// </summary>
    /// <seealso cref="ICancelTransferResponseFactory" />
    public class CancelTransferResponseFactory : ICancelTransferResponseFactory
    {
        /// <summary>The currency repository</summary>
        private readonly ITPSupport _support;

        public CancelTransferResponseFactory(ITPSupport support)
        {
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        /// <summary>
        /// Creates a cancellation response using information from the transfer details
        /// </summary>
        /// <param name="thirdPartyCancellationResponse">The third party cancellation response returned from the interface</param>
        /// <returns>A book response</returns>
        public Cancel.Response Create(ThirdPartyCancellationResponse thirdPartyCancellationResponse)
        {
            string cancellationReference = thirdPartyCancellationResponse.TPCancellationReference;
            if(string.IsNullOrEmpty(cancellationReference))
            {
                cancellationReference = "[Failed]";
            }

            var response = new Cancel.Response()
            {
                SupplierCancellationReference = cancellationReference,
            };

            return response;
        }

        /// <summary>
        /// Creates a cancellation fees response using information from the transfer details
        /// </summary>
        /// <param name="fees">The third party cancellation fees returned from the interface</param>
        /// <param name="transferDetails">The Transfer Details</param>
        /// <returns>A book response</returns>
        public async Task<Precancel.Response> CreateFeesResponseAsync(ThirdPartyCancellationFeeResult fees, TransferDetails transferDetails)
        {
            var response = new Precancel.Response()
            {
                Amount = fees.Amount + 0.00M,
                CurrencyCode = await _support.TPCurrencyLookupAsync(transferDetails.Source, fees.CurrencyCode),
                Warnings = transferDetails.Warnings.Select(w => w.Text).ToList()
            };

            return response;
        }
    }
}