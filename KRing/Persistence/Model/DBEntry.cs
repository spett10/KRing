using System.Collections.Generic;
using System.Security;
using KRing.Extensions;

namespace KRing.Persistence.Model
{
    /* Maybe this should be a private class of DbController? */
    /* Noone else should know about this class as it stands.. */
    public class DBEntry
    {
        public string Domain { get; private set; }
        public SecureString Password { get; set; }

        public DBEntry(string domain, SecureString password)
        {
            Domain = domain;
            Password = password;
        }

        public List<string> ToStrings()
        {
            List<string> properties = new List<string>();

            properties.Add(this.Domain);
            properties.Add(Password.ConvertToUnsecureString());

            return properties;
        }

        ~DBEntry()
        {
            Password.Dispose();
        }
    }


}
