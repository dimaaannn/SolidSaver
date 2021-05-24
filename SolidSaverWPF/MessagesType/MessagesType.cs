using SWAPIlib.BaseTypes;
using SWAPIlib.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSaverWPF.MessagesType
{
    /// <summary>
    /// Для загрузки списков свойств
    /// </summary>
    public class PropertyMessage
    {
        public IEnumerable<IPropertySet> PropertySet;
        public PropAction Action;
        public enum PropAction
        {
            None,
            Show,
            Get
        }
    }

    /// <summary>
    /// Сообщение с моделью
    /// </summary>
    public class ModelMessage
    {
        public ModelMessage(ISwModelWrapper modelWrapper, ModelMessageAction action = ModelMessageAction.None)
        {
            SwModel = modelWrapper;
            Action = action;
        }
        private ModelMessageAction _Action;
        public ModelMessageAction Action
        {
            get { return _Action; }
            set { _Action = value; }
        }

        private ISwModelWrapper _SwModel;
        /// <summary>
        /// Модель
        /// </summary>
        public ISwModelWrapper SwModel
        {
            get { return _SwModel; }
            set { _SwModel = value; }
        }

    }
    [Flags]
    public enum ModelMessageAction
    {
        None = 0,
        SetAsMainModel = 1 << 0
    }

    public class SelectionMessage<T>
    {
        public SelectionMessage(object sender, T selection)
        {
            Sender = sender;
            Selection = selection;
        }
        public object Sender { get; set; }
        public T Selection { get; set; }
    }

    /// <summary>
    /// Передача URI в сообщении
    /// </summary>
    public class PathMessage
    {
        public PathMessage(string path, FolderMessageAction action = FolderMessageAction.None)
        {
            Path = path;
            FolderAction = action;
        }

        private string _Path;
        /// <summary>
        /// Ссылка на папку или файл
        /// </summary>
        public string Path
        {
            get { return _Path; }
            set { _Path = value; }
        }

        /// <summary>
        /// Тип события
        /// </summary>
        public FolderMessageAction FolderAction { get; set; }

    }

    /// <summary>
    /// События для пути
    /// </summary>
    [Flags]
    public enum FolderMessageAction
    {
        None = 0,
        SetAsWorkFolder = 1 << 0
    }
}
