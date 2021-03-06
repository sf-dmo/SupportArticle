using System.Security.Cryptography;

namespace Code.Core.KeyStretching
{
    public class PBKDF2KeyStretching : IKeyStretching
    {
        public const int DEFAULT_ITERATION_COUNT = 1500000; // > 1 sec  < 1.5 sec
        public const int DEFAULT_OUTPUT_SIZE = 32; // 256 bits
        public readonly byte[] DEFAULT_SALT = new byte[] { 0x45, 0x35, 0x26, 0x80, 0x25, 0xA1, 0x5B, 0xEF, 0xE4, 0x49, 0xA4, 0xBA, 0xC4, 0xD5, 0xCE, 0x6F };

        /// <summary>
        /// Permet d'étendre le mot de passe
        /// </summary>
        /// <param name="key">Mot de passe à étendre</param>
        /// <param name="salt">Données aléatoires</param>
        /// <param name="iteration">Le nombre d'itération</param>
        /// <param name="outputSize">Le nombre d'octet souhaité en sortie</param>
        /// <returns></returns>
        public byte[] Stretching(byte[] key, byte[] salt, int iteration, int outputSize)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(key, salt, iteration))
            {
                return pbkdf2.GetBytes(outputSize);
            }
        }

        public byte[] Stretching(byte[] key, byte[] salt, int iteration)
        {
            return Stretching(key, salt, iteration, DEFAULT_OUTPUT_SIZE);
        }

        public byte[] Stretching(byte[] key, byte[] salt)
        {
            return Stretching(key, salt, DEFAULT_ITERATION_COUNT, DEFAULT_OUTPUT_SIZE);
        }

        public byte[] Stretching(byte[] key, int outputsize)
        {
            return Stretching(key, DEFAULT_SALT, DEFAULT_ITERATION_COUNT, outputsize);
        }

        public byte[] Stretching(byte[] key)
        {
            return Stretching(key, DEFAULT_SALT, DEFAULT_ITERATION_COUNT, DEFAULT_OUTPUT_SIZE);
        }
    }
}
