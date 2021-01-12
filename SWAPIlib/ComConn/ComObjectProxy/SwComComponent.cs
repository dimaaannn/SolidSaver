using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using SolidWorks.Interop.sldworks;
using SwConst;

namespace SWAPIlib.ComConn.Proxy
{
    public static class ComponentProxy
    {
        /// <summary>
        /// Get modelDoc2 from component2
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns></returns>
        public static ModelDoc2 GetModelDoc2(Component2 swComp)
        {
            var model = swComp.GetModelDoc2();
            var ret = model as ModelDoc2;
            return ret;
        }


        /// <summary>
        /// Получить родительский компонент
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns>Null if not exist</returns>
        public static Component2 GetParent(Component2 swComp)
        {
            //Метод Component.GetParent() не работает
            //Прикручен костыль

            Component2 retComp = null;

            if (!swComp.IsRoot() && !swComp.IsSuppressed())
            {
                ModelDoc2 swModel = swComp.GetModelDoc2();
                Configuration conf = swModel.ConfigurationManager
                    .ActiveConfiguration;

                retComp = conf.GetRootComponent3(false);
            }
            return retComp;
        }

        /// <summary>
        /// Get root assembly component
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns>Assembly root (not null)</returns>
        public static Component2 GetRoot(Component2 swComp)
        {
            Component2 retComp = swComp;
            int EmergencyBreak = 10;
            while (!retComp.IsRoot())
            {
                retComp = GetParent(retComp);
                if (EmergencyBreak-- <= 0)
                    break;
            }

            return retComp;
        }

        /// <summary>
        /// Имя зависимой конфигурации компонента сборки
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns></returns>
        public static string RefConfigName(Component2 swComp)
        {
            return swComp.ReferencedConfiguration;
        }

        /// <summary>
        /// Статус компонента сборки
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns></returns>
        public static swComponentSuppressionState_e GetSuppressionState(Component2 swComp)
        {
            swComponentSuppressionState_e ret = default;
            ret = (swComponentSuppressionState_e)swComp.GetSuppression();
            return ret;
        }

        /// <summary>
        /// Получить дочерние компоненты компонента
        /// Для подсчёта использовать GetChildrenCount
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns>Пустой массив если отсутствуют</returns>
        public static Component2[] GetChildren(Component2 swComp)
        {
            Component2[] retArr = null;
            Component2 testcomp;
            object[] temp = swComp.GetChildren();
            retArr = ServiceCl.ObjArrConverter<Component2>(temp);
            return retArr;
        }

        /// <summary>
        /// Имя компонента
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns></returns>
        public static string GetName(Component2 swComp)
        {
            return swComp?.Name2;
        }

        /// <summary>
        /// Переименовать компонент
        /// </summary>
        /// <param name="swComp"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static bool SetName(Component2 swComp, string newName)
        {
            bool ret = false;
            swComp.Name2 = newName;
            if (swComp.Name == newName)
                ret = true;
            return ret;
        }

        /// <summary>
        /// Имя связанной конфигурации компонента
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns></returns>
        public static string GetRefConfig(Component2 swComp)
        {
            return swComp.ReferencedConfiguration;
        }

        /// <summary>
        /// Задать имя связанной конфигурации
        /// </summary>
        /// <param name="swComp"></param>
        /// <param name="ConfName"></param>
        /// <returns></returns>
        public static bool SetRefConfig(Component2 swComp, string ConfName)
        {
            bool ret = false;
            swComp.ReferencedConfiguration = ConfName;
            if (swComp.ReferencedConfiguration == ConfName)
                ret = true;
            return ret;
        }

        public static string GetPathName(Component2 swComp)
        {
            return swComp.GetPathName();
        }

        /// <summary>
        /// Отображение компонента сборки
        /// </summary>
        /// <param name="swComp"></param>
        /// <returns></returns>
        public static AppCompVisibility GetVisibleStatus(Component2 swComp)
        {
            return PartTypeChecker.Visibility(swComp.Visible);
        }
        /// <summary>
        /// Задать отображение компонента сборки
        /// </summary>
        /// <param name="swComp"></param>
        /// <param name="visState"></param>
        public static void SetVisibleStatus(Component2 swComp, AppCompVisibility visState)
        {
            swComp.Visible = (int)visState;
        }

        /// <summary>
        /// Габаритный размер
        /// </summary>
        /// <param name="swComp"></param>
        /// <param name="includeSketches"></param>
        /// <returns></returns>
        public static Box? GetBox(Component2 swComp, bool includeSketches = false)
        {
            Box? ret = null;
            if (swComp != null)
            {
                var points = swComp.GetBox(
                    IncludeRefPlanes: false,
                    IncludeSketches: includeSketches);

                if (!(points is DBNull))
                    ret = new Box(points);
            }

            return ret;
        }
    }
}
