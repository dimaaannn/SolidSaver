using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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
            MessengerInstance.Register<ModelMessage>(this, SetDocName);
        }

        private string _DocName;
        public string DocName { get => _DocName; set => Set(ref _DocName, value); }

        private string _FolderName;
        public string FolderName { get => _FolderName; set => Set(ref _FolderName, value); }

        private ICommand _SelectFolderCommand;
        public ICommand SelectFolderCommand => _SelectFolderCommand
            ?? (_SelectFolderCommand = new RelayCommand(SelectMainFolder));

        private void SetDocName(ModelMessage wrapper)
        {
            DocName = wrapper.SwModel.Title;
        }

        private void SelectMainFolder()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var dialogResult =  dialog.ShowDialog();
                FolderName = dialogResult.ToString();
            }

        }
    }
}
