using System.Security;

namespace KRing.Core
{
    public class User
    {
        public string UserName { get; private set; }
        public bool IsLoggedIn { get; private set; }
        public SecureString Password { get; private set; }
        public Cookie Cookie { get; private set; }

        public User(string Name, bool LoggedIn, SecureString password, Cookie cookie)
        {
            UserName = Name;
            IsLoggedIn = LoggedIn;
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
