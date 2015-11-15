using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WNApplications.SWSC.Data;
using WNApplications.SWSC.UI.Common;

namespace WNApplications.SWSC.UI
{
    class UsersViewModel : ViewModelBase
    {
        private ObservableCollection<User> users;
        private User selectedUser;
        private string settingsParsedMsg;

        private ICommand addUserCommand;
        private ICommand deleteUserCommand;
        private ICommand moveUpCommand;
        private ICommand moveDownCommand;

        private ICommand okCommand;
        private ICommand runSWCommand;

        public ObservableCollection<User> Users
        {
            get
            {
                return users;
            }

            set
            {
                users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public User SelectedUser
        {
            get
            {
                return selectedUser;
            }

            set
            {
                selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public ICommand AddUserCommand
        {
            get
            {
                if (addUserCommand==null)
                {
                    addUserCommand = new CommandBase(i => AddUser(), null);
                }
                return addUserCommand;
            }
        }

        public ICommand DeleteUserCommand
        {
            get
            {
                if (deleteUserCommand == null)
                {
                    deleteUserCommand = new CommandBase(i => DeleteUser(), i => SelectedUser != null);
                }
                return deleteUserCommand;
            }
        }

        public ICommand MoveUpCommand
        {
            get
            {
                if (moveUpCommand==null)
                {
                    moveUpCommand = new CommandBase(i => MoveUp(), i => SelectedUser != null);
                }
                return moveUpCommand;
            }
        }

        public ICommand MoveDownCommand
        {
            get
            {
                if(moveDownCommand==null)
                {
                    moveDownCommand = new CommandBase(i => MoveDown(), i => SelectedUser != null);
                }
                return moveDownCommand;
            }
        }

        public ICommand OkCommand
        {
            get
            {
                if (okCommand==null)
                {
                    okCommand = new CommandBase(i => SaveUsers(), null);
                }
                return okCommand;
            }
        }

        public Action CloseAction
        {
            get; set;
        }

        public ICommand RunSWCommand
        {
            get
            {
                if(runSWCommand==null)
                {
                    runSWCommand = new CommandBase(i => RunSolidWorks(), i => SelectedUser!=null && SelectedUser.Name == Environment.UserName);
                }
                return runSWCommand;
            }
        }

        public string SettingsParsedMsg
        {
            get
            {
                return settingsParsedMsg;
            }

            set
            {
                settingsParsedMsg = value;
                OnPropertyChanged(nameof(SettingsParsedMsg));
            }
        }

        public UsersViewModel()
        {
            users = new ObservableCollection<User>(SWDataManager.Instance.GetUsers());
        }

        private void AddUser()
        {
            User newUser = new User() { Name = "xyz", FullName = "Imię Nazwisko" };
            Users.Add(newUser);
            SelectedUser = newUser;
        }

        private void DeleteUser()
        {
            Users.Remove(SelectedUser);
        }

        private void MoveDown()
        {
            int selectedIndex = Users.IndexOf(SelectedUser);

            if (selectedIndex < Users.Count - 1)
                Users.Move(selectedIndex, selectedIndex + 1);
        }

        private void MoveUp()
        {
            int selectedIndex = Users.IndexOf(SelectedUser);

            if (selectedIndex > 0)
                Users.Move(selectedIndex, selectedIndex - 1);
        }

        private void SaveUsers()
        {
            SWDataManager.Instance.SetUsers(Users);
            CloseAction();
        }

        private void RunSolidWorks()
        {
            if (MainViewModel.Instance.SwVersions == null || MainViewModel.Instance.SwVersions.Count == 0)
            {
                return;
            }

            SWSetting dummySWSetting = new SWSetting();

            ConfigurationVersionViewModel viewModel = new ConfigurationVersionViewModel(dummySWSetting);
            viewModel.Mode = LaunchMode.UserSetting;

            IConfigurationVersionModalDialog view = ServiceProvider.Instance.Get<IConfigurationVersionModalDialog>();
            view.BindViewModel(viewModel);
            view.Closing += viewModel.OnCancel;
            view.ShowDialog();

            if (!String.IsNullOrEmpty(dummySWSetting.RegSettings))
            {
                SelectedUser.SettingPath = dummySWSetting.RegSettings;
                SettingsParsedMsg = "Pobrano ustawienia dla " + Environment.UserName + ". \nZapisz by zachować zmiany.";
            }
        }
    }
}
