using System.Text.RegularExpressions;
using System.Security;
using KRingCore.Core.Interfaces;
using Krypto.Extensions;
using System.Collections.Generic;

namespace KRingCore.Core
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

    public class PasswordLengthRule : IPasswordRule
    {
        public int Apply(string password)
        {
            int score = 0;
            
            if(password.Length >= 8) {
                score++;

                //For each additional multiple of 4, add 1 point. 
                int multiple = 0;
                int lenght = password.Length - 8;
                int roundTrip = 4;

                while(lenght >= roundTrip)
                {
                    lenght = lenght - roundTrip;
                    multiple++;
                }

                score = score + multiple;                
            }

            return score;
        }
    }

    public class PasswordDigitsRule : IPasswordRule
    {
        public int Apply(string password)
        {
            int score = 0;

            if (Regex.IsMatch(password, @"[0-9]+(\.[0-9][0-9]?)?", RegexOptions.ECMAScript)) { score++; }

            return score;
        }
    }

    public class PasswordCapitalRule : IPasswordRule
    {
        public int Apply(string password)
        {
            int score = 0;
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$", RegexOptions.ECMAScript)) { score++; }
            return score;
        }
    }

    public class PasswordSpecialCharacterRule : IPasswordRule
    {
        public int Apply(string password)
        {
            int score = 0;
            if (Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript)) { score++; }
            return score;
        }
    }

    /* Credit goes to: http://stackoverflow.com/questions/12899876/checking-strings-for-a-strong-enough-password */
    public class PasswordAdvisor
    {
        private static List<IPasswordRule> rules = 
            new List<IPasswordRule> {
                new PasswordLengthRule(),
                new PasswordDigitsRule(),
                new PasswordCapitalRule(),
                new PasswordSpecialCharacterRule() };

        public static PasswordScore CheckStrength(string password)
        {
            int score = 0;

            if (password.Length < 1) { return PasswordScore.Blank; }
            if (password.Length < 4) { return PasswordScore.VeryWeak; }

            foreach(var rule in rules)
            {
                score += rule.Apply(password);
            }
            
            return (PasswordScore)score;
        }

        public static SecureString CheckPasswordWithUserInteraction(IPasswordUI ui)
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
                    case PasswordScore.Medium:
                        ui.MessageToUser("Your password is too weak due to lack of special characters, digits and/or upper/lower case variation");
                        passwordStrongEnough = false;
                        break;                    
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
