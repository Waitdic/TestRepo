namespace iVectorOne.Web.Adaptors.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Newtonsoft.Json;
    using ThirdParty.Models;

    public class FileAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private List<Subscription> _users = new();
        private bool _initialised = false;

        public FileAuthenticationProvider(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Subscription> Authenticate(string username, string password)
        {
            if (!_initialised)
            {
                string userFile = Path.Combine(_webHostEnvironment.ContentRootPath, "TempJson\\users.json");

                byte[] result;

                using (var SourceStream = File.Open(userFile, FileMode.Open))
                {
                    result = new byte[SourceStream.Length];
                    await SourceStream.ReadAsync(result.AsMemory(0, (int)SourceStream.Length));
                }

                var users = JsonConvert.DeserializeObject<List<Subscription>>(System.Text.Encoding.ASCII.GetString(result));
                if (users != null)
                {
                    _users = users;
                }

                _initialised = true;
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("No login or password provided");
            }

            return _users.FirstOrDefault(x => x.Login == username && x.Password == password)!;
        }
    }
}
