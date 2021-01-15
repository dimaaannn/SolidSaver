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

namespace SWAPIlib.Controller
{


    //Template for AsmComponent

    public interface IComponentControl : 
        IModelControl<IAppComponent>,

        IEnumerable<IComponentControl>,
        IEnumerator<IComponentControl>
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
    public class ComponentControl : ModelControl<IAppComponent>, IComponentControl
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
                    if (SubComponentCount > 0)
                    {
                        foreach (var component in Appmodel.GetComponents(true))
                        {
                            _subComponents.Add(new ComponentControl(component));
                        }
                    }
                }
                return _subComponents;
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
            if (iterablePosition < SubComponentCount - 1)
            {
                iterablePosition++;
                _current = SubComponents[iterablePosition];
                subEnum = _current.GetEnumerator();
                return true;
            }
            else if (subEnum != null && subEnum.MoveNext())
            {
                _current = subEnum.Current;
                return true;
            }
            else
            {
                Reset();
                return false;
            }

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
