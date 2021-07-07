using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits.Actions
{

    public class ActionUnit
    {
        public string Name { get; set; }



    }


    public class SaveSheetMetalList
    {
        public const string SUBFOLDER_NAME = "Развёртки";
        public const string SUBFOLDER_KEY = "dxfFolder";

        public ITable GlobalSettings { get; protected set; }

        public List<TableLog> Proceed(ITable table)
        {
            var ret = new List<TableLog>();

            ret.Add(FolderSettings().Proceed(table));
            ret.Add(ModelOptions().Proceed(table));
            ret.Add(DxfFolderPathBuilder().Proceed(table));

            return ret;
        }

        public ActionList GetList()
        {
            var ret = new ActionList();

            ret.AddRange(FolderSettings());
            ret.AddRange(ModelOptions());


            return ret;
        }

        public static ActionList DxfFolderPathBuilder(string subFolderKey = SUBFOLDER_KEY)
        {
            ITableAction textBuilderSettings = CellFactoryBuilder
                .Create()
                .Reference(
                    Table.Prop.TextBuilderCell.BuildSettings( reftable =>
                    {
                        string workFolder = reftable.GetCell(ModelEntities.Folder.ToString()).ToString();
                        string filePath = reftable.GetCell(ModelEntities.FileName.ToString()).ToString();
                        string partFileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                        string subFolder = reftable.GetCell(subFolderKey).ToString();
                        string snomination = reftable.GetCell("Наименование").ToString();
                        string sdesignation = reftable.GetCell("Обозначение").ToString();

                        string savingFileName = $"{snomination}-{sdesignation}_{partFileName}";

                        //Replace invalid chars in fileName - create new interface
                        savingFileName = string.Join("_",
                            savingFileName.Split(
                                System.IO.Path.GetInvalidFileNameChars()));

                        return System.IO.Path.Combine(workFolder, subFolder, savingFileName);
                    }   
                ))
                .WithKey(Table.Prop.TextBuilderCell.SETTINGS_KEY)
                .Build();

            var textBuilderAction =  CellFactoryBuilder
                .Create(ModelPropertyNames.TextBuilder)
                .WithSettings(textBuilderSettings)
                .Build();

            return ActionList.DefaultBuilder(builder => builder.Add(textBuilderAction));
        }

        public static ActionList ModelOptions()
        {
            return ActionList.DefaultBuilder(builder =>
            {
                CellFactoryBuilder
                    .Create(ModelPropertyNames.FileName)
                    .Build()
                    .AddTo(builder);
                CellFactoryBuilder
                    .Create(ModelPropertyNames.ActiveConfigName)
                    .Build()
                    .AddTo(builder);
            });

        }

        public static ActionList FolderSettings(
            string subFolderKey = SUBFOLDER_KEY, 
            string subFolderName = SUBFOLDER_NAME)
        {
            return ActionList.DefaultBuilder(builder =>
            {
                CellFactoryBuilder
                    .Create(ModelPropertyNames.WorkFolder)
                    .Build()
                    .AddTo(builder);
                CellFactoryBuilder
                    .Create(subFolderName)
                    .WithKey(subFolderKey)
                    .Build()
                    .AddTo(builder);
            });
        }
    }
}
