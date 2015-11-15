using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WNApplications.SWSC.Data
{
    /// <summary>
    /// A representative of SolidWorks Installation
    /// </summary>
    public class SWVersion : IComparable<SWVersion>
    {
        #region fields
        private string displayName;
        private string installLocation;
        private string exePath;
        private string regPath;
        #endregion

        [XmlElement]
        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
            }
        }
        [XmlElement]
        public string InstallLocation
        {
            get { return installLocation; }
            set { installLocation = value; }
        }

        [XmlElement]
        public string ExePath
        {
            get { return exePath; }
            set { exePath = value; }
        }

        [XmlElement]
        public string RegPath
        {
            get { return regPath; }
            set { regPath = value; }
        }

        public SWVersion() { }

        public SWVersion(string name, string location) : this(name, location, "", "")
        {
        }

        public SWVersion(string name, string location, string exePath, string regPath)
        {
            this.displayName = name;
            this.installLocation = location;
            this.exePath = exePath;
            this.regPath = regPath;
        }

        public int CompareTo(SWVersion other)
        {
            return this.DisplayName.CompareTo(other.DisplayName);
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
