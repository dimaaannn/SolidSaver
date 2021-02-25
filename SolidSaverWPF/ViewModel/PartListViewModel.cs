using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SWAPIlib.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel
{
    public class PartListViewModel : ViewModelBase
    {
        public PartListViewModel()
        {
            rootModel = new LinkedModel();
        }

        

        //private ICommand _LoadActivePartCommand;
        //public ICommand LoadActivePartCommand => _LoadActivePartCommand ?? _LoadActivePartCommand = new RelayCommand()


        /// <summary>
        /// Имя активной детали
        /// </summary>
        private string _PartName;
        public string PartName
        {
            get { return _PartName; }
            set { Set(ref _PartName, value); }
        }

        public void LoadActiveDocument()
        {
            rootModel.LoadModel();
            rootModel.SetCurrentModelAsMain();
            MainPart = new MainPartControl(rootModel);
        }

        /// <summary>
        /// Основная модель
        /// </summary>
        private IMainPartControl _MainPart;
        public IMainPartControl MainPart { get => _MainPart; set => Set(ref _MainPart, value); }

        private ILinkedModel rootModel;
    }
}
