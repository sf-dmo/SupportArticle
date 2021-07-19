using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ExoBF.Prepared
{
    public class DotNetCrypt
    {
        public const string OUT_CRYPT_FILENAME = @"DotNet.Crypt";
        public const string OUT_D_CRYPT_FILENAME = @"DotNet.txt";
        private CryptMaterial _KeyMaterial { get; }

        public DotNetCrypt()
        {
            _KeyMaterial = new AESCryptMaterial(Encoding.UTF8.GetBytes(Password.DOT_NET_MDP));
        }

        public void Crypt(string fullFilename)
        {
            byte[] buf = new byte[4096];
            using (FileStream fsIn = new FileStream(fullFilename, FileMode.Open))
            {
                using (FileStream fsOut = new FileStream(OUT_CRYPT_FILENAME, FileMode.Create))
                {
                    using (CryptoStream cs = new CryptoStream(fsOut, _KeyMaterial.GetCrypto(), CryptoStreamMode.Write))
                    {
                        int byteRead = 0;
                        do
                        {
                            byteRead = fsIn.Read(buf);
                            if (byteRead != 0)
                                cs.Write(buf, 0, byteRead);
                        } while (byteRead > 0);
                    }
                }
            }
        }


        public void DCrypt(string fullFilename)
        {
            byte[] buf = new byte[4096];
            using (FileStream fsIn = new FileStream(fullFilename, FileMode.Open))
            {
                using (FileStream fsOut = new FileStream(OUT_D_CRYPT_FILENAME, FileMode.Create))
                {
                    using (CryptoStream cs = new CryptoStream(fsOut, _KeyMaterial.GetDeCrypto(), CryptoStreamMode.Write))
                    {
                        int byteRead = 0;
                        do
                        {
                            byteRead = fsIn.Read(buf);
                            if (byteRead != 0)
                                cs.Write(buf, 0, byteRead);
                        } while (byteRead > 0);
                    }
                }
            }
        }
    }
}
