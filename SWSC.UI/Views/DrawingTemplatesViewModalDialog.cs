using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNApplications.SWSC.UI
{
    class DrawingTemplatesViewModalDialog : IDrawingTemplatesModalDialog
    {
        private DrawingTemplatesView drawingTemplatesView;

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

        private DrawingTemplatesView GetDialog()
        {
            if (drawingTemplatesView == null)
            {
                drawingTemplatesView = new DrawingTemplatesView();
                drawingTemplatesView.Closed += new EventHandler(DialogClosed);
            }
            return drawingTemplatesView;
        }

        private void DialogClosed(object sender, EventArgs e)
        {
            drawingTemplatesView = null;
        }
    }
}
