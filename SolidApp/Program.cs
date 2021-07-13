using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SolidDrawing;
//using SolidWorks;
using SolidWorks.Interop.sldworks;
using System.Diagnostics;
using System.IO;
using SwConst;
using System.Linq;
using System.Threading;
using System.Security.Policy;
using SWAPIlib;
using System.Collections;
using SWAPIlib.ComConn;
using SWAPIlib.BaseTypes;
using SWAPIlib.Controller;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib.MProperty;
using SWAPIlib.MProperty.Getters;
using SWAPIlib.Global;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolidApp.SW;
using Microsoft.Extensions.Logging;
using System.Security.AccessControl;
using System.Security.Principal;

namespace SolidApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Startup.Init(args);
            var logger = Startup.ServiceProvider.GetService<ILogger<Program>>();
            var swConnector = Startup.ServiceProvider.GetService<ISwConnector>();

            //swConnector.Connect();


            logger.LogTrace("testTrace");

            string folderPath = @"\\sergeant\Техотдел\Технологический - Общие документы\Общая\Красиков\Temp 2\UML\TestFolder\test.txt";

            var info = new FileInfo(folderPath);

            var creationTime = info.CreationTime;

            var accInfo = info.GetAccessControl();


            var directory = info.Directory;

            var dirSecurity = directory.GetAccessControl();

            IdentityReference idRef = dirSecurity.GetOwner(typeof(SecurityIdentifier));
            NTAccount ntAcc = idRef.Translate(typeof(NTAccount)) as NTAccount;

            Console.WriteLine(ntAcc.Value);




            Console.ReadKey();
        }

        public static void SWLibTest()
        {
            SwAppControl.Connect();

            //Get Raw document
            var appmodel = ModelClassFactory.ActiveDoc;

            //Print root title
            AppAssembly rootAsm = null;
            ModelDoc2 testRawModel = null;
            Component2 TestRawComponent = null;
            List<IAppComponent> compList = null;
            if (appmodel.SwModel is AssemblyDoc swAsm)
            {
                rootAsm = new AppAssembly(appmodel.SwModel);
                Console.WriteLine($"RootDoc: {rootAsm.Title}");
                compList = rootAsm.GetComponents(true);
                testRawModel = compList.First().SwModel;
                TestRawComponent = compList.First().SwCompModel;
            }
        }
    }


    

    public class NotifyOnInstance
    {
        public NotifyOnInstance()
        {
            Console.WriteLine("Class created");
        }
        public NotifyOnInstance(string s)
        {
            Console.WriteLine($"Class created with {s}");
        }
    }

}
