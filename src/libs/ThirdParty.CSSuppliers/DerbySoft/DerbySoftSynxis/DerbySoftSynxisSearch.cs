namespace ThirdParty.CSSuppliers.DerbySoft
{
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4;

    public class DerbySoftSynxisSearch : DerbySoftBookingUsbV4Search
    {
        public DerbySoftSynxisSearch(IDerbySoftSynxisSettings settings, ILogger<DerbySoftSynxisSearch> logger)
            : base(settings, logger)
        {
        }

        public override string Source => ThirdParties.DERBYSOFTSYNXIS;
    }
}