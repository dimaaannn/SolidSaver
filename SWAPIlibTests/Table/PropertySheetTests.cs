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
    public class PropertySheetTests
    {
        private static TableView tableView;
        public static IPropertyCell propCell;
        public static ITable TargetTable;

        public static string tableName = "SomeName"; //задано в внешней функции

        public static string propValue = "Some value";

        [ClassInitialize]
        public static void CreateProperty(TestContext context)
        {
            TargetTable = SWConnections.GetActiveModelTarget();
            var tempCell = new UserPropertyCell(TargetTable as ITargetTable);

            TargetTable.Add(UserPropertyCell.ConfigNameKey, new ActiveConfigNameCell(TargetTable as ITargetTable), false);
            TargetTable.Add(UserPropertyCell.PropNameKey, new TextCell("Наименование"), false);

            tableView = new TableView(TargetTable);

            propCell = tempCell;
        }

        [TestMethod()]
        public void PropertySheetTest()
        {
            var propSheet = new PropertySheet();
            propSheet.Add(new TextCell(propValue));
            propSheet.Add(TargetTable);

            var firstProp = propSheet.Properties.First();
            Assert.AreEqual(propValue, firstProp.Text);
            Assert.AreEqual(tableName, propSheet.TableViews.First().Name);
        }

        [TestMethod()]
        public void GetTableTest()
        {
            var propSheet = new PropertySheet();
            propSheet.Add(TargetTable);

            Assert.AreEqual(tableName, propSheet.GetTable(tableName).Name);
        }

        [TestMethod()]
        public void UpdatePropertiesTest()
        {
            string tempText = "text";
            var propSheet = new PropertySheet();
            propSheet.Add(new TextCell(propValue));

            var firstProp = propSheet.Properties.First();
            Assert.AreEqual(propValue, firstProp.Text);

            firstProp.TempText = tempText;
            Assert.AreEqual(tempText, firstProp.TempText);
            Assert.AreEqual(propValue, firstProp.Text);
            Assert.IsTrue(firstProp.IsNotSaved);

            propSheet.UpdateProperties();
            Assert.AreEqual(propValue, firstProp.Text);
            Assert.IsNull(firstProp.TempText);

        }

        [TestMethod()]
        public void WritePropertiesTest()
        {
            string tempText = "text";
            var propSheet = new PropertySheet();
            propSheet.Add(new TextCell(propValue));

            var firstProp = propSheet.Properties.First();
            Assert.AreEqual(propValue, firstProp.Text);

            firstProp.TempText = tempText;
            Assert.AreEqual(tempText, firstProp.TempText);
            Assert.AreEqual(propValue, firstProp.Text);
            Assert.IsTrue(firstProp.IsNotSaved);

            propSheet.WriteProperties();
            Assert.AreEqual(tempText, firstProp.Text);
            Assert.IsNull(firstProp.TempText);
        }
    }
}