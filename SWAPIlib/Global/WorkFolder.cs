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
        /// Путь к основной открытой детали
        /// </summary>
        public static string MainPartPath
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
        public static string ModelRootFolder
        {
            get => string.IsNullOrEmpty(_modelRootFolder) ?
                System.IO.Path.GetDirectoryName(MainPartPath) :
                _modelRootFolder;
            set => _modelRootFolder = value;
        }
        static string _modelRootFolder;
        /// <summary>
        /// Рабочая папка проекта
        /// </summary>
        public static string MainFolder { get; set; } = ModelRootFolder;

    }

}
