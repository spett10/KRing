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
        public string Username { get; private set; }
        public string Domain { get; private set; }
        public SecureString Password { get; private set; }

        public DBEntryDTO(string username, string domain, SecureString password)
        {
            Username = username;
            Domain = domain;
            Password = password;
        }

        ~DBEntryDTO()
        {
            Password.Dispose();
        }
    }
}
