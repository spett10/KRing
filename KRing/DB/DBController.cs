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
        private readonly string INSECURE_HARDCODED_KEY = "Yellow Submarine"; //Todo: Use userstored RSA key to encrypt a key for the DB stuff.. its shown on MSDN.

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

                IsDBPopulated = int.TryParse(storedCound, out count);

                Console.WriteLine("count {0}", count.ToString());
                if (count > 0)
                {
                    byte[] KEY = Encoding.ASCII.GetBytes(INSECURE_HARDCODED_KEY);
                        
                    for(int i = 0; i < count; i++)
                    {
                        /* Read properties for each entry */
                        string domainBase64 = db.ReadLine();
                        byte[] domainRaw = Convert.FromBase64String(domainBase64);

                        string domainIV = db.ReadLine();

                        string passwordBase64 = db.ReadLine();
                        byte[] passwordRaw = Convert.FromBase64String(passwordBase64);

                        string passwordIV = db.ReadLine();

                        /* decrypt */
                        byte[] domainPlain = CryptoWrapper.CBC_Decrypt(domainRaw, KEY, Convert.FromBase64String(domainIV));
                        string domain = Encoding.UTF8.GetString(domainPlain);

                        byte[] passwordPlain = CryptoWrapper.CBC_Decrypt(passwordRaw, KEY, Convert.FromBase64String(passwordIV));
                        string password = Encoding.UTF8.GetString(passwordPlain);

                        SecureString securePassword = new SecureString();
                        securePassword.PopulateWithString(password);

                        /* create entry in DBController */
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

        public void Write(string password)
        {

            byte[] raw_key = Encoding.ASCII.GetBytes(INSECURE_HARDCODED_KEY);

            using (StreamWriter sw = new StreamWriter(DBPath, false))
            {
                sw.WriteLine(EntryCount);

                foreach (var entr in Entries)
                {
                    byte[] domainIV = Authenticator.GenerateSalt();
                    byte[] domainData = Encoding.UTF8.GetBytes(entr.Domain);
                    byte[] domainEncrypted = CryptoWrapper.CBC_Encrypt(domainData, raw_key, domainIV);
                    string encryptedDomainB64 = Convert.ToBase64String(domainEncrypted);

                    sw.WriteLine(encryptedDomainB64);
                    sw.WriteLine(Convert.ToBase64String(domainIV));

                    byte[] passwordIV = Authenticator.GenerateSalt();
                    string encryptedPasswordB64 = Convert.ToBase64String(
                        CryptoWrapper.CBC_Encrypt(
                            Encoding.UTF8.GetBytes(
                                entr.Password.ConvertToUnsecureString()), raw_key, passwordIV));

                    sw.WriteLine(encryptedPasswordB64);
                    sw.WriteLine(Convert.ToBase64String(passwordIV));
                }
            }
        }
    }
}
