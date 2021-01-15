using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;


namespace SWAPIlib.ComConn.ComObjectProxy
{
    public interface IAppSelMgr
    {
        ISelectionMgr SwSelMan { get; }

    }

    public static class AppSelMgr
    {

        /// <summary>
        /// Объект менеджера выделений SW
        /// </summary>
        public static ISelectionMgr SwSelMan
        {
            get
            {
                if(_swSelMan == null)
                {
                    var mainmodel = SWAPIlib.ComConn.SwAppControl.MainModel;
                    _swSelMan = mainmodel.ISelectionManager;
                }
                return _swSelMan;
            }
        }
        private static SelectionMgr _swSelMan;
        public static int SelectionGroup { get; set; } = -1;
        /// <summary>
        /// Количество выделенных объектов
        /// </summary>
        public static int CurrentSelectCount => SwSelMan.GetSelectedObjectCount2(SelectionGroup); //-1 - all models, 0 - only whithout mark
        /// <summary>
        /// Выбрать компонент
        /// </summary>
        /// <param name="component">Компонент SW</param>
        /// <param name="Append">Добавить к выделению</param>
        /// <returns></returns>
        public static bool Select(Component2 component, bool Append=false)
        {
            return component.Select4(Append, null, false);
        }
        /// <summary>
        /// Возможность выделения деталей в SW
        /// </summary>
        public static bool EnableSelection
        {
            get => SwSelMan.EnableSelection;
            set => SwSelMan.EnableSelection = value;
        }
        /// <summary>
        /// Групповое выделение компонентов
        /// </summary>
        public static Component2[] SelectedComponents
        {
            get
            {
                var ret = new Component2[CurrentSelectCount];
                int selCount = CurrentSelectCount;
                for (int i = 0; i < selCount; i++)
                {
                    ret[i] = SwSelMan.GetSelectedObjectsComponent3(i+1, SelectionGroup);
                }
                return ret;
            }
            set
            {
                foreach (var comp in value)
                {
                    Select(comp);
                }
            }
        }
        

    }

    
        
    
}
