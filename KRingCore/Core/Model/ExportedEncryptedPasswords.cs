using Newtonsoft.Json;

namespace KRingCore.Core.Model
{
    //TODO: add a version? 
    public class ExportedEncryptedPasswords
    {
        public string EncryptedPasswordsBase64 { get; set; }
        public string EncryptionKeyIvBase64 { get; set; }
        public string MacKeyIvBase64 { get; set; }
        public string EncryptionIvBase64 { get; set; }
        public string CiphertextTagBase64 { get; set; }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ExportedEncryptedPasswordsWithIntegrity : ExportedEncryptedPasswords
    {
        public string PayloadTagBase64 { get; set; }

        public ExportedEncryptedPasswords GetPayload()
        {
            return new ExportedEncryptedPasswords()
            {
                EncryptedPasswordsBase64 = this.EncryptedPasswordsBase64,
                EncryptionKeyIvBase64 = this.EncryptionKeyIvBase64,
                MacKeyIvBase64 = this.MacKeyIvBase64,
                EncryptionIvBase64 = this.EncryptionIvBase64,
                CiphertextTagBase64 = this.CiphertextTagBase64
            };
        }

        new public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
