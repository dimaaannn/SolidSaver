using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolidSaverWPF.Data;
using SolidSaverWPF.MessagesType;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn;
using SWAPIlib.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel
{
    public class DocLoaderViewModel2 : ViewModelBase
    {

        public DocLoaderViewModel2()
        {
            DocumentList = new ObservableCollection<IModelWrapper>();
            DocLoader = new DocLoader()
            {
                ModelAction = modelw =>
                {
                    if (modelw.DocType == SWAPIlib.AppDocType.swASM)
                    DocumentList.Add(modelw);
                }
            };
        }
        private int _SelectedIndex;
        private CancellationTokenSource cTS;
        
        public DocLoader DocLoader { get; }

        public ObservableCollection<IModelWrapper> DocumentList { get; }

        public int SelectedIndex { get => _SelectedIndex; 
            set => Set(ref _SelectedIndex, value); }

        private async void UpdateDocumentList()
        {
            using (cTS = new CancellationTokenSource())
            {
                DocumentList.Clear();
                await DocLoader.GetOpenedDocumentsAsync(cTS.Token);
            }
        }

        private void LoadSelected()
        {
            if (DocLoader.IsBusy)
                cTS.Cancel();

            var userSelection = DocumentList[SelectedIndex];
            Variables.SetMainModel(userSelection.ConvertToOldWrapper());
        }

        private void CancelTask()
        {
            cTS.Cancel();
        }


        private ICommand cancelTaskCommand;
        public ICommand CancelTaskCommand => cancelTaskCommand ?? (
            cancelTaskCommand = new RelayCommand(CancelTask, CancelTaskCommancCanExecute));
        private bool CancelTaskCommancCanExecute() =>
            DocLoader.IsBusy;

        #region UpdateCommand
        /// <summary>
        /// Загрузить список видимых документов
        /// </summary>
        public ICommand UpdateListCommand => _UpdateListCommand ?? (_UpdateListCommand = new RelayCommand(UpdateDocumentList, UpdateListCommandCanExecute));
        private ICommand _UpdateListCommand;
        private bool UpdateListCommandCanExecute() => SwAppControl.ComConnected;
        #endregion

        #region LoadDocumentCommand
        /// <summary>
        /// Загрузить выбранный документ
        /// </summary>
        public ICommand LoadDocumentCommand => _LoadDocumentCommand ?? (_LoadDocumentCommand = new RelayCommand(LoadSelected, LoadDocumentCommandCanExecute));
        private ICommand _LoadDocumentCommand;
        private bool LoadDocumentCommandCanExecute()
        {
            return SwAppControl.ComConnected
                && SelectedIndex >= 0
                && SelectedIndex < DocumentList.Count;
        }
        #endregion




    }

    public class DocLoaderViewModel : ViewModelBase
    {

        private List<ISwModelWrapper> _OpenedModels;
        /// <summary>
        /// Список открытых моделей
        /// </summary>
        public List<ISwModelWrapper> OpenedModels {
            get
            {
                if (_OpenedModels is null)
                    UpdateList();
                return _OpenedModels;
            }
            private set => Set(ref _OpenedModels, value); }

        private int _SelectedIndex;
        public int SelectedIndex { get => _SelectedIndex;
            set => Set(ref _SelectedIndex, value);
        }


        #region UpdateCommand
        /// <summary>
        /// Загрузить список видимых документов
        /// </summary>
        public ICommand UpdateListCommand => _UpdateListCommand ?? (_UpdateListCommand = new RelayCommand(UpdateList, UpdateListCommandCanExecute));
        private ICommand _UpdateListCommand;
        private bool UpdateListCommandCanExecute() => SwAppControl.ComConnected;
        #endregion

        #region LoadDocumentCommand
        /// <summary>
        /// Загрузить выбранный документ
        /// </summary>
        public ICommand LoadDocumentCommand => _LoadDocumentCommand ?? (_LoadDocumentCommand = new RelayCommand(LoadSelected, LoadDocumentCommandCanExecute));
        private ICommand _LoadDocumentCommand;
        private bool LoadDocumentCommandCanExecute()
        {
            return SwAppControl.ComConnected
                && SelectedIndex >= 0
                && SelectedIndex < OpenedModels.Count;
        }
        #endregion


        public void UpdateList()
        {
            if (SwAppControl.ComConnected)
            {
                OpenedModels = OpenedDocs.GetVisibleAssembly();
                SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Задать главную модель
        /// </summary>
        public void LoadSelected()
        {
            Variables.SetMainModel(OpenedModels[SelectedIndex]);
            //MessengerInstance.Send(new ModelMessage(OpenedModels[SelectedIndex], ModelMessageAction.SetAsMainModel));
        }
    }
}
