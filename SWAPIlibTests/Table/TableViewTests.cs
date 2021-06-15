using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWAPIlib.Table;
using SWAPIlib.Table.SWProp;
using SWAPIlibTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table.Tests
{
    [TestClass()]
    public class TableViewTests
    {
        public static IPropertyCell propCell;
        public static ITable TargetTable;

        public static string tableName = "SomeName"; //задано в внешней функции

        [ClassInitialize]
        public static void CreateProperty(TestContext context)
        {
            TargetTable = SWConnections.GetActiveModelTarget();
            var tempCell = new UserPropertyCell(TargetTable as ITargetTable);

            TargetTable.Add(UserPropertyCell.ConfigNameKey, new ActiveConfigNameCell(TargetTable as ITargetTable), false);
            TargetTable.Add(UserPropertyCell.PropNameKey, new TextCell("Наименование"), false);

            propCell = tempCell;
        }

        [TestMethod()]
        public void CreateTableViewTest()
        {
            var tableView = new TableView(TargetTable);
            Assert.IsTrue(tableView.IsReferencedTable);
            Assert.AreEqual(tableName, tableView.Name);
            Assert.AreEqual(2, tableView.Properties.Count());
        }

        [TestMethod]
        public void GetPropertyView()
        {
            var tableView = new TableView(TargetTable);
            var cells = tableView.Properties;

            var confNameCell = cells.Where(cell => cell.Name == UserPropertyCell.ConfigNameKey).FirstOrDefault();

            Assert.IsNotNull(confNameCell.Text);
            Assert.IsTrue(confNameCell.IsTargeted);
        }
    }
}