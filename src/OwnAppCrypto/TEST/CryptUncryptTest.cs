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
            FileInfo ficipherText = new FileInfo(filename + ".crypt");

            Assert.IsTrue(fiPlain.Exists);
            Assert.IsTrue(ficipherText.Exists);
            // don't forget padding file with CBC mode
            Assert.IsTrue(ficipherText.Length == (fiPlain.Length + KeyMaterial.KEY_MATERIAL_DATA_SIZE));
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
        public void Uncrypt_File_With_One_Char_Modified()
        {
            byte[] mdp = DataPath.Mdp;
            string filename = DataPath.MODIFIED_ONE_CHAR_CRYPT;
            SecureFile sf = new SecureFile();
            sf.Decrypt(filename, mdp);
            FileInfo fiPlain = new FileInfo(DataPath.SIMPLE_PLAIN_TEXT);
            FileInfo fiCrypt = new FileInfo(DataPath.MODIFIED_ONE_CHAR_UNCRYPT);
            Assert.IsTrue(fiCrypt.Exists);
            Assert.IsTrue(fiCrypt.Length == fiPlain.Length);

            byte[] plain = File.ReadAllBytes(DataPath.SIMPLE_PLAIN_TEXT);
            byte[] cryptThenPlain = File.ReadAllBytes(DataPath.MODIFIED_ONE_CHAR_UNCRYPT);
            int nbDiff = 0;
            for (int i = 0; i < plain.Length; i++)
            {
                if (plain[i] != cryptThenPlain[i])
                    nbDiff++;
            }
            Assert.IsTrue(nbDiff == 1);
        }

        [TestMethod]
        public void Uncrypt_File_Without_Header()
        {
            byte[] mdp = DataPath.Mdp;
            string filename = DataPath.FILE_CRYPT_ONLY_CRYPT;
            SecureFile sf = new SecureFile();
            sf.Decrypt(filename, mdp);

            FileInfo fiCrypt = new FileInfo(DataPath.FILE_CRYPT_ONLY_UNCRYPT);
            Assert.IsTrue(fiCrypt.Exists);
            Assert.IsTrue(fiCrypt.Length == 0); // header == 48, file without header == 20 length.
        }

        [TestMethod]
        public void Uncrypt_Modified_key_only()
        {
            byte[] mdp = DataPath.Mdp;
            string filename = DataPath.MODIFIED_KEY_ONLY_CRYPT;
            SecureFile sf = new SecureFile();
            sf.Decrypt(filename, mdp);

            FileInfo fiPlain = new FileInfo(DataPath.SIMPLE_PLAIN_TEXT);
            FileInfo fiCrypt = new FileInfo(DataPath.MODIFIED_KEY_ONLY_UNCRYPT);
            Assert.IsTrue(fiCrypt.Exists);
            Assert.IsTrue(fiCrypt.Length == fiPlain.Length);
            Assert.IsTrue(fiCrypt.Length == fiPlain.Length);
            Assert.IsFalse(File.ReadAllText(fiCrypt.FullName).SequenceEqual(File.ReadAllText(fiPlain.FullName)));
        }

        [TestMethod]
        public void Uncrypt_With_Bad_Mdp()
        {
            SecureFile sf = new SecureFile();
            sf.Encrypt(DataPath.SIMPLE_PLAIN_TEXT, DataPath.Mdp);

            sf.Decrypt(DataPath.SIMPLE_PLAIN_TEXT_CRYPT, DataPath.BadMdp);

            FileInfo fiPlain = new FileInfo(DataPath.SIMPLE_PLAIN_TEXT);
            FileInfo fiCrypt = new FileInfo(DataPath.SIMPLE_PLAIN_TEXT_UNCRYPT);
            Assert.IsTrue(fiCrypt.Exists);
            Assert.IsTrue(fiCrypt.Length == fiPlain.Length);
        }

        [TestMethod]
        public void AES_SIV_Is_StreamEncryption()
        {
            SecureFile sf = new SecureFile();
            sf.Encrypt(DataPath.SIMPLE_PLAIN_TEXT, DataPath.Mdp);

            using (FileStream fs = File.Open(DataPath.SIMPLE_PLAIN_TEXT_CRYPT, FileMode.Open))
            {
                fs.Seek(65, SeekOrigin.Begin);
                byte br = (byte)fs.ReadByte();
                fs.Seek(-1, SeekOrigin.Current);
                fs.WriteByte((byte)((br == 0x09) ? 0x0A : 0x09));
            }

            sf.Decrypt(DataPath.SIMPLE_PLAIN_TEXT_CRYPT, DataPath.Mdp);

            FileInfo fiPlain = new FileInfo(DataPath.SIMPLE_PLAIN_TEXT);
            FileInfo fiCrypt = new FileInfo(DataPath.SIMPLE_PLAIN_TEXT_UNCRYPT);
            Assert.IsTrue(fiCrypt.Exists);
            Assert.IsTrue(fiCrypt.Length == fiPlain.Length);
            byte[] plain = File.ReadAllBytes(DataPath.SIMPLE_PLAIN_TEXT);
            byte[] cryptThenPlain = File.ReadAllBytes(DataPath.SIMPLE_PLAIN_TEXT_UNCRYPT);
            int nbDiff = 0;
            for(int i =0; i < plain.Length; i++)
            {
                if (plain[i] != cryptThenPlain[i])
                    nbDiff++;
            }
            Assert.IsTrue(nbDiff == 1);
        }
    }
}
