using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRing
{
    public class User
    {
        public string UserName { get; private set; }
        public bool IsLoggedIn { get; private set; }


        public User(string Name, bool LoggedIn)
        {
            UserName = Name;
            IsLoggedIn = LoggedIn;
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
