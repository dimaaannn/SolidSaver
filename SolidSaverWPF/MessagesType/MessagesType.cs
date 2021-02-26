using SWAPIlib.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSaverWPF.MessagesType
{
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

    public enum ModelMessageAction
    {
        None,
        SetAsMainModel
    }
}
