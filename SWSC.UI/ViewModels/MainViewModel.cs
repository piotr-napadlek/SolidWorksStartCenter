using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WNApplications.SWSC.Data;
using WNApplications.SWSC.UI.Common;
using WNApplications.SWSC.Utilities;

namespace WNApplications.SWSC.UI
{
    internal class MainViewModel : ViewModelBase
    {
        private ObservableCollection<SWSetting> swSettings;
        private ObservableCollection<SWVersion> swVersions;
        private SWSetting selectedSetting;
        private SWVersion selectedVersion;
        private string selectedSubproject;
        private bool isSWRunning;
        private SolidWorks solidWorks;

        private int launchProgress = 0;
        private string progressMessage = "Gotowy";

        private static MainViewModel _instance; //singleton

        private ICommand editSettings;
        private ICommand editUsers;
        private ICommand backup;
        private ICommand launch;

        private MainViewModel()
        {
            if (swSettings == null)
            {
                swSettings = new ObservableCollection<SWSetting>(SWDataManager.Instance.GetSettingsData());
            }
            if (swVersions == null)
                swVersions = new ObservableCollection<SWVersion>(SWDataManager.Instance.GetSolidWorksVersions());

            SelectedVersion = SwVersions[SwVersions.Count - 1]; //default version is the latest

            foreach (var setting in SwSettings)
            {
                if (setting.IsDefault)
                {
                    SelectedSetting = setting;
                    break;
                }
            }

            if (SelectedSetting == null && SwSettings.Count > 0)
            {
                SelectedSetting = SwSettings[0];
            }
            CheckBoundVersion();
        }

        private void CheckBoundVersion()
        {
            if (SelectedSetting.BoundVersion != null && SelectedSetting.BoundVersion.DisplayName!=null && 
                SwVersions.Where(i => i.DisplayName==SelectedSetting.BoundVersion.DisplayName).First()!=null)
            {
                SelectedVersion = SwVersions.Where(i => i.DisplayName == SelectedSetting.BoundVersion.DisplayName).First();
            }
        }

        #region Properties
        public static MainViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainViewModel();
                return _instance;
            }
        }

        public ObservableCollection<SWSetting> SwSettings
        {
            get
            {
                return swSettings;
            }

            set
            {
                swSettings = value;
                OnPropertyChanged("SwSettings");
            }
        }

        public ObservableCollection<SWVersion> SwVersions
        {
            get
            {
                return swVersions;
            }

            set
            {
                swVersions = value;
                OnPropertyChanged("SwVersions");
            }
        }

        public SWSetting SelectedSetting
        {
            get
            {
                return selectedSetting;
            }

            set
            {
                selectedSetting = value;
                OnPropertyChanged("SelectedSetting");
                if (SelectedSetting != null)
                {
                    CheckBoundVersion();
                }
            }
        }

        public SWVersion SelectedVersion
        {
            get
            {
                return selectedVersion;
            }

            set
            {
                selectedVersion = value;
                OnPropertyChanged("SelectedVersion");
            }
        }

        #endregion

        #region Commands

        public ICommand ShowConfigDialogCommand
        {
            get
            {
                if (editSettings == null)
                {
                    editSettings = new CommandBase(i => ShowConfigDialog(), null);
                }
                return editSettings;
            }
        }

        public ICommand Launch
        {
            get
            {
                if (launch==null)
                {
                    launch = new CommandBase(i => RunSolidWorks(), i => SelectedSetting != null && SelectedVersion != null);
                }
                return launch;
            }
        }

        public int LaunchProgress
        {
            get
            {
                return launchProgress;
            }

            set
            {
                launchProgress = value;
                OnPropertyChanged(nameof(LaunchProgress));
            }
        }

        public string ProgressMessage
        {
            get
            {
                return progressMessage;
            }

            set
            {
                progressMessage = value;
                OnPropertyChanged(nameof(ProgressMessage));
            }
        }

        public ICommand Backup
        {
            get
            {
                if (backup==null)
                {
                    backup = new CommandBase(i => CreateBackup(), i => SelectedVersion != null);
                }
                return backup;
            }
        }

        public ICommand EditUsers
        {
            get
            {
                if (editSettings == null)
                {
                    editUsers = new CommandBase(i => ShowUsersDialog(), null);
                }
                return editUsers;
            }
        }

        #endregion

        #region Methods

        private void ShowConfigDialog()
        {
            ConfigViewModel viewModel = new ConfigViewModel();
            IModalDialog configDialog = ServiceProvider.Instance.Get<IConfigModalDialog>();
            viewModel.CloseAction = new Action(configDialog.Close);
            configDialog.BindViewModel(viewModel);
            configDialog.ShowDialog();

            OnPropertyChanged(nameof(SwSettings));
            OnPropertyChanged(nameof(SelectedSetting));
        }

        private void ShowUsersDialog()
        {
            UsersViewModel viewModel = new UsersViewModel();
            IModalDialog usersDialog = ServiceProvider.Instance.Get<IUsersModalDialog>();
            viewModel.CloseAction = new Action(usersDialog.Close);

            usersDialog.BindViewModel(viewModel);
            usersDialog.ShowDialog();
        }

        private void RunSolidWorks()
        {
            RegistryEntryList regList = new RegistryEntryList();

            if (SelectedSetting.RegSettings != null && 
                File.Exists(AppDomain.CurrentDomain.BaseDirectory + SelectedSetting.RegSettings) && 
                regList.DeserializeRegEntries(AppDomain.CurrentDomain.BaseDirectory + SelectedSetting.RegSettings))
            {
                ProgressMessage = "Wczytuję ustawienia";
                RegistryTool.GetInstance().WriteSWRegistrySettings(SelectedVersion, regList.RegEntries);
                ProgressMessage = "Wczytano ustawienia";
                LaunchProgress = 20;
            }

            RegistryTool.GetInstance().OverrideSWSettings(SelectedVersion, SelectedSetting);
            ProgressMessage = "Nadpisano ustawienia szablonów";
            LaunchProgress = 30;

            User userSettings = SWDataManager.Instance.GetUsers().Find(i => i.Name == Environment.UserName);

            if (userSettings!=null && 
                !String.IsNullOrEmpty(userSettings.SettingPath) && 
                File.Exists(AppDomain.CurrentDomain.BaseDirectory + userSettings.SettingPath) && 
                regList.DeserializeRegEntries(AppDomain.CurrentDomain.BaseDirectory + userSettings.SettingPath))
            {
                RegistryTool.GetInstance().WriteSWRegistrySettings(SelectedVersion, regList.RegEntries);
                ProgressMessage = "Wczytano ustawienia użytkownika";
                LaunchProgress = 40;
            }

            solidWorks = new SolidWorks(SelectedVersion);
            solidWorks.ProcessStarted += ProcessLaunchStarted;
            solidWorks.InstanceStarted += InstanceObtained;
            solidWorks.ProcessEnded += ProcessExited;
            solidWorks.SWIdle += SWReadyIdle;

            solidWorks.StartRunningProcess();
        }

        private void SWReadyIdle()
        {
            LaunchProgress = 100;
            ProgressMessage = "SolidWorks uruchomiony";
        }

        private void ProcessExited()
        {
            LaunchProgress = 0;
            ProgressMessage = "Gotowy";
            isSWRunning = false;
        }

        private void InstanceObtained()
        {
            LaunchProgress = 90;
            ProgressMessage = "Obiekt SldApp zainicjowany";
            isSWRunning = true;
        }

        private void ProcessLaunchStarted()
        {
            LaunchProgress = 60;
            ProgressMessage = "Uruchamiam SolidWorks";
        }

        private void CreateBackup()
        {
            string regClr = @"Windows Registry Editor Version 5.00

[-HKEY_CURRENT_USER\" + SelectedVersion.RegPath + @"]
";
            if (File.Exists("C:\\Temp\\reg.reg"))
            {
                File.Delete("C:\\Temp\\reg.reg");
            }

            Process regProc = new Process();
            regProc.StartInfo.FileName = "regedit";
            regProc.StartInfo.Arguments = "/e C:\\Temp\\reg.reg " + "\"HKEY_CURRENT_USER\\" + SelectedVersion.RegPath + "\"";
            regProc.Start();
            regProc.WaitForExit();

            if (File.Exists("C:\\Temp\\reg.reg"))
            {
                string regFile = File.ReadAllText("C:\\Temp\\reg.reg").Replace("Windows Registry Editor Version 5.00", regClr);

                string buTimeStamp = DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToString("HHmmss") + "_" + Environment.UserName;

                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "backup\\"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "backup\\");
                }

                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "backup\\" + buTimeStamp + ".reg", regFile);
                ProgressMessage = "Kopia ustawień " + buTimeStamp + ".reg";
            }
            else
            {
                ProgressMessage = "Błąd przy tworzeniu kopii zapasowej!";
            }
        }
        #endregion
    }
}
