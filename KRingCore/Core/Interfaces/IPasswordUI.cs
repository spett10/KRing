using System.Security;

namespace KRingCore.Core.Interfaces
{
    public interface IPasswordUI
    {
        void MessageToUser(string msg);

        SecureString RequestPassword(string msg);
    }
}
