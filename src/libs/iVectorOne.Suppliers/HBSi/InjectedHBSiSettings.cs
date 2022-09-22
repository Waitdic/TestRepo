namespace iVectorOne.Suppliers.HBSi
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne;
    using iVectorOne.Support;

    public class InjectedHBSiSettings : SettingsBase, IHBSiSettings
    {
        protected override string Source => string.Empty;

        public string AdditionalRoomTypeInfoValues(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AdditionalRoomTypeInfoValues", tpAttributeSearch, Source);
        }

        public string AgentAddress(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentAddress", tpAttributeSearch, Source);
        }

        public string AgentCity(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentCity", tpAttributeSearch, Source);
        }

        public string AgentCountryCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentCountryCode", tpAttributeSearch, Source);
        }

        public string AgentEmailAddress(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentEmailAddress", tpAttributeSearch, Source);
        }

        public string AgentPhoneNumber(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentPhoneNumber", tpAttributeSearch, Source);
        }

        public string AgentPostCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AgentPostCode", tpAttributeSearch, Source);
        }

        public bool AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch, Source).ToSafeBoolean();
        }

        public string ChainCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ChainCode", tpAttributeSearch, Source);
        }

        public int ConfirmationCheckRepeatInMs(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ConfirmationCheckRepeatInMs", tpAttributeSearch, Source).ToSafeInt();
        }

        public string Currency(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Currency", tpAttributeSearch, Source);
        }

        public string DirectBillID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("DirectBillID", tpAttributeSearch, Source);
        }

        public bool ExcludeNRF(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ExcludeNRF", tpAttributeSearch, Source).ToSafeBoolean();
        }

        public string GenericURL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("GenericURL", tpAttributeSearch, Source);
        }

        public string HotelCodesWithAdditionalRoomTypeInfo(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("HotelCodesWithAdditionalRoomTypeInfo", tpAttributeSearch, Source);
        }

        public int MaxConfirmationWaitTimeInSeconds(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaxConfirmationWaitTimeInSeconds", tpAttributeSearch, Source).ToSafeInt();
        }

        public string MaxRoomOccupancy(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MaxRoomOccupancy", tpAttributeSearch, Source);
        }

        public bool MealFromDescription(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MealFromDescription", tpAttributeSearch, Source).ToSafeBoolean();
        }

        public bool MRFirstRoomOnly(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("MRFirstRoomOnly", tpAttributeSearch, Source).ToSafeBoolean();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch, Source).ToSafeInt();
        }

        public string PackageRateCode(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PackageRateCode", tpAttributeSearch, Source);
        }

        public string PartnerName(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PartnerName", tpAttributeSearch, Source);
        }

        public string Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch, Source);
        }

        public string PaymentMethod(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("PaymentMethod", tpAttributeSearch, Source);
        }

        public string RatePlanCodes(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RatePlanCodes", tpAttributeSearch, Source);
        }

        public int RequestTimeOutSeconds(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("RequestTimeOutSeconds", tpAttributeSearch, Source).ToSafeInt();
        }

        public bool ReturnOpaqueRates(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ReturnOpaqueRates", tpAttributeSearch, Source).ToSafeBoolean();
        }

        public string SalesChannel(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SalesChannel", tpAttributeSearch, Source);
        }

        public string SupplierErrata(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("SupplierErrata", tpAttributeSearch, Source);
        }

        public string Target(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Target", tpAttributeSearch, Source);
        }

        public bool UseGZip(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseGZip", tpAttributeSearch, Source).ToSafeBoolean();
        }

        public bool UseLeadGuestDetails(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UseLeadGuestDetails", tpAttributeSearch, Source).ToSafeBoolean();
        }

        public bool UsePassengerAge(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UsePassengerAge", tpAttributeSearch, Source).ToSafeBoolean();
        }

        public string User(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("User", tpAttributeSearch, Source);
        }

        public string Version(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Version", tpAttributeSearch, Source);
        }

        public bool WaitForSupplierReference(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("WaitForSupplierReference", tpAttributeSearch, Source).ToSafeBoolean();
        }
    }
}
