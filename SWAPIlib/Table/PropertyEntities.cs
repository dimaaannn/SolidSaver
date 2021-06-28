using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table
{
    /// <summary>
    /// Имена конкретных свойств
    /// </summary>
    public enum ModelPropertyNames
    {
        None,
        ActiveConfigName,
        ConfigNamesList,
        FileName,
        FilePath,
        Title,
        UserProperty,
        WorkFolder,
        TextBuilder,
        SaveSheetMetal
    }

    /// <summary>
    /// Имена сущностей для использования в параметрах
    /// </summary>
    public enum ModelEntities
    {
        None,
        ConfigName,
        FileName,
        UserSelection,
        Title,
        UserProperty,
        UserPropertyName,
        Folder,
        SubFolder,
        IsSheetMetal,
        TextBuilderSettings,
        FilePath
    }
}
