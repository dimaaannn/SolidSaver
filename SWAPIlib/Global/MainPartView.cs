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
    public interface IMainPartView : INotifyPropertyChanged, IEnumerable<IComponentControl>
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
            assemblyIterator = new AsmIterator(this.RootComponents);
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

        public IEnumerator<IComponentControl> GetEnumerator() => assemblyIterator;

        IEnumerator IEnumerable.GetEnumerator() => assemblyIterator;
        /// <summary>
        /// Итерация сквозь сборку
        /// </summary>
        AsmIterator assemblyIterator;
    }


    public class AssemblyTree : IEnumerator<IComponentControl>
    {
        public IAppModel RootModel { get; private set; }
        public ObservableCollection<IComponentControl> SubComponents { get; set; }

        #region IEnumerator
        private int _CurrentNum = 0;
        private IComponentControl _CurrentTopLevelObj;

        public IComponentControl Current { get; private set; }
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            while (_CurrentNum < SubComponents.Count)
            {
                if (SubComponents[_CurrentNum].MoveNext())
                {
                    Current = SubComponents[_CurrentNum].Current;
                    return true;
                }
                else _CurrentNum++;
            }
            return false;
        }

        public void Reset() { _CurrentNum = 0; Current = null; }
        public void Dispose() => Reset(); 
        #endregion

        /// <summary>
        /// Компонент под номером
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IComponentControl this[int index] => SubComponents[index];

    }

    /// <summary>
    /// Костыль для правильной последовательности внутренних моделей в главной сборке
    /// </summary>
    class AsmIterator :
        IEnumerable<IComponentControl>,
        IEnumerator<IComponentControl>
    {
        public AsmIterator(ObservableCollection<IComponentControl> rootParts)
        {
            this.rootParts = rootParts;
        }
        private ObservableCollection<IComponentControl> rootParts;

        public bool MoveNext()
        {
            bool ret = false;

            if (CurrentIsSended && iterablePosition < rootParts.Count)
            {
                if (rootParts[iterablePosition].MoveNext())
                {
                    _current = rootParts[iterablePosition].Current;
                    ret = true;
                }
                else
                {
                    CurrentIsSended = false;
                    ret = true;
                }
            }
            else if (!CurrentIsSended && ++iterablePosition < rootParts.Count)
            {
                _current = rootParts[iterablePosition];
                CurrentIsSended = true;
                ret = true;
            }
            else
            {
                Reset();
                ret = false;
            }

            return ret;
        }

        public void Reset()
        {
            iterablePosition = -1;
            _current = null;
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public IEnumerator<IComponentControl> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        /// <summary>
        /// Текущая позиция итерирования
        /// </summary>
        private int iterablePosition = -1;
        bool CurrentIsSended = false;

        private IComponentControl _current;
        object IEnumerator.Current => _current;
        public IComponentControl Current => _current;
    }

}
