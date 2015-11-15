using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNApplications.SWSC.UI
{
    class UsersViewModalDialog : IUsersModalDialog
    {
        private UsersView usersView;

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

        private UsersView GetDialog()
        {
            if (usersView == null)
            {
                usersView = new UsersView();
                usersView.Closed += new EventHandler(DialogClosed);
            }
            return usersView;
        }

        private void DialogClosed(object sender, EventArgs e)
        {
            usersView = null;
        }
    }
}
