using KRingCore.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Core.Model
{
    public class SymmetricKey : IDisposable 
    {
        private static readonly IReadOnlyCollection<int> allowedKeySizesInBytes = new ReadOnlyCollection<int>(new List<int>() { 32 });

        public byte[] Key { get; private set; }

        public SymmetricKey(SecureString password, byte[] salt)
        {
            Key = CryptoHashing.DeriveKeyFromPasswordAndSalt(password, salt, allowedKeySizesInBytes.Max());
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Array.Clear(this.Key, 0, this.Key.Length);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SymmetricKey() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
