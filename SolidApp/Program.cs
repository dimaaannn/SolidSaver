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

namespace SolidApp
{
    class Program
    {
        static string Version = "0.51 Beta";

        static void Main(string[] args)
        {
            //var swModel = AppConsole.LoadActiveDoc();
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



            var swApp = SwAppControl.swApp;

            string pathToFile = @"\\sergeant\Техотдел\Технологический - Общие документы\Общая\Красиков\VBA\SolidWorks\Тестовая сборка\2670 Основа топпера.SLDPRT";

            var savePath = @"\\sergeant\Техотдел\Технологический - Общие документы\Общая\Красиков\VBA\SolidWorks\Тестовая сборка\Tests\TestSave.dxf";

            int longstatus = 0;
            int longwarnings = 0;
            ModelDoc2 Part = swApp.OpenDoc6(pathToFile, 1, 0, "", ref longstatus, longwarnings);

            Part = swApp.ActiveDoc;

            ModelView myModelView = Part.ActiveView;

            myModelView.FrameLeft = 0;
            myModelView.FrameTop = 21;
            myModelView = Part.ActiveView;

            myModelView.FrameState = (int)swWindowState_e.swWindowMaximized;
            swApp.ActivateDoc2("2670 Основа топпера", false, ref longstatus);
            Part = swApp.ActiveDoc;

            bool boolstatus = Part.Extension.SelectByID2("Развертка1", "BODYFEATURE", 0, 0, 0, true, 0, null, 0);
            longstatus = Part.SaveAs3(savePath, 0, 0);
            myModelView = Part.ActiveView;
            myModelView.FrameState = (int) swWindowState_e.swWindowMaximized;
            Part = swApp.ActiveDoc;


            //Console.ReadKey();
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




    public class TreeEnumNode :IEnumerator<TreeEnumNode>, IEnumerable<TreeEnumNode>
    {
        public TreeEnumNode()
        {
            SubNodes = new List<TreeEnumNode>();
        }
        public string Name { get; set; }
        public List<TreeEnumNode> SubNodes { get; set; }


        private int position = -1;
        private IEnumerator<TreeEnumNode> subEnum;
        private TreeEnumNode _current;
        public TreeEnumNode Current
        {
            get
            {
                return _current;
            }
        }
        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            if (position < SubNodes.Count - 1)
            {                
                if (subEnum != null && subEnum.MoveNext())
                {
                    _current = subEnum.Current;
                }
                else
                {
                    position++;
                    _current = SubNodes[position];
                    subEnum = _current.GetEnumerator();
                }
                return true;
            }
            else
                return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }

        //Testing

        public IEnumerator<TreeEnumNode> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }


    public class RangeContaiter :IEnumerable
    {
        public RangeContaiter()
        {
            RangeList = System.Linq.Enumerable.Range(1, 4).ToList();
        }
        List<int> RangeList;

        public IEnumerator GetEnumerator()
        {
            return RangeList.GetEnumerator();
        }
    }

    public class ContainerContaiter :IEnumerable
    {
        public ContainerContaiter(int count)
        {
            containers = new List<RangeContaiter>(50);

            for(int i = 0; i < count; i++)
            {
                containers.Add(new RangeContaiter());
            }
        }
        public List<RangeContaiter> containers;

        public IEnumerator GetEnumerator()
        {
            foreach(var item in containers)
            {
                foreach(var subitem in item)
                {
                    yield return subitem;
                }
            }
        }
    }

}
