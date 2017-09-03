using KRingCore.Interfaces;
using KRingCore.Persistence.Interfaces;
using System.Security;

namespace KRingCore.Persistence.Repositories
{
    /// <summary>
    /// Stores password encrypted with each value relevant for the cryptography as Newline Seperated Values in the underlying file.
    /// This does not give random access, but only allows the entire file to be read or written in one go. This
    /// is on purpose, in essence to implement an O(N) ORAM - if we always read and write all entries, and they are always
    /// encrypted with semantic security, no adversary can decipher whether any password were updated or not. 
    /// </summary>
    public class NsvStoredPasswordIO : IStoredPasswordIO
    {
        public IStoredPasswordReader Reader { get; }

        public IStoredPasswordWriter Writer { get; }

        public NsvStoredPasswordIO(SecureString password, byte[] encrKey, byte[] macKey, IDataConfig config)
        {
            Reader = new ReadToEndStoredPasswordReader(password, encrKey, macKey, config);

            Writer = new NsvStoredPasswordWriter(password, encrKey, macKey, config);
        }
    }

}
