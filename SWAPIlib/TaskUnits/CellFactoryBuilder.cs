using NLog;
using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits
{
    public interface ICellFactoryBuilder
    {

    }


    public class CellFactoryBuilder
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ICellFactoryTemplate cellFactoryTemplate;
         
        public CellFactoryBuilder(ICellFactoryTemplate cellFactoryTemplate)
        {
            this.cellFactoryTemplate = cellFactoryTemplate;
            logger.Debug("Instanced with {factoryTemplate}", cellFactoryTemplate);
        }

        public CellFactoryBuilderPreform FromTemplate()
        {
            throw null;
        }

        public CellFactoryBuilderPreform New(string text)
        {
            throw null;
        }

        protected ICellFactory Build(CellFactoryBuilderPreform preform)
        {
            
        }

        public class CellFactoryBuilderPreform
        {
            private readonly CellFactoryBuilder parentBuilder;

            private CellFactoryBuilderPreform(CellFactoryBuilder parentBuilder)
            {
                this.parentBuilder = parentBuilder;
            }

            private ICell CellReference { get; set; }
            public string Key { get; set; }
            public bool OverrideKey { get; set; }
            public 

            public CellFactoryBuilderPreform WithKey(string key)
            {
                this.Key = key;
                return this;
            }
        }
    }
///Создать расширение для IFactoryProvider
///FactoryBuilder.Create/.Add(someFactory).WithSettings(someSettingsTable).WithKey(key).Build()
}
