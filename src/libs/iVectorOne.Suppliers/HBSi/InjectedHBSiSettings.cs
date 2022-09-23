namespace iVectorOne.Suppliers.HBSi
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Support;

    public class InjectedHBSiSettings : SettingsBase, IHBSiSettings
    {
        protected override string Source => string.Empty;

        public string AdditionalRoomTypeInfoValues(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AdditionalRoomTypeInfoValues", tpAttributeSearch, source);
        }

        public string AgentAddress(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentAddress", tpAttributeSearch, source);
        }

        public string AgentCity(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentCity", tpAttributeSearch, source);
        }

        public string AgentCountryCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentCountryCode", tpAttributeSearch, source);
        }

        public string AgentEmailAddress(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentEmailAddress", tpAttributeSearch, source);
        }

        public string AgentPhoneNumber(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentPhoneNumber", tpAttributeSearch, source);
        }

        public string AgentPostCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AgentPostCode", tpAttributeSearch, source);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string ChainCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ChainCode", tpAttributeSearch, source);
        }

        public int ConfirmationCheckRepeatInMs(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ConfirmationCheckRepeatInMs", tpAttributeSearch, source).ToSafeInt();
        }

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Currency", tpAttributeSearch, source);
        }

        public string DirectBillID(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("DirectBillID", tpAttributeSearch, source);
        }

        public bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ExcludeNRF", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("GenericURL", tpAttributeSearch, source);
        }

        public string HotelCodesWithAdditionalRoomTypeInfo(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("HotelCodesWithAdditionalRoomTypeInfo", tpAttributeSearch, source);
        }

        public int MaxConfirmationWaitTimeInSeconds(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MaxConfirmationWaitTimeInSeconds", tpAttributeSearch, source).ToSafeInt();
        }

        public string MaxRoomOccupancy(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MaxRoomOccupancy", tpAttributeSearch, source);
        }

        public bool MealFromDescription(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MealFromDescription", tpAttributeSearch, source).ToSafeBoolean();
        }

        public bool MRFirstRoomOnly(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("MRFirstRoomOnly", tpAttributeSearch, source).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch, source).ToSafeInt();
        }

        public string PackageRateCode(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("PackageRateCode", tpAttributeSearch, source);
        }

        public string PartnerName(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("PartnerName", tpAttributeSearch, source);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Password", tpAttributeSearch, source);
        }

        public string PaymentMethod(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("PaymentMethod", tpAttributeSearch, source);
        }

        public string RatePlanCodes(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("RatePlanCodes", tpAttributeSearch, source);
        }

        public int RequestTimeOutSeconds(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("RequestTimeOutSeconds", tpAttributeSearch, source).ToSafeInt();
        }

        public bool ReturnOpaqueRates(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("ReturnOpaqueRates", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string SalesChannel(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SalesChannel", tpAttributeSearch, source);
        }

        public string SupplierErrata(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("SupplierErrata", tpAttributeSearch, source);
        }

        public string Target(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Target", tpAttributeSearch, source);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UseGZip", tpAttributeSearch, source).ToSafeBoolean();
        }

        public bool UseLeadGuestDetails(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UseLeadGuestDetails", tpAttributeSearch, source).ToSafeBoolean();
        }

        public bool UsePassengerAge(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("UsePassengerAge", tpAttributeSearch, source).ToSafeBoolean();
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("User", tpAttributeSearch, source);
        }

        public string Version(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("Version", tpAttributeSearch, source);
        }

        public bool WaitForSupplierReference(IThirdPartyAttributeSearch tpAttributeSearch, string source)
        {
            return Get_Value("WaitForSupplierReference", tpAttributeSearch, source).ToSafeBoolean();
        }
    }
}
