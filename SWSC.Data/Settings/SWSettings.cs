using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WNApplications.SWSC.Data
{

    public class SWSetting
    {
        private int settingID;
        private string projectName;
        private bool containsSubprojects;
        private bool isDefault;
        private string[] subProjects;
        private bool notLocked;
        private string mainPath;
        private string workPath;
        private string normalizedPath;
        private string boughtPath;
        private string archivePath;
        private string outPath;
        private string inPath;
        private string remarkDirectory;
        private string drawingTableDirectory;
        private string pdmTemplateDirectory;
        private List<DrawingTemplate> drwTemplates;
        private string tableTemplateDirectory;
        private string regSettings;
        private SWVersion boundVersion;

        //public SWSetting()
        //{
        //    //this.DrwTemplates = new List<DrawingTemplate>();
        //}

        [XmlAttribute]
        public int SettingID
        {
            get
            {
                return settingID;
            }

            set
            {
                settingID = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string ProjectName
        {
            get
            {
                return projectName;
            }

            set
            {
                projectName = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public bool ContainsSubprojects
        {
            get
            {
                return containsSubprojects;
            }

            set
            {
                containsSubprojects = value;
            }
        }

        [XmlElement]
        public bool IsDefault
        {
            get
            {
                return isDefault;
            }

            set
            {
                isDefault = value;
            }
        }

        [XmlArrayItem(IsNullable = false)]
        public string[] SubProjects
        {
            get
            {
                return subProjects;
            }

            set
            {
                subProjects = value;
            }
        }

        [XmlElement]
        public bool NotLocked
        {
            get
            {
                return notLocked;
            }

            set
            {
                notLocked = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string MainPath
        {
            get
            {
                return mainPath;
            }

            set
            {
                mainPath = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string WorkPath
        {
            get
            {
                return workPath;
            }

            set
            {
                workPath = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string NormalizedPath
        {
            get
            {
                return normalizedPath;
            }

            set
            {
                normalizedPath = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string BoughtPath
        {
            get
            {
                return boughtPath;
            }

            set
            {
                boughtPath = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string ArchivePath
        {
            get
            {
                return archivePath;
            }

            set
            {
                archivePath = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string OutPath
        {
            get
            {
                return outPath;
            }

            set
            {
                outPath = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string InPath
        {
            get
            {
                return inPath;
            }

            set
            {
                inPath = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string RemarkDirectory
        {
            get
            {
                return remarkDirectory;
            }

            set
            {
                remarkDirectory = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string DrawingTableDirectory
        {
            get
            {
                return drawingTableDirectory;
            }

            set
            {
                drawingTableDirectory = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string PdmTemplateDirectory
        {
            get
            {
                return pdmTemplateDirectory;
            }

            set
            {
                pdmTemplateDirectory = value;
            }
        }

        [XmlArrayItem("DrwTemplates", typeof(DrawingTemplate), IsNullable = false)]
        public List<DrawingTemplate> DrwTemplates
        {
            get
            {
                return drwTemplates;
            }

            set
            {
                drwTemplates = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string TableTemplateDirectory
        {
            get
            {
                return tableTemplateDirectory;
            }

            set
            {
                tableTemplateDirectory = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public string RegSettings
        {
            get
            {
                return regSettings;
            }

            set
            {
                regSettings = value;
            }
        }

        [XmlElement(IsNullable = true)]
        public SWVersion BoundVersion
        {
            get
            {
                return boundVersion;
            }

            set
            {
                boundVersion = value;
            }
        }

        public override string ToString()
        {
            return ProjectName;
        }

        public SWSetting CloneSetting()
        {
            return (SWSetting)this.MemberwiseClone();
        }
    }

//   [XmlRoot(IsNullable = false)]
    public class DrawingTemplate : INotifyPropertyChanged
    {
        private int templateID;
        private string drwTemplateName;
        private string slddrtPath;

        public event PropertyChangedEventHandler PropertyChanged;

        [XmlAttribute()]
        public int TemplateID
        {
            get
            {
                return templateID;
            }

            set
            {
                templateID = value;
            }
        }

        [XmlElement(IsNullable = false)]
        public string DrwTemplateName
        {
            get
            {
                return drwTemplateName;
            }

            set
            {
                drwTemplateName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        [XmlElement(IsNullable = false)]
        public string SlddrtPath
        {
            get
            {
                return slddrtPath;
            }

            set
            {
                slddrtPath = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        [XmlIgnore]
        public string DisplayName
        {
            get
            {
                return drwTemplateName + " = " + slddrtPath;
            }
        }

        private void OnPropertyChanged(string name)
        {
            if(this.PropertyChanged!=null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public DrawingTemplate Clone()
        {
            return (DrawingTemplate)this.MemberwiseClone();
        }
    }
}
