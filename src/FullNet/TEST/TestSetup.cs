using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;

namespace TEST
{
    [TestClass]
    public abstract class CryptUncryptTestSetup
    {
        private void SecureDelete(string filename)
        {
            int nbTry = 0;
        start:
            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                }
                catch (IOException io) when (io.Message.StartsWith("The process cannot access the file"))
                {
                    if (nbTry < 3)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        nbTry++;
                        goto start;
                    }
                }
            }
        }

        [TestInitialize]
        public void InitTest()
        {
            SecureDelete(DataPath.SIMPLE_PLAIN_TEXT_CRYPT);
            SecureDelete(DataPath.SIMPLE_PLAIN_TEXT_UNCRYPT);
            SecureDelete(DataPath.FILE_CRYPT_ONLY_UNCRYPT);
            SecureDelete(DataPath.MODIFIED_ONE_CHAR_UNCRYPT);
            SecureDelete(DataPath.MODIFIED_KEY_ONLY_UNCRYPT);
        }
    }
}
