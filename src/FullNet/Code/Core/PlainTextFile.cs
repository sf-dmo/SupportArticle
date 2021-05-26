using System;
using System.IO;
using System.Security.Cryptography;

namespace Code.Core
{
    public class PlainTextFile
    {
        private FileInfo _Fi { get; }
        public long MAX_FILE_SIZE = 1099511627776; // 1TO
        private IKeyMaterial _KeyMaterial { get; set; }

        public PlainTextFile(FileInfo fi, IKeyMaterial keyMaterial)
        {
            _Fi = fi;
            _KeyMaterial = keyMaterial;
            if (!fi.Exists)
                throw new FileNotFoundException();
            if (fi.Length > MAX_FILE_SIZE)
                throw new InvalidOperationException("File size > 1 TO");
        }

        public Stream GetValidInputStream()
        {
            return new FileStream(_Fi.FullName, FileMode.Open, FileAccess.Read);
        }

        public Stream GetValidOutputStream(byte[] mdpKey)
        {
            _KeyMaterial.Write(ref mdpKey);
            FileStream fsOut = new FileStream(_Fi.FullName + ".crypt", FileMode.Create, FileAccess.Write);
            fsOut.Write(_KeyMaterial.EncryptedKeyFile);
            fsOut.Flush();

            // Target
            Stream cs = new CryptoStream(fsOut, _KeyMaterial.Cipher.CreateEncryptor(), CryptoStreamMode.Write, false);
            return cs;
        }
    }
}
