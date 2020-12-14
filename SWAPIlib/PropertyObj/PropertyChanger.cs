using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWAPIlib;

namespace SWAPIlib.PropertyObj
{
    public delegate ISwProperty PropConstructor(AppModel appModel, string ConfigName);
    public delegate string ValueConverter(string Value);

    public interface IPropertyChanger
    {
        string SearchValue { get; set; }
        string NewValue { get; set; }
        void ProceedValues();
        void WriteValues();
        void RestoreValues();

        ValueConverter StringConv { get; set; }
        ObservableCollection<AppModel> Models { get; }
        ObservableCollection<ISwProperty> Properties { get; }
        bool AllConfigurations { get; set; }
    }

    public interface IPropModifier
    {
        /// <summary>
        /// Имя детали
        /// </summary>
        string PartName { get; }
        /// <summary>
        /// Модель детали
        /// </summary>
        AppModel Model { get; set; }
        /// <summary>
        /// Список конфигураций
        /// </summary>
        ObservableCollection<string> ConfigNames { get; set; }
        /// <summary>
        /// Словарь старых значений
        /// </summary>
        Dictionary<string, string> OldValues { get; }
        /// <summary>
        /// Словарь объектов свойств
        /// </summary>
        Dictionary<string, ISwProperty> SwPropList { get; set; }
        /// <summary>
        /// Конструктор свойств
        /// </summary>
        PropConstructor propConstructor { get; set; }
        /// <summary>
        /// Записать новые значения
        /// </summary>
        void WriteValues();
        /// <summary>
        /// Восстановить значения
        /// </summary>
        void RestoreValues();
        /// <summary>
        /// Показать все конфигурации
        /// </summary>
        bool AllConfiguration { get; set; }

    }

    class PropertyChangerBase  //: IPropertyChanger
    {

        
    }
}
