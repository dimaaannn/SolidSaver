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
            DocLoader = new DocLoader()                                     //Обработчик загруженных документов
            {
                ModelAction = modelw =>
                {
                    if (
                    modelw.DocType == SWAPIlib.AppDocType.swASM             //Если документ имеет тип "сборка"
                    && DocumentList.Contains(modelw) == false)              //И ещё не присутствует в списке
                    {
                        DocumentList.Add(modelw);                           //Добавить деталь в список доступных для загрузки
                    }
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
                DocumentList.Clear();                                           //Очистить список
                await DocLoader.GetActiveDoc(cTS.Token, DocLoader.ModelAction); //Добавить в список АКТИВНЫЙ документ
                _LoadDocumentCommand.RaiseCanExecuteChanged();                  //Обновить статус блокировки кнопки
                if (DocumentList.Count > 0)                                     //Если список не пустой
                    SelectedIndex = 0;                                          //Сделать активным первый пункт
                await DocLoader.GetOpenedDocumentsAsync(cTS.Token);             //Загрузить в фоне все остальные документы
            }
        }

        private async void LoadSelected()                                       //Загрузить выбранный элемент
        {
            if (DocLoader.IsBusy)
                CancelTask();
            isBusy = true;
            _LoadDocumentCommand.RaiseCanExecuteChanged();
            var userSelection = DocumentList[SelectedIndex];                                   //Получить выбранный пользователем элемент
            await Task.Run(() => Variables.SetMainModel(userSelection.ConvertToOldWrapper())); //Установить модель в качестве главной
            isBusy = false;
            _LoadDocumentCommand.RaiseCanExecuteChanged();
        }

        private void CancelTask()                                                               //отменить фоновую загрузку списка
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
        private bool UpdateListCommandCanExecute() => 
            SwAppControl.ComConnected
            && DocLoader.IsBusy == false;
        #endregion

        #region LoadDocumentCommand
        /// <summary>
        /// Загрузить выбранный документ
        /// </summary>
        public ICommand LoadDocumentCommand => _LoadDocumentCommand ?? (_LoadDocumentCommand = new RelayCommand(LoadSelected, LoadDocumentCommandCanExecute));
        private RelayCommand _LoadDocumentCommand;
        private bool isBusy;

        private bool LoadDocumentCommandCanExecute()
        {
            return SwAppControl.ComConnected
                && SelectedIndex >= 0
                && SelectedIndex < DocumentList.Count
                && isBusy == false;
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
        private RelayCommand _LoadDocumentCommand;
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
