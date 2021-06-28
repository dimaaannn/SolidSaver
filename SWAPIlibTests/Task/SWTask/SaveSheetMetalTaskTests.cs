using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolidWorks.Interop.sldworks;
using SWAPIlib.Table;
using SWAPIlib.TaskUnits.SWTask;
using SWAPIlibTests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskUnits.SWTask.Tests
{
    [TestClass()]
    public class SaveSheetMetalTaskTests
    {
        private static ModelDoc2 mainModel;
        private static CellFactoryTemplate cellProvider;
        public static Component2[] components;
        private static ModelDoc2 sheetModel;
        private static TargetTable<ModelDoc2> targetTable;
        private static string saveFolder;

        [ClassInitialize]
        public static void Initialise(TestContext context)
        {

            mainModel = SWConnections.GetActiveModel();
            cellProvider = new CellFactoryTemplate();
            components = SWAPIlib.ComConn.Proxy.AsmDocProxy.GetComponents(mainModel);

            sheetModel = GetSheetModel(components);

            targetTable = new TargetTable<ModelDoc2>(sheetModel);
            saveFolder = @"\\sergeant\Техотдел\Технологический - Общие документы\Общая\Красиков\VBA\SolidWorks\Тестовая сборка\Tests\";
        }

        public static ModelDoc2 GetSheetModel(Component2[] components)
        {
            foreach (var comp in components)
            {
                Debug.WriteLine(comp.Name2);
            }
            foreach (var comp in components)
            {
                Debug.WriteLine(comp.Name2);
                if(comp.GetModelDoc2() is ModelDoc2 model)
                {
                    if (SWAPIlib.ComConn.Proxy.PartDocProxy.IsSheetMetal(model))
                    {
                        var sheetBody = SWAPIlib.ComConn.Proxy.PartDocProxy
                            .GetBodies(model);
                        if (sheetBody.Count() == 1)
                        {
                            return model;
                        }
                    }
                }
            }
            return null;
        }

        [TestMethod()]
        public void CheckTargetTypeTest()
        {
            bool isValid = SaveSheetMetalTask.CheckTargetType(targetTable, null);
            Assert.IsTrue(isValid);
        }

        [TestMethod()]
        public void UpdateTest()
        {
            targetTable.Add(ModelEntities.FileName.ToString(),
                new TextCell(saveFolder), false);

            var saveSheetTask = new SaveSheetMetalTask(targetTable);
            Assert.IsNotNull(saveSheetTask.Path);

            saveSheetTask.Update();
            Assert.IsNotNull(saveSheetTask.Path);

            string prevPath = saveSheetTask.Path;
            saveSheetTask.TempText = "test";
            Assert.AreNotEqual(prevPath, saveSheetTask.Path);
            Assert.AreEqual(prevPath, saveSheetTask.Text);
            Assert.IsNotNull(saveSheetTask.TempText);

            saveSheetTask.Update();
            Assert.AreEqual(prevPath, saveSheetTask.Path);
            Assert.IsNull(saveSheetTask.TempText);
            Assert.IsNotNull(saveSheetTask.Text);
        }

        [TestMethod()]
        public void WriteValueTest()
        {
            string modelFileName = System.IO.Path.GetFileName(sheetModel.GetPathName());
            Assert.IsFalse(string.IsNullOrEmpty(modelFileName));
            string savingPath = System.IO.Path.Combine(saveFolder, modelFileName);

            targetTable.Add(ModelEntities.FileName.ToString(),
                new TextCell(savingPath), 
                false);

            var saveSheetTask = new SaveSheetMetalTask(targetTable);

            bool status = saveSheetTask.WriteValue();
            Assert.IsTrue(status);
            var replaceExtension = Path.Combine(saveFolder,
                Path.GetFileNameWithoutExtension(savingPath) + ".dxf");

            Assert.IsTrue(System.IO.File.Exists(replaceExtension));
        }

    }
}