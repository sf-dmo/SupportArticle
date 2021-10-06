using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace ExoBF.Prepared
{
    public abstract class CryptMaterial
    {
        protected ReadOnlyCollection<byte> Password { get; }
        protected SymmetricAlgorithm SymCryptAlog { get; set; }

        protected CryptMaterial(byte[] password)
        {
            Password = new ReadOnlyCollection<byte>(MD5.HashData(password));
        }

        protected CryptMaterial(string password)
        {
            Password = new ReadOnlyCollection<byte>(MD5.HashData(Encoding.UTF8.GetBytes(password)));
        }

        public virtual ICryptoTransform GetCrypto()
        {
            return SymCryptAlog.CreateEncryptor();
        }

        public virtual ICryptoTransform GetDeCrypto()
        {
            return SymCryptAlog.CreateDecryptor();
        }
    }
}
