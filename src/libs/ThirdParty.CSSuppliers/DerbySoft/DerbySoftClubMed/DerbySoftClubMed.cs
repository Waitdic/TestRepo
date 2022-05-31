namespace ThirdParty.CSSuppliers.DerbySoft
{
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using usbv4 = DerbySoftBookingUsbV4;

    public class DerbySoftClubMed : usbv4.DerbySoftBookingUsbV4
    {
        public DerbySoftClubMed(
            IDerbySoftClubMedSettings settings,
            ITPSupport support,
            HttpClient httpClient,
            ILogger<DerbySoftClubMed> logger)
            : base(settings, support, httpClient, logger)
        {
        }

        public override string Source
        {
            get => ThirdParties.DERBYSOFTCLUBMED;
        }

    }
}

