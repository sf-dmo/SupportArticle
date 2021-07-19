using System.Security.Cryptography;
using System.Text;

namespace ExoBF.Prepared
{
    public abstract class CryptMaterial
    {
        protected byte[] _Mdp { get; }
        protected SymmetricAlgorithm _SymCryptAlog;

        public CryptMaterial(byte[] password)
        {
            _Mdp = MD5.Create().ComputeHash(password);
        }

        public CryptMaterial(string password)
        {
            _Mdp = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public virtual ICryptoTransform GetCrypto()
        {
            return _SymCryptAlog.CreateEncryptor();
        }

        public virtual ICryptoTransform GetDeCrypto()
        {
            return _SymCryptAlog.CreateDecryptor();
        }
    }

    public class AESCryptMaterial : CryptMaterial
    {
        public AESCryptMaterial(byte[] password)
           : base(password)
        {
            _SymCryptAlog = Aes.Create();
            _SymCryptAlog.Key = _Mdp;
            _SymCryptAlog.IV = new byte[16];
            _SymCryptAlog.IV.Initialize();
        }

        public AESCryptMaterial(string password)
            : base(password)
        {
            _SymCryptAlog = Aes.Create();
            _SymCryptAlog.Key = _Mdp;
            _SymCryptAlog.IV = new byte[16];
            _SymCryptAlog.IV.Initialize();
        }

        public override ICryptoTransform GetCrypto()
        {
            return _SymCryptAlog.CreateEncryptor();
        }

        public override ICryptoTransform GetDeCrypto()
        {
            return _SymCryptAlog.CreateDecryptor();
        }
    }
}
