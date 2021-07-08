using Ninject;
using SWAPIlib.BaseTypes;
using SWAPIlib.Table;
using SWAPIlib.TaskUnits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskCollection
{


    public interface IActionUnit
    {
        IActionUnitResult Proceed(IExtendedTable table);
    }

    public interface IActionUnitResult
    {
    }



    /// <summary>
    /// Temp class for run test
    /// </summary>
    public class TableProvider //TODO refactor provider
    {
        public ITableCollection UserSelectedModels()
        {
            var tableCollection = Initialiser.kernel.Get<ITableCollection>();
            var partProvider = Initialiser.kernel.Get<SelectedModelProvider>();
            tableCollection.GetFromProvider(partProvider);
            return tableCollection;
        }
    }


}
