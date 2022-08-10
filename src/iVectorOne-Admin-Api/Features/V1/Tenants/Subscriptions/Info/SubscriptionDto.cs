﻿namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Info
{
    public record SubscriptionDto
    {
        public int SubscriptionId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool? DummyResponses { get; set; }

        public short PropertyTprequestLimit { get; set; }

        public short SearchTimeoutSeconds { get; set; }

        public bool LogMainSearchError { get; set; }

        public string CurrencyCode { get; set; } = string.Empty;

        public string Environment { get; set; } = string.Empty;
    }
}
