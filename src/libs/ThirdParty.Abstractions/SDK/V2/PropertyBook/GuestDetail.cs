namespace ThirdParty.SDK.V2.PropertyBook
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using ThirdParty.SDK.V2;

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
        /// Gets or sets the date of birth.
        /// </summary>
        public DateTime DateOfBirth { get; set; }
    }
}