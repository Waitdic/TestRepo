namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using System;
    using System.Collections.Generic;
    using Models.Common;
    using ThirdParty.Constants;

    public static class Helper
    {
        public static List<string> iVectorConnectSources = new()
        {
            ThirdParties.BOOKABED,
            ThirdParties.IMPERATORE,
        };

        public static LoginDetails GetLoginDetails(IThirdPartyAttributeSearch searchDetails, IiVectorConnectSettings settings, string source)
        {
            return new LoginDetails
            {
                Login = settings.Login(searchDetails, source),
                Password = settings.Password(searchDetails, source),
                AgentReference = settings.AgentReference(searchDetails, source),
                SellingCurrencyID = settings.SellingCurrencyID(searchDetails, source)
            };
        }

        public static string GetStringToBeHidden(string delimStart, string delimEnd, string warning)
        {
            string stringToBeHidden = string.Empty;
            int loginStartIndex = warning.IndexOf(delimStart, StringComparison.Ordinal);
            int iLoginEndIndex = warning.IndexOf(delimEnd, StringComparison.Ordinal);

            if (loginStartIndex > -1 && iLoginEndIndex > -1)
            {
                stringToBeHidden = warning.Substring(loginStartIndex + delimStart.Length + 1,
                    iLoginEndIndex - loginStartIndex - delimStart.Length);
            }

            return stringToBeHidden;
        }

        public static string RemoveLoginDetailsFromWarnings(string warning)
        {
            string sLoginName = GetStringToBeHidden("<Login>", "</Login>", warning);
            string sPassword = GetStringToBeHidden("<Password>", "</Password>", warning);
            warning = warning.Replace(sLoginName, string.Empty);
            warning = warning.Replace(sPassword, string.Empty);
            return warning;
        }
    }
}
