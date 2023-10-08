using System.Security.Cryptography;

namespace AthelornTheSorceSmith.Assets.Scripts.Core.SaveSystem.Utilities
{
    public static class CryptoUtilities
    {
        public static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 256 bits
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
