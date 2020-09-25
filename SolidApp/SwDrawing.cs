using SldWorks;
using SwConst;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidDrawing
{
    class SwDrawing
    {
        private static swDocumentTypes_e DocType = swDocumentTypes_e.swDocDRAWING;
        private ModelDoc2 DrawModel;
        private DrawingDoc DrawDoc;

        public SwDrawing(SldWorks.ModelDoc2 drawModel)
        {
            DrawModel = drawModel;
            if (DrawModel.GetType() == (int)DocType)
            {
                DrawDoc = (DrawingDoc)DrawModel;
                Debug.Print("SwDrawing initialised");
            }
            else throw new System.TypeLoadException("Модель не является чертежом");
        }

        public string[] GetSheetNames()
        {
            string[] testArray = new string[] { "aa", "bb" };
            return testArray;
        }

    }
}
