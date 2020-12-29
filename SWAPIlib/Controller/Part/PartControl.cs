using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SWAPIlib.Controller
{

    public interface IPartList : INotifyPropertyChanged
    {
        ObservableCollection<IPartList> PartCollection { get; set; }
        int SelectionNum { get; set; }
    }

}
