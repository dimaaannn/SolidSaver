namespace SWAPIlib.Table
{
    public class TextCell : BaseCell, IWritableCell
    {
        private string tempText;

        public string TempText { get => tempText; set { OnPropertyChanged(); tempText = value; } }

        public override bool Update()
        {
            TempText = null;
            return true;
        }

        public bool WriteValue()
        {
            Text = TempText;
            return true;
        }
    }
}
