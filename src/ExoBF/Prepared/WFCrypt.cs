using System;
using System.IO;

namespace ExoBF.Prepared
{
    public class WFPrepared
    {
        public static void DoSample()
        {
            string fileToEncrypt = Path.Combine(Environment.CurrentDirectory, "FileToEncrypt");
            fileToEncrypt = Path.Combine(fileToEncrypt, "SimpleText.txt");

            AESCrypt aesCrypt = new AESCrypt();
            aesCrypt.Crypt(fileToEncrypt);

            DotNetCrypt dotNetCrypt = new DotNetCrypt();
            dotNetCrypt.Crypt(fileToEncrypt);

            OwnAppCrypt ownAppCrypt = new OwnAppCrypt();
            ownAppCrypt.Crypt(fileToEncrypt);
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
                            AESCrypt dcrypt = new AESCrypt();
                            dcrypt.DCrypt(file);
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
                            OwnAppCrypt dcrypt = new OwnAppCrypt();
                            dcrypt.DCrypt(file);
                        }
                        break;
                }
            }
        }
    }
}
