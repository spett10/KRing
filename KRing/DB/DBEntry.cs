using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRing.DTO;
using System.Security;
using System.ComponentModel;

namespace KRing.DB
{
    /* Maybe this should be a private class of DBController? */
    /* Noone else should know about this class as it stands.. */
    public class DBEntry
    {
        public string Domain { get; private set; }
        public string DomainIV { get; private set; }
        public SecureString Password { get; private set; } 
        public string PasswordIV { get; private set; }


        public DBEntry(string domain, SecureString password, string domainIV, string passwordIV)
        {
            Domain = domain;
            Password = password;
            PasswordIV = passwordIV;
            DomainIV = domainIV;
        }

        public List<string> ToStrings()
        {
            List<string> properties = new List<string>();

            properties.Add(this.Domain);
            properties.Add(Password.ConvertToUnsecureString());

            return properties;
        }


    }


}
