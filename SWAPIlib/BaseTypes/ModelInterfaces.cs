using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib.MProperty;

namespace SWAPIlib
{
    public interface IAppModel : IComparable
    {

        //TODO refactor interface
        bool IsExist { get; }
        ModelDoc2 SwModel { get; }
        /// <summary>
        /// Тип документа
        /// </summary>
        AppDocType  DocType { get; }
        /// <summary>
        /// Имя файла модели
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// Путь к файлу
        /// </summary>
        string Path { get; }
        /// <summary>
        /// Наименование модели
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Close file event
        /// </summary>
        event EventHandler<SwEventArgs> FileIsClosed;
        /// <summary>
        /// Набор глобальных свойств
        /// </summary>
        IFileModelProp GlobalModelProp { get; } //TODO remove
        /// <summary>
        /// Список свойств модели
        /// </summary>
        List<ISwProperty> PropList { get;}  //TODO create propList class
        bool VisibState { get; set; }
        /// <summary>
        /// Список конфигураций
        /// </summary>
        List<string> ConfigList { get; }
        /// <summary>
        /// Имя активной конфигурации
        /// </summary>
        string ActiveConfigName { get; set; }
        /// <summary>
        /// Задать активную конфигурацию
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        bool SetActiveConfig(string configName);
        /// <summary>
        /// Список параметров модели (Пользовательские свойства)
        /// </summary>
        string[] ParameterList { get; }
        string this [string paramName] { get; set; }
        string this [string configName, string paramName] { get; set; }
        bool SetParameterVal(string configName, string paramName, string newValue);

        IModelEntity ModelEntity { get; }
    }

    //TODO сделать интерфейс сборки
    /// <summary>
    /// Интерфейс модели сборки
    /// </summary>
    public interface IAppAssembly : IAppModel
    {
        int ComponentCount(bool TopLevelOnly);
        List<IAppComponent> GetComponents(bool TopLevelOnly);
        
    }


    public interface IAppComponent :IAppModel
    {
        /// <summary>
        /// Класс детали
        /// </summary>
        AppModel PartModel { get; }
        /// <summary>
        /// Модель Component2
        /// </summary>
        Component2 SwCompModel { get; }
        /// <summary>
        /// Погашенность компонента
        /// </summary>
        AppSuppressionState SuppressionState { get; }
        /// <summary>
        /// Исключено из спецификации
        /// </summary>
        bool ExcludeFromBOM { get; set; }
        /// <summary>
        /// Дочерние компоненты
        /// </summary>
        /// <param name="TopLeverOnly">Только верхнего уровня</param>
        /// <returns></returns>
        List<IAppComponent> GetComponents(bool TopLeverOnly);
        /// <summary>
        /// Коренной компонент
        /// </summary>
        /// <returns></returns>
        IAppComponent GetRootComponent();
        /// <summary>
        /// родительский компонент
        /// </summary>
        /// <returns></returns>
        IAppComponent GetParent();
        /// <summary>
        /// Количество дочерних компонентов
        /// </summary>
        /// <returns></returns>
        int GetChildrenCount();
        SWAPIlib.ComConn.MaterialProperty MaterialColor { get; set; }

    }

    public interface ISwPart : IAppModel, IConfProperty
    {
        int BodyCount { get; }
        string ConfigName { get; set; }
        bool IsSheetMetal { get; }
        bool HasDrawing { get; }
        bool SheetThickness(int BodyIndex);
        bool ExportDXF(string path);
        bool ExportModel(string path);
        bool ExportDrawing(string path);
    }



}
