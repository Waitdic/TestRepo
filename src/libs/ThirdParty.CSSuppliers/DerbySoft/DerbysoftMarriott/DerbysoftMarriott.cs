namespace ThirdParty.CSSuppliers
{
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;

    public class DerbySoftMarriott : DerbySoftBookingUsbV4
    {
        public DerbySoftMarriott(
            IDerbySoftMarriottSettings settings,
            ITPSupport support,
            HttpClient httpClient,
            ILogger<DerbySoftMarriott> logger)
            : base(settings, support, httpClient, logger)
        {
        }

        public override string Source
        {
            get => ThirdParties.DERBYSOFTSMARRIOTT;
        }
    }
}
