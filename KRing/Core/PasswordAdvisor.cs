using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KRing.Core
{
    public enum PasswordScore
    {
        Blank = 0,
        VeryWeak = 1, 
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }

    /* Credit goes to: http://stackoverflow.com/questions/12899876/checking-strings-for-a-strong-enough-password */
    class PasswordAdvisor
    {
        public static PasswordScore CheckStrength(string password)
        {
            int score = 0;

            if (password.Length < 1) { return PasswordScore.Blank; }
            if (password.Length < 4) { return PasswordScore.VeryWeak; }

            if (password.Length >= 8) { score++; }
            if (password.Length >= 12) {  score++; }
            if (Regex.IsMatch(password, @"[0-9]+(\.[0-9][0-9]?)?", RegexOptions.ECMAScript)) { score++; }
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$", RegexOptions.ECMAScript)) { score++; }
            if (Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript)) { score++; }
            
            return (PasswordScore)score;
        }
    }
}
