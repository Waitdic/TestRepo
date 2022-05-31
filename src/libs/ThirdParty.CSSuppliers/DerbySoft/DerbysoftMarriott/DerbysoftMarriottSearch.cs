namespace ThirdParty.CSSuppliers.DerbySoft
{
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4;

    public class DerbySoftMarriottSearch : DerbySoftBookingUsbV4Search
    {
        public DerbySoftMarriottSearch(IDerbySoftMarriottSettings settings, ILogger<DerbySoftBookingUsbV4Search> logger)
            : base(settings, logger)
        {
        }

        public override string Source => ThirdParties.DERBYSOFTSMARRIOTT;
    }
}