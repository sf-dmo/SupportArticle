using Code;
using Code.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace TEST
{
    [TestClass]
    public class CryptUncryptTest : CryptUncryptTestSetup
    {
        [TestMethod]
        public void mdp_must_be_clear_as_soons_as_possible()
        {
            byte[] mdp = DataPath.Mdp;
            string filename = DataPath.SIMPLE_PLAIN_TEXT;
            SecureFile sf = new SecureFile();
            sf.Encrypt(filename, mdp);
            Assert.IsNotNull(mdp);
            Assert.IsTrue(mdp.All((by) => by == 0));

            mdp = DataPath.Mdp;
            sf.Decrypt(DataPath.SIMPLE_PLAIN_TEXT_CRYPT, mdp);
            Assert.IsNotNull(mdp);
            Assert.IsTrue(mdp.All((by) => by == 0));
        }

        [TestMethod]
        public void cipherText_length_must_be_greater_than_plainText_length_of_KEY_MATERIAL_DATA_SIZE_bytes()
        {
            byte[] mdp = DataPath.Mdp;
            string filename = DataPath.SIMPLE_PLAIN_TEXT;
            SecureFile sf = new SecureFile();
            sf.Encrypt(filename, mdp);

            FileInfo fiPlain = new FileInfo(filename);
            long mod = fiPlain.Length % 16;
            long plainTextPaddLength = fiPlain.Length + (mod == 0 ? 0 : 16 - mod);
            FileInfo ficipherText = new FileInfo(filename + ".crypt");

            Assert.IsTrue(fiPlain.Exists);
            Assert.IsTrue(ficipherText.Exists);
            // don't forget padding file with CBC mode
            Assert.IsTrue(ficipherText.Length == (plainTextPaddLength + KeyMaterial.KEY_MATERIAL_DATA_SIZE));
        }

        [TestMethod]
        public void Uncipher_cipherText_must_be_same_like_plainText()
        {
            byte[] mdp = DataPath.Mdp;
            string filename = DataPath.SIMPLE_PLAIN_TEXT;
            SecureFile sf = new SecureFile();
            sf.Encrypt(filename, mdp);
            mdp = DataPath.Mdp;
            sf.Decrypt(DataPath.SIMPLE_PLAIN_TEXT_CRYPT, mdp);

            FileInfo fiPlain = new FileInfo(filename);
            FileInfo fiUncipherText = new FileInfo(DataPath.SIMPLE_PLAIN_TEXT_UNCRYPT);

            Assert.IsTrue(fiPlain.Exists);
            Assert.IsTrue(fiUncipherText.Exists);

            Assert.IsTrue(fiPlain.Length == fiUncipherText.Length);
            Assert.IsTrue(fiPlain.Length < 1 * 1024 * 1024);

            Assert.IsTrue(File.ReadAllBytes(fiPlain.FullName).SequenceEqual(File.ReadAllBytes(fiUncipherText.FullName)));
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void Uncrypt_File_With_One_Char_Modified()
        {
            byte[] mdp = DataPath.Mdp;
            string filename = DataPath.MODIFIED_ONE_CHAR_CRYPT;
            SecureFile sf = new SecureFile();
            sf.Decrypt(filename, mdp);

            Assert.IsFalse(File.Exists(DataPath.MODIFIED_ONE_CHAR_UNCRYPT));
        }

        [TestMethod]
        public void Uncrypt_File_Without_Header()
        {
            byte[] mdp = DataPath.Mdp;
            string filename = DataPath.FILE_CRYPT_ONLY_CRYPT;
            SecureFile sf = new SecureFile();
            sf.Decrypt(filename, mdp);
            FileInfo fi = new FileInfo(DataPath.FILE_CRYPT_ONLY_UNCRYPT);
            Assert.IsTrue(fi.Exists);
            Assert.IsTrue(fi.Length == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void Uncrypt_Modified_key_only()
        {
            byte[] mdp = DataPath.Mdp;
            string filename = DataPath.MODIFIED_KEY_ONLY_CRYPT;
            SecureFile sf = new SecureFile();
            sf.Decrypt(filename, mdp);
            FileInfo fi = new FileInfo(DataPath.MODIFIED_KEY_ONLY_UNCRYPT);
            Assert.IsTrue(fi.Exists);
            Assert.IsTrue(fi.Length == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void Uncrypt_With_Bad_Mdp()
        {
            SecureFile sf = new SecureFile();
            sf.Encrypt(DataPath.SIMPLE_PLAIN_TEXT, DataPath.Mdp);

            sf.Decrypt(DataPath.SIMPLE_PLAIN_TEXT_CRYPT, DataPath.BadMdp);
        }
    }
}
