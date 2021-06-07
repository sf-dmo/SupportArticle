namespace Code.Core.KeyStretching
{
    public interface IKeyStretching
    {
        byte[] Stretching(byte[] key, int outputSize);
        byte[] Stretching(byte[] key);
    }
}