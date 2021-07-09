using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskCollection
{
    public interface IActionManager : INotifyPropertyChanged
    {
        ITableCollection TableCollection { get; }
        IActionUnit NextAction { get; }
        ITargetProvider PartProvider { get; set; }
        void SetNextAction(IActionUnit actionUnit);
        void RunNext();
        void SkipNext();
        void Clear();
    }

    public class ActionManager : IActionManager
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ITableCollection TableCollection => throw new NotImplementedException();

        public ITargetProvider PartProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IActionUnit NextAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void RunNext()
        {
            throw new NotImplementedException();
        }

        public void SkipNext()
        {
            throw new NotImplementedException();
        }

        public void SetNextAction(IActionUnit actionUnit)
        {
            throw new NotImplementedException();
        }
    }
}
