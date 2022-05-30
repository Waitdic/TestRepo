namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public sealed class ImperatoreSearch : IVCSearchBase
    {
        public ImperatoreSearch(IImperatoreSettings settings, ISerializer serializer, ILogger<ImperatoreSearch> logger)
            : base(settings, serializer, logger)
        {
        }

        public override string Source => ThirdParties.IMPERATORE;
    }
}