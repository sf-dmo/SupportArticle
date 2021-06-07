using Code.Core;
using System.IO;

namespace Code
{
    public class SecureFile
    {
        public void Encrypt(string filename, byte[] key)
        {
            using (KeyMaterial km = new KeyMaterial())
            {
                FileInfo plainfile = new FileInfo(filename);
                PlainTextFile ptf = new PlainTextFile(plainfile, km);
                using (Stream input = ptf.GetValidInputStream())
                {
                    using (Stream output = ptf.GetValidOutputStream(key))
                    {
                        ReadWrite(input, output);
                    }
                }
            }
        }

        public void Decrypt(string filename, byte[] key)
        {
            using (KeyMaterial km = new KeyMaterial())
            {
                FileInfo plainfile = new FileInfo(filename);
                CipherTextFile ctf = new CipherTextFile(plainfile, km);
                using (Stream input = ctf.GetValidInputStream(key))
                {
                    using (Stream output = ctf.GetValidOutputStream())
                    {
                        ReadWrite(input, output);
                    }
                }
            }
        }

        private void ReadWrite(Stream input, Stream output)
        {
            byte[] buffer = new byte[1024];
            int byteRead = 0;
            do
            {
                byteRead = input.Read(buffer);
                if (byteRead != 0)
                    output.Write(buffer, 0, byteRead);
            } while (byteRead > 0);

            input.Flush();
            output.Flush();
        }
    }
}
