
using SolidWorks.Interop.sldworks;
using SwConst;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        private DrawingDoc _swDraw;
        public string[] SheetNames
        {
            get { return _swDraw.GetSheetNames(); }
        }

        public SwDrawing(ModelDoc2 drawModel) //Инициализация из класса модели
        {
            DrawModel = drawModel;
            if (DrawModel.GetType() == (int)DocType)
            {
                _swDraw = (DrawingDoc)DrawModel;
                Debug.Print("SwDrawing initialised");
            }
            else throw new System.TypeLoadException("Модель не является чертежом");
        }

        public SwDrawing(DrawingDoc drawModel) //Инициализация из класса чертежа (НЕ ТЕСТИРОВАНО)
        {
            if (!(drawModel is null))
            {
                _swDraw = drawModel;
                Debug.Print("SwDrawing initialised");
            }
            else throw new System.ArgumentNullException("Документ чертежа не инициализирован");
        }

        public bool SavePreview(string bmpPath, int height = 1000, int width = 1000)  //Сохранить превью как BMP файл
        {
            return DrawModel.SaveBMP(bmpPath, height, width);
        }



    }

    class SwExporter
    {
        private ISldWorks _swApp;
        private ExportPdfData _swExport;
        private ModelDoc2 _swModel;

        public SwExporter(ISldWorks swApp, ModelDoc2 swModel)
        {
            if (!(swApp is null))
            { 
                _swApp = swApp;

                _swExport = swApp.GetExportFileData((int)swExportDataFileType_e.swExportPdfData);
                _swModel = swModel;
                Debug.Print("SwExporter: Class created");
            }
            else throw new TypeLoadException("SwExporter: Приложение не инициализированно");
        }

        public bool ExportPdf(string Path)
        {
            int errors = 0;
            int output = 0;
            var sheetNames = new string[] { "Лист1", "Лист2"};
            string FileName = "\\\\sergeant\\Техотдел\\Технологический - Общие документы\\Общая\\Красиков\\VBA\\SolidWorks\\Тестовая сборка\\Test.pdf";
            _swExport.SetSheets((int)swExportDataSheetsToExport_e.swExportData_ExportSpecifiedSheets, sheetNames);
            Boolean ret = false;

            ret = _swModel.Extension.SaveAs(FileName, 0, 0, _swExport, ref errors, ref output);
            Console.WriteLine($"Output = {output}\nErrors = {errors}");
            return ret;
        }
    }
}
