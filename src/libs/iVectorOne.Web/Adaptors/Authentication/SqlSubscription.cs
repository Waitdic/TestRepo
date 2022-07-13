namespace iVectorOne.Web.Adaptors.Authentication
{
    using System.Collections.Generic;
    using ThirdParty.Search.Settings;

    public class SqlSubscription
    {
        public int SubscriptionID { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? Environment { get; set; }
        public bool DummyResponses { get; set; }
        public Settings? TPSettings { get; set; }
        public List<Configuration> Configurations { get; set; } = new();
    }

    public class Configuration
    {
        public string? Supplier { get; set; }
        public List<Attribute>? Attributes { get; set; } = new();
    }

    public class Attribute
    {
        public string? AttributeName { get; set; }
        public string? AttributeValue { get; set; }
    }
}