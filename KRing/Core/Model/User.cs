using System.Security;

namespace KRing.Core.Model
{
    public class User
    {
        public string UserName { get; private set; }
        public bool IsLoggedIn { get; private set; }
        public SecureString Password { get; set; }
        public Cookie Cookie { get; private set; }

        public User(string name, bool loggedIn, SecureString password, Cookie cookie)
        {
            UserName = name;
            IsLoggedIn = loggedIn;
            Password = password;
            Cookie = cookie;
        }

        public void Login()
        {
            this.IsLoggedIn = true;
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
