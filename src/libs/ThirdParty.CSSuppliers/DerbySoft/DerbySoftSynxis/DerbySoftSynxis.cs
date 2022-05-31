namespace ThirdParty.CSSuppliers.DerbySoft
{
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using usbv4 = DerbySoftBookingUsbV4;

    public class DerbySoftSynxis : usbv4.DerbySoftBookingUsbV4
    {
        public DerbySoftSynxis(
            IDerbySoftSynxisSettings settings,
            ITPSupport support,
            HttpClient httpClient,
            ILogger<DerbySoftSynxis> logger)
            : base(settings, support, httpClient, logger)
        {
        }

        public override string Source
        {
            get => ThirdParties.DERBYSOFTSYNXIS;
        }
    }
}

