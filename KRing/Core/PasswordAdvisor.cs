using System.Text.RegularExpressions;
using KRing.Interfaces;
using System.Security;
using KRing.Extensions;

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
    public class PasswordAdvisor
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

        public static SecureString CheckPasswordWithUserInteraction(IUserInterface ui)
        {
            SecureString password = ui.RequestPassword("\nPlease enter a password");

            bool passwordStrongEnough = false;

            while (!passwordStrongEnough)
            {
                /* check strength of password */
                PasswordScore score = PasswordAdvisor.CheckStrength(password.ConvertToUnsecureString());

                switch (score)
                {
                    case PasswordScore.Blank:
                    case PasswordScore.VeryWeak:
                    case PasswordScore.Weak:
                        ui.MessageToUser("Your password is too weak due to lack of special characters, digits and/or upper/lower case variation");
                        passwordStrongEnough = false;
                        break;
                    case PasswordScore.Medium:
                    case PasswordScore.Strong:
                    case PasswordScore.VeryStrong:
                        ui.MessageToUser("Your password is strong enough to be used! :)");
                        passwordStrongEnough = true;
                        break;
                }

                if (!passwordStrongEnough) password = ui.RequestPassword("\nPlease enter a stronger password");
            }

            return password;
        }
    }
}
