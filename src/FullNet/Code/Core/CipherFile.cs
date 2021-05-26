using System.IO;
using System.Security.Cryptography;

namespace Code.Core
{
    public class CipherTextFile
    {
        private FileInfo _Fi { get; }
        private IKeyMaterial _KeyMaterial { get; set; }

        public CipherTextFile(FileInfo fi, IKeyMaterial keyMaterial)
        {
            _Fi = fi;
            _KeyMaterial = keyMaterial;
            if (!fi.Exists)
                throw new FileNotFoundException();
        }

        public Stream GetValidInputStream(byte[] mdpKey)
        {
            FileStream fsIn = new FileStream(_Fi.FullName, FileMode.Open, FileAccess.Read);
            byte[] keyMaterial = new byte[KeyMaterial.KEY_MATERIAL_DATA_SIZE];
            fsIn.Read(keyMaterial);
            _KeyMaterial.Read(ref mdpKey, keyMaterial);

            return fsIn;
        }

        public Stream GetValidOutputStream()
        {
            FileStream fsOut = new FileStream(_Fi.FullName + ".original", FileMode.Create, FileAccess.Write);
            Stream cs = new CryptoStream(fsOut, _KeyMaterial.Cipher.CreateDecryptor(), CryptoStreamMode.Write, false);
            return cs;
        }
    }
}
