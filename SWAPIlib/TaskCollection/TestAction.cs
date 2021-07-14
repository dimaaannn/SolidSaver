using Ninject;
using SWAPIlib.Property.ModelProperty;
using SWAPIlib.Table;
using SWAPIlib.TaskUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskCollection
{
    public class TestAction
    {
        private readonly ITaskUnitFactory taskUnitFactory;
        private readonly ITaskServices taskServices;

        public TestAction(ITaskUnitFactory taskUnitFactory, ITaskServices taskServices)
        {
            this.taskUnitFactory = taskUnitFactory;
            this.taskServices = taskServices;
        }


        public const string SAVING_FILE_NAME_KEY = "PartName";
        public const string NOMINATION_KEY       = "Обозначение";
        public const string DESIGNATION_KEY      = "Наименование";


        public IActionUnit GetGlobalInfoUnit()
        {
            IActionList newActionList = taskUnitFactory.CreateActionList();

            AddGlobalModelOptions(newActionList);
            AddUserProperty(newActionList, NOMINATION_KEY);
            AddUserProperty(newActionList, DESIGNATION_KEY);

            IActionUnit ret = taskServices.CreateActionUnit(newActionList);
            return ret;
        }

        public void AddGlobalModelOptions(IActionList targetList)
        {
            ICellFactoryBuilder factoryBuilder = taskUnitFactory.CreateCellFactoryBuilder();
            ICell textBuilderSettingsGetFileName = Table.Prop.TextBuilderCell.BuildSettings(
                refTable =>
                {
                    string filePath =
                        refTable.GetCell(Table.ModelEntities.FileName.ToString()).Text;
                    return System.IO.Path.GetFileName(filePath);
                });

            ITableAction createTextBuilderSettings = factoryBuilder
                .FromReference(textBuilderSettingsGetFileName)
                .WithKey(Table.Prop.TextBuilderCell.SETTINGS_KEY)
                .Build();



            var test = factoryBuilder.From(Table.ModelPropertyNames.FileName);
            var test2 = test.Build();
            targetList.Add(test2);
                
            factoryBuilder.From(Table.ModelPropertyNames.ActiveConfigName)
                .Build().AddTo(targetList);
            factoryBuilder.From(Table.ModelPropertyNames.TextBuilder)
                .WithKey(SAVING_FILE_NAME_KEY)
                .WithSettings(createTextBuilderSettings)
                .Build()
                .AddTo(targetList);
        }


        public void AddUserProperty(IActionList targetList, string userPropertyName)
        {
            ICellFactoryBuilder factoryBuilder = taskUnitFactory.CreateCellFactoryBuilder();
            if (userPropertyName is null)
            {
                throw new ArgumentNullException(nameof(userPropertyName));
            }

            var settings = factoryBuilder
                .From(userPropertyName)
                .WithKey(Table.ModelEntities.UserPropertyName)
                .Build();

            factoryBuilder.From(Table.ModelPropertyNames.UserProperty)
                .WithKey(userPropertyName)
                .WithSettings(settings)
                .Build()
                .AddTo(targetList);
        }

    }
}
