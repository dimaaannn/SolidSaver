using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Controller
{


    //Template for AsmComponent


    public interface IPartControl<out T> where T : ISwModel
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
    }

    public interface IComponentControl : IPartControl<IAppComponent>
    {
        int SubComponentCount { get; }
        /// <summary>
        /// Дочерние компоненты
        /// </summary>
        List<IComponentControl> SubComponents { get; }
        /// <summary>
        /// Имя связанной конфигурации
        /// </summary>
        string RefConfig { get; }
    }

    /// <summary>
    /// Контроллер моделей
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PartControl<T> : IPartControl<T> where T : ISwModel
    {
        /// <summary>
        /// PartControl constructor
        /// </summary>
        /// <param name="part"></param>
        public PartControl(T part)
        {
            Appmodel = part;

            if (part.SwModel != null)
                Parttyper = new PartTyper(part);
        }

        public PartControl() { }
        public bool IsSelected { get; set; } = false;
        public virtual T Appmodel { get; set; }
        public IPartTyper Parttyper { get; protected set; }
        public AppDocType PartType => Appmodel.DocType;
        public string Title => Appmodel.Title;
        public override string ToString()
        {
            return $"{Appmodel.FileName}:{PartType}";
        }

    }

    /// <summary>
    /// Контроллер компонентов сборки
    /// </summary>
    public class ComponentControl : PartControl<IAppComponent>, IComponentControl
    {
        public ComponentControl(IAppComponent component) : base(component) { }
        List<IComponentControl> _subComponents = null;
        public List<IComponentControl> SubComponents
        {
            get
            {
                if (_subComponents == null)
                {
                    _subComponents = new List<IComponentControl>();
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
        public string RefConfig => Appmodel.RefConfigName;

        public int SubComponentCount => PartType == AppDocType.swASM ?
            Appmodel.GetChildrenCount() : 0;

        public override string ToString()
        {
            return $"{Title}:{PartType}";
        }
    }
}
