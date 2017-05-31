﻿using KRingCore.Core;
using KRingCore.Extensions;
using KRingCore.Persistence.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Configuration;
using KRingCore.Persistence.Interfaces;
using KRingCore.Persistence;
using KRingCore.Interfaces;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Persistence.Repositories
{
    public class StoredPasswordRepository : ReleasePathDependent, IStoredPasswordRepository
    {
        private readonly IDataConfig _dataConfig;
        private readonly int _count;
        private List<StoredPassword> _entries;
        
        private byte[] _saltForEncrKey;
        private byte[] _saltForMacKey;
        private byte[] _encrKey;
        private byte[] _macKey;

        private readonly int _keyLength = 32;
        private readonly int _ivLength = 16;

        private readonly SecureString _password;

        public int EntryCount => _entries.Count;
        public bool DecryptionErrorOccured { get; private set; }
        public bool EncryptionErrorOccured { get; private set; }

        /// <summary>
        /// If you create a new Repository object, and the underlying DB already exists
        /// The password must be the same. Otherwise, the decryption will fail and an error must be thrown.
        /// </summary>
        /// <param name="password"></param>
        public StoredPasswordRepository(SecureString password, byte[] encrKeySalt, byte[] macKeySalt)
        {
#if DEBUG
            _dataConfig = new DataConfig(
                               ConfigurationManager.AppSettings["relativedbPathDebug"],
                               ConfigurationManager.AppSettings["relativeconfigPathDebug"]);
#else
            _dataConfig = new DataConfig(
                               base.ReleasePathPrefix() + ConfigurationManager.AppSettings["relativedbPath"],
                               base.ReleasePathPrefix() + ConfigurationManager.AppSettings["relativeconfigPath"]);
#endif

            DecryptionErrorOccured = false;
            EncryptionErrorOccured = false;

            _count = _dataConfig.GetStorageCount();
            _saltForEncrKey = encrKeySalt;
            _saltForMacKey = macKeySalt;

            _password = password;
            _encrKey = DeriveKey(password, _saltForEncrKey);
            _macKey = DeriveKey(password, _saltForMacKey);
            
            if(IsDbEmpty())
            {
                _entries = new List<StoredPassword>();
                DeleteAllEntries();
            }
            else
            {
                _entries = LoadEntriesFromDb();
            }            
        }

        public StoredPasswordRepository(SecureString password, byte[] encrKeySalt, byte[] macKeySalt, IDataConfig config)
        {
            _dataConfig = config;
            
            DecryptionErrorOccured = false;
            EncryptionErrorOccured = false;

            _count = _dataConfig.GetStorageCount();
            _saltForEncrKey = new byte[_ivLength];
            _saltForEncrKey = encrKeySalt;
            _saltForMacKey = macKeySalt;

            _password = password;
            _encrKey = DeriveKey(password, _saltForEncrKey);
            _macKey = DeriveKey(password, _saltForMacKey);

            if (IsDbEmpty())
            {
                _entries = new List<StoredPassword>();
                DeleteAllEntries();
            }
            else
            {
                _entries = LoadEntriesFromDb();
            }
        }

        ~StoredPasswordRepository()
        {
            CryptoHashing.ZeroOutArray(ref _encrKey);
            CryptoHashing.ZeroOutArray(ref _macKey);
        }

        public void DeleteEntry(string domain)
        {
            var entry = _entries.SingleOrDefault(e => e.Domain.Equals(domain));
            if (entry != null)
            {
                _entries.Remove(entry);
            }
            else
                throw new ArgumentException("domain does not exist to delete");
        }

        public void DeleteEntry(int index)
        {
            var entry = _entries.ElementAt(index);
            if (entry != null)
            {
                _entries.Remove(entry);
            }
            else
                throw new ArgumentException("domain does not exist to delete");
        }

        public void UpdateEntry(StoredPassword updatedEntry)
        {
            var entry =
                _entries.FirstOrDefault(e => e.Domain.Equals(updatedEntry.Domain, StringComparison.OrdinalIgnoreCase));
            if (entry != null)
            {
                entry.PlaintextPassword = updatedEntry.PlaintextPassword;
            }
            else throw new ArgumentException("No such Domain");
        }
        
        public StoredPassword GetEntry(int index)
        {
            return _entries.ElementAt(index);
        }

        public string GetPasswordFromDomain(string domain)
        {
            return _entries.Where(e => e.Domain == domain).Select(e => e.PlaintextPassword).First();
        }

        public string GetPasswordFromCount(int count)
        {
            return _entries.ElementAt(count).PlaintextPassword;
        }

        public void DeleteAllEntries()
        {
            _entries.Clear();

            DeleteDb();
        }
        
        public List<StoredPassword> GetEntries()
        {
            return _entries;
        }

        public bool ExistsEntry(string domain)
        {
            return _entries.Any(e => e.
                               Domain.
                               ToString().
                               Equals(domain, StringComparison.CurrentCulture));
        }

        public void AddEntry(StoredPassword newDbEntry)
        {
            bool duplicateExists = _entries.Exists(
                                            e => e.
                                            Domain.
                                            Equals(newDbEntry.Domain, StringComparison.CurrentCulture));

            if (!duplicateExists)
            {
                _entries.Add(newDbEntry);
            }
            else
            {
                throw new ArgumentException("Error: Domain Already Exists");
            }
        }

        public bool IsDbEmpty()
        {
            return _count <= 0;
        } 

        public async Task WriteEntriesToDbAsync()
        {
            using (FileStream fileStream = new FileStream(_dataConfig.dbPath, FileMode.Create))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                foreach(var entr in _entries)
                {
                    try
                    {
                        /* Encrypt */
                        var rawDomain = Encoding.UTF8.GetBytes(entr.Domain);
                        var rawPassword = entr.PlaintextPassword;
                        var rawPass = Encoding.UTF8.GetBytes(rawPassword);

                        var ivForDomain = CryptoHashing.GenerateSalt(_ivLength);
                        var ivForPass = CryptoHashing.GenerateSalt(_ivLength);

                        var domainCipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawDomain, ivForDomain, _encrKey, _macKey);
                        var passCipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawPass, ivForPass, _encrKey, _macKey);

                        /* write domain, tag, iv */
                        await streamWriter.WriteLineAsync(domainCipher.GetCipherAsBase64());
                        await streamWriter.WriteLineAsync(domainCipher.GetTagAsBase64());
                        await streamWriter.WriteLineAsync(Convert.ToBase64String(ivForDomain));

                        /* write password, tag */
                        await streamWriter.WriteLineAsync(passCipher.GetCipherAsBase64());
                        await streamWriter.WriteLineAsync(passCipher.GetTagAsBase64());
                        await streamWriter.WriteLineAsync(Convert.ToBase64String(ivForPass));

                        UpdateConfig(_entries.Count);
                        EncryptionErrorOccured = false;                        
                    }
                    catch(Exception e)
                    {
                        EncryptionErrorOccured = true;
                    }
                }
            }            
        }

        public void WriteEntriesToDb()
        {
            using(FileStream fileStream = new FileStream(_dataConfig.dbPath, FileMode.Create))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                foreach (var entr in _entries)
                {
                    try
                    {
                        /* encrypt */
                        var rawDomain = Encoding.UTF8.GetBytes(entr.Domain);
                        var rawPassword = entr.PlaintextPassword;
                        var rawPass = Encoding.UTF8.GetBytes(rawPassword);

                        var ivForDomain = CryptoHashing.GenerateSalt(_ivLength);
                        var ivForPass = CryptoHashing.GenerateSalt(_ivLength);

                        var domainCipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawDomain, ivForDomain, _encrKey, _macKey);
                        var passCipher = Aes256AuthenticatedCipher.CBCEncryptThenHMac(rawPass, ivForPass, _encrKey, _macKey);

                        /* write domain, tag, iv */
                        streamWriter.WriteLine(domainCipher.GetCipherAsBase64());
                        streamWriter.WriteLine(domainCipher.GetTagAsBase64());
                        streamWriter.WriteLine(Convert.ToBase64String(ivForDomain));

                        /* write password, tag */
                        streamWriter.WriteLine(passCipher.GetCipherAsBase64());
                        streamWriter.WriteLine(passCipher.GetTagAsBase64());
                        streamWriter.WriteLine(Convert.ToBase64String(ivForPass));

                        EncryptionErrorOccured = false;
                    }
                    catch(Exception e)
                    {
                        EncryptionErrorOccured = true;
                    }                    
                }
            }
            
            UpdateConfig(_entries.Count);
        }

        public async Task<List<StoredPassword>> LoadEntriesFromDbAsync()
        {
            try
            {
                List<StoredPassword> entries = new List<StoredPassword>();

                using (FileStream fs = new FileStream(_dataConfig.dbPath, FileMode.Open))
                using (StreamReader streamReader = new StreamReader(fs))
                {
                    for (int i = 0; i < _count; i++)
                    {
                        /* READ */
                        var domainBase64 = await streamReader.ReadLineAsync();
                        var domainTagBase64 = await streamReader.ReadLineAsync();
                        var domainIvTask = streamReader.ReadLineAsync();

                        var domainCipher = new Aes256AuthenticatedCipher.AuthenticatedCiphertext(domainBase64, domainTagBase64);

                        var passwordBase64 = await streamReader.ReadLineAsync();
                        var passwordTag = await streamReader.ReadLineAsync();
                        var passwordIvTask = streamReader.ReadLineAsync();

                        var passwordCipher = new Aes256AuthenticatedCipher.AuthenticatedCiphertext(passwordBase64, passwordTag);

                        try
                        {
                            /* DECRYPT */
                            var domainIv = Convert.FromBase64String(await domainIvTask);
                            var passwordIv = Convert.FromBase64String(await passwordIvTask);

                            var domain = Aes256AuthenticatedCipher.VerifyMacThenCBCDecrypt(domainCipher, _encrKey, domainIv, _macKey);
                            var password = Aes256AuthenticatedCipher.VerifyMacThenCBCDecrypt(passwordCipher, _encrKey, passwordIv, _macKey);

                            StoredPassword newEntry = new StoredPassword(Encoding.UTF8.GetString(domain), Encoding.UTF8.GetString(password));
                            entries.Add(newEntry);

                            DecryptionErrorOccured = false;
                        }
                        catch (Exception)
                        {
                            DecryptionErrorOccured = true;
                        }

                    }
                }
                
                return entries;
            }
            catch(Exception)
            {
                throw new Exception("Could not load passwords - possibly data is corrupted!");
            }
        } 

        public List<StoredPassword> LoadEntriesFromDb()
        {
            try
            {
                List<StoredPassword> entries = new List<StoredPassword>();

                using (FileStream fs = new FileStream(_dataConfig.dbPath, FileMode.Open))
                using (StreamReader streamReader = new StreamReader(fs))
                {
                    for (int i = 0; i < _count; i++)
                    {
                        /* READ */
                        var domainBase64 = streamReader.ReadLine();
                        var domainTagBase64 = streamReader.ReadLine();
                        var domainIv = Convert.FromBase64String(streamReader.ReadLine());

                        var domainCipher = new Aes256AuthenticatedCipher.AuthenticatedCiphertext(domainBase64, domainTagBase64);

                        var passwordBase64 = streamReader.ReadLine();
                        var passwordTag = streamReader.ReadLine();
                        var passwordIv = Convert.FromBase64String(streamReader.ReadLine());

                        var passwordCipher = new Aes256AuthenticatedCipher.AuthenticatedCiphertext(passwordBase64, passwordTag);

                        try
                        {
                            /* DECRYPT */
                            var domain = Aes256AuthenticatedCipher.VerifyMacThenCBCDecrypt(domainCipher, _encrKey, domainIv, _macKey);
                            var password = Aes256AuthenticatedCipher.VerifyMacThenCBCDecrypt(passwordCipher, _encrKey, passwordIv, _macKey);

                            /* CREATE DBENTRY */
                            StoredPassword newEntry = new StoredPassword(Encoding.UTF8.GetString(domain), Encoding.UTF8.GetString(password));
                            entries.Add(newEntry);

                            DecryptionErrorOccured = false;
                        }
                        catch (Exception)
                        {
                            DecryptionErrorOccured = true;
                        }

                    }
                }
                return entries;
            }
            catch(Exception e)
            {
                throw new Exception("Could not load passwords - possibly data is corrupted!");
            }
        }
        
        private byte[] DeriveKey(SecureString password, byte[] iv)
        {
            return CryptoHashing.DeriveKeyFromPasswordAndSalt(password, iv, _keyLength);
        }

        private void DeleteDb()
        {
            FileUtil.FilePurge(_dataConfig.dbPath, "-");
            _dataConfig.ClearConfig();
        }

        private async Task DeleteDbAsync()
        {
            await FileUtil.FilePurgeAsync(_dataConfig.dbPath, "-");
            await _dataConfig.ClearConfigAsync();
        }

        private void UpdateConfig(int count)
        {
            _dataConfig.UpdateConfig(count);
        }

        private async Task UpdateConfigAsync(int count)
        {
            await _dataConfig.UpdateConfigAsync(count);
        }
    }
}