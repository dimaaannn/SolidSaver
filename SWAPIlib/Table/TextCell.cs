namespace SWAPIlib.Table
{
    public class TextCell : BaseCell, IWritableCell
    {
        private string tempText;
        public TextCell(string text)
        {
            Text = text;
        }
        public string TempText { get => tempText; set { OnPropertyChanged(); tempText = value; } }

        public override bool Update()
        {
            TempText = null;
            return true;
        }

        public bool WriteValue()
        {
            Text = TempText;
            TempText = null;
            return true;
        }
    }
}
