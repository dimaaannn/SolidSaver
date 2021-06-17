using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWAPIlib.Table;
using SWAPIlib.Table.SWProp;
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


        [ClassInitialize]
        public static void CreateProperty(TestContext context)
        {
            TargetTable = SWConnections.GetActiveModelTarget();
        }

        [TestMethod]
        public void FilePathTest()
        {
            var cell = new FilePathCell(TargetTable);
            Assert.IsNotNull(cell.Text);
            StringAssert.Matches(cell.Text, new System.Text.RegularExpressions.Regex(
                @".*\.(sldprt|sldasm)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase));

        }
    }
}
