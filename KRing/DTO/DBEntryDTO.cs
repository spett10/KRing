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
    public class DbEntryDto
    {
        public string Domain { get; private set; }
        public SecureString Password { get; private set; }

        public DbEntryDto(string domain, SecureString password)
        {
            Domain = domain;
            Password = password;
        }

        ~DbEntryDto()
        {
            Password.Dispose(); //Remove secure string with password from memory. 
        }
    }
}
