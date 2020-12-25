using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SWAPIlib.Controller
{
    public enum AppPartType
    {
        NOTPART,
        ASMPART,
        LIBPART,
        IMPORTPART,
        PROJECTPART,
        SHEETPART
    }

    public interface IPartTyper
    {
        /// <summary>
        /// Экземпляр детали
        /// </summary>
        ISwModel Appmodel { get; }
        /// <summary>
        /// Выбрано пользователем
        /// </summary>
        bool IsSelected { get; set; }
        /// <summary>
        /// Номер группы выделения
        /// </summary>
        int GroupNumber { get; set; }

        /// <summary>
        /// Является объектом детали
        /// </summary>
        bool IsPart { get; }
        /// <summary>
        /// Является сборкой
        /// </summary>
        bool IsAssembly { get; }
        /// <summary>
        /// Является листовой деталью
        /// </summary>
        bool IsSheetMetal { get; }
        /// <summary>
        /// Имеется чертёж с таким же именем файла
        /// </summary>
        bool IsHaveDrawing { get; }
        /// <summary>
        /// Находится в подпапке основной сборки
        /// </summary>
        bool InAssemblyFolder { get; }

    }

    public class PartTyper : IPartTyper
    {
        public ISwModel Appmodel => throw new NotImplementedException();

        /// <summary>
        /// Группы выделения
        /// </summary>
        System.Collections.BitArray _selectionInGroup =
            new System.Collections.BitArray(10);
        public bool IsSelected { 
            get => _selectionInGroup[GroupNumber]; 
            set => _selectionInGroup[GroupNumber] = value; }
        int _groupNumber = 0;
        public int GroupNumber
        {
            get => Math.Min(_groupNumber, _selectionInGroup.Length - 1);
            set => _groupNumber = Math.Min(value, _selectionInGroup.Length - 1);
        }

        public bool IsPart => Appmodel.DocType == AppDocType.swPART;
        public bool IsAssembly => Appmodel.DocType == AppDocType.swASM;
        public bool IsSheetMetal => IsPart ? PartDocProxy.IsSheetMetal(Appmodel.SwModel) : false;
        public bool IsHaveDrawing => CheckDrawing(Appmodel.Path);
        public bool InAssemblyFolder => throw new NotImplementedException();

        /// <summary>
        /// Проверить существование чертежа с тем же именем в папке
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckDrawing(string path)
        {
            string pathWithNewExt = Path.GetDirectoryName(path) + 
                "\\" +
                Path.GetFileNameWithoutExtension(path) + 
                ".SLDDRW";
            return File.Exists(pathWithNewExt);
        }
    }
}
