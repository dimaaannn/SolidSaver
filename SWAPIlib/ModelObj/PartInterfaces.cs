using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;

namespace SWAPIlib
{
    public interface ISwModel
    {
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
        event EventHandler<SwEventArgs> CloseFile;
        IFileModelProp GlobalModelProp { get; }
        /// <summary>
        /// Список свойств модели
        /// </summary>
        List<ISwProperty> PropList { get;}  //TODO create propList class
        
    }

    //TODO сделать интерфейс сборки
    /// <summary>
    /// Интерфейс модели сборки
    /// </summary>
    public interface ISwAssembly : ISwModel
    {
        int ComponentCount(bool TopLevelOnly);
        List<SwComponent> GetComponents(bool TopLevelOnly);
        /// <summary>
        /// Имя конфигурации
        /// </summary>
        string ConfigName { get; set; }
        List<string> ConfigList { get; }
    }


    public interface ISwComponent :ISwModel
    {
        Component2 SwComp { get; }
        /// <summary>
        /// Статус отображения
        /// </summary>
        AppSuppressionState SuppressionStatus { get; }
        /// <summary>
        /// Количество подкомпонентов
        /// </summary>
        /// <param name="TopLevelOnly"></param>
        /// <returns></returns>
        int ComponentCount { get; }
        /// <summary>
        /// Связанная конфигурация
        /// </summary>
        string ConfigName { get; set; }
        SwComponent GetRootComponent();
        /// <summary>
        /// Получить дочерние компоненты
        /// </summary>
        /// <param name="TopLevelOnly"></param>
        /// <returns></returns>
        List<SwComponent> GetComponents(bool TopLevelOnly);
        
    }

    public interface IAppComponent<out T> where T : ISwModel
         
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
        /// Имя связанной конфигурации
        /// </summary>
        string RefConfigName { get; set; }
        /// <summary>
        /// Статус отображения компонента
        /// </summary>
        AppCompVisibility VisibState { get; set; }
        /// <summary>
        /// Исключено из спецификации
        /// </summary>
        bool ExcludeFromBOM { get; set; }
        /// <summary>
        /// Дочерние компоненты
        /// </summary>
        /// <param name="TopLeverOnly">Только верхнего уровня</param>
        /// <returns></returns>
        List<AppComponent> GetComponents(bool TopLeverOnly);
        /// <summary>
        /// Коренной компонент
        /// </summary>
        /// <returns></returns>
        T GetRootComponent();
        /// <summary>
        /// родительский компонент
        /// </summary>
        /// <returns></returns>
        T GetParent();
        /// <summary>
        /// Количество дочерних компонентов
        /// </summary>
        /// <returns></returns>
        int GetChildrenCount();
        List<string> ConfigList { get; }
    }

    public interface ISwPart : ISwModel, IConfProperty
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
