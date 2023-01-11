namespace iVectorOne.Factories
{
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using Precancel = SDK.V2.PropertyPrecancel;
    using Cancel = SDK.V2.PropertyCancel;
    using iVectorOne.Models.Property;

    /// <summary>
    /// A factory that creates property cancellation responses using the provided property details
    /// </summary>
    /// <seealso cref="IPropertyBookResponseFactory" />
    public class CancelPropertyResponseFactory : ICancelPropertyResponseFactory
    {
        /// <summary>The currency repository</summary>
        private readonly ITPSupport _support;

        public CancelPropertyResponseFactory(ITPSupport support)
        {
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        /// <summary>
        /// Creates a cancellation response using information from the property details
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
        /// Creates a cancellation fees response using information from the property details
        /// </summary>
        /// <param name="fees">The third party cancellation fees returned from the interface</param>
        /// <param name="propertyDetails">The Property Details</param>
        /// <returns>A book response</returns>
        public async Task<Precancel.Response> CreateFeesResponseAsync(ThirdPartyCancellationFeeResult fees, PropertyDetails propertyDetails)
        {
            var response = new Precancel.Response()
            {
                Amount = fees.Amount + 0.00M,
                CurrencyCode = await _support.TPCurrencyLookupAsync(propertyDetails.Source, fees.CurrencyCode),
                Warnings = propertyDetails.Warnings.Select(w => w.Text).ToList()
            };

            return response;
        }
    }
}