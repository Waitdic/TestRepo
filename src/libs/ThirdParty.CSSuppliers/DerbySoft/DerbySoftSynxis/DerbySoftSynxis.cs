namespace ThirdParty.CSSuppliers
{
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class DerbySoftSynxis : DerbySoftBookingUsbV4
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

