namespace ThirdParty.CSSuppliers
{
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public class DerbySoftSynxisSearch : DerbySoftBookingUsbV4Search
    {
        public DerbySoftSynxisSearch(IDerbySoftSynxisSettings settings, ILogger<DerbySoftSynxisSearch> logger)
            : base(settings, logger)
        {
        }

        public override string Source => ThirdParties.DERBYSOFTSYNXIS;
    }
}