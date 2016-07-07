using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRing
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
