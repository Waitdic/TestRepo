using System.Threading.Tasks;
using iVectorOne.Interfaces;
using iVectorOne.Models;
using iVectorOne.Models.Property.Booking;

namespace iVectorOne.Suppliers.PremierInn
{
    public class PremierInn : IThirdParty, ISingleSource
    {
        public string Source { get; }
        public bool SupportsRemarks { get; }
        public bool SupportsBookingSearch { get; }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            throw new System.NotImplementedException();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            throw new System.NotImplementedException();
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            throw new System.NotImplementedException();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            throw new System.NotImplementedException();
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
            throw new System.NotImplementedException();
        }
    }
}
