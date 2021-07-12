using Ninject;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib
{
    public static class Initialiser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static StandardKernel kernel { get; private set; }

        static Initialiser()
        {
            kernel = new StandardKernel();
            kernel.Load<BaseTypes.WrapperNinjectBinding>();
            kernel.Load<TaskCollection.TaskCollectionNinjectBinding>();

        }

        public static void DIInit()
        {
            logger.Info("Startup initialisation");
        }
    }
}
