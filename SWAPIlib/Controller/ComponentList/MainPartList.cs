using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace SWAPIlib.Controller
{
    public interface IPartListTest<T1> where T1 : IAppModel
    {
        IModelControl<T1> GetSelectedPart { get; }
        ObservableCollection<IModelControl<T1>> PartCollection { get; set; }
        int SelectionNum { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        void ChangeSelection(object state);
        void OnPropertyChanged(string prop = "");
    }

    public class PartList<T1> : INotifyPropertyChanged, IPartListTest<T1> where T1 : IAppModel
    {
        public PartList()
        {
            PartCollection = new ObservableCollection<IModelControl<T1>>();
        }
        public ObservableCollection<IModelControl<T1>> PartCollection { get; set; }
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
        public IModelControl<T1> GetSelectedPart { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Test thread selection
        /// </summary>
        /// <param name="state"></param>
        public void ChangeSelection(object state)
        {
            for (int i = 0; i < 50; i++)
            {
                SelectionNum = i % 10;
                System.Threading.Thread.Sleep(1000);
            }
        }
    }

    public class ThreePartList : INotifyPropertyChanged
    {
        public ThreePartList()
        {
            _subComponentsPartList = new ObservableCollection<ThreePartList>();
        }
        public ThreePartList(IAppComponent component) : this()
        {
            Component = component;
        }
        public IAppComponent Component { get; }
        public int? ChildrenCount { get => Component?.GetChildrenCount(); }
        private ObservableCollection<ThreePartList> _subComponentsPartList;
        public ObservableCollection<ThreePartList> SubComponentsPartList
        {
            get
            {
                if(_subComponentsPartList.Count == 0 && ChildrenCount > 0)
                {
                    foreach (var comp in Component.GetComponents(true))
                    {
                        _subComponentsPartList.Add(new ThreePartList(comp));
                    }
                }
                return _subComponentsPartList;
            }
        }
        public string Title => Component.Title;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
