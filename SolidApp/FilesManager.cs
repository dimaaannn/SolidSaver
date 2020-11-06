using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Forms;


namespace SolidApp
{


    static class WorkFolder
    {
        static string defPath = "C:\\";
        static string _folderPath;
        static public string FolderPath 
        {
            get
            {
                return _folderPath;
            }
            set
            {
                _folderPath = value;
            }
        }


        public static string GetFolderDialog(string defaultPath = null)
        {
            
            string result = "";
            var dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = defaultPath ?? defPath;
            dialog.IsFolderPicker = true;
            var dialResult = dialog.ShowDialog();
            if (dialResult == CommonFileDialogResult.Ok)
            {
                result = dialog.FileName + "\\";
            }
            else result = defaultPath ?? defPath;
            return result;
        }

    }

    
}
