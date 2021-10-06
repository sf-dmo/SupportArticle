using System;
using System.IO;

namespace ExoBF.Prepared
{
    public static class WFPrepared
    {
        public static void DoSample()
        {
            string fileToEncrypt = Path.Combine(Environment.CurrentDirectory, "FileToEncrypt");
            fileToEncrypt = Path.Combine(fileToEncrypt, "SimpleText.txt");

            AESCrypt.Crypt(fileToEncrypt);

            DotNetCrypt dotNetCrypt = new DotNetCrypt();
            dotNetCrypt.Crypt(fileToEncrypt);

            OwnAppCrypt.Crypt(fileToEncrypt);
        }

        public static void DoCheckSample()
        {
            string[] fileToDecrypt = Directory.GetFiles(Environment.CurrentDirectory, "*.Crypt", SearchOption.AllDirectories);

            foreach(string file in fileToDecrypt)
            {
                switch(Path.GetFileName(file))
                {
                    case AESCrypt.OUT_CRYPT_FILENAME:
                        {
                            AESCrypt.DCrypt(file);
                        }
                        break;
                    case DotNetCrypt.OUT_CRYPT_FILENAME:
                        {
                            DotNetCrypt dcrypt = new DotNetCrypt();
                            dcrypt.DCrypt(file);
                        }
                        break;
                    default:
                        {
                            OwnAppCrypt.DCrypt(file);
                        }
                        break;
                }
            }
        }
    }
}
