using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidApp;
using SolidWorks.Interop.sldworks;
using SwConst;

namespace SolidApp
{
    public class SwModelManager
    {
        private static ISldWorks _swApp;
        public readonly swDocumentTypes_e DocType;
        private readonly ModelDoc2 _swModel;
        public string FilePath => _swModel.GetPathName();

        public static int FileExist(string filename)
        {
            SaFileStatus fileStatus = SaFileStatus.NotExist;
            if (System.IO.File.Exists(filename)) fileStatus = SaFileStatus.Exist;
            else
            {
                try
                {
                    System.IO.FileStream fs =
                        System.IO.File.Open(filename, System.IO.FileMode.Open,
                        System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
                    fs.Close();
                }
                catch (System.IO.IOException ex)
                {
                    fileStatus = SaFileStatus.ExistLocked;
                }
            }
            return (int)fileStatus;
        } //Проверяет заблокирован ли файл

        public string FolderPath
        {
            get
            {
                string result = this.FilePath;
                int lastSlash = result.LastIndexOf("\\");
                return result.Substring(0, lastSlash + 1);
            }
        }
        public string FileName
        {
            get
            {
                string result = this.FilePath;
                int lastSlash = result.LastIndexOf("\\");
                return result.Substring(lastSlash);
            }
        }
        public string FileNameWhithoutExt
        {
            get
            {
                string result = this.FilePath;
                int lastSlash = result.LastIndexOf("\\");
                int lastPoint = result.LastIndexOf(".");
                return result.Substring(lastSlash + 1, lastPoint - lastSlash - 1);
            }
        }
        private string GetDefaultPath
        {
            get
            {
                return string.Concat(this.FolderPath, this.FileNameWhithoutExt);
            }
        }
        private string GetFileExtension
        {
            get
            {
                string result = this.FilePath;
                int lastPoint = result.LastIndexOf(".");
                return result.Substring(lastPoint);
            }
        }

        public SwModelManager(ModelDoc2 swModel)
        {
            if(!(swModel is null))
            {
                if(_swApp is null)
                {
                    Debug.Print("geting SWapp");
                    _swApp = SolidTools.GetSWApp();
                } //Set swApp insance
                _swModel = swModel;
                DocType = (swDocumentTypes_e)swModel.GetType();
                Debug.Print("SwModelManager: SwModel initialised");
            }
            else
            {
                throw new ArgumentNullException("SwModelManager: Модель отсутствует");
            }
        }

        

        public bool SavePreview(string bmpPath = "", int height = 1000, int width = 1000)  //Сохранить превью как BMP файл
        {
            if (bmpPath == "") bmpPath = string.Concat(this.GetDefaultPath, ".bmp");
            return _swModel.SaveBMP(bmpPath, height, width);
        }

        public bool SaveAsCopy(string path = "")
        {
            if(path == "")
            {
                path = string.Concat(this.FolderPath, FileNameWhithoutExt, "-Copy", this.GetFileExtension);
            }
            Debug.WriteLine("SaveAsCopy: try saving to Path = {0}", path);
            int warnings = 0, errors = 0;
            bool ret;
            var swExportData = _swApp.GetExportFileData((int)swFileSaveTypes_e.swFileSaveAsCopy);
            ret = _swModel.Extension.SaveAs(path,
                (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                (int)swSaveAsOptions_e.swSaveAsOptions_Copy,
                swExportData, ref errors, ref warnings);
            Debug.WriteLine($"SaveAsCopy: errors = {errors}, warnings = {warnings}");
            return ret;
        }
    }

    enum SaFileStatus
    {
        NotExist,
        Exist,
        ExistLocked
    }; //Статус доступности файла

    public class SwPartManager : SwModelManager
    {
        private readonly PartDoc _swPartDoc;

        public SwPartManager(ModelDoc2 swpartdoc) :base(swpartdoc)
        {
            if(DocType == swDocumentTypes_e.swDocPART)
            {
                _swPartDoc = (PartDoc)swpartdoc;
                Debug.Print("SwPart initialised");
            }
            else
            {
                throw new TypeLoadException("SwPartManager: Wrong model type");
            }
        } //init PartDoc


    }

}

