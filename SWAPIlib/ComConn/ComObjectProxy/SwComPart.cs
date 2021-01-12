using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using SolidWorks.Interop.sldworks;
using SwConst;

namespace SWAPIlib.ComConn.Proxy
{
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

            for (int i = 0; i < featCount; ++i)
            {
                fArray[i] = (Feature)f[i];
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
            if (body != null)
            {
                Feature[] featList = GetFeatures(body);
                foreach (Feature feat in featList)
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
            if (swModel.GetType() == (int)swDocumentTypes_e.swDocPART)
            {
                //Найти тело с свойством IsSheetMetal
                Body2[] bodyList;
                bodyList = GetBodies(swModel);

                foreach (Body2 body in bodyList)
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
            if (bodyArr.Count() > 0)
            {
                foreach (Body2 body in bodyArr)
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
}
