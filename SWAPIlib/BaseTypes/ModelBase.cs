using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SwConst;
using System.Diagnostics;
using SWAPIlib.ComConn.Proxy;

namespace SWAPIlib.BaseTypes
{

    /// <summary>
    /// Базовый класс модели
    /// </summary>
    public class AppModel : IAppModel
    {
        private ModelDoc2 _swModel;

        public bool IsExist { get; private set; }
        public ModelDoc2 SwModel { get => _swModel; }
        public virtual AppDocType DocType { get; }
        public string FileName { get => System.IO.Path.GetFileName(Path); }
        public virtual string Title { get => ModelProxy.GetName(_swModel); }
        public string Path { get; }
        public List<ISwProperty> PropList { get; protected set; }
        public IFileModelProp GlobalModelProp { get; }

        public event EventHandler<SwEventArgs> CloseFile;

        /// <summary>
        /// ModelDoc2 Constructor
        /// </summary>
        /// <param name="swModel"></param>
        public AppModel(ModelDoc2 swModel)
        {
            PropList = new List<ISwProperty>();
            GlobalModelProp = new FileModelProp(this)
            {
                IsRoot = false,
            };
            IsExist = true;
            DocType = PartTypeChecker.GetSWType(swModel);

            if (swModel != null)
            {
                //TODO Add default properties to list
                _swModel = swModel;
                Path = ModelProxy.GetPathName(SwModel);


                #region EventProxy
                if (DocType == AppDocType.swASM)
                {
                    (swModel as AssemblyDoc).DestroyNotify += CloseFileHandler;
                }
                else if (DocType == AppDocType.swPART)
                {
                    (swModel as PartDoc).DestroyNotify += CloseFileHandler;
                }
                else if (DocType == AppDocType.swDRAWING)
                {
                    (swModel as DrawingDoc).DestroyNotify += CloseFileHandler;
                }
                #endregion
            }
            else
            {
                string msg = "new Model- null reference";
                Debug.WriteLine(msg);
                //throw new NullReferenceException(msg);
            }
            string succes = $"Class Model created - {DocType} - {FileName}";

        }

        private int CloseFileHandler()
        {
            IsExist = false;
            string evT = $"Document {FileName} closed";
            var evArg = new SwEventArgs(evT);
            CloseFile?.Invoke(this, evArg);
            return 0;
        }

        public int CompareTo(object other)
        {
            if (other is IAppModel model)
                return this.DocType.CompareTo(model.DocType);
            else return 0;
        }

        /// <summary>
        /// Список имён конфигураций
        /// </summary>
        public virtual List<string> ConfigList
        {
            get
            {
                if (_configList == null)
                    _configList = new List<string>(SwModel.GetConfigurationNames());
                return _configList;
            }
        }
        private List<string> _configList;

        public virtual bool VisibState
        {
            get => SwModel.Visible;
            set => SwModel.Visible = value;
        }
    }


    //TODO add ConfigName
    //public class AppPart  : ISwPart
    //{
    //    int BodyCount { get; }
    //    bool IsSheetMetal { get; }
    //    bool HasDrawing { get; }
    //    bool SheetThickness(int BodyIndex);
    //    bool ExportDXF(string path);
    //    bool ExportModel(string path);
    //    bool ExportDrawing(string path);
    //}
}
