using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNApplications.SWSC.UI
{
    class Bootstrapper
    {
        public static void Initialize()
        {
            ServiceProvider.RegisterServiceLocator(new UnityServiceLocator());

            ServiceProvider.Instance.Register<IConfigModalDialog, ConfigViewModalDialog>();
            ServiceProvider.Instance.Register<IUsersModalDialog, UsersViewModalDialog>();
            ServiceProvider.Instance.Register<IDrawingTemplatesModalDialog, DrawingTemplatesViewModalDialog>();
            ServiceProvider.Instance.Register<IConfigurationVersionModalDialog, ConfigurationVersionModalDialog>();
        }
    }
}
