using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNApplications.SWSC.UI
{
    class ConfigurationVersionModalDialog : IConfigurationVersionModalDialog
    {
        private ConfigurationVersionView configView;

        public event CancelEventHandler Closing;

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

        private ConfigurationVersionView GetDialog()
        {
            if (configView == null)
            {
                configView = new ConfigurationVersionView();
                configView.Closing += Closing;
                configView.Closing += DialogClosing;
                configView.Closed += new EventHandler(DialogClosed);
            }
            return configView;
        }

        private void DialogClosed(object sender, EventArgs e)
        {
            configView = null;
        }

        private void DialogClosing(object sender, CancelEventArgs e)
        {
            Closing(sender, e);
        }
    }
}
