using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using SWAPIlib.BaseTypes;
using System.ComponentModel;
using System.Collections;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib.ComConn;

namespace SWAPIlib.Controller
{


    //Template for AsmComponent

    public interface IComponentControl : 
        IModelControl<IAppComponent, IComponentSelector>,

        IEnumerable<IComponentControl>,
        IEnumerator<IComponentControl>
        , IComparable<IComponentControl>
    {
        /// <summary>
        /// Объект модели сборки
        /// </summary>
        AppModel PartModel { get; }
        /// <summary>
        /// Количество дочерних компонентов
        /// </summary>
        int SubComponentCount { get; }
        /// <summary>
        /// Погашен
        /// </summary>
        bool IsSuppressed { get; }
        /// <summary>
        /// Дочерние компоненты
        /// </summary>
        ObservableCollection<IComponentControl> SubComponents { get; }
        /// <summary>
        /// Имя связанной конфигурации
        /// </summary>
        string RefConfig { get; }
        /// <summary>
        /// Уровень сборки относительно Root
        /// </summary>
        int AssemblyLevel { get; }
        
    }

   
    /// <summary>
    /// Контроллер компонентов сборки
    /// </summary>
    public class ComponentControl : 
        ModelControl<IAppComponent, IComponentSelector>, IComponentControl
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="component"></param>
        public ComponentControl(IAppComponent component) : base(component) { }

        /// <summary>
        /// Модель детали
        /// </summary>
        public AppModel PartModel => Appmodel.PartModel;

        /// <summary>
        /// Количество дочерних компонентов
        /// </summary>
        public virtual int SubComponentCount => PartType == AppDocType.swASM ?
            Appmodel.GetChildrenCount() : 0;

        /// <summary>
        /// Внутренний список дочерних компонентов
        /// </summary>
        ObservableCollection<IComponentControl> _subComponents = null;

        /// <summary>
        /// Список дочерних компонентов
        /// </summary>
        public ObservableCollection<IComponentControl> SubComponents
        {
            get
            {
                if (_subComponents == null)
                {
                    _subComponents = new ObservableCollection<IComponentControl>();
                    LoadSubComponents();

                    if (_allSubComponents.Count > 0)
                        FilterSubComponents();
                }
                return _subComponents;
            }
        }

        private List<IComponentControl> _allSubComponents;
        /// <summary>
        /// Загрузить все компоненты в список
        /// </summary>
        private void LoadSubComponents()
        {
            if (_allSubComponents == null)
                _allSubComponents = new List<IComponentControl>();

            if (SubComponentCount > 0)
            {
                _allSubComponents.Clear();
                foreach (var component in Appmodel.GetComponents(true))
                {
                    _allSubComponents.Add(new ComponentControl(component));
                }
            }
            
        }

        /// <summary>
        /// Фильтр TODO
        /// </summary>
        private void FilterSubComponents()
        {
            var filter = from comp in _allSubComponents
                         orderby comp descending
                         select comp;
            _subComponents.Clear();
            foreach(var component in filter)
            {
                _subComponents.Add(component);
            }
        }

        public override IComponentSelector Modelselector
        {
            get
            {
                if (_modelSelector == null)
                    _modelSelector = new ComponentSelector(Appmodel);
                return (IComponentSelector) _modelSelector;
            }
        }

        /// <summary>
        /// Сокращённое имя компонента
        /// </summary>
        public override string Title => TitleFilter.Replace(base.Title, "");

        /// <summary>
        /// Уровень сборки относительно Root
        /// </summary>
        public int AssemblyLevel => LevelCounter.Matches(base.Title).Count;

        /// <summary>
        /// Имя зависимой конфигурации
        /// </summary>
        public string RefConfig => Appmodel.RefConfigName;

        public override bool IsSelected {
            get => Modelselector.IsSelected;
            set
            {
                Modelselector.IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Компонент погашен
        /// </summary>
        public bool IsSuppressed =>
            Appmodel?.SuppressionState != AppSuppressionState.Resolved ||
            Appmodel?.SuppressionState != AppSuppressionState.FullyResolved;

        /// <summary>
        /// Фильтр наименования
        /// </summary>
        protected static Regex TitleFilter =
            new Regex(@"^.*[\/]", RegexOptions.Compiled);
        /// <summary>
        /// Уровень сборки
        /// </summary>
        protected static Regex LevelCounter =
            new Regex(@"\/", RegexOptions.Compiled);
        /// <summary>
        /// Имя отображения
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{PartType}:{base.Title}";
        }

        public int CompareTo(IComponentControl other)
        {
            return this.PartType.CompareTo(other.PartType);
        }

        #region Enumerator
        private IComponentControl _current;
        object IEnumerator.Current => _current;
        public IComponentControl Current => _current;

        public IEnumerator<IComponentControl> GetEnumerator()
        {
            return this;
        }
        public bool MoveNext()
        {
            var ret = false;
            if (iterablePosition < SubComponents.Count && SubComponents.Count > 0)
            {
                iterablePosition++;
                _current = SubComponents[iterablePosition];
                subEnum = _current.GetEnumerator();
                ret = true;
            }
            else if (subEnum != null && subEnum.MoveNext())
            {
                _current = subEnum.Current;
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
            subEnum = null;
            _current = null;
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }


        /// <summary>
        /// Текущая позиция итерирования
        /// </summary>
        private int iterablePosition = -1;
        /// <summary>
        /// enumerator дочернего компонента
        /// </summary>
        private IEnumerator<IComponentControl> subEnum;
        #endregion
    }


}
