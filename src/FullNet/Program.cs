using Code;
using System;
using System.Text;

namespace HowTo
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] mdp = Encoding.ASCII.GetBytes("un mot de passe");
            string filename = "SimplePlainText.txt";

            SecureFile sf = new SecureFile();
            sf.Encrypt(filename, mdp); // mdp is cleared;

            mdp = Encoding.ASCII.GetBytes("un mot de passe");
            sf.Decrypt(filename + ".crypt", mdp);
            Console.WriteLine("Hello World!");
        }
    }
}
