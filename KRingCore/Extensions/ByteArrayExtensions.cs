using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Extensions
{
    public static class ByteArrayExtensions
    {
        public static void ZeroOut(this byte[] array)
        {
            var length = array.Length;
            for(int i = 0; i < length; i++)
            {
                array[i] = byte.MinValue;
            }
        }
    }
}
