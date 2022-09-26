namespace iVectorOne.SDK.V2.PropertyBook
{
    using Intuitive.Helpers.Extensions;
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    ///  The guest details
    /// </summary>
    public class GuestDetail
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public GuestType Type { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

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
    }
}