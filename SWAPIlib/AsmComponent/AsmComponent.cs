using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;

namespace SWAPIlib.AsmComponent
{
    public interface IAsmComponent
    {
        AppDocType DocType { get; }
        Component2 SwComponent { get; }

        
    }

    public class AsmComponent 
    {
        public virtual AppDocType DocType { get; private set; }

        public Component2 SwCompModel => _swCompModel;
        private Component2 _swCompModel;
    }
}
