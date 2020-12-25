using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Controller
{
    public interface IPartControl<T> where T : ISwModel
    {
        bool IsSelected { get; set; }
        /// <summary>
        /// Модель детали
        /// </summary>
        T Appmodel { get; set; }
        /// <summary>
        /// Объект выделения
        /// </summary>
        object Selector { get; }
        /// <summary>
        /// Тип детали
        /// </summary>
        AppDocType PartType { get; }
        /// <summary>
        /// Имя детали
        /// </summary>
        string Title { get; }
    }

    public interface IComponentControl : IPartControl<IAppComponent>
    {
        /// <summary>
        /// Дочерние компоненты
        /// </summary>
        List<IComponentControl> SubComponents { get; }
        /// <summary>
        /// Имя связанной конфигурации
        /// </summary>
        string RefConfig { get; }
    }

    public class PartControl<T> : IPartControl<T>
        where T : ISwModel
    {
        /// <summary>
        /// PartControl constructor
        /// </summary>
        /// <param name="part"></param>
        public PartControl(T part)
        {
            Appmodel = part;
        }
        public bool IsSelected { get; set; } = false;
        public T Appmodel { get; set; }
        public object Selector { get; }
        public AppDocType PartType => Appmodel.DocType;
        public string Title => Appmodel.Title;
        public override string ToString()
        {
            return $"{Appmodel.FileName}:{PartType}";
        }
        
    }

    public class ComponentControl : PartControl<IAppComponent>, IComponentControl
    {
        public ComponentControl(IAppComponent component) : base(component) { }
        List<IComponentControl> _subComponents = null;
        public List<IComponentControl> SubComponents
        {
            get
            {
                if (_subComponents != null)
                    return _subComponents;

                _subComponents = new List<IComponentControl>();

                if (Appmodel is IAppComponent mainComponent)
                {
                    if (mainComponent.GetChildrenCount() <= 0)
                        return _subComponents;
                    else
                    {
                        foreach (var component in mainComponent.GetComponents(true))
                        {
                            _subComponents.Add(new ComponentControl(component));
                        }
                    }

                }
                return _subComponents;
            }
        }
        public string RefConfig => Appmodel.RefConfigName;

        public override string ToString()
        {
            return $"{Title}:{PartType}";
        }
    }

}
