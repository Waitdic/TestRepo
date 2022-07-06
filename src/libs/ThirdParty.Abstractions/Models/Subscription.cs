namespace ThirdParty.Models
{
    using System.Collections.Generic;
    using ThirdParty.Search.Settings;

    /// <summary>
    /// The user object
    /// </summary>
    public class Subscription
    {
        public int SubscriptionID { get; set; }

        /// <summary>Gets or sets the login.</summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>Gets or sets the password.</summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>Gets or sets the environment.</summary>
        public string Environment { get; set; } = string.Empty;

        /// <summary>Gets or sets the configurations.</summary>
        public List<ThirdPartyConfiguration> Configurations { get; set; } = new();

        /// <summary>Gets or sets the third party settings.</summary>
        public Settings TPSettings { get; set; } = new();
    }
}