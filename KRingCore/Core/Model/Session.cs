using System;

namespace KRingCore.Core.Model
{
    public class Session
    {
        public User User { get; private set; }
        public DateTime Start { get; private set; }
        public bool IsLoggedIn { get; set; }

        /* TODO: flyt isloggedin til session */

        public Session(User User, bool isLoggedIn)
        {
            this.User = User;
            this.Start = DateTime.Now;
            this.IsLoggedIn = isLoggedIn;
        }

        public static Session DummySession()
        {
            return new Session(User.DummyUser(), false);
        }

    }
}
