namespace Krypto.Extensions
{
    public static class CharArrayExtension
    {
        public static void ZeroOut(this char[] array)
        {
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = (char)0;
            }
        }
    }
}
