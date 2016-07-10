using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace KRing.DTO
{
    public class DBEntryDTO
    {
        public string Domain { get; private set; }
        public SecureString Password { get; private set; }

        public DBEntryDTO(string domain, SecureString password)
        {
            Domain = domain;
            Password = password;
        }

        ~DBEntryDTO()
        {
            Password.Dispose(); //Remove secure string with password from memory. 
        }
    }
}
