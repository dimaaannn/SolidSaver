using SolidWorks.Interop.sldworks;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib.Property;
using SWAPIlib.Property.PropertyBase;
using System.Collections.Generic;
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
    public class SwModelWrapper : Target<ModelDoc2>, ISwModelWrapper
    {
        //public SwModelWrapper() { }
        public SwModelWrapper(ModelDoc2 model) : base(
            model
            ,ModelProxy.GetName(model)
            , ToTargetType(PartTypeChecker.GetSWType(model)))
        {
        }

        public string FilePath => ModelProxy.GetPathName(TTarget);
        public string FileName => Path.GetFileName(FilePath);
        public AppDocType DocType => PartTypeChecker.GetSWType(TTarget);

        public IAppModel GetAppModel() => ModelClassFactory.GetModel(TTarget);

        public override string ToString()
        {
            if (string.IsNullOrEmpty(FileName))
                return this.TargetName;
            else
                return this.FileName;
        }

        /// <summary>
        /// Преобразовать тип обёртки в тип свойства
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static TargetType ToTargetType(AppDocType type)
        {
            TargetType ret;
            switch (type)
            {
                case AppDocType.swNONE:
                    ret = TargetType.None;
                    break;
                case AppDocType.swPART:
                    ret = TargetType.Part;
                    break;
                case AppDocType.swASM:
                    ret = TargetType.Assembly;
                    break;
                case AppDocType.swDRAWING:
                    ret = TargetType.Drawing;
                    break;
                case AppDocType.swCOMPONENT:
                    ret = TargetType.Component;
                    break;
                default:
                    ret = TargetType.None;
                    break;
            }
            return ret;
        }

        #region TargetInterface

        public override string TargetInfo => $"{TargetType}:{FileName}";


        #endregion
    }
}
