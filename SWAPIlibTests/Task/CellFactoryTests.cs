using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWAPIlib.Table;
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
    public class CellFactoryTests
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

            cellProvider = new CellProviderTemplate();
        }

        [TestMethod()]
        public void CellFactoryTest()
        {
            var factory = new CellFactory() { 
                CellProvider = cellProvider.GetCellProvider(ModelPropertyNames.ActiveConfigName), 
                Key = ModelEntities.ConfigName.ToString()
                , OverrideKey = true
            };

            var targetTable = new TargetTable((TargetTable as ITargetTable).GetTarget());
            var tableInterface = targetTable as ITable;

            var factoryInterface = factory as ITableAction;
            var log = factoryInterface.Proceed(ref tableInterface, null);

            var property = tableInterface.GetCell(ModelEntities.ConfigName.ToString());

            Assert.IsNotNull(property);
            Assert.IsNotNull(property.Text);

        }


        [TestMethod()]
        public void CreateCellTest()
        {
            
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

            var cellProvider = new CellProviderTemplate();

            propFactoryList.Add(
                new CellFactory(
                    cellProvider.GetCellProvider(
                        ModelPropertyNames.ActiveConfigName))
                {
                    Key = ModelEntities.ConfigName.ToString(),
                    Name = "Имя конфигурации"
                }
                );

            propFactoryList.Add(
                new CellFactory(cellProvider.GetCellProvider(
                        ModelPropertyNames.UserProperty))
                {
                    Key = ModelEntities.UserProperty.ToString(),
                    Name = "Пользовательское свойство"
                }
                );

            var table = modelTable as ITable;
            var log = new List<TableLog>();
            foreach (var task in propFactoryList)
            {
                log.Add(task.Proceed(ref table, null));
            }

            Assert.AreEqual(2, log.Count);
            Assert.IsNotNull(table.Last().Value.Text);
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

            var cellProvider = new CellProviderTemplate();

            propFactoryList.Add(
                new CellFactory(
                    cellProvider.GetCellProvider(
                        ModelPropertyNames.ActiveConfigName))
                {
                    Key = ModelEntities.ConfigName.ToString(),
                    Name = "Имя конфигурации"
                }
                );

            propFactoryList.Add(
                new CellFactory(cellProvider.GetCellProvider(
                        ModelPropertyNames.UserProperty))
                {
                    Key = ModelEntities.UserProperty.ToString(),
                    Name = "Пользовательское свойство"
                }
                );

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