using System.Linq;
using System.Security.Cryptography;

namespace ExoBF.Prepared
{
    public class AESCryptMaterial : CryptMaterial
    {
        public AESCryptMaterial(byte[] password)
           : base(password)
        {
            SymCryptAlog = Aes.Create();
            SymCryptAlog.Key = Password.ToArray();
            SymCryptAlog.IV = new byte[16];
            SymCryptAlog.IV.Initialize();
        }

        public AESCryptMaterial(string password)
            : base(password)
        {
            SymCryptAlog = Aes.Create();
            SymCryptAlog.Key = Password.ToArray();
            SymCryptAlog.IV = new byte[16];
            SymCryptAlog.IV.Initialize();
        }

        public override ICryptoTransform GetCrypto()
        {
            return SymCryptAlog.CreateEncryptor();
        }

        public override ICryptoTransform GetDeCrypto()
        {
            return SymCryptAlog.CreateDecryptor();
        }
    }
}
