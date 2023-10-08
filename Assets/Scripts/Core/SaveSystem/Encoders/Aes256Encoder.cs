using AthelornTheSorceSmith.Assets.Scripts.Core.SaveSystem.Utilities;
using System.IO;
using System.Security.Cryptography;

namespace AthelornTheSorceSmith.Assets.Scripts.Core.SaveSystem.Encoders
{
    public class Aes256Encoder : ISaveSystemEncoder
    {
        private readonly byte[] Key = (new byte[32] { 0x41, 0x74, 0x68, 0x65, 0x6C, 0x6F, 0x72, 0x6E, 0x54, 0x68, 0x65, 0x53, 0x6F, 0x72, 0x63, 0x65,
                                                      0x53, 0x6D, 0x69, 0x74, 0x68, 0x00, 0x00, 0x00, 0x00, 0x41, 0x74, 0x68, 0x65, 0x6C, 0x6F, 0x72 });

        public byte[] Encode(byte[] data)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = CryptoUtilities.Generate256BitsOfRandomEntropy();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.FlushFinalBlock();
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        public byte[] Decode(byte[] encodedData)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = CryptoUtilities.Generate256BitsOfRandomEntropy();

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(encodedData, 0, encodedData.Length);
                        csDecrypt.FlushFinalBlock();
                        return msDecrypt.ToArray();
                    }
                }
            }
        }
    }
}
