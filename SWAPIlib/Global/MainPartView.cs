using SWAPIlib.Controller;
using SWAPIlib.PropertyObj;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Global
{
    public interface IMainPartView : INotifyPropertyChanged
    {
        /// <summary>
        /// Загруженная модель
        /// </summary>
        IRootModel Rootmodel { get; }
        /// <summary>
        /// Компоненты модели
        /// </summary>
        SWAPIlib.Controller.ThreePartList PartList { get; }
        /// <summary>
        /// Выбранный компонент
        /// </summary>
        SWAPIlib.Controller.IComponentControl SelectedComp { get; set; }
        /// <summary>
        /// Свойства выбранного компонента
        /// </summary>
        List<ISwProperty> SelectedCompProp { get; }
        /// <summary>
        /// Список компонентов корневой сборки (тестовый)
        /// </summary>
        ObservableCollection<SWAPIlib.Controller.IComponentControl> RootComponents { get; }


    }

    public class MainPartView : IMainPartView
    {
        public MainPartView(IRootModel rootModel)
        {
            Rootmodel = rootModel;
            RootComponents = new ObservableCollection<IComponentControl>();

            //Добавить контроллеры компонентов
            foreach(var comp in Rootmodel.SubComponents)
            {
                var compControl = new SWAPIlib.Controller.ComponentControl(comp);
                RootComponents.Add(compControl);
            }

        }

        public IRootModel Rootmodel { get; private set; }

        public ThreePartList PartList { get; }

        SWAPIlib.Controller.IComponentControl _selectedComp = null;
        public SWAPIlib.Controller.IComponentControl SelectedComp
        {
            get => _selectedComp;
            set
            {
                _selectedComp = value;
                OnPropertyChanged("SelectedComp");
                OnPropertyChanged("SelectedCompProp");
            }
        }
        public List<ISwProperty> SelectedCompProp => SelectedComp.Appmodel.PropList;


        public ObservableCollection<IComponentControl> RootComponents { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
