namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Constants;

    public sealed class BookaBedSearch : IVCSearchBase
    {
        public BookaBedSearch(IBookabedSettings settings, ISerializer serializer, ILogger<BookaBedSearch> logger)
            : base(settings, serializer, logger)
        {
        }

        public override string Source => ThirdParties.BOOKABED;
    }
}