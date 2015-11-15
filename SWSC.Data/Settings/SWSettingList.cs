using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WNApplications.SWSC.Data
{
    [XmlRoot(ElementName = "SWSettingsList", IsNullable = true)]
    public class SWSettingsList
    {
        private List<SWSetting> swSetting;

        [XmlArrayItem(typeof(SWSetting), IsNullable = true)]
        public List<SWSetting> SwSettings
        {
            get
            {
                return swSetting;
            }

            set
            {
                swSetting = value;
            }
        }

        public SWSettingsList()
        {
            swSetting = new List<SWSetting>();
        }

        public bool SerializeSettings(string filePath = "settings.xml")
        {
            try
            {
                XmlSerializer xmlSr = new XmlSerializer(typeof(SWSettingsList));
                TextWriter streamWriter = new StreamWriter(filePath, false);
                xmlSr.Serialize(streamWriter, this);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                return false;
            }
            return true;
        }

        public bool DeserializeSettings(string filePath = "settings.xml")
        {
            try
            {
                XmlSerializer xmlSr = new XmlSerializer(typeof(SWSettingsList));
                TextReader streamReader = new StreamReader(filePath);
                SWSettingsList tempSettingsContainer = (SWSettingsList)xmlSr.Deserialize(streamReader);
                this.swSetting = tempSettingsContainer.swSetting;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return "";
        }
    }
}
