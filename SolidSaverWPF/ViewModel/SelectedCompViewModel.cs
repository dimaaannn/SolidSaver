using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolidSaverWPF.Data;
using SolidSaverWPF.MessagesType;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn;
using SWAPIlib.Controller;
using SWAPIlib.Global;
using SWAPIlib.MProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel
{
    /// <summary>
    /// Отображает свойства выбранного пользователем компонента
    /// </summary>
    public class SelectedCompViewModel : ViewModelBase
    {
        public SelectedCompViewModel()
        {
            MainModel.MainModelChanged += ClearData;
            MessengerInstance.Register<SelectionMessage<object>>(this, ComponentSelected);
        }


        private IComponentControl componentControl;
        /// <summary>
        /// Выбранный компонент
        /// </summary>
        public IComponentControl SelectedComponent { get => componentControl; set => Set(ref componentControl, value); }

        /// <summary>
        /// Список свойств
        /// </summary>
        public List<IPropertyModel> PropertyModel => SelectedComponent.PartModel.PropList;

        /// <summary>
        /// Компонент выбран пользователем
        /// </summary>
        /// <param name="obj"></param>
        private void ComponentSelected(SelectionMessage<object> obj)
        {
            switch (obj.Selection)
            {
                case IComponentControl comp:
                    SelectedComponent = comp;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Очистить список
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearData(object sender, EventArgs e)
        {
            SelectedComponent = null;
        }

    }
}
