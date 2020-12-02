using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib
{
    public enum AppDocType
    {
        swNONE = 0,
        swPART = 1,
        swASM = 2,
        swDRAWING = 3,
        swCOMPONENT = 5
    }

    public enum AppPartType
    {
        NOTPART,
        ASMPART,
        LIBPART,
        IMPORTPART,
        PROJECTPART,
        SHEETPART
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
}
