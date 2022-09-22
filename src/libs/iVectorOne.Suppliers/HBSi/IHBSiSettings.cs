using System;
using System.Collections.Generic;
using System.Text;

namespace iVectorOne.Suppliers.HBSi
{
    public interface IHBSiSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch);
        string User(IThirdPartyAttributeSearch tpAttributeSearch);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch);
        string Target(IThirdPartyAttributeSearch tpAttributeSearch);
        string Version(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch);
        string Currency(IThirdPartyAttributeSearch tpAttributeSearch);
        bool ReturnOpaqueRates(IThirdPartyAttributeSearch tpAttributeSearch);
        string PartnerName(IThirdPartyAttributeSearch tpAttributeSearch);
        string PaymentMethod(IThirdPartyAttributeSearch tpAttributeSearch);
        string DirectBillID(IThirdPartyAttributeSearch tpAttributeSearch);
        int ConfirmationCheckRepeatInMs(IThirdPartyAttributeSearch tpAttributeSearch);
        int RequestTimeOutSeconds(IThirdPartyAttributeSearch tpAttributeSearch);
        string SalesChannel(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UseLeadGuestDetails(IThirdPartyAttributeSearch tpAttributeSearch);
        bool UsePassengerAge(IThirdPartyAttributeSearch tpAttributeSearch);
        string PackageRateCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentPhoneNumber(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentEmailAddress(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentAddress(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentCity(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentPostCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string AgentCountryCode(IThirdPartyAttributeSearch tpAttributeSearch);
        string SupplierErrata(IThirdPartyAttributeSearch tpAttributeSearch);
        bool WaitForSupplierReference(IThirdPartyAttributeSearch tpAttributeSearch);
        int MaxConfirmationWaitTimeInSeconds(IThirdPartyAttributeSearch tpAttributeSearch);
        string ChainCode(IThirdPartyAttributeSearch tpAttributeSearch);
        bool MRFirstRoomOnly(IThirdPartyAttributeSearch tpAttributeSearch);
        bool MealFromDescription(IThirdPartyAttributeSearch tpAttributeSearch);
        string RatePlanCodes(IThirdPartyAttributeSearch tpAttributeSearch);
        string MaxRoomOccupancy(IThirdPartyAttributeSearch tpAttributeSearch);
        string HotelCodesWithAdditionalRoomTypeInfo(IThirdPartyAttributeSearch tpAttributeSearch);
        string AdditionalRoomTypeInfoValues(IThirdPartyAttributeSearch tpAttributeSearch);
    }
}
