using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.PropertyObj
{
    class PropertyUI
    {
        public PropertyChanger PropChanger { get; protected set; }
        public Dictionary<string, PropConstructor> Constructors { get; set; }
        public string ActiveConstructor { get; set; }
        public PropConstructor GetConstructor();


    }
}
