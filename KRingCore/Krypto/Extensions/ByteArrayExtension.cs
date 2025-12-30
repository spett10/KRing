namespace KRingCore.Krypto.Extensions
{
    public static class ByteArrayExtensions
    {
        public static void ZeroOut(this byte[] array)
        {
            var length = array.Length;
            for (int i = 0; i < length; i++)
            {
                array[i] = byte.MinValue;
            }
        }
    }
}
