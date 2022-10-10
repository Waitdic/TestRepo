using System;
using System.Collections.Generic;
using System.Text;

namespace iVectorOne.Suppliers.HBSi
{
    public interface IHBSiSettings
    {
        string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string User(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Target(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SupplierName(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string Version(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool ReturnOpaqueRates(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string PartnerName(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string PaymentMethod(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int ConfirmationCheckRepeatInMs(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int RequestTimeOutSeconds(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SalesChannel(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool UseLeadGuestDetails(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool UsePassengerAge(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string PackageRateCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string AgentPhoneNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string AgentEmailAddress(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string AgentAddress(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string AgentCity(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string AgentPostCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string AgentCountryCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string SupplierErrata(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool WaitForSupplierReference(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        int MaxConfirmationWaitTimeInSeconds(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string ChainCode(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool MRFirstRoomOnly(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        bool MealFromDescription(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string RatePlanCodes(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string MaxRoomOccupancy(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string HotelCodesWithAdditionalRoomTypeInfo(IThirdPartyAttributeSearch tpAttributeSearch, string source);
        string AdditionalRoomTypeInfoValues(IThirdPartyAttributeSearch tpAttributeSearch, string source);
    }
}
