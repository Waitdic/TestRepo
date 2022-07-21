namespace iVectorOne.Web.Adaptors.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Data;
    using Intuitive.Helpers.Extensions;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using iVectorOne.Models;
    using iVectorOne.Search.Settings;

    public class SqlAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IMemoryCache _cache;
        private readonly ISql _sql;

        public SqlAuthenticationProvider(IMemoryCache cache, ISql sql)
        {
            _cache = Ensure.IsNotNull(cache, nameof(cache));
            _sql = Ensure.IsNotNull(sql, nameof(sql));
        }

        public async Task<Subscription> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("No login or password provided");
            }

            var hashedPassword = GetHash(password);
            string cacheKey = "User_" + username + "_" + hashedPassword;
            
            async Task<List<Subscription>> cacheBuilder()
            {
                var json = await _sql.ReadScalarAsync<string>("exec Get_Configurations");
                var sqlSubscriptions = JsonConvert.DeserializeObject<List<SqlSubscription>>(json);

                return MapSqlUser(sqlSubscriptions).ToList();
            }

            var users = await _cache.GetOrCreateAsync(cacheKey, cacheBuilder, 60);

            return users.FirstOrDefault((o) => o.Login == username && o.Password == hashedPassword)!;
        }

        /// <summary>
        /// Maps user details returned from databse with the user model
        /// </summary>
        /// <param name="sqlSubscriptions"> A list of user details returned from database</param>
        /// <returns>A list of users</returns>
        private IEnumerable<Subscription> MapSqlUser(List<SqlSubscription>? sqlSubscriptions)
        {
            if (sqlSubscriptions == null)
            {
                yield break;
            }

            foreach (var sqlSubscription in sqlSubscriptions)
            {
                var subscription = new Subscription()
                {
                    SubscriptionID = sqlSubscription.SubscriptionID,
                    Login = sqlSubscription.Login!,
                    Password = sqlSubscription.Password!,
                    Environment = sqlSubscription.Environment!,
                    TPSettings = sqlSubscription.TPSettings!,
                };

                var configs = sqlSubscription.Configurations
                    .Select(x => new ThirdPartyConfiguration
                        {
                            Supplier = x.Supplier!,
                            Configurations = x.Attributes!
                                .ToDictionary(x => x.AttributeName!, x => x.AttributeValue!)
                        })
                    .ToList();

                subscription.Configurations = configs;
                yield return subscription;
            }
        }

        /// <summary>
        /// Calculates a hash value of provided input
        /// </summary>
        /// <param name="input">The input to be hashed</param>
        /// <returns>A hashed input</returns>
        private string GetHash(string input)
        {
            var sb = new StringBuilder();
            using (var sha256Hash = SHA256.Create())
            {
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }
            }

            return sb.ToString();
        }
    }
}