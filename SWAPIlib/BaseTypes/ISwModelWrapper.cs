using SolidWorks.Interop.sldworks;
using SWAPIlib.ComConn.Proxy;
using System.IO;

namespace SWAPIlib.BaseTypes
{
    /// <summary>
    /// Легковесная обёртка над классом модели
    /// </summary>
    public interface ISwModelWrapper
    {
        /// <summary>
        /// Имя файла
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// Полный путь к файлу
        /// </summary>
        string FilePath { get; }
        /// <summary>
        /// Имя
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Модель привязки
        /// </summary>
        ModelDoc2 SwModel { get; set; }
        /// <summary>
        /// Тип документа
        /// </summary>
        AppDocType DocType { get; }
        /// <summary>
        /// Сгенерировать класс приложения
        /// </summary>
        IAppModel GetAppModel();
    }

    /// <summary>
    /// Класс обёртка над моделью
    /// </summary>
    public class SwModelWrapper : ISwModelWrapper
    {
        public SwModelWrapper() { }
        public SwModelWrapper(ModelDoc2 model) : this()
        {
            _SwModel = model;
        }
        private ModelDoc2 _SwModel;
        public ModelDoc2 SwModel { get => _SwModel; set => _SwModel = value; }

        public string FilePath => ModelProxy.GetPathName(SwModel);
        public string FileName => Path.GetFileName(FilePath);
        public string Title => ModelProxy.GetName(SwModel);
        public AppDocType DocType => PartTypeChecker.GetSWType(SwModel);
        public IAppModel GetAppModel() => ModelClassFactory.GetModel(SwModel);
    }
}
