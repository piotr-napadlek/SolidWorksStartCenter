using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WNApplications.SWSC.Data
{
    public class User : INotifyPropertyChanged
    {
        private string name;
        private string fullName;
        private string settingPath;

        [XmlElement]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        [XmlElement]
        public string FullName
        {
            get
            {
                return fullName;
            }

            set
            {
                fullName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        [XmlIgnore]
        public string DisplayName
        {
            get
            {
                return Name + " - " + FullName;
            }
        }

        [XmlElement]
        public string SettingPath
        {
            get
            {
                return settingPath;
            }

            set
            {
                settingPath = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
