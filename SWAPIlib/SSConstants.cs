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
}
