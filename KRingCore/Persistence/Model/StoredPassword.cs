using System.Collections.Generic;
using System.Security;
using KRingCore.Extensions;

namespace KRingCore.Persistence.Model
{
    /* Maybe this should be a private class of DbController? */
    /* Noone else should know about this class as it stands.. */
    public class StoredPassword
    {
        public string Domain { get; private set; }
        public string Username { get; private set; }
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

        public StoredPassword(string domain, string username, string password)
        {
            Domain = domain;
            Username = username;
            Password = new SecureString();
            Password.PopulateWithString(password);
        }

        public StoredPassword(string domain, string username, char[] password)
        {
            Domain = domain;
            Username = username;
            Password = new SecureString();
            foreach(var c in password)
            {
                Password.AppendChar(c);
            }
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
