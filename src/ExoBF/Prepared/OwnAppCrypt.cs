using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ExoBF.Prepared
{
    public class OwnAppCrypt
    {
        public const string OUT_CRYPT_FILENAME = @"OwnApp.Crypt";

        public void Crypt(string fullFilename)
        {
            Code.SecureFile s = new Code.SecureFile();
            s.Encrypt(fullFilename, Encoding.UTF8.GetBytes(Password.OWN_APP_CRYPT_MDP));
        }


        public void DCrypt(string fullFilename)
        {
            Code.SecureFile s = new Code.SecureFile();
            s.Decrypt(fullFilename, Encoding.UTF8.GetBytes(Password.OWN_APP_CRYPT_MDP));
        }
    }
}
