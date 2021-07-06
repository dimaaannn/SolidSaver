using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits.Actions
{
    public class SaveSheetMetalList
    {
        public const string SUBFOLDER_NAME = "Развёртки";
        public const string SUBFOLDER_KEY = "dxfFolder";


        public ActionList GetList()
        {
            var ret = new ActionList();

            ret.AddRange(GetFolderSettings());
            ret.AddRange(GetModelOptions());


            return ret;
        }

        public static ITableAction DxfFolderPathBuilder()
        {
            return CellFactoryBuilder.Create()
                .AddSettings
        }

        public static ITableAction[] GetModelOptions()
        {
            var ret = new List<ITableAction>();

            ret.Add(CellFactoryBuilder.Create()
                .New(Table.ModelPropertyNames.FileName).Build());
            ret.Add(CellFactoryBuilder.Create()
                .New(Table.ModelPropertyNames.ActiveConfigName).Build());

            return ret.ToArray();
        }

        public static ITableAction[] GetFolderSettings(string subfolderName = SUBFOLDER_NAME)
        {
            var ret = new List<ITableAction>();

            ret.Add( 
                CellFactoryBuilder.Create()
                    .New(Table.ModelPropertyNames.WorkFolder).Build());
            ret.Add(
                CellFactoryBuilder.Create().New(subfolderName).WithKey(SUBFOLDER_KEY).Build());
            return ret.ToArray();
        }
    }
}
