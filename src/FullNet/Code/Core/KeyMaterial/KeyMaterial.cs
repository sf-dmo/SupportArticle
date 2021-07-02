using Code.Core.KeyStretching;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Code.Core
{
    public class KeyMaterial : IKeyMaterial
    {
        public SymmetricAlgorithm Cipher { get; private set; }
        private SymmetricAlgorithm _KeyCipher { get; set; }
        private byte[] _EncryptedKeyFile;
        public byte[] EncryptedKeyFile { get { return _EncryptedKeyFile; } }

        public const int KEY_MATERIAL_DATA_SIZE = KEY_DATA_SIZE + IV_DATA_SIZE;
        public const int KEY_DATA_SIZE = 32;
        public const int IV_DATA_SIZE = 16;


#if DEBUG
        public byte[] DEBUG_PlainKeyFile = new byte[KEY_MATERIAL_DATA_SIZE];
#endif
        public KeyMaterial()
        {
            _EncryptedKeyFile = new byte[KEY_MATERIAL_DATA_SIZE];
        }


        private void PrepareKey(ref byte[] mdpKey)
        {
            byte[] key0 = null;
            byte[] aesKey = new byte[KEY_DATA_SIZE];
            byte[] aesIV = new byte[IV_DATA_SIZE];
            try
            {
                IKeyStretching pBKDF2Key = new PBKDF2KeyStretching();

                key0 = pBKDF2Key.Stretching(mdpKey, KEY_MATERIAL_DATA_SIZE);
                Array.Copy(key0, aesKey, KEY_DATA_SIZE);
                Array.Copy(key0, KEY_DATA_SIZE, aesIV, 0, IV_DATA_SIZE);
                Aes aesEngine = AesCng.Create();
                aesEngine.Key = aesKey;
                aesEngine.IV = aesIV;
                aesEngine.Mode = CipherMode.CBC;
                aesEngine.Padding = PaddingMode.None;
                _KeyCipher = aesEngine;
            }
            finally
            {
                WipeMemory.WipeByte(ref mdpKey);
                WipeMemory.WipeByte(ref key0);
                WipeMemory.WipeByte(ref aesKey);
                WipeMemory.WipeByte(ref aesIV);
            }
        }

        private byte[] PackKeyFile(ref byte[] key, ref byte[] iv)
        {
            try
            {
                byte[] keyFile = new byte[KEY_MATERIAL_DATA_SIZE];
                Array.Copy(key, keyFile, KEY_DATA_SIZE);
                Array.Copy(iv, 0, keyFile, KEY_DATA_SIZE, IV_DATA_SIZE);
                return keyFile;
            }
            finally
            {
                WipeMemory.WipeByte(ref key);
                WipeMemory.WipeByte(ref iv);
            }
        }

        private (byte[], byte[]) UnPackKeyFile(ref byte[] keyFile)
        {
            try
            {
                byte[] key = new byte[KEY_DATA_SIZE];
                byte[] iv = new byte[IV_DATA_SIZE];
                Array.Copy(keyFile, key, KEY_DATA_SIZE);
                Array.Copy(keyFile, KEY_DATA_SIZE, iv, 0, IV_DATA_SIZE);
                return (key, iv);
            }
            finally
            {
                WipeMemory.WipeByte(ref keyFile);
            }
        }


        public void Write(ref byte[] mdpKey)
        {
            byte[] plainKeyFile = null;
            try
            {
                PrepareKey(ref mdpKey);

                Aes aesEngine = AesCng.Create();
                aesEngine.GenerateKey();
                aesEngine.GenerateIV();
                aesEngine.Mode = CipherMode.CBC;
                aesEngine.Padding = PaddingMode.PKCS7;
                Cipher = aesEngine;

                byte[] key = new byte[KEY_DATA_SIZE];
                byte[] iv = new byte[IV_DATA_SIZE];
                Array.Copy(aesEngine.Key, key, KEY_DATA_SIZE);
                Array.Copy(aesEngine.IV, iv, IV_DATA_SIZE);

                plainKeyFile = PackKeyFile(ref key, ref iv);

#if DEBUG
                Array.Copy(plainKeyFile, DEBUG_PlainKeyFile, plainKeyFile.Length);
#endif

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cipherStream = new CryptoStream(ms, _KeyCipher.CreateEncryptor(), CryptoStreamMode.Write))
                    {

                        cipherStream.Write(plainKeyFile);
                        if (!cipherStream.HasFlushedFinalBlock)
                            cipherStream.FlushFinalBlock();
                    }
                    ms.Flush();
                    _EncryptedKeyFile = ms.ToArray();
                }
            }
            finally
            {
                WipeMemory.WipeByte(ref plainKeyFile);
                _KeyCipher.Clear();
                _KeyCipher.Dispose();
                _KeyCipher = null;
            }
        }

        public void Read(ref byte[] mdpKey, byte[] encryptedKeyFile)
        {
            byte[] plainKeyFile = new byte[KEY_MATERIAL_DATA_SIZE];
            byte[] key = null;
            byte[] iv = null;
            try
            {
                PrepareKey(ref mdpKey);

                using (MemoryStream ms = new MemoryStream(encryptedKeyFile))
                {
                    using (CryptoStream cipherStream = new CryptoStream(ms, _KeyCipher.CreateDecryptor(), CryptoStreamMode.Read))
                    {

                        cipherStream.Read(plainKeyFile);
                        if (!cipherStream.HasFlushedFinalBlock)
                            cipherStream.FlushFinalBlock();
                    }

                    ms.Flush();
                }


#if DEBUG
                Array.Copy(plainKeyFile, DEBUG_PlainKeyFile, plainKeyFile.Length);
#endif
                (key, iv) = UnPackKeyFile(ref plainKeyFile);

                Aes aesEngine = AesCng.Create();
                aesEngine.Key = key;
                aesEngine.IV = iv;
                aesEngine.Mode = CipherMode.CBC;
                aesEngine.Padding = PaddingMode.PKCS7;
                Cipher = aesEngine;
            }
            finally
            {
                WipeMemory.WipeByte(ref plainKeyFile);
                WipeMemory.WipeByte(ref key);
                WipeMemory.WipeByte(ref iv);
                _KeyCipher.Clear();
                _KeyCipher.Dispose();
                _KeyCipher = null;
            }
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Cipher?.Dispose();
                    _KeyCipher?.Dispose();
                    WipeMemory.WipeByte(ref _EncryptedKeyFile);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
