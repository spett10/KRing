using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace KRing
{
    public class User
    {
        public string UserName { get; private set; }
        public bool IsLoggedIn { get; private set; }
        public SecureString Password { get; private set; }

        public User(string Name, bool LoggedIn, SecureString password)
        {
            UserName = Name;
            IsLoggedIn = LoggedIn;
            Password = password;
        }

        public void Logout()
        {
            this.IsLoggedIn = false;
        }

        public override string ToString()
        {
            return UserName;
        }

    }
}
