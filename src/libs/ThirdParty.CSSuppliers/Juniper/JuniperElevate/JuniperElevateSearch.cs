namespace ThirdParty.CSSuppliers.Juniper
{
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public class JuniperElevateSearch : JuniperBaseSearch
    {
        public JuniperElevateSearch(IJuniperElevateSettings settings, ISerializer serializer)
            : base(settings, serializer)
        {
        }

        public override string Source => ThirdParties.JUNIPERELEVATE;
    }
}