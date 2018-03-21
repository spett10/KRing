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
        //TODO: need refresh thingie, so it can replace itself, so the underlying keys are regenerated if neccesary. also so you dont have to call explicit constructor. 
    }
}
