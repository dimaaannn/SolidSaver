using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace SWAPIlib.Controller
{


    //Template for AsmComponent
    

    public interface IComponentControl : IPartControl<IAppComponent>
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
    public class ComponentControl : PartControl<IAppComponent>, IComponentControl
    {
        public ComponentControl(IAppComponent component) : base(component) { }

        public int SubComponentCount => PartType == AppDocType.swASM ?
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

        public override string ToString()
        {
            return $"{Title}:{PartType}";
        }
    }
}
