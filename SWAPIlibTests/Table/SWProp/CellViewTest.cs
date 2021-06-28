using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWAPIlibTests;
using System.Linq;

namespace SWAPIlib.Table.SWProp.Tests
{
    [TestClass]
    public class CellViewTest
    {
        public static IPropertyCell propCell;
        [ClassInitialize]
        public static void CreateProperty(TestContext context)
        {
            var targetTable = SWConnections.GetActiveModelTarget();
            var tempCell = new UserPropertyCell(targetTable);

            targetTable.Add(UserPropertyCell.ConfigNameKey, new ActiveConfigNameCell(targetTable), false);
            targetTable.Add(UserPropertyCell.PropNameKey, new TextCell("Наименование"), false);

            propCell = tempCell;
        }

        [TestMethod]
        public void CreatePropertyCellViewTest()
        {
            var cellView = new CellView(propCell);
            Assert.IsNotNull(cellView.Text);
            Assert.IsTrue(cellView.IsTargeted);
            Assert.IsTrue(cellView.IsReferenced);
            Assert.IsTrue(cellView.IsWritable);
            Assert.IsFalse(cellView.IsNotSaved);
            Assert.IsNotNull(cellView.Name);
        }


        [TestMethod]
        public void CreateNullCellView()
        {
            var cellView = new CellView(null);
            Assert.IsNull(cellView.Text);
            Assert.IsFalse(cellView.IsTargeted);
            Assert.IsFalse(cellView.IsReferenced);
            Assert.IsFalse(cellView.IsWritable);
            Assert.IsFalse(cellView.IsNotSaved);
            Assert.IsNull(cellView.Name);

            var settings = cellView.SettingsList;
            Assert.AreEqual(0, settings.Count());

        }

        [TestMethod]
        public void GetSettingsListTest()
        {
            var cellView = new CellView(propCell);
            var tempList = cellView.SettingsList;
            Assert.AreEqual(2, tempList.Count());

            var viewNames = tempList.Select(v => v.Name).ToArray();


            CollectionAssert.Contains(viewNames, UserPropertyCell.ConfigNameKey);
            CollectionAssert.Contains(viewNames, UserPropertyCell.PropNameKey);
        }

        [TestMethod]
        public void CreateSimpleCellViewTest()
        {
            string data = "Some text";
            string testName = "TestName";
            var cellView = new CellView(new TextCell(data));

            Assert.IsNotNull(cellView.Text);
            Assert.IsFalse(cellView.IsTargeted);
            Assert.IsFalse(cellView.IsReferenced);
            Assert.IsTrue(cellView.IsWritable);
            Assert.IsFalse(cellView.IsNotSaved);
            Assert.IsNull(cellView.Name);

            cellView.Name = testName;
            Assert.AreEqual(testName, cellView.Name);
        }

        [TestMethod]
        public void ChangeValueTest()
        {
            string data = "Some text";
            string changedText = "Some New text";
            string testName = "TestName";
            var cellView = new CellView(new TextCell(data)) { Name = testName };

            cellView.TempText = changedText;
            Assert.IsTrue(cellView.IsNotSaved);
            cellView.Write();
            Assert.IsFalse(cellView.IsNotSaved);
        }
    }
}