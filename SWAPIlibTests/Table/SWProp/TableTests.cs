using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SWAPIlib.Table.SWProp.Tests
{
    [TestClass]
    public class TableTests
    {
        string tableName = "tableName";
        string FieldKey1 = "fieldKey1";
        string FieldKey2 = "fieldKey2";
        static string CellText1 = "CellText1";
        static string CellText2 = "CellText2";
        ICell cell1 = new TextCell(CellText1);
        ICell cell2 = new TextCell(CellText2);

        [TestMethod]
        public void CreateTableTest()
        {
            var table = new TableList() { { FieldKey1, cell1, true } };
            Assert.AreEqual(CellText1, table.GetCell(FieldKey1).Text);
        }

        [TestMethod]
        public void TableUnionTest()
        {
            var table = new TableList() { { FieldKey1, cell1, true } };
            var table2 = new TableList() { { FieldKey2, cell2, true } };
            table2.CopyTo(table, false);

            Assert.AreEqual(CellText1, table.GetCell(FieldKey1).Text);
            Assert.AreEqual(CellText2, table.GetCell(FieldKey2).Text);
        }

        [TestMethod]
        public void TableOverrideTest()
        {
            string cellNewText = "NewText";
            var table = new TableList() { { FieldKey1, cell1, true } };
            var table2 = new TableList() { { FieldKey1, new TextCell(cellNewText), true } };

            table2.CopyTo(table, false);
            Assert.AreEqual(CellText1, table.GetCell(FieldKey1).Text);

            table2.CopyTo(table, true);
            Assert.AreEqual(cellNewText, table.GetCell(FieldKey1).Text);

        }
    }
}