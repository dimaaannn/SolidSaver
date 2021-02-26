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
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel
{
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
            OpenedModels = OpenedDocs.GetVisibleDocs();
            SelectedIndex = 0;
        }

        /// <summary>
        /// Сообщение об установке в качестве главной модели
        /// </summary>
        public void LoadSelected()
        {
            MessengerInstance.Send(new ModelMessage(OpenedModels[SelectedIndex], ModelMessageAction.SetAsMainModel));
        }
    }
}
