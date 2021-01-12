using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using SWAPIlib.BaseTypes;
using System.ComponentModel;

namespace SWAPIlib.Controller
{


    //Template for AsmComponent

    public interface IPartControl1<out T> : INotifyPropertyChanged where T : IAppModel
    {
        bool IsSelected { get; set; }
        /// <summary>
        /// Модель детали
        /// </summary>
        T Appmodel { get; }
        /// <summary>
        /// Объект выделения
        /// </summary>
        IPartTyper Parttyper { get; }
        /// <summary>
        /// Тип детали
        /// </summary>
        AppDocType PartType { get; }
        /// <summary>
        /// Имя детали
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Активная группа выделения
        /// </summary>
        int SelectionGroup { get; set; }
    }


    public interface IComponentControl : IModelControl<IAppComponent>
    {
        AppModel PartModel { get; }
        int SubComponentCount { get; }
        /// <summary>
        /// Имя связанной конфигурации
        /// </summary>
        string RefConfig { get; }
        bool IsSuppressed { get; }
        /// <summary>
        /// Дочерние компоненты
        /// </summary>
        ObservableCollection<IComponentControl> SubComponents { get; }
    }

   

    /// <summary>
    /// Контроллер компонентов сборки
    /// </summary>
    public class ComponentControl : ModelControl<IAppComponent>, IComponentControl
    {
        public ComponentControl(IAppComponent component) : base(component) { }

        public virtual int SubComponentCount => PartType == AppDocType.swASM ?
            Appmodel.GetChildrenCount() : 0;
        ObservableCollection<IComponentControl> _subComponents = null;
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

        public AppModel PartModel => Appmodel.PartModel;
        public string RefConfig => Appmodel.RefConfigName;
        public bool IsSuppressed =>
            Appmodel?.SuppressionState != AppSuppressionState.Resolved ||
            Appmodel?.SuppressionState != AppSuppressionState.FullyResolved;
        public override string Title => TitleFilter.Replace(base.Title, "");

        public override string ToString()
        {
            return $"{Title}:{PartType}";
        }

        protected static Regex TitleFilter =
            new Regex(@"^.*[\/]", RegexOptions.Compiled);
    }

    public class ComponentAssembly : ComponentControl
    {
        public ComponentAssembly(IAppComponent component) : base(component) { }
        public new AppAssembly PartModel => Appmodel.PartModel as AppAssembly;
        public override int SubComponentCount => PartModel.ComponentCount(true);
        public string TestAssemblyProperty { get; set; } = "All fine";
    }
}
