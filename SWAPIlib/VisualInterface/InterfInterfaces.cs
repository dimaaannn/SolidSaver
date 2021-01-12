using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using SWAPIlib.BaseTypes;

namespace SWAPIlib.VisualInterface
{
    public interface IMainModel : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Коренная модель
        /// </summary>
        AppModel RootModel { get; set; }
        /// <summary>
        /// Тип документа модели
        /// </summary>
        AppDocType DocType { get; }
        /// <summary>
        /// Имя документа
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Путь к файлу
        /// </summary>
        string Path { get; }
        /// <summary>
        /// Имя активной конфигурации
        /// </summary>
        string ActiveConfigName { get; set; }

        /// <summary>
        /// Активный список свойств
        /// </summary>
        IList<ISwProperty> ActivePropList { get; }
        /// <summary>
        /// Класс обработки имени проекта
        /// </summary>
        IFileModelProp ProjectNameProp { get; }
        /// <summary>
        /// Дочерние компоненты сборки
        /// </summary>
        List<IAppComponent> SubComponents { get; }

        /// <summary>
        /// Загрузить активную модель
        /// </summary>
        /// <param name="pathToModel"></param>
        /// <returns></returns>
        bool GetMainModel(string pathToModel = null);
        /// <summary>
        /// Загружать компоненты только верхнего уровня
        /// </summary>
        bool TopLevelOnly { get; set; }
        /// <summary>
        /// Загрузить список дочерних компонентов
        /// </summary>
        /// <returns></returns>
        bool GetSubComponents();
        bool LoadActiveModel();


        event EventHandler<SwEventArgs> CloseRootModel;

    }

 


}
