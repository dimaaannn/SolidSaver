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
        public SaveSheetMetalTask(ITargetTable<ModelDoc2> refTable) : base(refTable)
        {
            
        }
        public override string Name => "SaveSheetMetal";

        public override string Info => "Сохранить листовую развёртку материала";

        public string TempText { get; set; }
        public string Path { get => TempText ?? Text; 
            set { OnPropertyChanged(); TempText = value; } }

        public override bool Update()
        {
            Text = RefTable.GetCell(ModelEntities.FileName.ToString())?.Text;
            TempText = null;
            return Text != null;
        }

        public bool WriteValue()
        {
            bool ret = false;
            string directory = GetDirectory();
            string fileName = GetFileName();
            string extension = ".dxf";
            string combinedPath = System.IO.Path.Combine(directory, fileName + extension);

            bool isDirectoryExist = System.IO.Directory.Exists(directory);
            if (!isDirectoryExist)
                isDirectoryExist = CreateDirectory(directory);

            if (isDirectoryExist)
            {
                ret = SaveDxf(model: (ModelDoc2) Target, path: combinedPath);
            }

            return ret;
        }

        bool SaveDxf(ModelDoc2 model, string path)
        {
            bool ret = false;
            ret = SWAPIlib.ComConn.Proxy.PartDocProxy.ExportDXF(model, path);
            return ret;
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
            if(obj is ModelDoc2 model)
            {
                ret = SWAPIlib.ComConn.Proxy.PartDocProxy.IsSheetMetal(model);
            }
            return ret;
        }

    }
}
