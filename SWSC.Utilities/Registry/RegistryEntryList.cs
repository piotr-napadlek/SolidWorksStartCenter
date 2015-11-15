using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WNApplications.SWSC.Data
{
    [XmlRoot(ElementName ="RegistryEntryList", IsNullable = true)]
    public class RegistryEntryList
    {
        private List<RegistryEntry> regEntries;

        [XmlArrayItem(typeof(RegistryEntry), IsNullable =true)]
        public List<RegistryEntry> RegEntries
        {
            get
            {
                return regEntries;
            }

            set
            {
                regEntries = value;
            }
        }

        public RegistryEntryList()
        {
            regEntries = new List<RegistryEntry>();
        }

        public RegistryEntryList(IEnumerable<RegistryEntry> entries)
        {
            regEntries = new List<RegistryEntry>(entries);
        }

        public bool SerializeRegEntries(string filePath)
        {
            try
            {
                XmlSerializer xmlSr = new XmlSerializer(typeof(RegistryEntryList));
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

        public bool DeserializeRegEntries(string filePath)
        {
            try
            {
                XmlSerializer xmlSr = new XmlSerializer(typeof(RegistryEntryList));
                TextReader streamReader = new StreamReader(filePath);
                RegistryEntryList tempRegEntries = (RegistryEntryList)xmlSr.Deserialize(streamReader);
                this.regEntries = tempRegEntries.regEntries;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                return false;
            }
            return true;
        }
    }
}
