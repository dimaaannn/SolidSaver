using SWAPIlib.Controller;
using SWAPIlib.MProperty;
using SWAPIlib.PropertyObj;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Global
{
    public interface IMainPartViewControl : INotifyPropertyChanged, IEnumerable<IComponentControl>
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

    public class MainPartViewControl : IMainPartViewControl
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="rootModel"></param>
        public MainPartViewControl(IRootModel rootModel)
        {
            Rootmodel = rootModel;
            //RootComponents = new ObservableCollection<IComponentControl>();
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
        public ObservableCollection<IComponentControl> RootComponents => RootAssemblyTree.SubComponents; // { get; private set; }

        public int ActiveSelectionGroup { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public void ReloadCompList()
        {
            RootAssemblyTree = new AssemblyTree(this.Rootmodel.appModel);
        }

        public IEnumerator<IComponentControl> GetEnumerator() => RootAssemblyTree;

        IEnumerator IEnumerable.GetEnumerator() => RootAssemblyTree;

        /// <summary>
        /// Иерархическое представление деталей сборки
        /// </summary>
        AssemblyTree RootAssemblyTree;
    }

    /// <summary>
    /// Ienumerator для компонентов
    /// </summary>
    public class AssemblyTree : IEnumerator<IComponentControl>, IEnumerable<IComponentControl>
    {

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="appModel"></param>
        public AssemblyTree(IAppModel appModel, Func<IComponentControl, bool> compFilter = null)
        {
            SubComponents = new ObservableCollection<IComponentControl>();
            SubComponentFilter = compFilter;
            if (appModel is IAppAssembly appAsm)
            {
                foreach (var comp in appAsm.GetComponents(true))
                {
                    var compControl = new SWAPIlib.Controller.ComponentControl(comp);
                    SubComponents.Add(compControl);
                }
            }
        }

        private IAppModel NodeModel;
        public ObservableCollection<IComponentControl> SubComponents { get; set; }

        /// <summary>
        /// Фильтр загрузки компонентов
        /// </summary>
        public Func<IComponentControl, bool> SubComponentFilter;

        #region IEnumerator
        private int _CurrentNum = 0;
        private bool _IsTopLevelObjReturned = false;

        public IComponentControl Current { get; private set; }
        object IEnumerator.Current => Current;

        public virtual bool MoveNext()
        {
            //if (_CurrentNum < 0)
            //{
            //    _CurrentNum++;
            //    if (NodeModel is IComponentControl nodeComp
            //        && (SubComponentFilter?.Invoke(nodeComp) ?? true))
            //    {

            //        Current = nodeComp;
            //        return true;
            //    }
            //}
            while (_CurrentNum < SubComponents.Count)
            {
                if(!_IsTopLevelObjReturned)
                {
                    _IsTopLevelObjReturned = true;
                    Current = SubComponents[_CurrentNum];
                    return true;
                }
                if (SubComponents[_CurrentNum].MoveNext())
                {
                    Current = SubComponents[_CurrentNum].Current;

                    if (SubComponentFilter?.Invoke(Current) ?? true)
                        return true;
                    else
                        continue;
                }
                else
                {
                    _IsTopLevelObjReturned = false;
                    _CurrentNum++;
                }
            }
            return false;
        }

        public void Reset() { _CurrentNum = 0; Current = null; }
        public void Dispose() => Reset();

        public IEnumerator<IComponentControl> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
        #endregion

        /// <summary>
        /// Компонент под номером
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IComponentControl this[int index] => SubComponents[index];

    }

}
