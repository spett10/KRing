using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KRing.Interfaces
{
    public interface UserInterface
    {
        void StartupMessage();

        void WelcomeMessage(User User);

        string RequestPassword();

        string RequestUserName();

        void BadLogin();

    }
}
