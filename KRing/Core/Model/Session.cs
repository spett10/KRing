using System;
using System.Security;

namespace KRing.Core.Model
{
    public class Session
    {
        public User User { get; private set; }
        public DateTime Start { get; private set; }

        public Session(User User)
        {
            this.User = User;
            this.Start = DateTime.Now;
        }

        public static Session DummySession()
        {
            return new Session(User.DummyUser());
        }

    }
}
