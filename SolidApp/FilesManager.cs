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
    static class Test
    {
        [STAThread]
        public static void Main()
        {

            string testpath = @"c:\Users\Красиков\source\repos\SolidApp\";
            string s = "";

            var fw = new FilesWorker();
            s = fw.GetFolderDialog(testpath);

            var dial = new FolderBrowserDialog
            {
                SelectedPath = testpath
            };


            using (dial)
            {
                if (dial.ShowDialog() == DialogResult.OK)
                {
                    s = dial.SelectedPath;
                }
            }

            var fileDial = new OpenFileDialog
            {
                Multiselect = false,
                Title = "test",
                InitialDirectory = testpath
            };

            using (fileDial)
            {
                if(fileDial.ShowDialog() == DialogResult.OK)
                {
                    s = fileDial.FileName;
                }
            }

                Console.WriteLine(s);
            Console.ReadKey();

            //var fw = new FilesWorker();
            //Task.Run(() => fw.GetFolderDialog(s));

        }
    }


    class FilesWorker
    {

        public string GetFolderDialog(string defaultPath)
        {
            

            string result = "";
            var dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = defaultPath;
            dialog.IsFolderPicker = true;
            dialog.ShowDialog();
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Console.WriteLine("SelectedFolder = " + dialog.FileName);
                result = dialog.FileName;
            }
            return result;
        }

    }

    
}
