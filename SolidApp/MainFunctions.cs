using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidApp
{
    #region Функции


    public static class AppMethods
    {
        public static IMainInterface MainUI;
        public static void Init()
        {
            MainUI.Hello();
        }

    }


    #endregion

    #region Interfaces

    public interface IMainInterface
    {
        void Hello();
        void Bye();
        /// <summary>
        /// Запустить анимацию загрузки
        /// </summary>
        /// <param name="message">Текст загрузки</param>
        void ShowProcess(string message);
        int UserSelect(string[] variants);
        string WorkFolder { get; set; }

    }

    public interface IPartInterface
    {
        void ShowFileName(string filename);
        void ShowDocType(string docType);
        string ActiveConfigName { get; set; }
        void PropList(ref Dictionary<string, string> propDict);
        void OperationStatus(ref Dictionary<string, string> operationsDict);

    }

    #endregion

}
