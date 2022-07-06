namespace ThirdParty.CSSuppliers.AmadeusHotels.Support
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    public class PasswordDigest
    {
        private static int NonceLength => 11;

        public string Password { get; set; }
        public string TimeStamp { get; set; }
        public string Base64Nonce { get; set; }
        public string DigestedPassword { get; set; }

        public PasswordDigest(string password)
        {
            Password = password;
            TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            GenerateNonce();
            DigestPassword();
        }

        private void GenerateNonce()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var base64Nonce = new StringBuilder();
            var random = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[5];
            for (int index = 1; index <= NonceLength; index++)
            {
                random.GetBytes(buffer);
                var iRandomNumber = BitConverter.ToUInt32(buffer, 0);
                var iNonceNumber = Convert.ToInt32(iRandomNumber % validChars.Length);
                base64Nonce.Append(iNonceNumber);
            }

            // The length of base 64 number should be a multiple of 4. 
            int base64NonceLength = base64Nonce.Length % 4;
            if (base64NonceLength > 0)
            {
                for (int i = 1; i <= 4 - base64NonceLength; i++)
                    base64Nonce.Append("1");
            }

            Base64Nonce = base64Nonce.ToString();
        }

        private void DigestPassword()
        {
            byte[] hashedPassword = SHA1Hash(Encoding.UTF8.GetBytes(Password));
            byte[] combinedPassword = BuildCombinedPassword(hashedPassword);
            byte[] hashedCombinedPassword = SHA1Hash(combinedPassword);

            DigestedPassword = Convert.ToBase64String(hashedCombinedPassword);
        }

        private byte[] BuildCombinedPassword(byte[] hashedPassword)
        {
            var combinedPasswordBytes = new List<byte>();

            byte[] nonceBytes = Convert.FromBase64String(Base64Nonce);
            byte[] timeStampBytes = Encoding.UTF8.GetBytes(TimeStamp);

            combinedPasswordBytes.AddRange(nonceBytes);
            combinedPasswordBytes.AddRange(timeStampBytes);
            combinedPasswordBytes.AddRange(hashedPassword);
            
            return combinedPasswordBytes.ToArray();
        }

        private byte[] SHA1Hash(byte[] dataBytes)
        {
            #pragma warning disable SCS0006
            using var sha1 = new SHA1Managed();
            #pragma warning restore SCS0006
            return sha1.ComputeHash(dataBytes);
           
        }
    }
}
