using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SwConst;

namespace SWAPIlib
{
    
    public static class PartTypeChecker
    {

        /// <summary>
        /// Определить тип модели
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AppDocType GetSWType(ModelDoc2 model)
        {
            AppDocType ret = AppDocType.swNONE;
            swDocumentTypes_e swType;
            if (model is ModelDoc2 swModel)
            {
                swType = (swDocumentTypes_e)swModel.GetType();
                switch (swType)
                {
                    case swDocumentTypes_e.swDocASSEMBLY:
                        ret = AppDocType.swASM;
                        break;
                    case swDocumentTypes_e.swDocPART:
                        ret = AppDocType.swPART;
                        break;
                    case swDocumentTypes_e.swDocDRAWING:
                        ret = AppDocType.swDRAWING;
                        break;
                }
            }

            return ret;
        }


        /// <summary>
        /// Получить статус отображения компонента
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static AppSuppressionState GetAppSuppressionState(Component2 component)
        {
            AppSuppressionState ret = AppSuppressionState.None;
            if(component != null)
            {
                var compType = ComponentProxy.GetSuppressionState(component);
                ret = ConvertSuppressionState(compType);
            }
            return ret;

        }

        public static AppSuppressionState ConvertSuppressionState(swComponentSuppressionState_e compType)
        {
            AppSuppressionState ret = AppSuppressionState.None;
            switch (compType)
            {
                case swComponentSuppressionState_e.swComponentSuppressed:
                    ret = AppSuppressionState.Suppressed;
                    break;
                case swComponentSuppressionState_e.swComponentLightweight:
                    ret = AppSuppressionState.FullyLightweight;
                    break;
                case swComponentSuppressionState_e.swComponentFullyResolved:
                    ret = AppSuppressionState.FullyResolved;
                    break;
                case swComponentSuppressionState_e.swComponentResolved:
                    ret = AppSuppressionState.Resolved;
                    break;
                case swComponentSuppressionState_e.swComponentFullyLightweight:
                    ret = AppSuppressionState.FullyLightweight;
                    break;
            }
            return ret;
        }

        public static AppCompVisibility Visibility(int visState)
        {
            AppCompVisibility ret;
            switch (visState)
            {
                case 0:
                    ret = AppCompVisibility.Hidden;
                    break;
                case 1:
                    ret = AppCompVisibility.Visible;
                    break;
                default:
                    ret = AppCompVisibility.Visible;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Является ли модель сборкой
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public static bool IsAsm(AppModel appModel)
        {
            if (appModel.DocType == AppDocType.swASM)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Является ли модель деталью
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public static bool IsPart(AppModel appModel)
        {
            if (appModel.DocType == AppDocType.swPART)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Содержит листовые тела
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public static bool IsSheet(AppModel appModel)
        {
            return PartDocProxy.IsSheetMetal(appModel.SwModel);
        }

        /// <summary>
        /// Существование черетежа с тем же именем
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public static bool IsHaveDrawing(AppModel appModel)
        {
            string partName = System.IO.Path.GetFileNameWithoutExtension(appModel.Path);
            string folder = System.IO.Path.GetDirectoryName(appModel.Path);

            string drawingExtension = ".SLDDRW";
            string DrawPath = $"{folder}\\{partName}{drawingExtension}";

            return System.IO.File.Exists(DrawPath);
        }


    }


}
