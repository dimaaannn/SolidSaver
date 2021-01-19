using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using SWAPIlib.ComConn;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn.Proxy;

namespace SWAPIlib.Global
{
    public static class GlobalOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        static GlobalOptions()
        {
            SWAPIlib.ComConn.SwAppControl.MainModelChanged += SwAppControl_MainModelChanged;
        }

        /// <summary>
        /// Путь к основной открытой детали
        /// </summary>
        public static string RootModelPath
        {
            get
            {
                if (string.IsNullOrEmpty(_mainPartPath))
                    _mainPartPath = ModelProxy.GetPathName(SwAppControl.MainModel);
                return _mainPartPath;
            }
            set
            {
                _mainPartPath = value;
            }
        }
        static string _mainPartPath;
        /// <summary>
        /// Рабочая папка сборки
        /// </summary>
        public static string RootFolder
        {
            get => string.IsNullOrEmpty(_rootFolder) ?
                System.IO.Path.GetDirectoryName(RootModelPath) :
                _rootFolder;
            set => _rootFolder = value;
        }
        static string _rootFolder;
        /// <summary>
        /// Рабочая папка проекта
        /// </summary>
        public static string WorkFolder { get; set; } = RootFolder;

        /// <summary>
        /// Clear root path if main model changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SwAppControl_MainModelChanged(object sender, EventArgs e)
        {
            RootFolder = null;
        }
    }

}
