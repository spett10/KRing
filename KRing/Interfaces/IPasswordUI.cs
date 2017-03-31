using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace KRing.Interfaces
{
    public interface IPasswordUI
    {
        void MessageToUser(string msg);

        SecureString RequestPassword(string msg);
    }
}
