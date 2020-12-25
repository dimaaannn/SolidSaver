using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Controller
{
    public interface IPartControl<T> where T : ISwModel
    {
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
        /// <summary>
        /// Дочерние компоненты
        /// </summary>
        List<IPartControl<IAppComponent>> SubComponents { get; }
    }

    class PartControl<T> : IPartControl<T>
        where T : ISwModel
    {
        public PartControl(T part)
        {
            Appmodel = part;
        }
        public T Appmodel { get; set; }
        public object Selector { get; }
        public AppDocType PartType => Appmodel.DocType;
        public string Title => Appmodel.Title;
        List<IPartControl<IAppComponent>> _subComponents = null;
        public List<IPartControl<IAppComponent>> SubComponents
        {
            get
            {
                if (_subComponents != null)
                    return _subComponents;

                _subComponents = new List<IPartControl<IAppComponent>>();

                if(Appmodel is IAppComponent mainComponent)
                {
                    if (mainComponent.GetChildrenCount() <= 0)
                        return _subComponents;
                    else
                    {
                        foreach (var component in mainComponent.GetComponents(true))
                        {
                            _subComponents.Add(new PartControl<IAppComponent>(component));
                        }
                    }

                }
                return _subComponents;
            }
        }
    }
}
