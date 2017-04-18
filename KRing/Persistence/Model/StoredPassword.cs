using System.Collections.Generic;
using System.Security;
using KRing.Extensions;

namespace KRing.Persistence.Model
{
    /* Maybe this should be a private class of DbController? */
    /* Noone else should know about this class as it stands.. */
    public class StoredPassword
    {
        public string Domain { get; private set; }
        public string PlaintextPassword
        {   get
            {
                var plain = Password.ConvertToUnsecureString();
                PlaintextPassword = plain;
                return plain;
            }

            set
            {
                Password = new SecureString();
                Password.PopulateWithString(value);
            }
        }

        private SecureString Password { get; set; }

        public StoredPassword(string domain, string password)
        {
            Domain = domain;
            Password = new SecureString();
            Password.PopulateWithString(password);
        }

        public List<string> ToStrings()
        {
            List<string> properties = new List<string>();
            
            properties.Add(this.Domain);
            properties.Add(PlaintextPassword);

            return properties;
        }

        ~StoredPassword()
        {
            Password.Dispose();
        }
    }


}
