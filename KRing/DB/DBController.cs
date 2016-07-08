using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KRing.DTO;


namespace KRing.DB
{
    /* We cant salt and store these passwords, since its just data, we are not trying to log a user in, we are trying to give them */
    /* Their specific decrypted data (which happens to be passwords) on demand once they are logged in */
    /* TODO: all strings, perhaps not username, in the below, should be encrypted. But we want I/O to work first */
    public class DBController
    {
        /// <summary>
        /// Singleton pattern, since we use the same file throughout, so we only want one accessing it at any time. 
        /// </summary>
        private static DBController instance;

        private readonly string DBPath = "..\\..\\Data\\db.txt";

        public List<DBEntry> Entries { get; private set; }
        public int EntryCount { get; private set; }

        public static DBController Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new DBController();
                }

                return instance;
            }
        }

        private DBController()
        {
            Entries = new List<DBEntry>();
            using (StreamReader sr = new StreamReader(DBPath))
            {
                EntryCount = Int32.Parse(sr.ReadLine());

                for(int i = 0; i < EntryCount; i++)
                {
                    string username = sr.ReadLine();
                    string domain = sr.ReadLine();
                    string password = sr.ReadLine();
                    string salt = sr.ReadLine();

                    DBEntry newEntry = new DBEntry(new User(username, false), domain, password, salt);

                    Entries.Add(newEntry);
                }

            }

        }
        

        public void AddEntry(DBEntryDTO newDTO)
        {

            byte[] salt = Authenticator.GenerateSalt();
            string salt64 = Convert.ToBase64String(salt);

            DBEntry newEntry = new DBEntry(newDTO.User, newDTO.Domain, newDTO.Password.ConvertToUnsecureString(), salt64);

            Entries.Add(newEntry);
            EntryCount++;
        }
        
        /* we could call write after addentry? */
        public void Write()
        {
            using (StreamWriter sw = new StreamWriter(DBPath))
            {
                sw.WriteLine(EntryCount.ToString());

                foreach (DBEntry dbe in Entries)
                {
                    List<string> strings = dbe.ToStrings();

                    foreach(var str in strings)
                    {
                        sw.WriteLine(str.ToString());
                    }
                }
            }   
        }
    }
}
