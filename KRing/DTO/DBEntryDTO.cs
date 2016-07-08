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
        public User User { get; private set; }
        public string Domain { get; private set; }
        public SecureString Password { get; private set; }

        public DBEntryDTO(User user, string domain, SecureString password)
        {
            User = user;
            Domain = domain;
            Password = password;
        }

        ~DBEntryDTO()
        {
            Password.Dispose(); //Remove secure string with password from memory. 
        }
    }
}
