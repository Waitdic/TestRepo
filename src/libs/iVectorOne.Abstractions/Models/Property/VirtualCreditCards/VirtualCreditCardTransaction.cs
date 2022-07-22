namespace iVectorOne.Models.Property.VirtualCreditCards
{
    using System;
    using System.Collections.Generic;

    public class VirtualCardReturn
    {
        public bool Success { get; set; }
        public List<string> Warnings { get; set; } = new();
        public decimal Amount { get; set; }
        public int CurrencyID { get; set; }
        public int CardTypeID { get; set; }
        public string CardReferenceID { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
        public string ExpiryMonth { get; set; } = string.Empty;
        public string ExpiryYear { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
        public string ImageString { get; set; } = string.Empty;
        public string CardStatus { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public decimal OldToNewExchangeRate { get; set; }
        public int NewCurrencyID { get; set; }
        public decimal NewCurrencyAmount { get; set; }
    }
}