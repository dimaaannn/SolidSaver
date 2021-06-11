using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolidWorks.Interop.sldworks;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib.Property;
using SWAPIlib.Table.SWProp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table.SWProp.Tests
{
    [TestClass()]
    public class ActiveConfigNameCellTests
    {
        private IAppModel appModel;
        private ModelDoc2 activeModel;
        private ActiveConfigNameCell configNameCell;
        private bool IsNotFailed = true;
        ITarget iTarget;
        private ITargetTable refTable;

        [TestMethod]
        [Priority(0)]
        public void A1LoadDocument()
        {
            SwAppControl.Connect();
            activeModel = SwAppControl.ActiveModel;

            IsNotFailed = activeModel != null;
            Assert.IsNotNull(activeModel);
        }

        [TestMethod]
        public void A2GetAppModel()
        {
            if (IsNotFailed)
            {
                appModel = ModelClassFactory.ActiveDoc;
                IsNotFailed = appModel != null;
                Assert.IsNotNull(appModel);
            }
            else 
                Assert.Fail();
        }

        [TestMethod]
        [Priority(1)]
        public void B1GetAssembly()
        {
            SwAppControl.Connect();
            appModel = ModelClassFactory.ActiveDoc;
            Assert.AreEqual(AppDocType.swASM, appModel.DocType);
        }



        [TestMethod()]
        [Priority(5)]
        public void C2CreateTarget()
        {
            SwAppControl.Connect();
            appModel = ModelClassFactory.ActiveDoc;
            activeModel = appModel.SwModel;

            iTarget = new Target<ModelDoc2>(activeModel, "testTarget");
            Assert.IsNotNull(iTarget.GetTarget());
            
        }

        [TestMethod]
        public void GetConfigNameTest()
        {
            SwAppControl.Connect();
            appModel = ModelClassFactory.ActiveDoc;
            activeModel = appModel.SwModel;


            refTable = new TargetTable(activeModel);
            configNameCell = new ActiveConfigNameCell(refTable);

            string result = configNameCell.Text;
            Assert.IsTrue(!string.IsNullOrEmpty(result));
        }

        [TestMethod()]
        public void UpdateTest()
        {
            SwAppControl.Connect();
            appModel = ModelClassFactory.ActiveDoc;
            activeModel = appModel.SwModel;


            refTable = new TargetTable(activeModel);
            configNameCell = new ActiveConfigNameCell(refTable) ;

            string result = configNameCell.Text;
            Assert.IsTrue(!string.IsNullOrEmpty(result));
            Assert.IsTrue( configNameCell.Update());
            Assert.IsTrue(!string.IsNullOrEmpty(result));

        }
    }
}