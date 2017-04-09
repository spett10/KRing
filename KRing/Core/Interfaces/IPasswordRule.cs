using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRing.Core.Interfaces
{
    public interface IPasswordRule
    {
        int Apply(string password);
    }
}
