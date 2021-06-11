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
        void Update();
    }

    public interface IWritableCell : ICell
    {
        string TempText { get; set; }
        bool WriteValue();
    }

    public interface ITableCell : ICell
    {
        ITable RefTable { get; set; }
    }

    public abstract class BaseCell : ICell
    {
        private string text;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string Text { get => text; protected set { OnPropertyChanged("Text"); text = value; } }
        public abstract void Update();

    }

    public class TextCell : BaseCell, IWritableCell
    {
        private string tempText;

        public string TempText { get => tempText; set { OnPropertyChanged(); tempText = value; } }

        public override void Update()
        {
            TempText = null;
        }

        public bool WriteValue()
        {
            Text = TempText;
            return true;
        }
    }
}
