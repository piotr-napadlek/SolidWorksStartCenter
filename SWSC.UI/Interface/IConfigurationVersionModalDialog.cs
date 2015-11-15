using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNApplications.SWSC.UI
{
    interface IConfigurationVersionModalDialog : IModalDialog
    {
        event CancelEventHandler Closing;
    }
}
