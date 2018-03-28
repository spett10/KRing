using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KRingCore.Persistence.Interfaces;
using KRingCore.Persistence.Model;
using KRingCore.Core.Services;
using KRingCore.Interfaces;
using System.Security.Cryptography;
using System.Security;

namespace KRingCore.Persistence.Repositories
{
    class JsonStoredPasswordIO : IStoredPasswordIO
    {
        public IStoredPasswordReader Reader { get; }

        public IStoredPasswordWriter Writer { get; }

        public JsonStoredPasswordIO(IDataConfig dataConfig, SecureString password)
        {
            Reader = new JsonPasswordReader(dataConfig, password);

            Writer = new JsonPasswordWriter(dataConfig, password); 
        }

    }

    internal class JsonPasswordReader : IStoredPasswordReader
    {
        private readonly IDecryptingPasswordImporter _importer;
        private readonly IDataConfig _dataConfig;
        private readonly IStreamReadToEnd _streamReader;
        private readonly SecureString _password;

        public JsonPasswordReader(IDataConfig dataConfig, SecureString password)
        {
            _importer = new DecryptingPasswordImporter(new Security.KeyGenerator(), password);
            _dataConfig = dataConfig;
            _password = password;
            _streamReader = new StreamReaderToEnd();

            DecryptionErrorOccured = false;
        }

        public bool DecryptionErrorOccured { get; private set; }

        public List<StoredPassword> LoadEntriesFromDb()
        {
            try
            {
                return _importer.ImportPasswords(_dataConfig.dbPath, _streamReader);
            }
            catch (Exception)
            {
                DecryptionErrorOccured = true;
                return new List<StoredPassword>();
            }            
        }

        public async Task<List<StoredPassword>> LoadEntriesFromDbAsync()
        {
            try
            {
                return await _importer.ImportPasswordsAsync(_dataConfig.dbPath, _streamReader);
            }
            catch (Exception)
            {
                DecryptionErrorOccured = true;
                return new List<StoredPassword>();
            }
        }
    }

    internal class JsonPasswordWriter : IStoredPasswordWriter
    {
        private readonly IEncryptingPasswordExporter _exporter;
        private readonly IDataConfig _dataConfig;
        private readonly IStreamWriterToEnd _streamWriter;
        private readonly SecureString _password;

        public bool EncryptionErrorOccured { get; private set; }

        public JsonPasswordWriter(IDataConfig dataConfig, SecureString password)
        {
            _exporter = new EncryptingPasswordExporter(new Security.KeyGenerator(), password);
            _dataConfig = dataConfig;
            _streamWriter = new StreamWriterToEnd();
            _password = password;

            EncryptionErrorOccured = false;
        }
        
        public void WriteEntriesToDb(List<StoredPassword> list)
        {
            try
            {
                var data = _exporter.ExportPasswords(list);
                _streamWriter.WriteToNewFile(_dataConfig.dbPath, data);
            }
            catch(Exception)
            {
                EncryptionErrorOccured = true;
            }
        }

        public async Task WriteEntriesToDbAsync(List<StoredPassword> list)
        {
            try
            {
                var data = await _exporter.ExportPasswordsAsync(list);
                await _streamWriter.WriteToNewFileAsync(_dataConfig.dbPath, data);
            }
            catch (Exception)
            {
                EncryptionErrorOccured = true;
            }
        }
    }
}
