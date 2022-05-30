namespace ThirdParty.CSSuppliers.iVectorConnect
{
    using System;
    using Models.Common;

    public static class Helper
    {
        public static LoginDetails GetLoginDetails(IThirdPartyAttributeSearch searchDetails, IIVectorConnectSettings settings)
        {
            return new LoginDetails
            {
                Login = settings.Login(searchDetails),
                Password = settings.Password(searchDetails),
                AgentReference = settings.AgentReference(searchDetails),
                SellingCurrencyID = settings.SellingCurrencyID(searchDetails)
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
