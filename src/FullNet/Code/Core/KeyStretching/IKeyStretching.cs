namespace Code.Core.KeyStretching
{
    public interface IKeyStretching
    {
        byte[] Stretching(byte[] key, byte[] salt, int iteration, int outputSize);
        byte[] Stretching(byte[] key, byte[] salt, int iteration);
        byte[] Stretching(byte[] key, byte[] salt);
        byte[] Stretching(byte[] key, int outputSize);
        byte[] Stretching(byte[] key);
    }
}