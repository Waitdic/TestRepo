namespace ThirdParty.CSSuppliers.DerbySoft
{
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.DerbySoft;
    using ThirdParty.Lookups;
    using usbv4 = DerbySoftBookingUsbV4;

    public class DerbySoftMarriott : usbv4.DerbySoftBookingUsbV4
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
