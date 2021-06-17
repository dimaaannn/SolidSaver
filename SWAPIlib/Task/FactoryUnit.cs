using SWAPIlib.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Task
{
    public class FactoryUnit
    {
        List<ICellFactory> GetFactoryList()
        {
            var ret = new List<ICellFactory>();

            var activeModel = SWAPIlib.ComConn.SwAppControl.ActiveModel;
            var modelTable = new TargetTable(activeModel);
            modelTable.Add(
                ModelEntities.UserPropertyName.ToString(),
                new TextCell("Обозначение"),
                false);

            var cellProvider = new CellProviderTemplate();

            ret.Add(
                new CellFactory(
                    cellProvider.GetCellProvider(
                        ModelPropertyNames.ActiveConfigName))
                );

            ret.Add(
                new CellFactory(cellProvider.GetCellProvider(
                        ModelPropertyNames.UserProperty))
                );

            return ret;

        }
    }
}
