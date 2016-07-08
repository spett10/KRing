using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KRing.DB
{
    /* We cant salt and store these passwords, since its just data, we are not trying to log a user in, we are trying to give them */
    /* Their specific decrypted data (which happens to be passwords) on demand once they are logged in */
    public class DBController
    {
        private readonly string DBPath = "..\\..\\Data\\db.txt";
        public ICollection<DBEntry> Entries { get; set; }

        /* Todo: Add a DBEntry to udnerlying file */
        public void AddEntry(User user, string domain, string password, string salt)
        {
            DBEntry newEntry = new DBEntry(user, domain, password, salt);

            List<string> strings = newEntry.ToStrings();

            using (StreamWriter sw = new StreamWriter(DBPath))
            {
                foreach(var str in strings)
                {
                    sw.WriteLine(str);
                }
            }
        }

        /* Todo: Read all passwords into class upon initialization */

        /* Todo: Reencrypt all passwords and write them back upon any modification (O(N) ORAM STYLE) */
    }
}
