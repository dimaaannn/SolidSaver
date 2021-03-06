﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
//using Sld = SldWorks;
using SolidApp;
using SolidWorks.Interop.sldworks;
using SwConst;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.CodeDom;

namespace SolidApp
{
    public class SwModelManager
    {
        private static ISldWorks _swApp;
        /// <summary>
        /// swApp singleton
        /// </summary>
        public static ISldWorks swApp => SwProcess.swApp;

        public readonly swDocumentTypes_e DocType;
        private readonly ModelDoc2 _swModel;
        private PartPrpManager _PrpMan;
        private SwExporter _swExporter;
        public string FilePath => _swModel.GetPathName();
        public PartPrpManager PrpMan
        {
            get
            {
                if (_PrpMan is null)
                    _PrpMan = PartPrpManager.CreateInstance(_swModel);
                return _PrpMan;
            }
        }

        public SwExporter Export
        {
            get
            {
                if (_swExporter is null)
                    _swExporter = new SwExporter(_swModel);
                return _swExporter;
            }
        }

        public ModelDoc2 swModel => _swModel;
        public string FolderPath => Path.GetDirectoryName(FilePath);
        public string FileName => Path.GetFileName(FilePath);
        public string FileNameWhithoutExt => Path.GetFileNameWithoutExtension(FilePath);
        private string GetDefaultPath
        {
            get
            {
                return string.Concat(this.FolderPath, this.FileNameWhithoutExt);
            }
        }
        public string GetFileExtension
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
        public bool SavePreview(string bmpPath = null, int height = 1000, int width = 1000)  //Сохранить превью как BMP файл
        {
            if (string.IsNullOrEmpty( bmpPath)) bmpPath = string.Concat(this.GetDefaultPath, ".bmp");
            return _swExporter.SavePreview(_swModel, bmpPath);
        }

        /// <summary>
        /// Сохранить копию документа
        /// </summary>
        /// <param name="path">По умолчанию - в папке документа</param>
        /// <returns>Результат выполнения</returns>
        public bool SaveAsCopy(string path = null)
        {
            bool ret = false;
            if (string.IsNullOrEmpty( path)) 
                path = string.Concat(this.FolderPath, FileNameWhithoutExt, "-Copy", this.GetFileExtension);

            ret = _swExporter.Copy(_swModel, path, true);
            return ret;
        }

        /// <summary>
        /// Сохранить документ чертежа в PDF 
        /// </summary>
        /// <param name="path">Путь сохранения</param>
        /// <param name="sheetnames">Имена листов для сохранения</param>
        /// <returns>Результат</returns>
        public bool Draw2Pdf(string path = null, string[] sheetnames = null)
        {
            
            bool ret = false;
            if (string.IsNullOrEmpty(path))
                path = string.Concat(this.FolderPath, FileNameWhithoutExt, ".pdf");

            Debug.Print("Draw2pdf: begin saving to path {0}", path);
            if (DocType == swDocumentTypes_e.swDocDRAWING)
            {
                ret = _swExporter.SavePdf(_swModel, path, true, sheetnames);
            }
            return ret;
        }
    }

    /// <summary>
    /// Прокси класс методов ModelDoc
    /// </summary>
    public static class ModelProxy
    {
        public static string GetName(ModelDoc2 swModel)
        {
            return swModel.GetTitle();
        }

        public static string GetPathName(ModelDoc2 swModel)
        {
            return swModel.GetPathName();
        }

        /// <summary>
        /// Открыть документ
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="options">Опции открытия документа</param>
        /// <param name="confName">Имя конфигурации</param>
        /// <returns></returns>
        public static ModelDoc2 Open(string filePath,
            swOpenDocOptions_e options, 
            string confName = "")
        {
            swDocumentTypes_e partType = swDocumentTypes_e.swDocNONE;

            switch(Path.GetExtension(filePath).ToUpper())
            {
                case ".SLDASM":
                    partType = swDocumentTypes_e.swDocASSEMBLY;
                    break;
                case ".SLDPRT":
                    partType = swDocumentTypes_e.swDocPART;
                    break;
                case ".SLDDRW":
                    partType = swDocumentTypes_e.swDocDRAWING;
                    break;
            }

            ModelDoc2 swModel = default;
            int e = 0, w = 0;

            if (!string.IsNullOrEmpty(filePath))
                swModel = SwProcess.swApp.OpenDoc6(
                    filePath,
                    (int)partType,
                    (int)options, 
                    confName,
                    ref e,
                    ref w);

            return swModel;
        }

        /// <summary>
        /// Сохранить документ
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="path">сохранить как</param>
        /// <param name="options">Опции сохранения def= copy</param>
        /// <returns></returns>
        public static bool SaveDocument(ModelDoc2 swModel, 
            string path = null, 
            swSaveAsOptions_e options = swSaveAsOptions_e.swSaveAsOptions_Copy)
        {
            bool ret = false;
            int res;
            if (path == null)
            {
                swModel.Save2(true);
                ret = true;
            }
            else if (!string.IsNullOrEmpty(path))
            {
                res = swModel.SaveAs3(path, 
                    (int) swSaveAsVersion_e.swSaveAsCurrentVersion, 
                    (int)options);
                ret = true;
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


    /// <summary>
    /// Экспорт моделей SolidWorks
    /// </summary>
    public class SwExporter
    {
        private ISldWorks _swApp => SwProcess.swApp;
        private ModelDoc2 _swModel;
        
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
        public SwExporter(ModelDoc2 swModel)
        {
            _swModel = swModel;
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
                    throw new FileLoadException("Ошибка!\nФайл {0} \nзаблокирован для записи!", path);
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

        //Todo Добавить адаптивный экспорт для моделей и сборок
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
        /// Экспорт листов чертежа в PDF
        /// </summary>
        /// <param name="swDrawing">Деталь чертежа</param>
        /// <param name="path">Путь сохранения</param>
        /// <param name="overwrite">Перезаписать</param>
        /// <param name="sheetNames">Имена листов для экспорта (опционально)</param>
        /// <returns>Статус</returns>
        public bool SavePdf(DrawingDoc swDrawing, 
            string path,
            bool overwrite = false,
            string[] sheetNames = null)
        {
            bool ret = false;

            var swModel = (ModelDoc2)swDrawing;

            swExportDataSheetsToExport_e exportSheets = 
                swExportDataSheetsToExport_e.swExportData_ExportSpecifiedSheets;

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

        public bool SaveDxf(string filePath)
        {
            bool ret = false;

            if (_swModel.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                if (FileChecker(filePath, true))
                {
                    PartDoc swPart = (PartDoc)_swModel;
                    ret = swPart.ExportFlatPatternView(filePath, (int)swExportFlatPatternViewOptions_e.swExportFlatPatternOption_RemoveBends);
                }
            }
            else
                throw new FormatException("SaveDxf: Wrong document type");

            return ret;
        }
    }


    /// <summary>
    /// Управление параметрами детали ModelDoc2
    /// </summary>
    public static class ConfigProxy
    {

        public static bool IsPartOrAsm(ModelDoc2 swModel)
        {
            bool ret = false;
            var docType = (swDocumentTypes_e)swModel.GetType();
            if (docType == swDocumentTypes_e.swDocASSEMBLY || docType == swDocumentTypes_e.swDocPART)
                ret = true;
            return ret;
        }

        /// <summary>
        /// Получить имя активной конфигурации
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        public static string GetActiveConfName(ModelDoc2 swModel)
        {
            string ret = null;
            if (IsPartOrAsm(swModel))
            {
                ret = swModel.IGetActiveConfiguration().Name;
            }
            return ret;
        }
        /// <summary>
        /// Отобразить конфигурацию
        /// </summary>
        /// <param name="swModel">Модель</param>
        /// <param name="confName">Имя конфигурации</param>
        /// <returns>Конфигурация активна</returns>
        public static bool SetActiveConf(ModelDoc2 swModel, string confName)
        {
            bool ret = false;
            if (IsPartOrAsm(swModel))
            {
                if (swModel.IGetActiveConfiguration().Name == confName)
                    ret = true;
                else
                    ret = swModel.ShowConfiguration2(confName);
            }
            return ret;
        }
        
        /// <summary>
        /// Получить спискок параметров и значений в конфигурации
        /// </summary>
        /// <param name="swModel">Модель</param>
        /// <param name="configName">Имя конфигурации</param>
        /// <returns>Словарь значений</returns>
        public static Dictionary<string, string> GetParamsDict(ModelDoc2 swModel, string configName)
        {
            var ret = new Dictionary<string, string> { };
            object names = null, values = null;
            bool bret = false;

            if (IsPartOrAsm(swModel))
            {
                bret = swModel.ConfigurationManager.GetConfigurationParams(
                    configName, 
                    out names, 
                    out values);
            }

            if (bret)
            {
                string[] nam = (string[]) names;
                var val = (string[])values;

                for(int i = 0; i < nam.Count(); ++i)
                {
                    ret.Add(nam[i], val[i]);
                }
            }
            return ret;
        }

        /// <summary>
        /// Получить список имён конфигураций
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        public static string[] GetConfigList(ModelDoc2 swModel)
        {
            string[] configsList = new string[] { };
            if (IsPartOrAsm(swModel))
            {
                configsList = swModel.GetConfigurationNames();
            }
            return configsList;
        }

        /// <summary>
        /// Получить объект конфигурации
        /// </summary>
        /// <param name="swModel">Модель</param>
        /// <param name="configName">Имя конфигурации</param>
        /// <returns>Конфигурация</returns>
        public static Configuration GetConfByName(ModelDoc2 swModel, string configName)
        {
            Configuration conf = null;
            if (IsPartOrAsm(swModel))
            {
                conf = swModel.GetConfigurationByName(configName);
            }
            return conf;
        }

        /// <summary>
        /// Получить вычисленное значение параметра конфигурации
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="configName">Имя конфигурации</param>
        /// <param name="fieldName">Имя параметра</param>
        /// <returns>Значение</returns>
        public static string GetConfParamValue(ModelDoc2 swModel, string configName, string fieldName)
        {
            string ret = null;
            if (IsPartOrAsm(swModel))
            {
                ret = swModel.GetCustomInfoValue(configName, fieldName);
            }
            return ret;
        }
        /// <summary>
        /// Получить параметр конфигурации
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="configName">Имя конфигурации</param>
        /// <param name="fieldName">Имя параметра</param>
        /// <returns></returns>
        public static string GetConfParam(ModelDoc2 swModel, string configName, string fieldName)
        {
            string ret = null;
            if (IsPartOrAsm(swModel))
            {
                ret = swModel.CustomInfo2[configName, fieldName];
            }
            return ret;
        }
        /// <summary>
        /// Установить значение параметра конфигурации
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="configName">Имя конфигурации</param>
        /// <param name="fieldName">Имя параметра</param>
        /// <param name="fieldVal">Значение параметра</param>
        /// <returns>Статус операции</returns>"
        public static bool SetConfParam(ModelDoc2 swModel, string configName, string fieldName, string fieldVal)
        {
            bool ret = false;
            if (IsPartOrAsm(swModel))
            {
                swModel.CustomInfo2[configName, fieldName] = fieldVal;

                if(swModel.CustomInfo2[configName, fieldName] == fieldVal)
                    ret = true;
            }
            return ret;
        }

    }


    //Добавить проверку типа детали
    public class PartPrpManager
    {
        private readonly ModelDoc2 _swModel;
        private swDocumentTypes_e _docType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swmodel">ModelDoc2 model</param>
        public PartPrpManager(ModelDoc2 swmodel)
        {
            if(!(swmodel is null))
            {
                _swModel = swmodel;
                _docType = (swDocumentTypes_e) _swModel.GetType();
                Debug.Print("PrpMan: Created instance");
            }
        }
        /// <summary>
        /// CreateNewInstance
        /// </summary>
        /// <param name="modeldoc"></param>
        /// <returns></returns>
        public static PartPrpManager CreateInstance(ModelDoc2 modeldoc)
        {
            return new PartPrpManager(modeldoc);
        }
        //Private Property
        private bool _isSheetMetal = false;
        private double _sheetThickness = -1;

        //Private methods
        /// <summary>
        /// Get feature by name
        /// </summary>
        /// <param name="featName">Feature type name</param>
        /// <returns>Feature object</returns>
        private Feature GetFeatureByType(string featName)
        {
            object[] modelFeat = _swModel.FeatureManager.GetFeatures(true);
            Feature swFeat = null;
            foreach (Feature feat in modelFeat)
            {
                Debug.WriteLine($"FeatName = {feat.GetTypeName()}");
                if (feat.GetTypeName() == featName)
                {
                    swFeat = feat;
                    break;
                }
            }
            //Debug.WriteLine("Get feature: Feature is {0}", swFeat is null ? "Not Found" : "Found");
            return swFeat;
        }
        private Body2[] GetBodies()
        {
            object[] bodiesArray;
            if(_swModel.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                PartDoc swpart = (PartDoc)_swModel;
                bodiesArray = swpart.GetBodies2((int)swBodyType_e.swAllBodies, true);
                return Array.ConvertAll(bodiesArray, item => (Body2)item);
            }
            return null;
        }

        //Public Property
        public string Title => _swModel.GetTitle();
        public swDocumentTypes_e DocType => (swDocumentTypes_e)_swModel.GetType(); 
        /// <summary>
        /// is part have SheetMetal body
        /// </summary>
        public bool isSheet
        {
            get
            {
                bool ret = false;
                Body2[] bodiesArr = GetBodies();
                if(!(bodiesArr is null))
                {
                    foreach (Body2 body in bodiesArr)
                    {   
                        if (body.IsSheetMetal())
                        {
                            ret = true;
                            break;
                        }
                    }
                }
                return ret;
            }
        }
        /// <summary>
        /// Get active configuration
        /// </summary>
        public string GetActiveConf => _swModel.IGetActiveConfiguration().Name;

        //TODO Переделать функцию на поиск в body из isSheet
        /// <summary>
        /// Get sheet thickness in mm
        /// </summary>
        public double GetSheetThickness
        {
            get
            {
                double ret = 0;
                if (_sheetThickness == -1 && isSheet)
                {
                    var feat = GetFeatureByType("SheetMetal");

                    if (!(feat is null))
                        _sheetThickness = feat.IParameter("Толщина").Value; //Cache thickness
                    else 
                        _sheetThickness = 0;
                }

                ret = _sheetThickness;
                Debug.WriteLine($"Sheet thickness is {_sheetThickness}");
                return ret;
            }
        }

        //Public methods
        /// <summary>
        /// Switch active configuration to
        /// </summary>
        /// <param name="confName">Configuration name</param>
        /// <returns>Success</returns>
        public bool SetActiveConf(string confName)
        {
            bool ret;
            ret =  _swModel.ShowConfiguration2(confName); //Переключить конфигурацию
            _swModel.ForceRebuild3(false); //Перестроить модель
            Debug.WriteLine("PrpMan: WARNING! config {0} is {1}", confName, ret ? "active" : "not active");
            return ret;
        }

        /// <summary>
        /// Get parameter value
        /// </summary>
        /// <param name="paramName">Field name</param>
        /// <returns>null or Field value</returns>
        public string GetParam(string paramName)
        {
            
            string propVal = null, valout;
            bool bret = false;

            //Todo set confman singleton
            if(_docType == swDocumentTypes_e.swDocASSEMBLY || _docType == swDocumentTypes_e.swDocPART)
            {
                ConfigurationManager confMan = _swModel.ConfigurationManager;
                bret = confMan.ActiveConfiguration.CustomPropertyManager.Get3(paramName, true, out valout, out propVal);
            }


            return propVal;
        }
        /// <summary>
        /// Set property parameter
        /// </summary>
        /// <param name="paramName">Field name</param>
        /// <param name="paramVal">Field value</param>
        /// <returns>Success</returns>
        public bool SetParam(string paramName, string paramVal)
        {
            //Todo set confman singleton
            ConfigurationManager confMan = _swModel.ConfigurationManager;
            int ret;
            bool bret;
            ret = confMan.ActiveConfiguration.CustomPropertyManager.Set(paramName, paramVal);
            bret = ret >= 0;
            return bret;
        }

    }

    public static class SwFileManager
    {

        /// <summary>
        /// Check for existence of drawing with part name
        /// </summary>
        /// <param name="filePath">Path to part</param>
        /// <returns></returns>
        public static bool isDrawExcist(string filePath)
        {
            string drawName = Path.ChangeExtension(filePath, "SLDDRW");
            return File.Exists(drawName);
        }

        /// <summary>
        /// Open drawing doc if exist
        /// </summary>
        /// <param name="fileModelPath">Path to part!</param>
        /// <param name="swModel">Out DrawingDoc</param>
        /// <returns>Success</returns>
        public static bool OpenDraw(string fileModelPath, out DrawingDoc swModel)
        {
            string drawName = Path.ChangeExtension(fileModelPath, "SLDDRW");
            bool ret = false;
            swModel = null;
            int temp = 0;
            if (File.Exists(drawName))
            {
                swModel = SwProcess.swApp.OpenDocSilent(drawName, (int)swDocumentTypes_e.swDocDRAWING, ref temp);
            }

            if (!(swModel == null))
                ret = true;

            return ret;
        }

        public static bool CreateFolder(string folderPath)
        {
            bool ret = false;
            System.IO.DirectoryInfo dirInfo;
            if (System.IO.Directory.Exists(folderPath))
                ret = true;
            else
            {
                dirInfo = Directory.CreateDirectory(folderPath);
                ret = dirInfo.Exists;
            }
            return ret;
        }
    }

    /// <summary>
    /// Основные операции с приложением
    /// </summary>
    public static class SwProcess
    {
        private static Process _swProcess;
        private static ISldWorks _swApp;

        /// <summary>
        /// GetSolidWorks process
        /// </summary>
        public static Process swProcess
        {
            get
            {
                if(_swProcess is null)
                {
                    Process[] ProcessList;
                    ProcessList = Process.GetProcessesByName("SLDWORKS");
                    if (ProcessList.Count() > 0)
                    {
                        _swProcess = ProcessList.First();
                        _swProcess.EnableRaisingEvents = true;
                    }
                }
                return _swProcess;
            }
        }

        /// <summary>
        /// Check SolidWorks is running
        /// </summary>
        public static bool IsRunning
        {
            get
            {
                bool ret = false;
                if (!(swProcess is null))
                    ret = true;
                return ret;
            }
            
        }

        /// <summary>
        /// Подключение к com API
        /// </summary>
        /// <returns></returns>
        private static ISldWorks GetSWApp()
        {
            string progId = "SldWorks.Application";
            var progType = System.Type.GetTypeFromProgID(progId);

            ISldWorks swApp = System.Activator.CreateInstance(progType) as SolidWorks.Interop.sldworks.ISldWorks;
            return swApp;
        }

        /// <summary>
        /// Получить экземпляр АПИ
        /// </summary>
        public static ISldWorks swApp
        {
            get
            {
                if (_swApp is null)
                {
                    Debug.Print("geting SWapp");
                    _swApp = GetSWApp();
                }

                if (_swApp is null) 
                    throw new ArgumentNullException("SwApp is null");
                return _swApp;
            }
            
        }

    }
}

