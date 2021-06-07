using System;

namespace Code.Core
{
    public interface IKeyMaterial : IDisposable
    {
        BufferedBouncyCastleCryptoTransform Cipher { get; }
        byte[] EncryptedKeyFile { get; }

        void Read(ref byte[] mdpKey, byte[] encryptedKeyFile);
        void Write(ref byte[] mdpKey);
    }
}