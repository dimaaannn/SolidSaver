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
        static string _folderPath;
        static public string FolderPath 
        {
            get
            {
                if (string.IsNullOrEmpty(_folderPath))
                    _folderPath = GetFolderDialog();
                return _folderPath;
            }
            set
            {
                _folderPath = value;
            }
        }

        public static string GetFolderDialog(string defaultPath = "C:\\")
        {
            
            string result = "";
            var dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = defaultPath;
            dialog.IsFolderPicker = true;
            var dialResult = dialog.ShowDialog();
            if (dialResult == CommonFileDialogResult.Ok)
            {
                Console.WriteLine("SelectedFolder = " + dialog.FileName);
                result = dialog.FileName;
            }
            return result;
        }

    }

    
}
