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


            var ds = new DataSourceEnumerator() { Delay = 300 };


            PrintDataAsync(ds);

            Console.WriteLine("Some other work");
            Thread.Sleep(4000);
            //Console.ReadKey();
        }

        public static async void PrintDataAsync(DataSourceEnumerator intEnum)
        {
            //var taskList = new List<Task<int>>();
            while (intEnum.MoveNext())
            {

                await intEnum.GetNextAsync().ContinueWith(t => Console.WriteLine(t.Result));
            }
        }

        //public static IEnumerable<Task<int>> GetDataAsync(int delay = 300)
        //{
        //    var ds = new DataSourceEnumerator() { Delay = delay };

        //    while(ds.MoveNext())
        //    {
        //        yield return Task.Run<int>()
        //    }
        //}

    }


    public class DataSourceEnumerator : IEnumerator<int>
    {
        private int[] DataSource;
        private int index = -1;

        public DataSourceEnumerator()
        {
            DataSource = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }

        public int Current { get; private set; } = 0;
        public int Delay { get; set; } = 300;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            Reset();
        }

        public bool MoveNext()
        {
            //System.Threading.Thread.Sleep(Delay);

            if(++index < DataSource.Length)
            {
                Current = DataSource[index];
                return true;
            }
            else
                return false;
        }


        public bool HasMoreResults()
        {
            return ++index < DataSource.Length;
        }
        public async Task<int> GetNextAsync()
        {
            var tsk = Task.Run<int>(GetNext);
            int result;
            result = await tsk.ContinueWith(t => t.Result);
            return result;
        }

        public int GetNext()
        {
            System.Threading.Thread.Sleep(Delay);
            return Current;
        }

        public async Task<bool> MoveNextAsync()
        {
            return await Task.Run<bool>(MoveNext);
        }

        public void Reset()
        {
            Current = 0;
            index = -1;
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
