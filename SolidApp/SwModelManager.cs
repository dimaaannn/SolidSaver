using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sld = SldWorks;
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
            if (!System.IO.File.Exists(filename)) fileStatus = SaFileStatus.NotExist;
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
            if (!(swModel is null))
            {
                if (_swApp is null)
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
            if (path == "")
            {
                path = string.Concat(this.FolderPath, FileNameWhithoutExt, "-Copy", this.GetFileExtension);
            }
            Debug.WriteLine("SaveAsCopy: try saving to Path = {0}", path);
            int warnings = 0, errors = 0;
            bool ret = false;

            if (FileExist(path) == 0)
            {
                var swExportData = _swApp.GetExportFileData((int)swFileSaveTypes_e.swFileSave);
                ret = _swModel.Extension.SaveAs(path,
                    (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                    (int)swSaveAsOptions_e.swSaveAsOptions_Copy,
                    swExportData, ref errors, ref warnings);
                Debug.WriteLine($"SaveAsCopy: errors = {errors}, warnings = {warnings}\nSuccess = {ret}");
            }
            return ret;
        }
    }

    public enum SaFileStatus
    {
        NotExist,
        Exist,
        ExistLocked
    }; //Статус доступности файла

    public class SwExporter
    {
        private static ISldWorks _swApp;
        private int _warning, _errors;
        /// <summary>
        /// Ошибки при последнем сохранении
        /// </summary>
        public int Warnings { get => _warning; }
        /// <summary>
        /// Предупреждения при последнем сохранении
        /// </summary>
        public int Errors { get => _errors; }
        /// <summary>
        /// Конструктор класса SwExporter
        /// </summary>
        /// <param name="swApp">SolidWorks application instance</param>
        public SwExporter(ISldWorks swApp)
        {
            if(_swApp is null) _swApp = swApp;
            _warning = 0;
            _errors = 0;
            Debug.Print("SwExporter: Cls created");
        }

        /// <summary>
        /// Проверка существования и возможности записи в файл
        /// </summary>
        /// <param name="filename">Путь к файлу</param>
        /// <returns>0: Не существует, 1: Существует, 2: Существует, заблокирован</returns>
        public static SaFileStatus FileExist(string filename)
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
            return fileStatus;
        }

        /// <summary>
        /// Обработка действий перед записью в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="overwrite">Перезаписать если возможно</param>
        /// <returns>True если запись разрешена</returns>
        private bool FileChecker(string path, bool overwrite = false)
        {
            bool ret = false;
            SaFileStatus fileStatus;
            Debug.WriteLine("SwExporter: Path = {0}", path);
            fileStatus =  FileExist(path);
            
            switch(fileStatus)
            {
                case SaFileStatus.ExistLocked:
                    throw new System.IO.FileLoadException("Ошибка!\nФайл {0} \nзаблокирован для записи!", path);
                case SaFileStatus.Exist:
                    ret = overwrite;
                    break;
                case SaFileStatus.NotExist:
                    ret = true;
                    break;
            }
            Debug.Print("SwExporter: Запись в файл {0}", ret ? "Разрешена" : "Запрещена");
            return ret;
        }
        /// <summary>
        /// Сохранить копию модели
        /// </summary>
        /// <param name="swModel">Модель для сохранения</param>
        /// <param name="path">Путь сохранения</param>
        /// <param name="overwrite">Перезаписать?</param>
        /// <returns>Статус записи</returns>
        public bool Copy(ModelDoc2 swModel, string path, bool overwrite)
        {
            bool ret = false;
            if(FileChecker(path, overwrite))
            {
                var swExportData = _swApp.GetExportFileData((int)swFileSaveTypes_e.swFileSave);
                ret = swModel.Extension.SaveAs(path,
                    (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                    (int)swSaveAsOptions_e.swSaveAsOptions_Copy,
                    swExportData, ref _errors, ref _warning);
                Debug.WriteLine($"SwExporter_SaveCopy: errors = {_errors}, warnings = {_warning}\nSuccess = {ret}");
            }
            return ret;
        }


    }


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

