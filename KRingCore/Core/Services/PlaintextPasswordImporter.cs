using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KRingCore.Persistence.Model;
using System.IO;

namespace KRingCore.Core.Services
{
    /// <summary>
    /// Used to import a plaintext .txt file where each entry is in the order of domain newline username newline password newline and each 
    /// entry is seperated by a newline
    /// </summary>
    public class PlaintextPasswordImporter : IPasswordImporter
    {
        public List<StoredPassword> ImportPasswords(string filename)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                try
                {
                    var contents = streamReader.ReadToEnd();
                    var splitContents = contents.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    var count = splitContents.Count();
                    var entriesPerRoundtrip = 3;
                    var roundtrips = count / entriesPerRoundtrip;

                    var list = new List<StoredPassword>();

                    for (int i = 0; i < roundtrips; i++)
                    {
                        var currentEntry = splitContents.Skip(i * entriesPerRoundtrip).Take(entriesPerRoundtrip).ToArray();

                        var domain = _extractDomain(currentEntry);
                        var username = _extractUsername(currentEntry);
                        var password = _extractPassword(currentEntry);

                        var entry = new StoredPassword(domain, username, password);

                        list.Add(entry);
                    }

                    return list;
                }
                catch (Exception e)
                {
                    throw new FormatException("File does not contain a valid format.");
                }
            }
        }

        public async Task<List<StoredPassword>> ImportPasswordsAsync(string filename)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                try
                {
                    var contents = await streamReader.ReadToEndAsync();
                    var splitContents = contents.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    var count = splitContents.Count();
                    var entriesPerRoundtrip = 3;
                    var roundtrips = count / entriesPerRoundtrip;

                    var list = new List<StoredPassword>();

                    for (int i = 0; i < roundtrips; i++)
                    {
                        var currentEntry = splitContents.Skip(i * entriesPerRoundtrip).Take(entriesPerRoundtrip).ToArray();

                        var domain = _extractDomain(currentEntry);
                        var username = _extractUsername(currentEntry);
                        var password = _extractPassword(currentEntry);

                        var entry = new StoredPassword(domain, username, password);

                        list.Add(entry);
                    }

                    return list;
                }
                catch (Exception e)
                {
                    throw new FormatException("File does not contain a valid format.");
                }
            }
        }

        private delegate string ExtractTokenFromEntry(string[] array);

        private ExtractTokenFromEntry _extractDomain = a => { return a[0]; };
        private ExtractTokenFromEntry _extractUsername = a => { return a[1]; };
        private ExtractTokenFromEntry _extractPassword = a => { return a[2]; };
    }

    
}
