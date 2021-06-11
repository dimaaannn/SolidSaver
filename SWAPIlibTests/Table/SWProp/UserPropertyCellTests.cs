﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}