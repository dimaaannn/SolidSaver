using SolidWorks.Interop.sldworks;
using SWAPIlib.Table;
using SWAPIlib.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Task.SWTask
{


    public class SaveSheetMetalTask : PropertyCellBase, IWritableCell, IPathOption
    {
        private string tempText;

        public SaveSheetMetalTask(ITargetTable refTable) : base(refTable)
        {

        }
        public override string Name => "SaveSheetMetal";

        public override string Info => "Сохранить листовую развёртку материала";

        public string TempText { get => tempText; set { tempText = value; OnPropertyChanged(); } }
        public string Path
        {
            get => TempText ?? Text;
            set { OnPropertyChanged(); TempText = value; }
        }

        public override bool Update()
        {
            TempText = GetSettings(ModelEntities.FilePath.ToString())?.Text;
            return TempText != null;
        }

        public bool WriteValue()
        {
            bool ret = false;
            string directory = GetDirectory();
            string fileName = GetFileName();
            if (string.IsNullOrEmpty(fileName))
                fileName = "NoName";
            string extension = ".dxf";
            string combinedPath = System.IO.Path.Combine(directory, fileName + extension);

            bool isDirectoryExist = System.IO.Directory.Exists(directory);
            if (!isDirectoryExist)
                isDirectoryExist = CreateDirectory(directory);

            if (isDirectoryExist)
            {
                ret = SaveDxfFromAssembly(model: (ModelDoc2)Target, path: combinedPath);
                if (ret)
                {
                    Text = combinedPath;
                    TempText = null;
                    ret = IsFileExist(combinedPath);
                }
            }

            return ret;
        }

        static bool SaveDxfFromAssembly(ModelDoc2 model, string path)
        {
            bool ret = false;
            string modelName = model.GetTitle();
            int errors = 0;
            //Открыть деталь в отдельном окне
            ModelDoc2 tempModel = SWAPIlib.ComConn.SwAppControl.swApp.ActivateDoc2(modelName, false, ref errors);
            bool status = tempModel.EditRebuild3();
            //Сохранить развёртку
            ret = SWAPIlib.ComConn.Proxy.PartDocProxy.ExportDXF(tempModel, path);
            //закрыть окно детали
            SWAPIlib.ComConn.SwAppControl.swApp.CloseDoc(modelName);

            return ret;
        }

        static bool IsFileExist(string path)
        {

            var fileinfo = new System.IO.FileInfo(path);
            var length = fileinfo?.Length;
            return length > 10;

        }

        string GetDirectory() => System.IO.Path.GetDirectoryName(Path);
        string GetFileName() => System.IO.Path.GetFileNameWithoutExtension(Path);
        bool CreateDirectory(string path)
        {
            bool ret = true;
            try
            {
                System.IO.Directory.CreateDirectory(path);
            }
            catch (System.IO.IOException)
            {
                ret = false;
            }
            return ret;
        }

        public static bool CheckTargetType(ITable refTable, ITable settings)
        {
            bool ret = false;
            var obj = GetTargetObject(refTable: refTable, settings: settings);
            if (obj is ModelDoc2 model)
            {
                ret = SWAPIlib.ComConn.Proxy.PartDocProxy.IsSheetMetal(model);
            }
            return ret;
        }

    }
}
