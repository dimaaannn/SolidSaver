

using SolidApp;
using SolidWorks.Interop.sldworks;
//using SwConst;
using SolidWorks.Interop.swconst;
using System;
using System.Diagnostics;


namespace SolidDrawing
{
    class SwDrawing
    {
        private static swDocumentTypes_e DocType = swDocumentTypes_e.swDocDRAWING;
        private ISldWorks _swApp;
        private ModelDoc2 _drawModel;
        private DrawingDoc _swDraw;
        public string[] SheetNames => _swDraw.GetSheetNames();
        public string FilePath => _drawModel.GetPathName();
        public string FolderPath
        {
            get
            {
                string result = this.FilePath;
                int lastSlash = result.LastIndexOf("\\");
                return result.Substring(0, lastSlash + 1);
            }
        }
        public string FileName
        {
            get
            {
                string result = this.FilePath;
                int lastSlash = result.LastIndexOf("\\");
                return result.Substring(lastSlash);
            }
        }
        public string FileNameWhithoutExt
        {
            get
            {
                string result = this.FilePath;
                int lastSlash = result.LastIndexOf("\\");
                int lastPoint = result.LastIndexOf(".");
                return result.Substring(lastSlash + 1, lastPoint - lastSlash - 1);
            }
        }
        public string[] SheetsToSaving { get; set; }

        public SwDrawing(ModelDoc2 drawModel) //Инициализация из класса модели
        {
            _drawModel = drawModel;
            if (_drawModel.GetType() == (int)DocType)
            {
                _swApp = SwProcess.swApp;
                _swDraw = (DrawingDoc)_drawModel;
                Debug.Print("SwDrawing initialised");
            }
            else throw new System.TypeLoadException("Модель не является чертежом");
        }

        public bool SavePreview(string bmpPath, int height = 1000, int width = 1000)  //Сохранить превью как BMP файл
        {
            return _drawModel.SaveBMP(bmpPath, height, width);
        }

        public bool SavePdf(string filename = "", string[] sheets = null) //Сохранить листы SheetNames как PDF
        {
            var swExp = new SwDrawExporter(_swApp, _drawModel);
            
            if (sheets is null)
            {   //Проверяется существование массивов в классе и параметре. Если отсутствуют - используются все листы
                if (SheetsToSaving is null) SheetsToSaving = this.SheetNames;
            }
            else SheetsToSaving = sheets;
            
            if(filename == "")
            {
                filename = string.Concat(filename, this.FolderPath, this.FileNameWhithoutExt, ".pdf");
            } //Set filename as Title
            swExp.Path = filename;

            swExp.SheetNames = SheetsToSaving;
            return swExp.ExportPdf();
        }
    }

    class SwDrawExporter
    {
        private readonly ExportPdfData _swExport;
        private readonly ModelDoc2 _swModel;
        public string[] SheetNames { get; set; }
        public string Path { get; set; }

        public int errors = 0;
        public int output = 0;

        public SwDrawExporter(ISldWorks swApp, ModelDoc2 swModel)
        {
            if (!(swModel is null))
            {
                _swExport = swApp.GetExportFileData((int)swExportDataFileType_e.swExportPdfData);
                _swModel = swModel;
                Debug.Print("SwExporter: Class instanced");
            }
            else throw new TypeLoadException("SwExporter: Приложение не инициализированно");
        } //Class constructor
        
        public bool ExportPdf(string filename = "") 
        {
            Debug.Print("ExportPdf: begin");
            
            if (filename == "") filename = Path;

            if (IsFileLocked(filename))
            {
                Debug.Print("ExportPdf: File is locked");
                throw new System.IO.FileLoadException("ExportPdf: File is locked");
            }
            
            _swExport.SetSheets((int)swExportDataSheetsToExport_e.swExportData_ExportSpecifiedSheets, 
                SheetNames);

            bool ret = _swModel.Extension.SaveAs(Path, 0, 0, _swExport, ref errors, ref output);
            Debug.Print($"Output = {output}, Errors = {errors}");
            if (errors != 0)
            {
                Debug.Print("ExportPdf: Error! Abort.");
                ret = false;
            }
            return ret;
        } // Save file as

        public bool IsFileLocked(string filename = "")
        {   
            if (filename == "") filename = Path;
            bool Locked = false;
            try
            {
                System.IO.FileStream fs =
                    System.IO.File.Open(filename, System.IO.FileMode.Open,
                    System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
                fs.Close();
            }
            catch (System.IO.IOException ex)
            {
                Locked = true;
            }

            if (! System.IO.File.Exists(filename)) Locked = false;

            return Locked;
        } //Проверяет заблокирован ли файл
    }
}
