using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace KRing.DTO
{
    /// <summary>
    /// Data Transfer Object. We simply transfer data between UI and Backend (where we add more properties and behaviour) 
    /// </summary>
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
