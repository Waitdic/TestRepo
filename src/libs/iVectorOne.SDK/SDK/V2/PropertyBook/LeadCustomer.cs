namespace iVectorOne.SDK.V2.PropertyBook
{
    using Intuitive.Helpers.Extensions;
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// The lead customer on a booking
    /// </summary>
    public class LeadCustomer
    {
        /// <summary>
        /// Gets or sets the customer title.
        /// </summary>
        public string CustomerTitle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the customer.
        /// </summary>
        public string CustomerFirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the customer.
        /// </summary>
        public string CustomerLastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date of birth for serialisation.
        /// </summary>
        [JsonPropertyName("DateOfBirth")]
        public string DateOfBirthSerialised { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        [JsonIgnore]
        public DateTime DateOfBirth
            => string.IsNullOrEmpty(DateOfBirthSerialised) ? DateTimeExtensions.EmptyDate : DateOfBirthSerialised.ToSafeDate();

        /// <summary>
        /// Gets or sets the customer address1.
        /// </summary>
        public string CustomerAddress1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer address2.
        /// </summary>
        public string CustomerAddress2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer town city.
        /// </summary>
        public string CustomerTownCity { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer county.
        /// </summary>
        public string CustomerCounty { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer postcod.
        /// </summary>
        public string CustomerPostcode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer booking country cod.
        /// </summary>
        public string CustomerBookingCountryCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer phone.
        /// </summary>
        public string CustomerPhone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer mobile.
        /// </summary>
        public string CustomerMobile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer email.
        /// </summary>
        public string CustomerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer passport number.
        /// </summary>
        public string PassportNumber { get; set; } = string.Empty;
    }
}