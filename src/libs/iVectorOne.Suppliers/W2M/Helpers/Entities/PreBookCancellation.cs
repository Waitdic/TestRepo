using System;

namespace ThirdParty.CSSuppliers.Models.W2M
{
#pragma warning disable CS8618

    public class PreBookCancellation
    {
        public DateTime StartDate { get; set; }
        public DateTime RuleEndDate { get; set; }
        public decimal Amount { get; set; }
    }
}
