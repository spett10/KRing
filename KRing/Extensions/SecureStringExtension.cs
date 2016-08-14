using System;
using System.Runtime.InteropServices;
using System.Security;

namespace KRing.Extensions
{
    public static class SecureStringExtension
    {
        public static string ConvertToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally /* If we dont free the buffer after use, the password can be in unmanaged memory */
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static void PopulateWithString(this SecureString securePassword, string password)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            char[] characters = password.ToCharArray();
            foreach (char c in characters)
                securePassword.AppendChar(c);
        }
    }
}
