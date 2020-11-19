using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Policy;
using System.Runtime.CompilerServices;
using SolidWorks.Interop.sldworks;
using SwConst;

namespace SWAPIlib
{

    /// <summary>
    /// Основные операции с API
    /// </summary>
    public static class SwAPI
    {
        private static Process _swProcess;
        private static ISldWorks _swApp;

        /// <summary>
        /// Процесс SolidWorks найден
        /// </summary>
        public static event System.EventHandler SwIsRunning;
        public static event System.EventHandler SwIsDisposed
        {
            add => _swProcess.Disposed += value;
            remove => _swProcess.Disposed -= value;
        }
        public static event System.EventHandler ComConnected;
        

        /// <summary>
        /// GetSolidWorks process
        /// </summary>
        public static Process swProcess
        {
            get
            {
                if (_swProcess is null)
                {
                    Process[] ProcessList;
                    ProcessList = Process.GetProcessesByName("SLDWORKS");
                    if (ProcessList.Count() > 0)
                    {
                        _swProcess = ProcessList.First();
                        _swProcess.EnableRaisingEvents = true;
                        string evText = "SolidWorks process is running";
                        SwIsRunning?.Invoke(_swProcess, new SwEventArgs(evText));
                        Debug.WriteLine(evText);
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
            ISldWorks swApp = null;

            Debug.Print("geting SWapp");
            string evText = "Sw API connected";
            swApp = System.Activator.CreateInstance(progType) as SolidWorks.Interop.sldworks.ISldWorks;
            
            if(swApp != null)
            {
                Debug.WriteLine(evText);
                ComConnected?.Invoke(swApp, new SwEventArgs(evText));
            }
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
                    _swApp = GetSWApp();
                return _swApp;
            }

        }
    }

    /// <summary>
    /// Аргументы события
    /// </summary>
    public class SwEventArgs : EventArgs
    {
        public readonly string Text;

        public SwEventArgs(string text) =>
            Text = text;
        public SwEventArgs() => Text = null;
    }

    /// <summary>
    /// Управление конфигурациями детали ModelDoc2
    /// </summary>
    public static class ModelConfigProxy
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
                string[] nam = (string[])names;
                var val = (string[])values;

                for (int i = 0; i < nam.Count(); ++i)
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

                if (swModel.CustomInfo2[configName, fieldName] == fieldVal)
                    ret = true;
            }
            return ret;
        }

    }

    /// <summary>
    /// Вспомогательные функции
    /// </summary>
    public static class ServiceCl
    {
        /// <summary>
        /// Конвертировать массив объектов в массив типа
        /// </summary>
        /// <typeparam name="Tout">Тип элемента массива</typeparam>
        /// <param name="inputArray">Массив объектов</param>
        /// <returns>Типизированный массив</returns>
        public static Tout[] ObjArrConverter<Tout>( IList<object> inputArray)
        {
            Tout[] outputArray;

            int itemCounter = inputArray.Count();
            outputArray = new Tout[itemCounter];

            for(int i = 0; i < itemCounter; ++i)
            {
                outputArray[i] = (Tout) inputArray[i];
            }
            return outputArray;
        }

    }
    
    /// <summary>
    /// Основные операции ModelDoc2
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

            switch (System.IO.Path.GetExtension(filePath).ToUpper())
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
                swModel = SwAPI.swApp.OpenDoc6(
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
                    (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                    (int)options);
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Get Features from model
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="TopLevelOnly">Только верхнего уровня</param>
        /// <returns></returns>
        public static Feature[] GetFeatures(ModelDoc2 swModel, bool TopLevelOnly = true)
        {
            Feature[] fArray = null;
            object[] oArray;

            oArray = swModel.FeatureManager.GetFeatures(TopLevelOnly);
            int itemCounter = oArray.Count();

            fArray = new Feature[itemCounter];
            for (int i = 0; i < itemCounter; ++i)
            {
                fArray[i] = (Feature)oArray[i];
            }

            return fArray;
        }

        /// <summary>
        /// Сохранить превью в bmp
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CreateBmpPreview(
            ModelDoc2 swModel, 
            string path, 
            int height = 1000, 
            int width = 1000)
        {
            bool ret = false;

            ret = swModel.SaveBMP(path, height, width);
            return ret;
        }
    }


    public static class PartDocProxy 
    {
        /// <summary>
        /// Получить массив тел в детали
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        public static Body2[] GetBodies(ModelDoc2 swModel)
        {
            object[] bodyArr = null;

            if (swModel.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                PartDoc swpart = (PartDoc)swModel;
                bodyArr = swpart.GetBodies2((int)swBodyType_e.swAllBodies, true);
                return Array.ConvertAll(bodyArr, item => (Body2)item);
            }

            return bodyArr as Body2[];
        }

        /// <summary>
        /// Get Features list from body
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Feature[] GetFeatures(Body2 body)
        {
            object[] f = body?.GetFeatures();

            int featCount = f.Count();
            Feature[] fArray = new Feature[featCount];

            for(int i = 0; i < featCount; ++i)
            {
                fArray[i] = (Feature) f[i];
            }
            return fArray;
        }

        /// <summary>
        /// Get Features from model
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="TopLevelOnly">Только верхнего уровня</param>
        /// <returns></returns>
        public static Feature[] GetFeatures(ModelDoc2 swModel, bool TopLevelOnly = true)
        {
            return ModelProxy.GetFeatures(swModel, TopLevelOnly);
        }


        /// <summary>
        /// Получить тела листового металла из детали
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        public static Body2[] GetSheetBodies(ModelDoc2 swModel)
        {
            Body2[] retList = null;
            var tempList = new List<Body2>();
            if (swModel.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                //Найти тело с свойством IsSheetMetal
                Body2[] bodyList;
                bodyList = GetBodies(swModel);

                foreach (Body2 body in bodyList)
                {
                    if (body.IsSheetMetal())
                    {
                        tempList.Add(body);
                    }
                }
                retList = tempList.ToArray();
            }
            return retList;
        }

        /// <summary>
        /// Получить Feature листового металла из тела
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Feature GetSheetFeature(Body2 body)
        {
            Feature ret = null;
            if(body != null)
            {
                Feature[] featList = GetFeatures(body);
                foreach(Feature feat in featList)
                {
                    if (feat.GetTypeName() == "SheetMetal")
                    {
                        ret = feat;
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Быстрая проверка, содержит ли деталь листовые тела
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        public static bool IsSheetMetal(ModelDoc2 swModel)
        {
            bool ret = false;
            if(swModel.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                //Найти тело с свойством IsSheetMetal
                Body2[] bodyList;
                bodyList = GetBodies(swModel);

                foreach(Body2 body in bodyList)
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

        /// <summary>
        /// Получить список толщин тел в детали
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        public static double[] GetSheetThickness(ModelDoc2 swModel)
        {
            double[] ret = null;

            var tempList = new List<double>();
            var bodyArr = GetSheetBodies(swModel);
            if(bodyArr.Count() > 0)
            {
                foreach(Body2 body in bodyArr)
                    tempList.Add(GetSheetFeature(body).IParameter("Толщина").Value);

                ret = tempList.ToArray();
            }

            return ret;
        }

        /// <summary>
        /// Экспорт развёртки в DXF
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ExportDXF(ModelDoc2 swModel, string path)
        {
            bool ret = false;
            Debug.WriteLine("ExportDXF start");
            PartDoc partDoc = (PartDoc)swModel;
            if (partDoc != null)
            {
                swExportFlatPatternViewOptions_e sOptions =
                    swExportFlatPatternViewOptions_e.swExportFlatPatternOption_RemoveBends;
                ret = partDoc.ExportFlatPatternView(path, (int)sOptions);
                Debug.WriteLine("ExportDXF is {0}\n{1}", ret ? "success" : "failed", path);
            }
            else Debug.WriteLine("ExportDXF - can't convert to PartDoc");

            return ret;
        }

        
    }

    /// <summary>
    /// Методы чертежей
    /// </summary>
    public static class DrawDocProxy
    {
        /// <summary>
        /// Экспортировать чертёж в PDF
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="path">Файл сохранения</param>
        /// <param name="sheetNames">Имена листов для сохранения (опционально)</param>
        /// <returns></returns>
        public static bool ExportPDF(
            ModelDoc2 swModel, 
            string path, string[] 
            sheetNames = null)
        {
            bool ret = false;

            //Сохранить конкретные страницы чертежа

            swExportDataSheetsToExport_e SheetSavingMode =
                swExportDataSheetsToExport_e.swExportData_ExportSpecifiedSheets;
            if (sheetNames is null) SheetSavingMode = 
                    swExportDataSheetsToExport_e.swExportData_ExportAllSheets;

            if (swModel.GetType() != (int)swDocumentTypes_e.swDocDRAWING)
                throw new System.TypeLoadException("Модель не является чертежом");

            DrawingDoc swDrawing = (DrawingDoc)swModel;
            int _errors = 0, _warning = 0;

            //Параметры сохранения PDF
            ExportPdfData ExportPdfParams = 
                SwAPI.swApp.GetExportFileData((int)swExportDataFileType_e.swExportPdfData);
            ExportPdfParams.SetSheets((int)SheetSavingMode, sheetNames);

            ret = swModel.Extension.SaveAs(path, 0, 0, ExportPdfParams, ref _errors, ref _warning);
            Debug.Print("ExportPDF status {0}", ret ? "Success" : "Failed");

            return ret;
        }
    }

    /// <summary>
    /// Методы сборки
    /// </summary>
    public static class AsmDocProxy
    {
        /// <summary>
        /// Получить компоненты сборки
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="TopLevelOnly">Только верхнего уровня</param>
        /// <returns></returns>
        public static Component2[] GetComponents(ModelDoc2 swModel, bool TopLevelOnly = true)
        {
            Component2[] retArr = null;

            if(swModel.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
            {
                object[] tempArr;
                AssemblyDoc swAsm = swModel as AssemblyDoc;

                tempArr = swAsm.GetComponents(TopLevelOnly);

                retArr = ServiceCl.ObjArrConverter<Component2>(tempArr);
            }

            return retArr;
        }

        /// <summary>
        /// Габариты сборки
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        public static Box? GetBox(ModelDoc2 swModel)
        {
            Box? ret = null;

            if (swModel != null)
            {

                var points = (swModel as AssemblyDoc).GetBox(0);

                if (!(points is DBNull))
                    ret = new Box(points);
            }

            return ret;
        }

        /// <summary>
        /// Get selected components from assembly
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="SwSelectionMark"></param>
        /// <returns></returns>
        public static Component2[] GetSelectedComponent(ModelDoc2 swModel, int SwSelectionMark = 0)
        {
            Component2[] ret = null;
            var tempList = new List<Component2>();

            if(swModel is AssemblyDoc swAsm)
            {
                SelectionMgr swSelMan = swModel.SelectionManager;
                int selectCounter = swSelMan.GetSelectedObjectCount2(SwSelectionMark);
                Debug.WriteLine($"GetSelectedComponent2 count = {selectCounter}");

                for(; selectCounter >= 1; --selectCounter)
                {
                    var comp = (Component2) swSelMan.GetSelectedObjectsComponent2(selectCounter);
                    Debug.WriteLine($"Selected comp name = {(comp as Component2).Name}");
                    tempList.Add(comp);
                    //swSelectionReferenceTypes_e seltype = (swSelectionReferenceTypes_e)
                    //    swSelMan.GetSelectedObjectType2(selectCounter);
                }

                ret = tempList.ToArray();
            }     

            return ret;
        }



    }

    public static class ComponentProxy
    {
        /// <summary>
        /// Get modelDoc2 from component2
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns></returns>
        public static ModelDoc2 GetModelDoc2(Component2 swComp)
        {
            return swComp.GetModelDoc2() as ModelDoc2;
        }

        /// <summary>
        /// Получить родительский компонент
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns>Null if not exist</returns>
        public static Component2 GetParent(Component2 swComp)
        {
            //Метод Component.GetParent() не работает
            //Прикручен костыль

            Component2 retComp = null;

            if (!swComp.IsRoot())
            {
                ModelDoc2 swModel = swComp.GetModelDoc2();
                Configuration conf = swModel.ConfigurationManager
                    .ActiveConfiguration;

                retComp = conf.GetRootComponent3(false);
            }
            return retComp;
        }

        /// <summary>
        /// Get root assembly component
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns>Assembly root (not null)</returns>
        public static Component2 GetRoot(Component2 swComp)
        {
            Component2 retComp = swComp;
            int EmergencyBreak = 10;
            while (!retComp.IsRoot())
            {
                retComp = GetParent(retComp);
                if (EmergencyBreak-- <= 0)
                    break;
            }

            return retComp;
        }

        /// <summary>
        /// Имя зависимой конфигурации компонента сборки
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns></returns>
        public static string RefConfigName(Component2 swComp)
        {
            return swComp.ReferencedConfiguration;
        }

        /// <summary>
        /// Статус компонента сборки
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns></returns>
        public static swComponentSuppressionState_e GetSuppressionState(Component2 swComp)
        {
            swComponentSuppressionState_e ret = default;
            ret = (swComponentSuppressionState_e)swComp.GetSuppression();
            return ret;
        }

        /// <summary>
        /// Получить дочерние компоненты компонента
        /// Для подсчёта использовать GetChildrenCount
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns>Пустой массив если отсутствуют</returns>
        public static Component2[] GetChildren(Component2 swComp)
        {
            Component2[] retArr = null;
            Component2 testcomp;
            object[] temp = swComp.GetChildren();
            retArr = ServiceCl.ObjArrConverter<Component2>(temp);
            return retArr;
        }

        /// <summary>
        /// Габаритный размер
        /// </summary>
        /// <param name="swComp"></param>
        /// <param name="includeSketches"></param>
        /// <returns></returns>
        public static Box? GetBox(Component2 swComp, bool includeSketches = false)
        {
            Box? ret = null;
            if (swComp != null)
            {
                var points = swComp.GetBox(
                    IncludeRefPlanes: false, 
                    IncludeSketches: includeSketches);

                if(!(points is DBNull))
                    ret = new Box(points);
            }

            return ret;
        }
    }
}

