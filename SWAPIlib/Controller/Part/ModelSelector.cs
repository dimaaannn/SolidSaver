using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using SWAPIlib.ComConn.Proxy;
using SWAPIlib.Global;

namespace SWAPIlib.Controller
{
    //public enum AppPartType
    //{
    //    NOTPART,
    //    ASMPART,
    //    LIBPART,
    //    IMPORTPART,
    //    PROJECTPART,
    //    SHEETPART
    //}

    public interface IModelSelector
    {
        /// <summary>
        /// Экземпляр детали
        /// </summary>
        IAppModel Appmodel { get; }
        /// <summary>
        /// Выбрано пользователем
        /// </summary>
        bool IsSelected { get; set; }
        /// <summary>
        /// Номер группы выделения
        /// </summary>
        int GroupNumber { get; set; }
        /// <summary>
        /// Путь относительно RootFolder
        /// </summary>
        string RootSubFolder { get; }
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
        bool Visible { get; set; }
    }

    

    //TODO add fix to partTyper with virtual or hidden part
    public class ModelSelector : IModelSelector
    {
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="imodel"></param>
        public ModelSelector(IAppModel imodel)
        {
            Appmodel = imodel;

        }
        public ModelSelector(IModelControl<IAppModel> partcontrol) : this(partcontrol.Appmodel) { }
        public ModelSelector(IAppComponent component)
        {
            Appmodel = component.PartModel ?? component as IAppModel;
        }

        public IAppModel Appmodel { get; set; }

        /// <summary>
        /// Выделение объекта
        /// </summary>
        #region свойства выделения объекта
        ///Количество групп выделений
        public static int SelectionArrayLength = 10;
        System.Collections.BitArray _selectionInGroup =
            new System.Collections.BitArray(SelectionArrayLength);
        public bool IsSelected { 
            get => _selectionInGroup[GroupNumber]; 
            set => _selectionInGroup[GroupNumber] = value; }
        int _groupNumber = 0;
        public int GroupNumber
        {
            get => Math.Min(_groupNumber, _selectionInGroup.Length - 1);
            set => _groupNumber = Math.Min(value, _selectionInGroup.Length - 1);
        }
        #endregion
        public string RootSubFolder
        {
            get
            {
                string rootModelFolder = GlobalOptions.RootFolder;
                if (Appmodel.Path.ToLowerInvariant().
                    Contains(rootModelFolder.ToLowerInvariant()))
                {
                    var ret = Appmodel.Path.Replace(rootModelFolder, "");
                    ret = ret.Replace(Appmodel.FileName, "");
                    return ret;
                }
                else return null;
            }
        }
        
        public bool IsPart => Appmodel.DocType == AppDocType.swPART;
        public bool IsAssembly => Appmodel.DocType == AppDocType.swASM;
        public bool IsSheetMetal => IsPart ? PartDocProxy.IsSheetMetal(Appmodel.SwModel) : false;
        public bool IsHaveDrawing => CheckDrawing(Appmodel.Path);
        public virtual bool Visible
        {
            get => Appmodel.SwModel.Visible;
            set => Appmodel.SwModel.Visible = value;
        }

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


    public interface IComponentSelector : IModelSelector
    {
        IAppComponent Appcomponent { get; }

        bool IsFixed { get; }
        bool IsMirrored { get; }
        bool IsPatternInstance { get; }
        bool IsSuppressed { get; }
        bool IsVirtual { get; }
        
    }

    public class ComponentSelector : ModelSelector, IComponentSelector
    {
        public ComponentSelector(IAppComponent comp) : base(comp)
        {
            Appcomponent = comp;
        }
        public IAppComponent Appcomponent { get; set; }


        public bool IsFixed => Appcomponent.SwCompModel.IsFixed();
        public bool IsMirrored => Appcomponent.SwCompModel.IsMirrored();
        public bool IsPatternInstance => Appcomponent.SwCompModel.IsPatternInstance();
        public bool IsSuppressed => Appcomponent.SwCompModel.IsSuppressed();
        public bool IsVirtual => Appcomponent.SwCompModel.IsVirtual;
        public override bool Visible
        {
            get => Appcomponent.SwCompModel.Visible == 1;
            set => Appcomponent.SwCompModel.Visible = value ? 1 : 0;
        }
    }
}
