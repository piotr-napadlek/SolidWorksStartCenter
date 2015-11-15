using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WNApplications.SWSC.Data
{
    [XmlRoot]
    public class UsersList
    {
        private List<User> users;

        [XmlArrayItem]
        public List<User> Users
        {
            get
            {
                return users;
            }

            set
            {
                users = value;
            }
        }

        public UsersList()
        {
            Users = new List<User>();
        }

        public UsersList(IEnumerable<User> inUsers)
        {
            Users = new List<User>(inUsers);
        }

        public bool SerializeUsers(string filePath = "users.xml")
        {
            try
            {
                XmlSerializer xmlSr = new XmlSerializer(typeof(UsersList));
                TextWriter streamWriter = new StreamWriter(filePath, false);
                xmlSr.Serialize(streamWriter, this);
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                return false;
            }
            return true;
        }

        public bool DeserializeUsers(string filePath = "users.xml")
        {
            try
            {
                XmlSerializer xmlSr = new XmlSerializer(typeof(UsersList));
                TextReader streamReader = new StreamReader(filePath);
                UsersList tempSettingsContainer = (UsersList)xmlSr.Deserialize(streamReader);
                this.users = tempSettingsContainer.users;
                streamReader.Close();
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
