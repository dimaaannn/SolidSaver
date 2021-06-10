using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWPF.BuissnesLogic
{
    public class JustData : ViewModelBase
    {
        
        private int iD = -1;
        private string name = "default";
        private int age = 0;

        public JustData() { }
        public JustData(int i) : base()
        {
            iD = i;
        }

        public string Name { get => name; set => Set(ref name, value); }
        public int Age { get => age; set => Set(ref age, value); }
        public int ID { get => iD; protected set => Set(ref iD, value); }
    }

    public class DataProvider
    {
        public static List<JustData> GetData() => GetData(14);
        public static List<JustData> GetData(int i)
        {
            var ret = new List<JustData>();
            var r = Enumerable.Range(0, i);
            ret.AddRange(
                r
                .Select(num => 
                    new JustData(num) { Name = $"Data-{num}" }
                ).ToList());
            return ret;
        }
    }
}
