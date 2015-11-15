using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WNApplications.SWSC.Data;
using WNApplications.SWSC.UI.Common;
using Ookii.Dialogs.Wpf;

namespace WNApplications.SWSC.UI
{
    class ConfigViewModel : ViewModelBase
    {
        #region Fields

        private ObservableCollection<SWSetting> swSettings;
        private SWSetting selectedSetting;

        private ICommand cloneSettingCommand;
        private ICommand deleteSettingCommand;
        private ICommand addSubProjectCommand;
        private ICommand deleteSubProjectCommand;
        private ICommand setFolderCommand;
        private ICommand editDrawingTemplatesCommand;
        private ICommand okCommand;
        private ICommand runSolidworksCommand;        
                
        #endregion

        #region Properties

        public SWSetting SelectedSetting
        {
            get
            {
                return selectedSetting;
            }

            set
            {
                selectedSetting = value;
                OnPropertyChanged(nameof(SelectedSetting));
                OnPropertyChanged(nameof(NotLocked));
                OnPropertyChanged(nameof(SubProjectsActive));
                OnPropertyChanged(nameof(IsDefault));
                OnPropertyChanged(nameof(ContainsSubprojects));
            }
        }

        public bool IsDefault
        {
            get
            {
                if (SelectedSetting != null)
                {
                    return SelectedSetting.IsDefault;
                }
                return false;
            }

            set
            {
                if (SelectedSetting.IsDefault == false)
                {
                    SwSettings.Where(i => i.IsDefault == true).First().IsDefault = false;
                    SelectedSetting.IsDefault = true;
                    OnPropertyChanged(nameof(IsDefault));
                }
            }
        }

        public bool ContainsSubprojects
        {
            get
            {
                if (SelectedSetting != null)
                {
                    return SelectedSetting.ContainsSubprojects;
                }
                return false;
            }

            set
            {
                SelectedSetting.ContainsSubprojects = value;

                if (ContainsSubprojects == false)
                {
                    SelectedSetting.SubProjects = null;
                }
                OnPropertyChanged(nameof(SubProjectsActive));
                OnPropertyChanged(nameof(ContainsSubprojects));
            }
        }

        public bool NotLocked
        {
            get
            {
                if (SelectedSetting != null)
                {
                    return SelectedSetting.NotLocked;
                }
                return false;
            }
        }

        public bool SubProjectsActive
        {
            get
            {
                return NotLocked && ContainsSubprojects;
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
                OnPropertyChanged(nameof(SwSettings));
            }
        }

        public string SubProject
        {
            get; set;
        }

        public ICommand CloneSettingCommand
        {
            get
            {
                if (cloneSettingCommand == null)
                    cloneSettingCommand = new CommandBase(i => Clone(), i => SelectedSetting!=null);
                return cloneSettingCommand;
            }
        }

        public ICommand DeleteSettingCommand
        {
            get
            {
                if (deleteSettingCommand == null)
                    deleteSettingCommand = new CommandBase(i => Delete(), i => (SelectedSetting != null && SelectedSetting.IsDefault == false && SelectedSetting.NotLocked));
                return deleteSettingCommand;
            }
        }

        public ICommand AddSubProjectCommand
        {
            get
            {
                if (addSubProjectCommand==null)
                {
                    addSubProjectCommand = new CommandBase(i => AddSubProject(), i => SubProjectsActive);
                }
                return addSubProjectCommand;
            }
        }

        public ICommand DeleteSubProjectCommand
        {
            get
            {
                if (deleteSubProjectCommand==null)
                {
                    deleteSubProjectCommand = new CommandBase(i => DeleteSubProject(), i => SubProjectsActive);
                }
                return deleteSubProjectCommand;
            }
        }

        public ICommand SetFolderCommand
        {
            get
            {
                if (setFolderCommand==null)
                {
                    setFolderCommand = new CommandBase(SetFolder, i => SelectedSetting != null);
                }
                return setFolderCommand;
            }
        }

        public ICommand EditDrawingTemplatesCommand
        {
            get
            {
                if(editDrawingTemplatesCommand==null)
                {
                    editDrawingTemplatesCommand = new CommandBase(i => ShowEditDrawingTemplatesView(), i => SelectedSetting != null);
                }
                return editDrawingTemplatesCommand;
            }
        }

        public Action CloseAction { get; set; }

        public ICommand OkCommand
        {
            get
            {
                if (okCommand==null)
                {
                    okCommand = new CommandBase(i => Ok(), null);
                }
                return okCommand;
            }
        }

        public ICommand RunSolidworksCommand
        {
            get
            {
                if(runSolidworksCommand==null)
                {
                    runSolidworksCommand = new CommandBase(i => RunSolidWorks(), i => SelectedSetting != null);
                }
                return runSolidworksCommand;
            }
        }

        #endregion

        public ConfigViewModel()
        {
            SwSettings = new ObservableCollection<SWSetting>();
            MainViewModel.Instance.SwSettings.ToList().ForEach(i => SwSettings.Add(i.CloneSetting())); //deep copy
            SelectedSetting = SwSettings[MainViewModel.Instance.SwSettings.IndexOf(MainViewModel.Instance.SelectedSetting)];
        }

        public override string ToString()
        {
            return SelectedSetting.ProjectName;
        }

        private void Clone()
        {
            SWSetting clonedSetting = SelectedSetting.CloneSetting();
            clonedSetting.ProjectName = clonedSetting.ProjectName + " - kopia";
            clonedSetting.IsDefault = false;
            clonedSetting.NotLocked = true;
            SwSettings.Add(clonedSetting);
            SelectedSetting = clonedSetting;
        }

        private void Delete()
        {
            ObservableCollection<SWSetting> tempCopy = new ObservableCollection<SWSetting>(SwSettings);
            if (SwSettings.Count > 1)
                tempCopy.Remove(SelectedSetting);

            SwSettings = tempCopy;
            SelectedSetting = SwSettings.FirstOrDefault();
        }

        private void RefreshList()
        {
            List<SWSetting> tempSettings = SwSettings.ToList();
            SWSetting tempSetting = SelectedSetting;
            SwSettings = null; //for instant combobox update, comment out if unnecesary
            SwSettings = new ObservableCollection<SWSetting>(tempSettings);
            SelectedSetting = tempSetting;
        }

        private void AddSubProject()
        {
            List<string> tempSubProjects = SelectedSetting.SubProjects==null ? new List<string>() : SelectedSetting.SubProjects.ToList();
            tempSubProjects.Add(SubProject);
            SelectedSetting.SubProjects = tempSubProjects.ToArray();
            OnPropertyChanged(nameof(SelectedSetting));
        }

        private void DeleteSubProject()
        {
            List<string> tempSubProjects = SelectedSetting.SubProjects.ToList();
            tempSubProjects.Remove(SubProject);
            SelectedSetting.SubProjects = tempSubProjects.ToArray();
            OnPropertyChanged(nameof(SelectedSetting));
        }

        private void SetFolder(object obj)
        {
            VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
            System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)obj;

            folderDialog.Reset();
            folderDialog.ShowNewFolderButton = true;
            folderDialog.UseDescriptionForTitle = true;
            folderDialog.Description = "Wskaż folder";

            if (System.IO.Directory.Exists(textBox.Text))
                folderDialog.SelectedPath = textBox.Text;
            else
                folderDialog.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            
            folderDialog.ShowDialog();

            textBox.Text = folderDialog.SelectedPath;
        }

        private void ShowEditDrawingTemplatesView()
        {
            DrawingTemplatesViewModel viewModel = new DrawingTemplatesViewModel(SelectedSetting);

            IDrawingTemplatesModalDialog view = ServiceProvider.Instance.Get<IDrawingTemplatesModalDialog>();
            view.BindViewModel(viewModel);
            viewModel.CloseAction = new Action(() => view.Close());

            view.ShowDialog();
            OnPropertyChanged(nameof(SelectedSetting));
        }

        private void Ok()
        {
            int selectedIndex = MainViewModel.Instance.SwSettings.IndexOf(MainViewModel.Instance.SelectedSetting);
            SWDataManager.Instance.UpdateSettingsData(SwSettings.ToList());
            MainViewModel.Instance.SwSettings = SwSettings;
            MainViewModel.Instance.SelectedSetting = MainViewModel.Instance.SwSettings[selectedIndex];
            CloseAction();
        }

        private void RunSolidWorks()
        {
            if (MainViewModel.Instance.SwVersions == null || MainViewModel.Instance.SwVersions.Count == 0)
            {
                return;
            }

            ConfigurationVersionViewModel viewModel = new ConfigurationVersionViewModel(SelectedSetting);
            viewModel.Mode = LaunchMode.ProjectSetting;

            IConfigurationVersionModalDialog view = ServiceProvider.Instance.Get<IConfigurationVersionModalDialog>();
            view.BindViewModel(viewModel);
            view.Closing += viewModel.OnCancel;
            view.ShowDialog();
        }
    }
}
