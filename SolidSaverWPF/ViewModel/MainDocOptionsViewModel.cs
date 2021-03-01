using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolidSaverWPF.Data;
using SolidSaverWPF.MessagesType;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn;
using SWAPIlib.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel
{
    public class MainDocOptionsViewModel : ViewModelBase
    {
        public MainDocOptionsViewModel()
        {
            GlobalEvents.MainModelChanged += MainModelChanged;
            GlobalEvents.WorkFolderChanged += UpdateWorkFolder;
        }

        private void MainModelChanged(object sender, EventArgs e)
        {
            DocName = System.IO.Path.GetFileName(Variables.GetMainModel().Path);
            UpdateWorkFolder(null, null);
        }

        private string _DocName;
        public string DocName { get => _DocName; set => Set(ref _DocName, value); }

        private string _FolderName;
        public string FolderName {
            get
            {
                if(string.IsNullOrWhiteSpace(_FolderName))
                    _FolderName = Variables.GetWorkFolder();
                return _FolderName;
            }
            set => Set(ref _FolderName, value); }

        private ICommand _SelectFolderCommand;
        public ICommand SelectFolderCommand => _SelectFolderCommand
            ?? (_SelectFolderCommand = new RelayCommand(SelectMainFolder));


        private void SelectMainFolder()
        {

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = Variables.GetWorkFolder();
                var dialogResult =  dialog.ShowDialog();
                if(dialogResult == DialogResult.OK && dialog.SelectedPath != FolderName)
                {
                    //if(Variables.SetWorkFolder(dialog.SelectedPath))
                    //    FolderName = dialog.SelectedPath;
                    MessengerInstance.Send<PathMessage>(
                        new PathMessage
                        (
                            path: dialog.SelectedPath,
                            action: FolderMessageAction.SetAsWorkFolder
                        )
                    );
                }
            }

        }

        private void UpdateWorkFolder(object o, EventArgs e)
        {
            FolderName = Variables.GetWorkFolder();
        }


    }
}
