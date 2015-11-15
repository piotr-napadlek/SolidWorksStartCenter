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
    class DrawingTemplatesViewModel : ViewModelBase
    {
        private SWSetting editedSetting;
        private ObservableCollection<DrawingTemplate> drawingTemplates;
        private DrawingTemplate selectedTemplate;

        private ICommand addTemplateCommand;
        private ICommand chooseFileCommand;
        private ICommand deleteTemplateCommand;
        private ICommand moveUpCommand;
        private ICommand moveDownCommand;
        private ICommand okCommand;
        
        public DrawingTemplatesViewModel(SWSetting editedSetting)
        {
            this.editedSetting = editedSetting;
            drawingTemplates = new ObservableCollection<DrawingTemplate>();
            editedSetting.DrwTemplates.ForEach(i => drawingTemplates.Add(i.Clone())); //deep copy of settings list
        }

        public ObservableCollection<DrawingTemplate> DrawingTemplates
        {
            get
            {
                return drawingTemplates;
            }
            set
            {
                drawingTemplates = value;
                OnPropertyChanged(nameof(DrawingTemplates));
            }
        }

        public DrawingTemplate SelectedTemplate
        {
            get
            {
                return selectedTemplate;
            }
            set
            {
                selectedTemplate = value;
                OnPropertyChanged(nameof(SelectedTemplate));
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public ICommand AddTemplateCommand
        {
            get
            {
                if(addTemplateCommand==null)
                {
                    addTemplateCommand = new CommandBase(i => AddTemplate(), null);
                }
                return addTemplateCommand;
            }
        }

        private void AddTemplate()
        {
            DrawingTemplate tempTemplate = new DrawingTemplate() { DrwTemplateName = "A?", SlddrtPath = " " };
            DrawingTemplates.Add(tempTemplate);
            SelectedTemplate = tempTemplate;
            OnPropertyChanged(nameof(DrawingTemplates));
        }

        public bool IsSelected
        {
            get
            {
                return SelectedTemplate != null;
            }
        }

        public ICommand ChooseFileCommand
        {
            get
            {
                if (chooseFileCommand==null)
                {
                    chooseFileCommand = new CommandBase(ChooseFile, i => SelectedTemplate != null);
                }
                return chooseFileCommand;
            }
        }

        public ICommand DeleteTemplateCommand
        {
            get
            {
                if(deleteTemplateCommand==null)
                {
                    deleteTemplateCommand = new CommandBase(i => DeleteTemplate(), i => SelectedTemplate != null);
                }
                return deleteTemplateCommand;
            }
        }

        public ICommand MoveUpCommand
        {
            get
            {
                if(moveUpCommand==null)
                {
                    moveUpCommand = new CommandBase(i => MoveUp(), i => SelectedTemplate != null);
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
                    moveDownCommand = new CommandBase(i => MoveDown(), i => SelectedTemplate != null);
                }
                return moveDownCommand;
            }
        }

        public ICommand OkCommand
        {
            get
            {
                if(okCommand==null)
                {
                    okCommand = new CommandBase(i => Ok(), null);
                }
                return okCommand;
            }
        }

        public Action CloseAction { get; set; }

        private void ChooseFile(object obj)
        {
            System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)obj;
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();

            fileDialog.Multiselect = false;
            fileDialog.CheckFileExists = true;
            fileDialog.Filter = "*.slddrt|*.SLDDRT";

            if (System.IO.File.Exists(textBox.Text) || System.IO.Directory.Exists(textBox.Text))
            {
                fileDialog.InitialDirectory = System.IO.Path.GetFullPath(textBox.Text);
            }
            else
            {
                fileDialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            }

            fileDialog.Title = "Wybierz plik szablonu";
            fileDialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fileDialog.FileName))
            {
                textBox.Text = fileDialog.FileName;
            }
            
        }

        private void DeleteTemplate()
        {
            DrawingTemplates.Remove(SelectedTemplate);
        }

        private void MoveUp()
        {
            int selectedIndex = DrawingTemplates.IndexOf(SelectedTemplate);

            if (selectedIndex > 0)
                DrawingTemplates.Move(selectedIndex, selectedIndex - 1);
        }

        private void MoveDown()
        {
            int selectedIndex = DrawingTemplates.IndexOf(SelectedTemplate);

            if (selectedIndex < DrawingTemplates.Count-1)
                DrawingTemplates.Move(selectedIndex, selectedIndex + 1);
        }

        private void Ok()
        {
            editedSetting.DrwTemplates = DrawingTemplates.ToList();
            CloseAction();
        }
    }
}
