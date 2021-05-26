using System;
using System.Security.Cryptography;

namespace Code.Core
{
    public interface IKeyMaterial : IDisposable
    {
        SymmetricAlgorithm Cipher { get; }
        byte[] EncryptedKeyFile { get; }

        void Read(ref byte[] mdpKey, byte[] encryptedKeyFile);
        void Write(ref byte[] mdpKey);
    }
}