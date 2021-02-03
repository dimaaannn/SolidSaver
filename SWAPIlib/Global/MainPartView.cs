using SWAPIlib.Controller;
using SWAPIlib.MProperty;
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
        /// Выбранный компонент
        /// </summary>
        SWAPIlib.Controller.IComponentControl SelectedComp { get; set; }
        /// <summary>
        /// Свойства выбранного компонента
        /// </summary>
        List<IPropertyModel> SelectedCompProp { get; }
        /// <summary>
        /// Список компонентов корневой сборки (тестовый)
        /// </summary>
        ObservableCollection<SWAPIlib.Controller.IComponentControl> RootComponents { get; }
        int ActiveSelectionGroup { get; set; }
        void ReloadCompList();

    }

    public class MainPartView : IMainPartView
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="rootModel"></param>
        public MainPartView(IRootModel rootModel)
        {
            Rootmodel = rootModel;
            RootComponents = new ObservableCollection<IComponentControl>();

            //Добавить контроллеры компонентов
            ReloadCompList();

        }
        /// <summary>
        /// Ссылка на коренную модель
        /// </summary>
        public IRootModel Rootmodel { get; private set; }
        /// <summary>
        /// Выделенный компонент
        /// </summary>
        SWAPIlib.Controller.IComponentControl _selectedComp = null;
        /// <summary>
        /// SelectedComp property
        /// </summary>
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
        /// <summary>
        /// Свойства выделенного компонента
        /// </summary>
        public List<IPropertyModel> SelectedCompProp => SelectedComp?.Appmodel.PropList;
        /// <summary>
        /// Корневые компоненты
        /// </summary>
        public ObservableCollection<IComponentControl> RootComponents { get; private set; }
        public int ActiveSelectionGroup { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public void ReloadCompList()
        {
            if (RootComponents.Count > 0)
                RootComponents.Clear();

            var filter = from comp in Rootmodel.SubComponents
                         orderby comp descending
                         select comp;

            foreach (var comp in filter)
            {
                var compControl = new SWAPIlib.Controller.ComponentControl(comp);
                RootComponents.Add(compControl);
            }
            

        }
    }

}
