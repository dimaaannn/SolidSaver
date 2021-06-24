using SWAPIlib.TaskUnits;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table
{
    public interface ICell : INotifyPropertyChanged
    {
        string Text { get; }
        bool Update();
    }

    public interface IWritableCell : ICell
    {
        string TempText { get; set; }
        bool WriteValue();
    }

    public interface IReferencedCell : ICell
    {
        ITable RefTable { get; set; }
    }

    public interface IPropertyCell : ICell, IReferencedCell
    {
        string Name { get; }
        string Info { get; }
        ITable Settings { get; set; }
    }


    public abstract class BaseCell : ICell
    {
        private string text;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public virtual string Text { get => text; protected set { text = value; OnPropertyChanged(); } }
        public abstract bool Update();

        public override string ToString()
        {
            return Text;
        }
    }


}
