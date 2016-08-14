using System;

namespace KRing.Core
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

    }
}
