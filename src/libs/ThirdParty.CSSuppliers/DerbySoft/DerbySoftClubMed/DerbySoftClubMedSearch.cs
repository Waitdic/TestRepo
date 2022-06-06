namespace ThirdParty.CSSuppliers.DerbySoft
{
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4;

    public class DerbySoftClubMedSearch : DerbySoftBookingUsbV4Search
    {
        public DerbySoftClubMedSearch(IDerbySoftClubMedSettings settings)
            : base(settings)
        {
        }

        public override string Source => ThirdParties.DERBYSOFTCLUBMED;
    }
}