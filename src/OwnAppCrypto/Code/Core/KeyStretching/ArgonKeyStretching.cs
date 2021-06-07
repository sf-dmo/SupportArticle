using SharpHash.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.Core.KeyStretching
{
    public class ArgonKeyStretching : IKeyStretching
    {     
        private static readonly byte[] _DefaultSalt = new byte[] { 0x05, 0x55, 0x25, 0x85, 0x95, 0xA5, 0x5B, 0xFF, 0xE4, 0x49, 0xA4, 0xBA, 0xC4, 0xD5, 0xEE, 0x6F };
        private IArgon2Parameters _Argon2id { get; }

        public ArgonKeyStretching()
        {
            _Argon2id = SharpHash.KDF.Argon2idParametersBuilder.Builder()
                .WithParallelism(8)
                .WithIterations(3)
                .WithSalt(_DefaultSalt)
                .WithSecret(null) // why ? means
                .WithAdditional(null) // why ? means
                .WithVersion(SharpHash.KDF.Argon2Version.a2vARGON2_VERSION_13)
                .WithMemoryAsKB((int)Math.Pow(2, 17))
                .Build();
        }

        public ArgonKeyStretching(int memoryPressure, int parallelizationPressure, int iteration)
        {
            _Argon2id = SharpHash.KDF.Argon2idParametersBuilder.Builder()
                .WithParallelism(parallelizationPressure)
                .WithIterations(iteration)
                .WithSalt(_DefaultSalt)
                .WithSecret(null) // why ? means
                .WithAdditional(null) // why ? means
                .WithVersion(SharpHash.KDF.Argon2Version.a2vARGON2_VERSION_13)
                .WithMemoryAsKB(memoryPressure)
                .Build();
        }

        public byte[] Stretching(byte[] key, int outputSize)
        {
            IPBKDF_Argon2 argon2 = SharpHash.Base.HashFactory.KDF.PBKDF_Argon2.CreatePBKDF_Argon2(key, _Argon2id);
            return argon2.GetBytes(outputSize);
        }

        public byte[] Stretching(byte[] key)
        {
            IPBKDF_Argon2 argon2 = SharpHash.Base.HashFactory.KDF.PBKDF_Argon2.CreatePBKDF_Argon2(key, _Argon2id);
            return argon2.GetBytes(32);
        }
    }
}
