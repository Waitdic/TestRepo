namespace ThirdParty
{
    using ThirdParty.Models.Property.Booking;

    /// <summary>Third party optional supplements</summary>
    public interface IThirdPartyOptionalSupplements : IThirdParty
    {
        /// <summary>Searches the optional supplements.</summary>
        /// <param name="propertyDetails">The o property details.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        bool SearchOptionalSupplements(PropertyDetails propertyDetails);
    }
}