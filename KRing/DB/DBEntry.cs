using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRing.DTO;
using System.ComponentModel;

namespace KRing.DB
{
    /* Maybe this should be a private class of DBController? */
    public class DBEntry
    {
        public string Domain { get; private set; }
        public string Password { get; private set; } //encrypted string, not plaintext password. 
        
        public DBEntry(string domain, string password)
        {
            Domain = domain;
            Password = password;
        }

        public List<string> ToStrings()
        {
            List<string> properties = new List<string>();
            foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this))
            {
                object value = descriptor.GetValue(this);
                properties.Add(value.ToString());
            }

            return properties;
        }


    }


}
