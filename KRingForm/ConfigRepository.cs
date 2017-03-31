using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KRingForm
{
    public class ConfigRepository
    {
        private string ConfigPath;
        private const string userStatusNode = "//applicationContext/doesUserExist";
        private const string nonExistStatus = "0";
        private const string doesExistStatus = "1";

        public ConfigRepository()
        {
            ConfigPath = ConfigurationManager.AppSettings["configPathDebug"];
        }

        public bool DoesUserExist()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(ConfigPath);

            var status = doc.SelectSingleNode(userStatusNode).InnerText;

            if (status.Equals(nonExistStatus)) return false;

            return true;
        }

        public void UserExists()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(ConfigPath);

            doc.SelectSingleNode(userStatusNode).InnerText = doesExistStatus;

            doc.Save(ConfigPath);
        }
    }
}
