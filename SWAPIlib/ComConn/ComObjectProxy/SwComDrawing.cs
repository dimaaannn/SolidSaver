using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using SolidWorks.Interop.sldworks;
using SwConst;

namespace SWAPIlib.ComConn.Proxy
{
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
                throw new TypeLoadException("Модель не является чертежом");

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

}
