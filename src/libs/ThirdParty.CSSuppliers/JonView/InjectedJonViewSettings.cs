﻿using Intuitive.Helpers.Extensions;
using ThirdParty.Constants;
using ThirdParty.Search.Settings;
using ThirdParty.Support;

namespace ThirdParty.CSSuppliers.JonView
{

    public class InjectedJonViewSettings : SettingsBase, IJonViewSettings
    {

        private readonly ThirdPartyConfiguration configuration;

        protected override string Source => ThirdParties.JONVIEW;

        public InjectedJonViewSettings(ThirdPartyConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool get_AllowCancellations(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("AllowCancellations", tpAttributeSearch).ToSafeBoolean();
        }

        public int get_OffsetCancellationDays(IThirdPartyAttributeSearch tpAttributeSearch, bool IsMandatory)
        {
            return Get_Value("OffsetCancellationDays", tpAttributeSearch).ToSafeInt();
        }

        public string get_URL(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("URL", tpAttributeSearch);
        }

        public string get_Password(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("Password", tpAttributeSearch);
        }

        public string get_UserID(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("UserID", tpAttributeSearch);
        }

        public string get_ClientLoc(IThirdPartyAttributeSearch tpAttributeSearch)
        {
            return Get_Value("ClientLoc", tpAttributeSearch);
        }
    }
}