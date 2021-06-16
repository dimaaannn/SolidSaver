using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWAPIlib.Table;
using SWAPIlib.Table.SWProp;
using SWAPIlib.Task;
using SWAPIlibTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Task.Tests
{
    [TestClass()]
    public class CellProviderTemplateTests
    {
        public static CellProviderTemplate cellProvider;
        public static IPropertyCell propCell;
        public static ITable TargetTable;

        public static ProviderName ProviderName = ProviderName.ActiveConfigName;

        public static string tableName = "SomeName"; //задано в внешней функции

        [ClassInitialize]
        public static void CreateProperty(TestContext context)
        {
            TargetTable = SWConnections.GetActiveModelTarget();

            //var tempCell = new UserPropertyCell(TargetTable as ITargetTable);

            //TargetTable.Add(UserPropertyCell.ConfigNameKey, new ActiveConfigNameCell(TargetTable as ITargetTable), false);
            //TargetTable.Add(UserPropertyCell.PropNameKey, new TextCell("Наименование"), false);

            //propCell = tempCell;

            cellProvider = new CellProviderTemplate();
        }

        [TestMethod()]
        public void GetCellProviderTest()
        {
            var configNameProp = cellProvider.GetCellProvider(ProviderName);
            Assert.IsNotNull(configNameProp);
        }

        [TestMethod]
        public void ConfigNameProviderTest()
        {
            var configNameProp = cellProvider.GetCellProvider(ProviderName);
            Assert.IsTrue( configNameProp.CheckTable(TargetTable, null));

            var property = configNameProp.GetCell(TargetTable, null);
            Assert.IsNotNull(property);
            Assert.IsNotNull(property.Text);
        }

    }
}