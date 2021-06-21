using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table.Prop
{
    public class TextBuilderCell : PropertyCellBase, IWritableCell
    {
        private string tempText;
        private StringBuilder stringBuilder = new StringBuilder();
        private StringBuilder StringBuilder { get => stringBuilder; set { stringBuilder = value; OnPropertyChanged("Text"); } }

        public const string SETTINGS_KEY = "TextBuilderSettings";

        /// <summary>
        /// Создать настройки
        /// </summary>
        /// <param name="textGetters"></param>
        /// <returns></returns>
        public static ICell<Func<ITable, string>[]> BuildSettings(
            params Func<ITable, string>[] textGetters)
        {
            return new Cell<Func<ITable, string>[]>(textGetters);
        }


        public TextBuilderCell(ITable refTable) : base(refTable: refTable)
        {
            RefTable = refTable;
            Settings = new TableList() { { SETTINGS_KEY, null, false } };
        }

        public override string Name => ModelPropertyNames.TextBuilder.ToString();
        public override string Info => "Текстовое значение, собранное из других свойств";
        
        public string TempText { get => tempText; set { tempText = value; OnPropertyChanged(); } }

        public override bool Update()
        {
            bool ret = false;
            StringBuilder.Clear();
            var settings = GetSettings(SETTINGS_KEY) as Cell<Func<ITable, string>[]>;
            if(settings?.Value?.Length > 0)
            {
                ret = true;
                foreach (var textGetter in settings.Value)
                {
                    stringBuilder.Append(textGetter(RefTable));
                }
            }

            Text = StringBuilder.ToString();
            return ret;
        }

        public bool WriteValue()
        {
            bool ret = false;
            if(TempText != null)
            {
                Text = TempText;
                ret = true;
            }
            return ret;
        }

        public static bool CheckTargetType(ITable refTable, ITable settings) =>
            refTable.GetCell(SETTINGS_KEY) is Cell<Func<ITable, string>[]>
            || settings.GetCell(SETTINGS_KEY) is Cell<Func<ITable, string>[]>;
    }



    
}
