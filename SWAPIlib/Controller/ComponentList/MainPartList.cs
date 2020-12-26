using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace SWAPIlib.Controller
{
    

    public class PartList<T1> : INotifyPropertyChanged
         where T1 :ISwModel
    {
        public PartList()
        {
            PartCollection = new ObservableCollection<IPartControl<T1>>();
        }
        public ObservableCollection<IPartControl<T1>> PartCollection { get; set; }
        private int _selectionNum = -1;
        public virtual int SelectionNum
        {
            get => _selectionNum;
            set
            {
                _selectionNum = value;
                OnPropertyChanged("SelectionNum");
            }
        }
        public IPartControl<T1> GetSelectedPart { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void ChangeSelection(object state)
        {
            for (int i = 0; i < 50; i++)
            {
                SelectionNum = i % 10;
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
