namespace iVectorOne.Factories
{
    using System.Threading.Tasks;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using Precancel = SDK.V2.PropertyPrecancel;
    using Cancel = SDK.V2.PropertyCancel;

    /// <summary>
    /// A factory that creates property cancellation responses using the provided property details
    /// </summary>
    public interface ICancelPropertyResponseFactory
    {
        /// <summary>
        /// Creates a cancellation response using information from the property details
        /// </summary>
        /// <param name="thirdPartyCancellationResponse">The third party cancellation response returned from the interface</param>
        /// <returns>A book response</returns>
        Cancel.Response Create(ThirdPartyCancellationResponse thirdPartyCancellationResponse);

        /// <summary>
        /// Creates a cancellation fees response using information from the property details
        /// </summary>
        /// <param name="fees">The third party cancellation fees returned from the interface</param>
        /// <param name="propertyDetails">The Property Details</param>
        /// <returns>A book response</returns>
        Task<Precancel.Response> CreateFeesResponseAsync(ThirdPartyCancellationFeeResult fees, PropertyDetails propertyDetails);
    }
}