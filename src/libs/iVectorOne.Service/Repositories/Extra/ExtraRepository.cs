namespace iVectorOne.Repositories
{
    using Intuitive.Data;
    using iVectorOne.SDK.V2.ExtraContent;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// A repository for returning extras from the database
    /// </summary>
    public class ExtraRepository : IExtraRepository
    {
        private readonly ISql _sql;
        public ExtraRepository(ISql sql)
        {
            _sql = sql;
        }

        /// <inheritdoc />
        public async Task<List<Extra>> GetAllExtras(string source)
        {
            var extras = await _sql.ReadSingleMappedAsync(
                "select ExtraID, ExtraName from Extra where Source = @source",
                async r => (await r.ReadAllAsync<Extra>()).ToList(),
            new CommandSettings().WithParameters(new { source }));
            return extras;
        }
    }
}
