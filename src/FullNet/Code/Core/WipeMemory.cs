using System;

namespace Code.Core
{
    public class WipeMemory
    {
        public static void WipeByte(ref byte[] toWipe)
        {
            if (toWipe == null)
                return;
            Array.Clear(toWipe, 0, toWipe.Length);
            toWipe = null;
        }
    }
}
