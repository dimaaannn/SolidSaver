using SWAPIlib.Table;
using System;
using System.Collections.Generic;


namespace SWAPIlib.TaskUnits2
{

    public interface ITaskFactory2
    {
        //ITaskFactorySettings TaskTemplate { get; }

    }


    public delegate bool ProviderInteractor(ITableProvider tableProvider);
    public delegate ICell CellGetterDelegate(ITableProvider tableProvider);

    public interface ITableProviderChecker
    {
        ProviderInteractor Check { get; }

    }

    public interface ICellProviderSettings : ITableProviderChecker
    {
        string Name { get; set; }
        string Key { get; set; }
        bool OverrideKey { get; set; }

        CellGetterDelegate GetCell { get; }
    }

    public interface ICellFactory
    {
        
    }
}
