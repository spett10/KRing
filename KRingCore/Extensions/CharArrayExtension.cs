using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Extensions
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
