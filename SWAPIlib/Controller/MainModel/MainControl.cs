using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Controller.MainModel
{
    public interface IRootModel
    {
        /// <summary>
        /// Коренная модель
        /// </summary>
        ISwModel RootModel { get; set; }
        /// <summary>
        /// Тип документа модели
        /// </summary>
        AppDocType DocType { get; }
        /// <summary>
        /// Имя документа
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Имя активной конфигурации
        /// </summary>
        string ActiveConfigName { get; set; }

    }
}
