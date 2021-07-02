using Code.Core.KeyStretching;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Code.Core
{
    public class KeyMaterial : IKeyMaterial
    {
        public BufferedBouncyCastleCryptoTransform Cipher { get; private set; }
        private StreamBouncyCastleCryptoTransform _KeyCipher { get; set; }
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


        private void PrepareKey(ref byte[] mdpKey, bool forEncryption)
        {
            byte[] key1 = null;
            try
            {
                IKeyStretching argonStret = new ArgonKeyStretching();
                key1 = argonStret.Stretching(mdpKey, 32);

                XSalsa20Engine sEngine = new XSalsa20Engine();
                sEngine.Init(forEncryption,
                    new ParametersWithIV(
                        new KeyParameter(key1),
                        new byte[24])
                    );

                _KeyCipher = new StreamBouncyCastleCryptoTransform(sEngine);
            }
            finally
            {
                WipeMemory.WipeByte(ref mdpKey);
                WipeMemory.WipeByte(ref key1);
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
                PrepareKey(ref mdpKey, true);

                Random sr = new SecureRandom(new CryptoApiRandomGenerator());

                CipherKeyGenerator keyGenerator = new CipherKeyGenerator();
                keyGenerator.Init(new KeyGenerationParameters(sr as SecureRandom, KEY_DATA_SIZE * 8));
                byte[] key = keyGenerator.GenerateKey();

                keyGenerator.Init(new KeyGenerationParameters(sr as SecureRandom, IV_DATA_SIZE * 8));
                byte[] iv = keyGenerator.GenerateKey();

                IBlockCipher engine = new SicBlockCipher(new AesEngine());

                KeyParameter kp = new KeyParameter(key);
                ParametersWithIV kpvi = new ParametersWithIV(kp, iv);

                engine.Init(true, kpvi);

                Cipher = new BufferedBouncyCastleCryptoTransform(engine);

                plainKeyFile = PackKeyFile(ref key, ref iv);

#if DEBUG
                Array.Copy(plainKeyFile, DEBUG_PlainKeyFile, plainKeyFile.Length);
#endif


                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cipherStream = new CryptoStream(ms, _KeyCipher, CryptoStreamMode.Write))
                    {

                        cipherStream.Write(plainKeyFile);
                        if (cipherStream.HasFlushedFinalBlock)
                            cipherStream.FlushFinalBlock();
                    }

                    ms.Flush();
                    _EncryptedKeyFile = ms.ToArray();
                }
            }
            finally
            {
                WipeMemory.WipeByte(ref plainKeyFile);
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
                PrepareKey(ref mdpKey, true);

                using (MemoryStream ms = new MemoryStream(encryptedKeyFile))
                {
                    using (CryptoStream cipherStream = new CryptoStream(ms, _KeyCipher, CryptoStreamMode.Read))
                    {

                        cipherStream.Read(plainKeyFile);
                        if (cipherStream.HasFlushedFinalBlock)
                            cipherStream.FlushFinalBlock();

                    }

                    ms.Flush();
                }

#if DEBUG
                Array.Copy(plainKeyFile, DEBUG_PlainKeyFile, plainKeyFile.Length);
#endif
                (key, iv) = UnPackKeyFile(ref plainKeyFile);

                IBlockCipher engine = new SicBlockCipher(new AesEngine());
                KeyParameter kp = new KeyParameter(key);
                ParametersWithIV kpvi = new ParametersWithIV(kp, iv);

                engine.Init(false, kpvi);
                Cipher = new BufferedBouncyCastleCryptoTransform(engine);
            }
            finally
            {
                WipeMemory.WipeByte(ref plainKeyFile);
                WipeMemory.WipeByte(ref key);
                WipeMemory.WipeByte(ref iv);
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
