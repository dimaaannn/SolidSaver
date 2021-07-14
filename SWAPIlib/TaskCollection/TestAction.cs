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
        private StandardKernel dIkernel;

        public TestAction(StandardKernel diKernel)
        {
            dIkernel = diKernel;
        }

        CellFactoryBuilderDI GetFactory()
        {
            return dIkernel.Get<CellFactoryBuilderDI>();
        }
        public List<IActionList> GetActions()
        {
            List<IActionList> ret = new List<IActionList>();

            var factoryBuilder = GetFactory();

            
            

            return ret;
        }


        public IActionUnit GetGlobalInfoUnit()
        {
            ITaskServices taskServices = dIkernel.Get<ITaskServices>();
            IActionList defaultModelInfoActions = GlobalModelOptions();
            IActionUnit ret = taskServices.CreateActionUnit(defaultModelInfoActions);
            return ret;
        }

        public IActionList GlobalModelOptions(string savingFileNameKey = "PartName")
        {

            IActionList ret = dIkernel.Get<IActionList>();
            var factoryBuilder = GetFactory();

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
            ret.Add(test2);
                
            factoryBuilder.From(Table.ModelPropertyNames.ActiveConfigName)
                .Build().AddTo(ret);
            factoryBuilder.From(Table.ModelPropertyNames.TextBuilder)
                .WithKey(savingFileNameKey)
                .WithSettings(createTextBuilderSettings)
                .Build()
                .AddTo(ret);

            return ret;
        }
    }
}
