namespace iVectorOne.Suppliers
{
    using iVectorOne.Models;
    using iVectorOne.Models.Extra;
    using iVectorOne.Extra;
    using System.Threading.Tasks;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;

    public abstract class TourPlanExtrasBase : IThirdParty, ISingleSource
    {
        public abstract string Source { get; }

        public Task<string> BookAsync(ExtraDetails extraDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<ThirdPartyCancellationResponse> CancelBookingAsync(ExtraDetails extraDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(ExtraDetails extraDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> PreBookAsync(ExtraDetails extraDetails)
        {
            throw new System.NotImplementedException();
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            throw new System.NotImplementedException();
        }

        public bool ValidateSettings(ExtraDetails extraDetails)
        {
            throw new System.NotImplementedException();
        }
    }
}
