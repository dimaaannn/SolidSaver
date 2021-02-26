using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolidSaverWPF.MessagesType;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn;
using SWAPIlib.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel
{
    public class MainDocOptionsViewModel : ViewModelBase
    {
        public MainDocOptionsViewModel()
        {
            MessengerInstance.Register<ISwModelWrapper>(this, SetDocName);
        }

        private string _DocName;
        public string DocName { get => _DocName; set => Set(ref _DocName, value); }

        private void SetDocName(ISwModelWrapper wrapper)
        {
            DocName = wrapper.Title;
        }
    }
}
