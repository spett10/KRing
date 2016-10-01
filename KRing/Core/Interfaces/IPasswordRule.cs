using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRing.Core.Interfaces
{
    interface IPasswordRule
    {
        int Apply(string password);
    }
}
