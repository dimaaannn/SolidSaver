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
using System.Data;

namespace SolidApp
{
    public class SwModelManager
    {
        private static ISldWorks _swApp;
        public readonly swDocumentTypes_e DocType;
        private readonly ModelDoc2 _swModel;
        public string FilePath => _swModel.GetPathName();

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

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="swModel">Модель документа</param>
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

        /// <summary>
        /// Сохранить картинку bmp с превью документа
        /// </summary>
        /// <param name="bmpPath">Путь сохранения. Пустой - сохраняет в папку документа</param>
        /// <param name="height">Высота в пикселях</param>
        /// <param name="width">Ширина в пикселях</param>
        /// <returns></returns>
        public bool SavePreview(string bmpPath = "", int height = 1000, int width = 1000)  //Сохранить превью как BMP файл
        {
            if (bmpPath == "") bmpPath = string.Concat(this.GetDefaultPath, ".bmp");
            var swExp = new SwExporter(_swApp);
            return swExp.SavePreview(_swModel, bmpPath);
        }

        /// <summary>
        /// Сохранить копию документа
        /// </summary>
        /// <param name="path">По умолчанию - в папке документа</param>
        /// <returns>Результат выполнения</returns>
        public bool SaveAsCopy(string path = "")
        {
            bool ret = false;
            var expMan = new SwExporter(_swApp);
            if (path == "") 
                path = string.Concat(this.FolderPath, FileNameWhithoutExt, "-Copy", this.GetFileExtension);

            ret = expMan.Copy(_swModel, path, true);
            return ret;
        }
    }

    public enum SaFileStatus
    {
        NotExist,
        Exist,
        ExistLocked
    }; //Статус доступности файла

    /// <summary>
    /// Экспорт моделей SolidWorks
    /// </summary>
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
            if(fileStatus == SaFileStatus.Exist)
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
            if (string.IsNullOrEmpty(path))
                throw new NoNullAllowedException("Путь сохранения не может быть пустым");
            fileStatus =  FileExist(path);
            
            _warning = 0; //Сбросить предупреждения
            _errors = 0;

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

        /// <summary>
        /// Экспорт листов чертежа в PDF
        /// </summary>
        /// <param name="swModel">Модель чертежа</param>
        /// <param name="path">Путь сохранения</param>
        /// <param name="overwrite">Перезаписать</param>
        /// <param name="sheetNames">Имена листов для экспорта (опционально)</param>
        /// <returns>Статус</returns>
        public bool SavePdf(ModelDoc2 swModel, 
            string path,
            bool overwrite = false,
            string[] sheetNames = null)
        {
            bool ret = false;
            DrawingDoc swDrawing;
            swExportDataSheetsToExport_e exportSheets = 
                swExportDataSheetsToExport_e.swExportData_ExportSpecifiedSheets;

            if (swModel.GetType() != (int)swDocumentTypes_e.swDocDRAWING) 
                throw  new System.TypeLoadException("Модель не является чертежом");
            
            swDrawing = (DrawingDoc)swModel;

            if (swDrawing is null) 
                throw new NullReferenceException("SavePdf: Wrong reference!");
            Debug.Print("SavePdf: begin.");

            if (sheetNames is null) exportSheets = swExportDataSheetsToExport_e.swExportData_ExportAllSheets;

            if(FileChecker(path, overwrite))
            {
                ExportPdfData ExportPdfParams = _swApp.GetExportFileData((int)swExportDataFileType_e.swExportPdfData);
                ExportPdfParams.SetSheets((int)exportSheets, sheetNames);
                ret = swModel.Extension.SaveAs(path, 0, 0, ExportPdfParams, ref _errors, ref _warning);
                Debug.Print("SavePdf status {0}", ret ? "Success" : "Failed");
            }
            return ret;
        }

        /// <summary>
        /// Сохранить .bmp файл с превью
        /// </summary>
        /// <param name="swModel">Модель документа</param>
        /// <param name="path">Путь файла сохранения</param>
        /// <param name="overwrite">Перезаписать если существует</param>
        /// <param name="height">Высота в px</param>
        /// <param name="width">Ширина в px</param>
        /// <returns>Статус выполнения</returns>
        public bool SavePreview(ModelDoc2 swModel, string path, bool overwrite = true, int height = 1000, int width = 1000)  //Сохранить превью как BMP файл
        {
            bool ret = false;
            if(FileChecker(path, overwrite))
            {
                ret =  swModel.SaveBMP(path, height, width);
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

