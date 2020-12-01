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
    }

    public interface ISwOperation
    {
        int Status { get; }
    }

    public interface ISwAssembly : ISwModel, IConfProperty
    {
        int ComponentCount(bool TopLevelOnly);
        IEnumerable<ISwComponent> GetComponents(bool TopLevelOnly);
    }


    public interface ISwComponent :ISwModel
    {
        Component2 SwComp { get; }
        bool SuppressionStatus { get; }
        int ComponentCount(bool TopLevelOnly);
        ISwComponent GetRootComponent();
        /// <summary>
        /// Получить дочерние компоненты
        /// </summary>
        /// <param name="TopLevelOnly"></param>
        /// <returns></returns>
        IEnumerable<ISwComponent> GetComponents(bool TopLevelOnly);
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

    public interface IConfProperty
    {
        string ActiveConfigName { get; }
        IEnumerable<string> ConfigNames { get; }
        IDictionary<string, string> ConfigPropertyList { get; }
        string GetPropValue(string confName, string fieldName);
        bool SetPropValue(string confName, string fieldName, string fieldVal);
    }

    /// <summary>
    /// Расширенные свойства модели
    /// </summary>
    public interface IAppModelProp
    {
        bool IsRoot { get; }
        string WorkFolder { get; set; }
        string ModelFolder { get; }
        bool GetProjectData { get; }  //TODO убрать Legacy features в отдельный класс
        string ProjectNumber { get; }
        string ProjectClient { get; }
        string ProjectName { get; }
        string ProjectType { get; }

        bool IsImported { get; }
        bool IsLibModel { get; }

        bool IsSheetPart { get; }
        bool IsHasDrawing { get; }

    }   

    public interface IPropertyManager : IEnumerable<ISwProperty>
    {
        AppModel SwModel { get; }
        void UpdateAll();
        void WriteAll();
        void AddItem(ISwProperty PropGetter);
        bool RemoveItem(int Index);
    }
    
    /// <summary>
    /// Объект свойства
    /// </summary>
    public interface ISwProperty
    {
        AppModel AppModel { get; set; }
        bool IsReadable { get; }
        bool IsWritable { get; }
        bool IsValid { get; }

        string UserName { get; set; }
        string PropertyName { get; }
        string PropertyValue { get; set; }
        string ConfigName { get; set; }
        
        void Update();
        bool WriteValue();

    }
}
