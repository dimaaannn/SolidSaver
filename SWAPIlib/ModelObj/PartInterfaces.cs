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
        IList<ISwProperty> PropList { get;}  //TODO create propList class
        
    }

    //TODO сделать интерфейс сборки
    /// <summary>
    /// Интерфейс модели сборки
    /// </summary>
    public interface ISwAssembly : ISwModel, IConfProperty
    {
        int ComponentCount(bool TopLevelOnly);
        IEnumerable<SwComponent> GetComponents(bool TopLevelOnly);
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
        SwComponent GetRootComponent();
        /// <summary>
        /// Получить дочерние компоненты
        /// </summary>
        /// <param name="TopLevelOnly"></param>
        /// <returns></returns>
        List<SwComponent> GetComponents(bool TopLevelOnly);
    }

    public interface ISwPart : ISwModel, IConfProperty
    {
        int BodyCount { get; }
        bool IsSheetMetal { get; }
        bool HasDrawing { get; }
        bool SheetThickness(int BodyIndex);
        bool ExportDXF(string path);
        bool ExportModel(string path);
        bool ExportDrawing(string path);
    }



}
