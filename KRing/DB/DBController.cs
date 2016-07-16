using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KRing.DTO;
using System.Security.Cryptography;
using System.Security;

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
        private readonly string INSECURE_HARDCODED_KEY = "Yellow Submarine";
        public byte[] HARDCODED_IV { get; set; }

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
            bool IsDBPopulated = false;
            int count = 0;
            using (StreamReader db = new StreamReader(DBPath))
            {
                string storedCound = db.ReadLine();

                IsDBPopulated = Int32.TryParse(storedCound, out count);

                Console.WriteLine("count {0}", count.ToString());
                if (count > 0)
                {
                    byte[] KEY = Encoding.ASCII.GetBytes(INSECURE_HARDCODED_KEY);
                    byte[] IV = Convert.FromBase64String(db.ReadLine());
                        
                    for(int i = 0; i < count; i++)
                    {
                        string domainBase64 = db.ReadLine();
                        byte[] domainRaw = Convert.FromBase64String(domainBase64);
                        byte[] domainPlain = CryptoWrapper.CBC_Decrypt(domainRaw, KEY, IV);
                        string domain = Encoding.ASCII.GetString(domainPlain);
                        
                        string passwordBase64 = db.ReadLine();
                        byte[] passwordRaw = Convert.FromBase64String(passwordBase64);
                        byte[] passwordPlain = CryptoWrapper.CBC_Decrypt(passwordRaw, KEY, IV);
                        string password = Encoding.ASCII.GetString(passwordPlain);
                        
                        SecureString securePassword = new SecureString();
                        securePassword.PopulateWithString(password);

                        DBEntry newEntry = new DBEntry(domain, securePassword);
                        Entries.Add(newEntry);
                        EntryCount++;
                    }
                }
                else
                {
                    EntryCount = 0;
                }
            }
        }
        
        

        public void AddEntry(DBEntryDTO newDTO)
        {
            DBEntry newEntry = new DBEntry(newDTO.Domain, newDTO.Password);
            Entries.Add(newEntry);
            EntryCount++;
        }
        
        /* we could call write after addentry? */
        public void Write(string password)
        {
            /* Are we sure these sizes are correct? */
            byte[] raw_key = Encoding.ASCII.GetBytes(INSECURE_HARDCODED_KEY);
            byte[] IV = Authenticator.GenerateSalt();
            HARDCODED_IV = IV;
            /*TODO: MAKE IV FOR EACH ENTRY SO ITS SEMANTICALLY SECURE!! */

            /* First, write count at top of DB. Then write IV, neither of these should be encrypted? */
            using (StreamWriter sw = new StreamWriter(DBPath, false))
            {
                sw.WriteLine(EntryCount);
                sw.WriteLine(Convert.ToBase64String(IV));
                
                /* Take each entry of Entries and extract properties, encrypt and write as base64 */
                foreach (var entr in Entries)
                {
                    List<string> properties = entr.ToStrings();

                    foreach (var str in properties)
                    {
                        Console.WriteLine("Trying to write {0}", str);
                        byte[] data = Encoding.ASCII.GetBytes(str);
                        byte[] encr = CryptoWrapper.CBC_Encrypt(data, raw_key, IV);

                        string encrBase64 = Convert.ToBase64String(encr);
                        sw.WriteLine(encrBase64);
                    }
                }
            }
        }
    }
}
