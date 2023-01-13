namespace iVectorOne_Admin_Api.Features.Helpers
{
    public static class AccountChecker
    {
        public static async Task<Account?> IsTenantAccountValid(AdminContext context, int tenantId, int accountId, CancellationToken cancellationToken)
        {
            return await context.Accounts
                .Where(x => x.TenantId == tenantId && x.AccountId == accountId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}
