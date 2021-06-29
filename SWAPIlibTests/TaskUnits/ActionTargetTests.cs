using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWAPIlib.Table;
using SWAPIlib.TaskUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits.Tests
{
    [TestClass()]
    public class ActionTargetTests
    {
        [TestMethod()]
        public void Create()
        {
            var table = new TableList();

            var actionTarget = new ActionTarget(table);
            var emptyActionTarget = new ActionTarget();
            Assert.AreEqual(table, actionTarget.GetTable());
            Assert.IsNull(emptyActionTarget.GetTable());

        }

        [TestMethod()]
        public void AddGetTest()
        {
            var table = new TableList();
            var emptyActionTarget = new ActionTarget();
            emptyActionTarget.Add(table, ITableType.Settings);

            Assert.AreEqual(table, emptyActionTarget.GetTable(ITableType.Settings));
            Assert.IsNull(emptyActionTarget.GetTable());
        }

        [TestMethod()]
        public void GetTargetTest()
        {
            string targetObj = "test";
            var target = new TargetWrapper(targetObj);
            var actionTarget = new ActionTarget() { TargetWrapper = target };

            Assert.AreEqual(targetObj, actionTarget.GetTarget<string>());
        }
    }
}