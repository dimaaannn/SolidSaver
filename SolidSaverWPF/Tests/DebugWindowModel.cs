using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SWAPIlib.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SolidSaverWPF.Tests
{
    public class DebugWindowModel : ViewModelBase
    {
        CancellationTokenSource cTS = new CancellationTokenSource();

        public DebugWindowModel()
        {
            StringData = new ObservableCollection<string>();
            if (IsInDesignMode)
            {
                Name = "DebugWindowDesign";

                StringData.Add("test string 1");
                StringData.Add("test string 2");
                StringData.Add("test string 3");
                StringData.Add("test string 4");
            }
        }

        public ObservableCollection<string> StringData { get; protected set; }

        public string Name { get; set; } = "DebugWindow";
        private bool isBusy;
        public bool IsBusy { get => isBusy; protected set => Set(ref isBusy, value); }

        public void LoadAllOpenedDocs()
        {
            StringData.Clear();
            cTS = new CancellationTokenSource();
            var ct = cTS.Token;
            ct.Register(() => IsBusy = false);
            IsBusy = true;
            var task = OpenedDocs.AddOpenedDocsAsync(AddObjToList, ct);
            
            task.ContinueWith(t => IsBusy = false);
        }

        private void AddObjToList(object model)
        {
            StringData.Add(model as string);
        }
        public void CancelTask()
        {
            cTS.Cancel();
        }

        private ICommand cancelTaskCommand;
        public ICommand CancelTaskCommand => cancelTaskCommand ?? (
            cancelTaskCommand = new RelayCommand(CancelTask, () => IsBusy));

        private ICommand loadDocsCommand;

        public ICommand LoadDocsCommand => loadDocsCommand ?? (
            loadDocsCommand = new RelayCommand(LoadAllOpenedDocs));

    }
}
