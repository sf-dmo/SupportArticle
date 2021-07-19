using System.Diagnostics;

namespace ExoBF.Prepared
{
    public class AESCrypt
    {
        public const string EXE_PATH = @"C:\Program Files\AESCrypt\aescrypt.exe";
        public const string OUT_CRYPT_FILENAME = @"AESCrypt.Crypt";
        public const string CRYPT_FORMAT = @" -e -p {0} -o {1} {2}";
        public const string OUT_D_CRYPT_FILENAME = @"AESCrypt.txt";
        public const string D_CRYPT_FORMAT = @" -d -p {0} -o {1} {2}";

        public void Crypt(string fullFilename)
        {
            Process aesCrypt = new Process();
            aesCrypt.StartInfo = new ProcessStartInfo();
            aesCrypt.StartInfo.FileName = EXE_PATH;
            aesCrypt.StartInfo.Arguments = string.Format(CRYPT_FORMAT, Password.AESCrypt_MDP, OUT_CRYPT_FILENAME, fullFilename);
            aesCrypt.Start();
            aesCrypt.WaitForExit();
        }


        public void DCrypt(string fullFilename)
        {
            Process aesCrypt = new Process();
            aesCrypt.StartInfo = new ProcessStartInfo();
            aesCrypt.StartInfo.FileName = EXE_PATH;
            aesCrypt.StartInfo.Arguments = string.Format(D_CRYPT_FORMAT, Password.AESCrypt_MDP, OUT_D_CRYPT_FILENAME, fullFilename);
            aesCrypt.Start();
            aesCrypt.WaitForExit();
        }
    }
}
