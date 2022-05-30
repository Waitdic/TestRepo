namespace ThirdParty.CSSuppliers
{
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;

    public class DerbySoftClubMed : DerbySoftBookingUsbV4
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

