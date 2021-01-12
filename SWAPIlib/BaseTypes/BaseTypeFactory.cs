using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SwConst;

namespace SWAPIlib.BaseTypes
{
    /// <summary>
    /// Фабрика типов модели
    /// </summary>
    public static class ModelClassFactory
    {
        public static AppModel GetModel(ModelDoc2 swModel)
        {
            AppDocType docType = PartTypeChecker.GetSWType(swModel);
            AppModel ret = null;
            switch (docType)
            {
                case AppDocType.swNONE:
                    break;
                case AppDocType.swASM:
                    ret = new AppAssembly(swModel);
                    break;
                default:
                    ret = new AppModel(swModel);
                    break;
            }
            return ret;
        }

        public static AppModel ActiveDoc
        {
            get => GetModel(SWAPIlib.ComConn.SwAppControl.ActiveModel);
        }
    }

}
