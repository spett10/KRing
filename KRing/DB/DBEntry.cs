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
        public User User { get; private set; }
        public string Domain { get; private set; }
        public string Password { get; private set; }
        public string Salt { get; private set; }

        public DBEntry(User user, string domain, string password, string salt)
        {
            User = user;
            Domain = domain;
            Password = password;
            Salt = salt;
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
