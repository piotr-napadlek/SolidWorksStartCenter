using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNApplications.SWSC.UI
{
    class ConfigViewModalDialog : IConfigModalDialog
    {
        private ConfigView configView;

        void IModalDialog.BindViewModel<TViewModel>(TViewModel viewModel)
        {
            GetDialog().DataContext = viewModel;   
        }

        void IModalDialog.Close()
        {
            GetDialog().Close();
        }

        void IModalDialog.ShowDialog()
        {
            GetDialog().ShowDialog();
        }

        private ConfigView GetDialog()
        {
            if (configView == null)
            {
                configView = new ConfigView();
                configView.Closed += new EventHandler(DialogClosed);
            }
            return configView;
        }

        private void DialogClosed(object sender, EventArgs e)
        {
            configView = null;
        }
    }
}
