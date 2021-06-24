using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWAPIlib.Table.Prop;
using SWAPIlib.TaskUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table.Prop.Tests
{
    [TestClass()]
    public class WorkFolderCellTests
    {
        [TestMethod]
        public void WorkFolderCellTest()
        {
            var cell = new WorkFolderCell();
            Assert.IsNotNull(cell.Text);
            StringAssert.Matches(cell.Text, new System.Text.RegularExpressions.Regex(
                @".*\\*.$", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }

    }
}