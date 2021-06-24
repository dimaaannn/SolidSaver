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

            DocLoader = new DocLoader() { ModelAction = mw => StringData.Add(mw.DocTitle) };
        }

        public DocLoader DocLoader { get; }
        public ObservableCollection<string> StringData { get; protected set; }

        public string Name { get; set; } = "DebugWindow";

        public void LoadAllOpenedDocs()
        {
            StringData.Clear();
            cTS = new CancellationTokenSource();
            var ct = cTS.Token;
            var t = DocLoader.GetOpenedDocumentsAsync(ct);
            //var task = OpenedDocs.AddOpenedDocsAsync(AddObjToList, ct);
            
        }

        public void CancelTask()
        {
            cTS.Cancel();
        }

        private ICommand cancelTaskCommand;
        public ICommand CancelTaskCommand => cancelTaskCommand ?? (
            cancelTaskCommand = new RelayCommand(CancelTask, () => DocLoader.IsBusy));

        private ICommand loadDocsCommand;

        public ICommand LoadDocsCommand => loadDocsCommand ?? (
            loadDocsCommand = new RelayCommand(LoadAllOpenedDocs));

    }
}
