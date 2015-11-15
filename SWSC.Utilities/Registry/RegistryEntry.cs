using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WNApplications.SWSC.Data
{
    public class RegistryEntry
    {
        private string keyName;
        private string entryName;
        private object entryValueAsObject;
        private string valueType;
        private string entryValueString;

        [XmlAttribute]
        public string KeyName
        {
            get
            {
                return keyName;
            }

            set
            {
                keyName = value;
            }
        }

        [XmlAttribute]
        public string EntryValue
        {
            get
            {
                return entryValueString;
            }
            set
            {
                entryValueString = value;
            }
        }

        [XmlAttribute]
        public string EntryName
        {
            get
            {
                return entryName;
            }

            set
            {
                entryName = value;
            }
        }

        [XmlAttribute]
        public string ValueType
        {
            get
            {
                return valueType;
            }

            set
            {
                valueType = value;
                entryValueAsObject = PerformTypeConversionToRegistry(entryValueString,value);
            }
        }

        [XmlIgnore]
        public object EntryValueAsObject
        {
            get
            {
                return entryValueAsObject;
            }
            set
            {
                entryValueAsObject = value;
            }
        }

        public RegistryEntry() { }

        public RegistryEntry(string key, string name, object value, string valueType)
        {
            keyName = key;
            entryName = name;
            entryValueAsObject = value;
            this.valueType = valueType;
            entryValueString = PerformTypeConversionFromRegistry(value);
        }

        public RegistryEntry(string key, string name, string value, string valueType)
        {
            keyName = key;
            entryName = name;
            entryValueString = value;
            this.valueType = valueType;
            entryValueAsObject = PerformTypeConversionToRegistry(value, valueType);
        }

        private object PerformTypeConversionToRegistry(string value, string type)
        {
            object tempObj = null;

            if(type == "System.Byte[]")
            {
                string[] values = value.Split(' ');
                List<byte> bytes = new List<byte>();

                foreach (string val in values)
                {
                    System.Byte tempByte;
                    byte.TryParse(val, out tempByte);
                    bytes.Add(tempByte);
                }
                tempObj = bytes.ToArray();
            }
            else
            {
                tempObj = Convert.ChangeType(value, Type.GetType(type));
            }

            return tempObj;
        }

        private string PerformTypeConversionFromRegistry(object value)
        {
            string retValue;

            if (value.GetType().ToString() == "System.Byte[]")
            {
                byte[] bytes = (byte[])value;
                List<string> strBytes = new List<string>();
                foreach (byte tempByte in bytes)
                {
                    strBytes.Add(tempByte.ToString());
                }
                retValue = String.Join(" ", strBytes);
            }
            else
            {
                retValue = value.ToString();
            }

            return retValue;
        }
    }
}

