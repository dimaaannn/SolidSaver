using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib
{
    [Flags]
    public enum AppDocType
    {
        swNONE = 0,
        swPART = 1 << 1,
        swASM = 1 << 2,
        swDRAWING = 1 << 3,
        swCOMPONENT = 1 << 5
    }

    
    /// <summary>
    /// Статус отображения компонента
    /// </summary>
    public enum AppSuppressionState
    {
        Suppressed = 0,
        Lightweight = 1,
        FullyResolved = 2,
        Resolved = 3,
        FullyLightweight = 4,
        None = 5
    }
    
    public enum AppCompVisibility
    {
        Hidden = 0,
        Visible = 1,
    }
}
