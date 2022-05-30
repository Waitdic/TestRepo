namespace ThirdParty.Factories
{
    using System.Threading.Tasks;
    using ThirdParty.Models.Property.Booking;
    using Prebook = SDK.V2.PropertyPrebook;
    using Book = SDK.V2.PropertyBook;
    using Cancel = SDK.V2.PropertyCancel;
    using Precancel = SDK.V2.PropertyPrecancel;
    using ThirdParty.Models;

    /// <summary>
    /// Factory that builds up property details from api requests, used to pass into the third party code
    /// </summary>
    public interface IPropertyDetailsFactory
    {
        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A property details object</returns>
        Task<PropertyDetails> CreateAsync(Precancel.Request request, User user);

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A property details object</returns>
        Task<PropertyDetails> CreateAsync(Cancel.Request request, User user);

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A property details object</returns>
        Task<PropertyDetails> CreateAsync(Book.Request request, User user);

        /// <summary>
        /// Creates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A property details object</returns>
        Task<PropertyDetails> CreateAsync(Prebook.Request request, User user);
    }
}