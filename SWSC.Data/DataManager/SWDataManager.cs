using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNApplications.SWSC.Data
{
    public class SWDataManager : IDataManager

    {
        #region SingletonImplementation
        private static SWDataManager _instance;

        private SWDataManager()
        { }

        public static SWDataManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SWDataManager();
                }
                return _instance;
            }
        }
        #endregion

        public List<SWSetting> GetSettingsData()
        {
            SWSettingsList swSettingsList = new SWSettingsList();
            if(swSettingsList.DeserializeSettings() == true)
            {
                return swSettingsList.SwSettings;
            }
            return null;
        }

        public SWVersion[] GetSolidWorksVersions()
        {
            SolidWorksVersions swVersions = new SolidWorksVersions();

            return swVersions.SWInstallations;
        }

        public void UpdateSettingsData(List<SWSetting> swSettings)
        {
            SWSettingsList swSettingsList = new SWSettingsList() { SwSettings = swSettings };
            swSettingsList.SerializeSettings();
        }

        public List<User> GetUsers()
        {
            UsersList userList = new UsersList();

            if (userList.DeserializeUsers())
            {
                return userList.Users;
            }
            else
            {
                return new List<User>();
            }
        }

        public bool SetUsers(IEnumerable<User> users)
        {
            UsersList usersList = new UsersList(users);
            return usersList.SerializeUsers();
        }
    }
}
