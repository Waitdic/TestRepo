﻿namespace iVectorOne_Admin_Api.Config.Models
{
    using System.Text.Json.Serialization;

    public class AccountDTO
    {
        public int AccountId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool? DummyResponses { get; set; }
        public short PropertyTprequestLimit { get; set; }
        public short SearchTimeoutSeconds { get; set; }
        public bool LogMainSearchError { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        [JsonIgnore]
        public string Status { get; set; } = null!;
        public bool IsActive => Status == RecordStatus.Active;
    }
}