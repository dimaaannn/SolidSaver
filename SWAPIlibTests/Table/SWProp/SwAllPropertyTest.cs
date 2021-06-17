using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWAPIlib.Table;
using SWAPIlib.Table.SWProp;
using SWAPIlib.Task;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlibTests.Table.SWProp
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class UserPropertyTestClass
    {
        private static ITargetTable TargetTable;
        private static ITable Table;
        private static CellProviderTemplate CellProvider;


        [ClassInitialize]
        public static void CreateProperty(TestContext context)
        {
            TargetTable = SWConnections.GetActiveModelTarget();
            Table = TargetTable;
            CellProvider = new CellProviderTemplate();
        }

        [TestMethod]
        public void FilePathTest()
        {
            var cell = new FilePathCell(TargetTable);
            Assert.IsNotNull(cell.Text);
            StringAssert.Matches(cell.Text, new System.Text.RegularExpressions.Regex(
                @".*\.(sldprt|sldasm)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase));

        }

        [TestMethod]
        public void FilePathFactoryTest()
        {
            var factory = new CellFactory(CellProvider.GetCellProvider(ModelPropertyNames.FilePath));

            factory.Proceed(ref Table, null);
            Assert.IsTrue(Table.Count() == 1);
            Assert.IsNotNull(Table.GetCell(ModelEntities.FileName.ToString()).Text);
        }
    }
}
