﻿using System;
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
        /// Имя внутренней папки проекта относительно главной сборки
        /// </summary>
        string RootSubFolder { get; }

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
        
        public bool IsPart => Appmodel.DocType == AppDocType.swPART;
        public bool IsAssembly => Appmodel.DocType == AppDocType.swASM;
        public bool IsSheetMetal => IsPart ? PartDocProxy.IsSheetMetal(Appmodel.SwModel) : false;
        public bool IsHaveDrawing => CheckDrawing(Appmodel.Path);
        public string RootSubFolder
        {
            get
            {
                string rootModelFolder = GlobalOptions.ModelRootFolder;
                if (Appmodel.Path.ToLowerInvariant().
                    Contains(rootModelFolder.ToLowerInvariant()))
                {
                    return Appmodel.Path.Replace(rootModelFolder + "\\", "");
                }
                else return null;
            }
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
}