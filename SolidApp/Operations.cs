using SldWorks;
using SwConst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolidApp
{
    public interface IPartUI
    {
        void Hello();
        void Bye();
        /// <summary>
        /// Запустить thread анимации с надписью
        /// </summary>
        /// <param name="eventMessage">Текст надписи</param>
        /// <returns></returns>
        object ShowProcess(string eventMessage);
        void ShowFileName(string fileName);
        void ShowDocType(swDocumentTypes_e docType);
        void ShowPropList(Dictionary<string, string> propDict);
        void Warning(string warnText);
        int ShowQuestion(string question, string[] answers);
        string RequestText(string textDescription);

    }

    public class ConsoleUI : IPartUI
    {

        public void ShowHello()
        {

        }
    }

    public static class BasicOperations
    {
        public static IPartUI UI;
        private static Thread ShowEvent;
        public static bool LockUntilSwStart(int sleepTime = 500)
        {
            ShowEvent = new Thread(() => UI.ShowProcess("Ожидание запуска SolidWorks"));
            ShowEvent.Start();

            while (SwProcess.IsRunning == false)
            {
                Thread.Sleep(sleepTime);
            }
            ShowEvent.Interrupt();
            return SwProcess.IsRunning;
        }

        public static IModelDoc2 GetActiveDoc(int sleepTime = 500)
        {
            ModelDoc2 swPart = SwProcess.swApp.ActiveDoc();

            ShowEvent = new Thread(() => UI.ShowProcess("Откройте деталь"));
            ShowEvent.Start();

            while (swPart is null)
            {
                Thread.Sleep(sleepTime);
                swPart = SwProcess.swApp.ActiveDoc();
            }
            ShowEvent.Interrupt();

            return swPart;
        }
    }

    //TODO создать класс с шаблонами имён и путями директорий

}
