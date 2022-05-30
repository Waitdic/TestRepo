namespace ThirdParty.Services
{
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.SDK.V2.PropertyBook;

    /// <summary>Class to Validate which suppliers need which supplier references</summary>
    public interface ISuppierReferenceValidator
    {
        /// <summary>Validates the supplier references needed for each supplier at book.</summary>
        /// <param name="details">The property details.</param>
        /// <param name="bookRequest">The booking request.</param>
        void ValidateBook(PropertyDetails details, Request bookRequest);

        /// <summary>Validates the supplier references needed for each supplier at cancel.</summary>
        /// <param name="details">The property details.</param>
        void ValidateCancel(PropertyDetails details);
    }
}