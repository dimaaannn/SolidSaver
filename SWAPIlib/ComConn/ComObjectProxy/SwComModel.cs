using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SolidWorks.Interop.sldworks;
using SwConst;


namespace SWAPIlib.ComConn.Proxy
{
    /// <summary>
    /// Основные операции ModelDoc2
    /// </summary>
    public static class ModelProxy
    {
        public static string GetName(ModelDoc2 swModel)
        {
            return swModel.GetTitle();
        }
        /// <summary>
        /// Gets the full path name for this document, including the file name. 
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        public static string GetPathName(ModelDoc2 swModel)
        {
            return swModel?.GetPathName();
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
        /// Получить параметр активной конфигурации
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="configName">Имя конфигурации</param>
        /// <param name="fieldName">Имя параметра</param>
        /// <returns></returns>
        public static string GetConfParam(ModelDoc2 swModel, string fieldName)
        {
            string ret = null;
            if (IsPartOrAsm(swModel))
            {
                ret = swModel.CustomInfo[fieldName];
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

        /// <summary>
        /// Установить значение параметра активной конфигурации
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="configName">Имя конфигурации</param>
        /// <param name="fieldName">Имя параметра</param>
        /// <param name="fieldVal">Значение параметра</param>
        /// <returns>Статус операции</returns>"
        public static bool SetConfParam(ModelDoc2 swModel, string fieldName, string fieldVal)
        {
            bool ret = false;
            if (IsPartOrAsm(swModel))
            {
                swModel.CustomInfo[fieldName] = fieldVal;

                if (swModel.CustomInfo[fieldName] == fieldVal)
                    ret = true;
            }
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

            if (swModel.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
            {
                object[] tempArr;
                AssemblyDoc swAsm = swModel as AssemblyDoc;

                tempArr = swAsm.GetComponents(TopLevelOnly);

                retArr = ServiceCl.ObjArrConverter<Component2>(tempArr);
            }

            return retArr;
        }

        /// <summary>
        /// Количество компонентов в сборке
        /// </summary>
        /// <param name="swModel"></param>
        /// <param name="TopLevelOnly">Только верхнего уровня</param>
        /// <returns></returns>
        public static int GetComponentCount(ModelDoc2 swModel, bool TopLevelOnly = true)
        {
            int ret = 0;
            if (swModel.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
            {
                AssemblyDoc swAsm = swModel as AssemblyDoc;
                ret = swAsm.GetComponentCount(TopLevelOnly);
            }
            return ret;
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

            if (swModel is AssemblyDoc swAsm)
            {
                SelectionMgr swSelMan = swModel.SelectionManager;
                int selectCounter = swSelMan.GetSelectedObjectCount2(SwSelectionMark);
                Debug.WriteLine($"GetSelectedComponent2 count = {selectCounter}");

                for (; selectCounter >= 1; --selectCounter)
                {
                    var comp = (Component2)swSelMan.GetSelectedObjectsComponent2(selectCounter);
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
}
