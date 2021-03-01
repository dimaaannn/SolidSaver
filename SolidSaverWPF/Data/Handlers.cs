using GalaSoft.MvvmLight;
using SolidSaverWPF.MessagesType;
using SWAPIlib;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn;
using SWAPIlib.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSaverWPF.Data
{
    public static class Variables
    {
        /// <summary>
        /// Задать рабочую папку
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool SetWorkFolder(string s)
        {
            bool ret = false;
            if(!string.IsNullOrEmpty(s) && Directory.Exists(s))
            {
                GlobalOptions.WorkFolder = s;
                ret = true;
            }
            return ret;
        }
        /// <summary>
        /// Текущая рабочая папка
        /// </summary>
        /// <returns></returns>
        public static string GetWorkFolder() => GlobalOptions.WorkFolder;

        public static ILinkedModel GetMainModel()
        {
            return MainModel.GetMainModel();
        }

        public static bool SetMainModel(ISwModelWrapper swModelWrapper)
        {
            return MainModel.SetMainModel(swModelWrapper);
        }
        
    }

    /// <summary>
    /// Глобальные события приложения
    /// </summary>
    public static class GlobalEvents
    {
        public static event EventHandler MainModelChanged
        {
            add => SwAppControl.MainModelChanged += value;
            remove => SwAppControl.MainModelChanged -= value;
        }

        public static event EventHandler WorkFolderChanged
        {
            add => GlobalOptions.WorkFolderChanged += value;
            remove => GlobalOptions.WorkFolderChanged -= value;
        }
    }


}
