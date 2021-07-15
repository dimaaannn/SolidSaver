using Ninject.Extensions.Factory;
using SWAPIlib.BaseTypes;
using SWAPIlib.Table;
using SWAPIlib.TaskUnits.Actions;
using SWAPIlib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits
{
    public class TaskUnitsNinjectBinding : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<ICellFactoryTemplate>()
                .To<CellFactoryTemplate>().InSingletonScope();
            Bind<ICellFactoryProvider>()
                .To<CellFactoryProvider>().InTransientScope();
            Bind<CellProviderBuilder>() //Непосредственный конструктор внутри фабрики
                .ToSelf().InTransientScope();
            Bind<CellFactoryBuilder>() 
                .ToSelf().InTransientScope();
            Bind<CellActionFactory>()
                .ToSelf().InTransientScope();
            Bind<ITable>().To<TableList>().InTransientScope();
            Bind<IActionList>().To<ActionList>().InTransientScope();

            Bind<CellFactoryBuilderDI>().ToSelf().InTransientScope();
            Bind<ICellFactoryBuilder>().To<CellFactoryBuilderDI>().InTransientScope();

            Bind<ModelActions>().ToSelf().InSingletonScope();


            Bind<ITaskUnitFactory>().ToFactory();



        }
    }


    public interface ITaskUnitFactory
    {
        //IExtendedTable CreateExtendedTable(ITarget2 target);
        /// <summary>
        /// конструктор обработчика внутри фабрики
        /// </summary>
        /// <returns></returns>
        CellProviderBuilder CreateCellProviderBuilder();
        /// <summary>
        /// Фабрика создающая ячейки при обработке. 
        /// Результат CellFactoryBuilder
        /// </summary>
        /// <returns></returns>
        CellActionFactory CreateCellActionFactory(ICellFactoryProvider cellProvider);
        ICellFactoryBuilder CreateCellFactoryBuilder();
        IPartWrapperFactory CreatePartWrapperFactory();
        IExtendedTable CreateExtendedTable();
        ITable CreateTable();
        IActionList CreateActionList();
    }

}
