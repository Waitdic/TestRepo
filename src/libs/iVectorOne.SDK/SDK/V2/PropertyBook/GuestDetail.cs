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
        [JsonInclude]
        [JsonPropertyName("DateOfBirth")]
        public string DateOfBirthSerialised
        {
            get
            {
                return DateOfBirth.ToString();
            }
            private set
            {
                // compatibility with V1 where we allow empty strings to be deserialized
                DateOfBirth = string.IsNullOrEmpty(value) ? DateTimeExtensions.EmptyDate : value.ToSafeDate();
            }
        }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        [JsonIgnore]
        public DateTime DateOfBirth { get; set; }
    }
}