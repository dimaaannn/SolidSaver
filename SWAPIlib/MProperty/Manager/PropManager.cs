using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib.MProperty;

namespace SWAPIlib.MProperty.Manager
{
    public interface ISearchSettings
    {
        bool UseRegExp { get; set; }
        bool CaseSensitive { get; set; }
        bool AllConfigurations { get; set; }
        SWAPIlib.PropertyObj.ITextReplacer TextReplacer { get; }
    }



    public class PropManager : INotifyPropertyChanged
    {
        public string SearchValue { get; set; }
        public string NewValue { get; set; }
        public ISearchSettings Settings { get; set; }
        public IPropGetter SinglePrototype { get; set; }
        public IEnumerable<IPropGetter> PropTemplates { get; set; }
        public IEnumerable<IAppModel> AppModels { get; set; }
        public IObservable<IPropGetter> PropViews { get; set; }

        public bool CreatePropView() { return false; }
        public void RunSearch() { }
        public void Restore() { }
        public void WriteValues() { }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
