using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWAPIlib.ComConn;
using SWAPIlib.Table.SWProp;
using SWAPIlibTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table.SWProp.Tests
{
    [TestClass()]
    public class UserPropertyCellTests
    {
        UserPropertyCell userProp;
        ITable settings;

        [TestMethod()]
        public void UserPropertyCellTest()
        {
            var targetTable = SWConnections.GetActiveModelTarget();
            userProp = new UserPropertyCell(targetTable);

            settings = new Table()
            {
                { UserPropertyCell.ConfigNameKey, new ActiveConfigNameCell(targetTable), false },
                { UserPropertyCell.PropNameKey, new TextCell("Наименование"), false }
            };

            userProp.Settings = settings;
            var result = userProp.Text;
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void UpdateTest()
        {
            var targetTable = SWConnections.GetActiveModelTarget();
            userProp = new UserPropertyCell(targetTable);

            settings = new Table()
            {
                { UserPropertyCell.ConfigNameKey, new ActiveConfigNameCell(targetTable), false },
                { UserPropertyCell.PropNameKey, new TextCell("Наименование"), false }
            };

            userProp.Settings = settings;
            userProp.Update();
            var result = userProp.Text;

            Assert.IsTrue( userProp.Update());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WriteValueTest()
        {
            var targetTable = SWConnections.GetActiveModelTarget();
            userProp = new UserPropertyCell(targetTable);

            string newText = "NewText";

            settings = new Table()
            {
                { UserPropertyCell.ConfigNameKey, new ActiveConfigNameCell(targetTable), false },
                { UserPropertyCell.PropNameKey, new TextCell("Наименование"), false }
            };

            userProp.Settings = settings;
            var result = userProp.Text;

            userProp.TempText = newText;

            Assert.IsNotNull(userProp.TempText);
            Assert.IsTrue(userProp.WriteValue());
            Assert.IsNull(userProp.TempText);
            Assert.AreEqual(newText, userProp.Text);

            userProp.TempText = result;
            Assert.IsTrue(userProp.WriteValue());
            Assert.IsNull(userProp.TempText);
            Assert.AreEqual(result, userProp.Text);


        }

        [TestMethod]
        public void GetSettingsKeys()
        {
            var targetTable = SWConnections.GetActiveModelTarget();
            userProp = new UserPropertyCell(targetTable);
            var settings = userProp.Settings.Select(keyval => keyval.Key);
            List<string> expected = new List<string>() { ModelEntities.UserPropertyName.ToString()
                , ModelEntities.ConfigName.ToString() };

            CollectionAssert.AreEquivalent(expected, settings.ToList());

            targetTable.Add(UserPropertyCell.ConfigNameKey, new ActiveConfigNameCell(targetTable), false);
            targetTable.Add(UserPropertyCell.PropNameKey, new TextCell("Наименование"), false);

            Assert.IsNotNull(userProp.Settings.GetCell(UserPropertyCell.PropNameKey));


            string currentNomination = userProp.Text;
            Assert.IsNotNull(currentNomination);

            userProp.Settings.Add(UserPropertyCell.PropNameKey, new TextCell("Обозначение"), true);
            userProp.Update();
            Assert.AreNotEqual(currentNomination, userProp.Text, "Текст изменился после смены пользовательского свойства");
        }
    }

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