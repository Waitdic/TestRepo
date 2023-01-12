namespace iVectorOne.Utility
{
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Constants;
    using iVectorOne.Repositories;
    using System.Threading.Tasks;

    public class ProcessorHelpers
    {
        public static async Task<int> GetISOCurrencyID(string source, string currencyCode, int accountId, ICurrencyLookupRepository currencyRepository)
        {
            int currencyId = 0;

            if (!string.IsNullOrWhiteSpace(currencyCode))
            {
                if (IsSingleTenant(source))
                {
                    currencyId = await currencyRepository.AccountCurrencyLookupAsync(accountId, currencyCode);
                }
                else
                {
                    currencyId = await currencyRepository.GetISOCurrencyIDFromSupplierCurrencyCodeAsync(source, currencyCode);
                }
            }

            return currencyId;
        }

        private static bool IsSingleTenant(string source)
            => source.InList(ThirdParties.OWNSTOCK, ThirdParties.CHANNELMANAGER);
    }
}
