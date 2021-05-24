using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWPF.BuissnesLogic
{
    public interface IUserData
    {
        int ID { get; set; }
        string Info { get; }
        string Name { get; set; }
    }

    public class UserData : IUserData
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public string Info => "UserDataClass";
    }

    public class UserData2 : IUserData
    {
        public int ID { get; set; } = 1;

        public string Info => "UserClass2";

        public string Name { get; set; } = "Vasya";
    }

    public static class Creator
    {
        public static SimpleIoc Ioc = SimpleIoc.Default;
        public static void InitIOC()
        {
            var ioc = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default;
            ioc.Register<IUserData>(() => new UserData2());
        }

        public static IUserData GetUserData()
        {
            InitIOC();

            return Ioc.GetInstance<IUserData>();

        }
    }
}
