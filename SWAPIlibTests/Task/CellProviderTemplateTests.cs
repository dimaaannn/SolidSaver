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

        public static ModelPropertyNames ConfigNameProviderName = ModelPropertyNames.ActiveConfigName;
        public static ModelPropertyNames UserPropertyProviderName = ModelPropertyNames.UserProperty;

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
            var configNameProp = cellProvider.GetCellProvider(ConfigNameProviderName);
            Assert.IsNotNull(configNameProp);
        }

        [TestMethod]
        public void ConfigNameProviderTest()
        {
            var configNameProp = cellProvider.GetCellProvider(ConfigNameProviderName);
            Assert.IsTrue(configNameProp.CheckTable(TargetTable, null));

            var property = configNameProp.GetCell(TargetTable, null);
            Assert.IsNotNull(property);
            Assert.IsNotNull(property.Text);
        }

        [TestMethod]
        public void InvalidTargetTest()
        {
            var simpleTable = new TableList() { { "key", new TextCell("some text"), false } };
            var targetTable = new TargetTable("abcd") { { "key2", new TextCell("ada"), false } };

            var property = cellProvider.GetCellProvider(ConfigNameProviderName);
            Assert.IsFalse(property.CheckTable(simpleTable, targetTable));
            Assert.IsFalse(property.CheckTable(targetTable, simpleTable));
            Assert.IsFalse(property.CheckTable(targetTable, null));
            Assert.IsFalse(property.CheckTable(null, targetTable));
            Assert.IsFalse(property.CheckTable(null, null));
        }

        [TestMethod]
        public void UserPropertyTest()
        {
            var activeConfFactory = cellProvider.GetCellProvider(ModelPropertyNames.ActiveConfigName);

            var targetObj = (TargetTable as ITargetTable).GetTarget();
            var targetTable = new TargetTable(targetObj) { { ModelEntities.UserPropertyName.ToString(), new TextCell("Обозначение"), false } };

            targetTable.Add(ModelEntities.ConfigName.ToString(), activeConfFactory.GetCell(targetTable, null), false);

            var userPropertyFactory = cellProvider.GetCellProvider(ModelPropertyNames.UserProperty);

            var userProperty = userPropertyFactory.GetCell(targetTable, null);

            Assert.IsNotNull(userProperty);
            Assert.IsNotNull(userProperty.Text, $"User property is {userProperty.Text}");
        }

    }

    [TestClass]
    public class CellProviderMethods
    {
        private static ITargetTable TargetTable;
        private static CellProviderTemplate cellProvider;

        [ClassInitialize]
        public static void CreateProperty(TestContext context)
        {
            TargetTable = SWConnections.GetActiveModelTarget();
            cellProvider = new CellProviderTemplate();
        }

        [TestMethod]
        public void ForkFolderCellFactoryTest()
        {
            ITable table = null;
            var factory = new CellFactory(cellProvider.GetCellProvider(ModelPropertyNames.WorkFolder));

            factory.Proceed(ref table, null);
            Assert.IsTrue(table.Count() == 1);
            Assert.IsNotNull(table.GetCell(ModelEntities.Folder.ToString()).Text);
        }
    }
}