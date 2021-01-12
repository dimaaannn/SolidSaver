using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using SolidWorks.Interop.sldworks;
using SwConst;



namespace SWAPIlib.ComConn.Proxy
{
    /// <summary>
    /// Вспомогательные функции
    /// </summary>
    public static class ServiceCl
    {
        /// <summary>
        /// Конвертировать массив объектов в массив типа
        /// </summary>
        /// <typeparam name="Tout">Тип элемента массива</typeparam>
        /// <param name="inputArray">Массив объектов</param>
        /// <returns>Типизированный массив</returns>
        public static Tout[] ObjArrConverter<Tout>(IList<object> inputArray)
        {
            Tout[] outputArray;

            int itemCounter = inputArray.Count();
            outputArray = new Tout[itemCounter];

            for (int i = 0; i < itemCounter; ++i)
            {
                outputArray[i] = (Tout)inputArray[i];
            }
            return outputArray;
        }
    }


}

