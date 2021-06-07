using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Code.Core
{
    public class BufferedBouncyCastleCryptoTransform : ICryptoTransform
    {
        private BufferedBlockCipher _Cipher { get; set; }

        public BufferedBouncyCastleCryptoTransform(IBlockCipher cipher)
        {
            _Cipher = new BufferedBlockCipher(cipher);
        }

        // means ?
        public bool CanReuseTransform => true;

        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => _Cipher.GetBlockSize();

        public int OutputBlockSize => _Cipher.GetBlockSize();

        public void Dispose()
        {
            _Cipher?.Reset();
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            return _Cipher.ProcessBytes(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            return _Cipher.DoFinal(inputBuffer, inputOffset, inputCount);
        }
    }
}
