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

    }

    public class AppModelProp : IAppModelProp
    {

    }
}
