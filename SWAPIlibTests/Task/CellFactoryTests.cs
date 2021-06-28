using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWAPIlib.Table;
using SWAPIlib.TaskUnits;
using SWAPIlibTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits.Tests
{
    [TestClass()]
    public class CellFactoryTests
    {

        public static CellFactoryTemplate cellProvider;
        public static IPropertyCell propCell;
        public static ITable TargetTable;

        public static ModelPropertyNames ConfigNameProviderName = ModelPropertyNames.ActiveConfigName;
        public static ModelPropertyNames UserPropertyProviderName = ModelPropertyNames.UserProperty;

        public static string tableName = "SomeName"; //задано в внешней функции

        [ClassInitialize]
        public static void CreateProperty(TestContext context)
        {
            TargetTable = SWConnections.GetActiveModelTarget();

            cellProvider = new CellFactoryTemplate();
        }

        [TestMethod()]
        public void CellFactoryTest()
        {
            var factory = new CellFactory(cellProvider.GetCellProvider(ModelPropertyNames.ActiveConfigName));

            var targetTable = new TargetTable((TargetTable as ITargetTable).GetTarget());
            var tableInterface = targetTable as ITable;

            var factoryInterface = factory as ITableAction;
            var log = factoryInterface.Proceed(ref tableInterface, null);

            var property = tableInterface.GetCell(ModelEntities.ConfigName.ToString());

            Assert.IsNotNull(property);
            Assert.IsNotNull(property.Text);

        }


        [TestMethod()]
        public void CreateCellFromProviderTemplate()
        {

            var factory = new CellFactory(cellProvider, ModelPropertyNames.WorkFolder);
            ITable table = null;
            var log = factory.Proceed(ref table, null);

            Assert.IsNotNull(table.First().Value.Text);
        }

        [TestMethod()]
        public void AddCellToTableTest()
        {
            var propFactoryList = new List<ICellFactory>();

            var activeModel = SWAPIlib.ComConn.SwAppControl.ActiveModel;
            var modelTable = new TargetTable(activeModel);

            //Создать текстовое свойство, просто добавить в список
            var paramCell = new TextCell("Обозначение");
            modelTable.Add(ModelEntities.UserPropertyName.ToString(), paramCell, false);

            var cellProvider = new CellFactoryTemplate();

            propFactoryList.Add(
                new CellFactory(
                    cellProvider.GetCellProvider(
                        ModelPropertyNames.ActiveConfigName)));

            propFactoryList.Add(
                new CellFactory(cellProvider.GetCellProvider(
                        ModelPropertyNames.UserProperty))
                );

            var table = modelTable as ITable;
            var log = new List<TableLog>();
            foreach (var task in propFactoryList)
            {
                log.Add(task.Proceed(ref table, null));
            }

            Assert.AreEqual(2, log.Count);
            var prop = table.Last().Value;
            prop.Update();
            Assert.IsNotNull(prop.Text);
        }

        [TestMethod]
        public void SettingsTableTest()
        {
            var propFactoryList = new List<ICellFactory>();

            var activeModel = SWAPIlib.ComConn.SwAppControl.ActiveModel;
            var modelTable = new TargetTable(activeModel);
            var propTable = new TableList();

            //Создать текстовое свойство, просто добавить в список
            var paramCell = new TextCell("Обозначение");
            propTable.Add(ModelEntities.UserPropertyName.ToString(), paramCell, false);

            var cellProvider = new CellFactoryTemplate();

            propFactoryList.Add(
                new CellFactory(
                    cellProvider.GetCellProvider(
                        ModelPropertyNames.ActiveConfigName)));

            propFactoryList.Add(
                new CellFactory(cellProvider.GetCellProvider(
                        ModelPropertyNames.UserProperty)));

            var table = modelTable as ITable;
            var log = new List<TableLog>();
            foreach (var task in propFactoryList)
            {
                log.Add(task.Proceed(ref table, propTable));
            }

            Assert.IsTrue(table.Count() == 2);
        }
    }
}