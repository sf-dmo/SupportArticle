using Org.BouncyCastle.Crypto.Generators;
using System;

namespace Code.Core.KeyStretching
{
    /// <summary>
    /// https://blog.filippo.io/the-scrypt-parameters/
    /// https://cryptobook.nakov.com/mac-and-key-derivation/scrypt
    /// </summary>
    public class ScryptKeyStretching : IKeyStretching
    {
        private static readonly byte[] _DefaultSalt = new byte[] { 0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xA1, 0xB3, 0xC3, 0xD4, 0xE5, 0xF6 };
        //   N:
        //     CPU/Memory cost parameter. Must be larger than 1, a power of 2 and less than
        //     2^(128 * r / 8)
        //     .
        private int N { get; }
        //   r:
        //     the block size, must be >= 1.
        public const int r = 8; // bit or byte ?
        //   p:
        //     Parallelization parameter. Must be a positive integer less than or equal to
        //     Int32.MaxValue / (128 * r * 8)
        //     .
        private int P { get; }

        // default 2.3 sec on Processeur Intel(R) Core(TM) i7-8550U CPU @ 1.80GHz
        public ScryptKeyStretching()
        {
            N = (int)Math.Pow(2, 19);
            P = 1;
        }

        public ScryptKeyStretching(int memoryPressure, int parallelizationPressure)
        {
            N = memoryPressure;
            P = parallelizationPressure;
        }

        // (int)Math.Pow(8, 2), 128, 32, 64);
        public byte[] Stretching(byte[] key, int outputSize = 64)
        {
            return SCrypt.Generate(key, _DefaultSalt, N, r, P, outputSize);
        }

        // (int)Math.Pow(8, 2), 128, 32, 64);
        public byte[] Stretching(byte[] key)
        {
            return SCrypt.Generate(key, _DefaultSalt, N, r, P, 32);
        }

    }
}
