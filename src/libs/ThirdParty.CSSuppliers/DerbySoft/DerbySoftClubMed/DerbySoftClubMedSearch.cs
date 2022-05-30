namespace ThirdParty.CSSuppliers
{
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public class DerbySoftClubMedSearch : DerbySoftBookingUsbV4Search
    {
        public DerbySoftClubMedSearch(IDerbySoftClubMedSettings settings, ILogger<DerbySoftClubMedSearch> logger)
            : base(settings, logger)
        {
        }

        public override string Source => ThirdParties.DERBYSOFTCLUBMED;
    }
}