using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace SWAPIlib.Table
{
    public interface ICellView : INotifyPropertyChanged
    {
        string Name { get; }
        string Info { get; }
        IEnumerable<ICellView> SettingsList { get; }

        bool IsWritable { get; }
        bool IsReferenced { get; }
        bool IsTargeted { get; }
        bool IsNotSaved { get; }

        string Text { get; }
        string TempText { get; set; }

        void Write();
        void Update();

    }


    


}
