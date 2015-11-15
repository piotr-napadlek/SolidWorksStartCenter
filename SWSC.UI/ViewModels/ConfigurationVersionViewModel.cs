using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WNApplications.SWSC.Data;
using WNApplications.SWSC.UI.Common;
using System.Windows.Input;
using System.ComponentModel;
using System.IO;
using WNApplications.SWSC.Utilities;

namespace WNApplications.SWSC.UI
{
    class ConfigurationVersionViewModel : ViewModelBase
    {
        private bool isSolidWorksRunning;
        private SWSetting editedSetting;
        private List<RegistryEntry> regEntries;
        private SldWorks.SldWorks swInstance;
        private bool isFormEnabled;
        private ICommand launchCommand;
        private SolidWorks solidWorks;
        private string formMessage;
        private string messageVisibility;

        public LaunchMode Mode
        {
            get; set;
        }

        public ObservableCollection<SWVersion> SwVersions
        {
            get
            {
                return MainViewModel.Instance.SwVersions;
            }
        }

        public SWVersion SelectedVersion
        {
            get
            {
                return MainViewModel.Instance.SelectedVersion;
            }
            set
            {
                MainViewModel.Instance.SelectedVersion = value;
            }
        }

        public bool IsSolidWorksRunning
        {
            get
            {
                return isSolidWorksRunning;
            }

            set
            {
                isSolidWorksRunning = value;
                OnPropertyChanged(nameof(IsSolidWorksRunning));
            }
        }

        public SWSetting EditedSetting
        {
            get
            {
                return editedSetting;
            }

            set
            {
                editedSetting = value;
            }
        }

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

        public SldWorks.SldWorks SwInstance
        {
            get
            {
                return swInstance;
            }

            set
            {
                swInstance = value;
            }
        }

        public bool IsFormEnabled
        {
            get
            {
                return isFormEnabled;
            }
            set
            {
                isFormEnabled = value;
                OnPropertyChanged(nameof(IsFormEnabled));
            }
        }

        public ICommand LaunchCommand
        {
            get
            {
                if (launchCommand == null)
                {
                    launchCommand = new CommandBase(i => RunSolidWorks(), i => SelectedVersion != null);
                }
                return launchCommand;
            }
        }

        public string FormMessage
        {
            get
            {
                return formMessage;
            }

            set
            {
                formMessage = value;
                OnPropertyChanged(nameof(FormMessage));
            }
        }

        public string MessageVisibility
        {
            get
            {
                return messageVisibility;
            }

            set
            {
                messageVisibility = value;
                OnPropertyChanged(nameof(MessageVisibility));
            }
        }

        public ConfigurationVersionViewModel(SWSetting editedSetting)
        {
            isSolidWorksRunning = false;
            this.editedSetting = editedSetting;

            if (EditedSetting.BoundVersion != null && SwVersions.Contains(EditedSetting.BoundVersion))
            {
                SelectedVersion = EditedSetting.BoundVersion;
            }

            IsFormEnabled = true;
            MessageVisibility = "Hidden";
        }

        private void RunSolidWorks()
        {
            if (Mode==LaunchMode.ProjectSetting)
            {
                RegistryEntryList regList = new RegistryEntryList();
                if (editedSetting.RegSettings != null && File.Exists(AppDomain.CurrentDomain.BaseDirectory + editedSetting.RegSettings) && regList.DeserializeRegEntries(AppDomain.CurrentDomain.BaseDirectory + editedSetting.RegSettings))
                {
                    RegistryTool.GetInstance().WriteSWRegistrySettings(SelectedVersion, regList.RegEntries);
                }

                RegistryTool.GetInstance().OverrideSWSettings(SelectedVersion, EditedSetting); 
            }

            solidWorks = new SolidWorks(SelectedVersion);
            solidWorks.ProcessStarted += ProcessLaunchStarted;
            solidWorks.InstanceStarted += InstanceObtained;
            solidWorks.ProcessEnded += ProcessExited;

            solidWorks.StartRunningProcess();
        }

        private void ProcessLaunchStarted()
        {
            IsFormEnabled = false;
            FormMessage = "Uruchamiam " + SelectedVersion.DisplayName;
            MessageVisibility = "Visible";
        }

        private void InstanceObtained()
        {
            IsSolidWorksRunning = true;
            FormMessage = "Uruchomiono " + SelectedVersion.DisplayName + ". \nZamknij SolidWorksa by zapisać ustawienia.";
        }

        private void ProcessExited()
        {
            IsFormEnabled = true;

            // MessageVisibility = "Hidden";

            FormMessage = "Pobrano ustawienia. \nZamknij to okno i zapisz ustawienia by wprowadzić zmiany.";

            if(IsSolidWorksRunning)
            {
                List<string> keysList = new List<string>();

                if(Mode==LaunchMode.ProjectSetting)
                {
                    keysList.Add(SelectedVersion.RegPath);
                }
                else if (Mode == LaunchMode.UserSetting)
                {
                    keysList.Add(SelectedVersion.RegPath + "\\User Interface");
                    keysList.Add(SelectedVersion.RegPath + "\\Menu Customizations");

                }

                RegistryEntryList regList = new RegistryEntryList(RegistryTool.GetInstance().ParseSWSettings(SelectedVersion, keysList));
                string timeStamp = DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToLongTimeString().GetHashCode().ToString();
                string savePath = AppDomain.CurrentDomain.BaseDirectory + "regsettings\\" + timeStamp + ".regsetting";
                if (regList.SerializeRegEntries(savePath))
                {
                    editedSetting.RegSettings = "regsettings\\" + timeStamp + ".regsetting";
                }

                editedSetting.BoundVersion = SelectedVersion;
            }
            else
            {
                MessageVisibility = "Hidden";
            }

            IsSolidWorksRunning = false;
        }

        public void OnCancel(object sender, CancelEventArgs e)
        {
            if (IsFormEnabled == false)
            {
                e.Cancel = true;
            }
        }
    }
}
