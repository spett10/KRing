using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Persistence.Interfaces
{
    public interface IStoredPasswordIO
    {
        IStoredPasswordReader Reader { get; }
        IStoredPasswordWriter Writer { get; }
    }
}
