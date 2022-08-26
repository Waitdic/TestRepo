namespace iVectorOne.Web.Adaptors.Authentication
{
    using System.Collections.Generic;
    using iVectorOne.Search.Settings;

    public class SqlAccount
    {
        public int AccountID { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
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