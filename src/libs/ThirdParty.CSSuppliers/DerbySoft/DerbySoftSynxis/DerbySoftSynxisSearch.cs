namespace ThirdParty.CSSuppliers.DerbySoft
{
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4;

    public class DerbySoftSynxisSearch : DerbySoftBookingUsbV4Search
    {
        public DerbySoftSynxisSearch(IDerbySoftSynxisSettings settings)
            : base(settings)
        {
        }

        public override string Source => ThirdParties.DERBYSOFTSYNXIS;
    }
}