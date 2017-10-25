using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRingCore.Core.Model
{
    public class ExportedEncryptedPasswords
    {
        public string EncryptedPasswordsBase64 { get; set; }
        public string EncryptionKeyIvBase64 { get; set; }
        public string MacKeyIvBase64 { get; set; }
        public string EncryptionIvBase64 { get; set; }
        public string TagBase64 { get; set; }
    }
}
