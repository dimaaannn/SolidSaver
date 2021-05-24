using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn;
using SWAPIlib.Controller;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Global
{
    public static class MainModel
    {
        static MainModel()
        {
            _MainModelsArchive = new List<ILinkedModel>();
            _MainModelControl = new MainPartControl();
        }
        private static ILinkedModel _CurrentMainmodel;
        private static IMainPartControl _MainModelControl;
        public static IMainPartControl MainModelControl => _MainModelControl;

        /// <summary>
        /// Текущая базовая модель
        /// </summary>
        /// <returns></returns>
        public static ILinkedModel GetMainModel() => _CurrentMainmodel; 


        /// <summary>
        /// Задать базовую модель
        /// </summary>
        /// <param name="linkedModel"></param>
        /// <returns></returns>
        public static bool SetMainModel(ILinkedModel linkedModel)
        { 
            var swmodel = linkedModel?.AppModel.SwModel;
            bool ret = false;
            if(swmodel != null)
            {
                if (_CurrentMainmodel != null)
                    MainModelsArchive.Add(_CurrentMainmodel);
                _CurrentMainmodel = linkedModel;
                SwAppControl.MainModel = swmodel;
                _MainModelControl.LinkedRootModel = _CurrentMainmodel;
                MainModelChanged?.Invoke(linkedModel, EventArgs.Empty);
                ret = true;
            }
            return ret;
        }

        public static bool SetMainModel(ISwModelWrapper swModelWrapper) =>
            SetMainModel(new LinkedModel(swModelWrapper.GetAppModel()));

        public static ObservableCollection<IComponentControl> SelectionList => _CurrentMainmodel?.SelectedComponents;

        public static event EventHandler MainModelChanged;

        private static List<ILinkedModel> _MainModelsArchive;

        public static List<ILinkedModel> MainModelsArchive => _MainModelsArchive;
    }
}
