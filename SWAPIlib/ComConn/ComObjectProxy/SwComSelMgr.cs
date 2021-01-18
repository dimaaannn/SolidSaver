using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;


namespace SWAPIlib.ComConn
{


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

        /// <summary>
        /// Объект свойств выделения SW
        /// </summary>
        public static SelectData SelData
        {
            get
            {
                if(_selData == null)
                {
                    _selData = SwSelMan.CreateSelectData();
                    _selData.Mark = -1; //Default selection mark
                }
                return _selData;
            }
            set => _selData = value;
        }
        private static SelectData _selData;
        /// <summary>
        /// Маркировка выделения
        /// </summary>
        public static int SelectionMark
        {
            get => SelData.Mark;
            set => SelData.Mark = value;
        }
        /// <summary>
        /// Количество выделенных объектов
        /// </summary>
        public static int CurrentSelectCount => SwSelMan.GetSelectedObjectCount2(SelectionMark); //-1 - all models, 0 - only whithout mark
        /// <summary>
        /// Выбрать компонент
        /// </summary>
        /// <param name="component">Компонент SW</param>
        /// <param name="Append">Добавить к выделению</param>
        /// <returns></returns>
        public static bool Select(Component2 component, bool Append=false)
        {
            return component.Select4(Append, SelData, false); //append, SelectData, show popup
        }
        /// <summary>
        /// Убрать выделение
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool DeSelect(Component2 component) => component.DeSelect();
        //public static bool Deselect(Component2 component) => SwSelMan.IDeSelect2()
        /// <summary>
        /// Возможность выделения деталей в SW
        /// </summary>
        public static bool DisableSelection
        {
            get => !SwSelMan.EnableSelection;
            set => SwSelMan.EnableSelection = !value;
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
                    ret[i] = SwSelMan.GetSelectedObjectsComponent3(i+1, SelectionMark);
                }
                return ret;
            }
            set
            {
                foreach (var comp in value)
                {
                    Select(comp, true);
                }
            }
        }
        

    }

    
        
    
}
