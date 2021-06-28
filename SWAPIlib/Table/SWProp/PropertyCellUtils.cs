using SolidWorks.Interop.sldworks;

namespace SWAPIlib.Table.SWProp
{
    internal static class PropertyCellUtils
    {
        public static bool TargetIsComponent(ITable refTable, ITable settings)
        {
            var obj = GetTargetObject(refTable, settings);
            return obj is Component2;
        }

        public static bool TargetIsModel(ITable refTable, ITable settings)
        {
            var obj = GetTargetObject(refTable, settings);
            return obj is ModelDoc2;
        }

        public static object GetTargetObject(ITable refTable, ITable settings)
        {
            object ret = null;
            if (settings is ITargetTable tTable)
                ret = tTable.GetTarget();
            else if (refTable is ITargetTable tTable2)
                ret = tTable2.GetTarget();

            return ret;
        }
    }
}
