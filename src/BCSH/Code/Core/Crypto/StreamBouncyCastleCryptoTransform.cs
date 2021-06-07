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
    public class StreamBouncyCastleCryptoTransform : ICryptoTransform
    {
        private BufferedStreamCipher _Cipher { get; set; }

        public StreamBouncyCastleCryptoTransform(IStreamCipher cipher)
        {
            _Cipher = new BufferedStreamCipher(cipher);
        }

        // means ?
        public bool CanReuseTransform => true;

        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => 1;

        public int OutputBlockSize => 1;

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
