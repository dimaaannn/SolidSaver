using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidApp
{
    #region Функции
    class MainFunctions
    {




    }



    #endregion


    public interface IMainInterface
    {
        void Hello();
        void Bye();
        /// <summary>
        /// Запустить анимацию загрузки
        /// </summary>
        /// <param name="message">Текст загрузки</param>
        void LoadMessage(string message);


    }

}
