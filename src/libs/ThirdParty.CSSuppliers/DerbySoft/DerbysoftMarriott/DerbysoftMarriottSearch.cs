namespace ThirdParty.CSSuppliers
{
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public class DerbySoftMarriottSearch : DerbySoftBookingUsbV4Search
    {
        public DerbySoftMarriottSearch(IDerbySoftMarriottSettings settings, ILogger<DerbySoftBookingUsbV4Search> logger)
            : base(settings, logger)
        {
        }

        public override string Source => ThirdParties.DERBYSOFTSMARRIOTT;
    }
}