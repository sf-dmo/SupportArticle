using System.Text;

namespace TEST
{
    public static class DataPath
    {
        public const string DIRECTORY = "Data";

        public const string SIMPLE_PLAIN_TEXT = DIRECTORY + @"\SimplePlainText.txt";
        public const string SIMPLE_PLAIN_TEXT_CRYPT = DIRECTORY + @"\SimplePlainText.txt.crypt";
        public const string SIMPLE_PLAIN_TEXT_UNCRYPT = DIRECTORY + @"\SimplePlainText.txt.crypt.original";

        public const string FILE_CRYPT_ONLY_CRYPT = DIRECTORY + @"\FileCryptOnly.crypt";
        public const string FILE_CRYPT_ONLY_UNCRYPT = DIRECTORY + @"\FileCryptOnly.crypt.original";

        public const string MODIFIED_ONE_CHAR_CRYPT = DIRECTORY + @"\ModifiedOneChar.txt.crypt";
        public const string MODIFIED_ONE_CHAR_UNCRYPT = DIRECTORY + @"\ModifiedOneChar.txt.crypt.original";

        public const string MODIFIED_KEY_ONLY_CRYPT = DIRECTORY + @"\ModifiedKeyOnly.crypt";
        public const string MODIFIED_KEY_ONLY_UNCRYPT = DIRECTORY + @"\ModifiedKeyOnly.crypt.original";


        public static byte[] Mdp { get { return Encoding.ASCII.GetBytes("un mot de passe"); } }
        public static byte[] BadMdp { get { return Encoding.ASCII.GetBytes("un mot de passe pas bon"); } }
    }
}
