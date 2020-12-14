using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.PropertyObj
{
    public delegate IEnumerable<T> TextGetter<T, T1>(T1 obj);
    public delegate bool TextSetter<T>(T obj, string setText);
    public delegate bool TextReplacer(string input, string searchString, string replaceString, out string output);


    public interface IStringChangerUnit<T, T1>
    {
        TextGetter<T, T1> textGetter { get; set; }
        TextSetter<T1> textSetter { get; set; }
        TextReplacer textReplacer { get; set; }
        Dictionary<T, bool> ReplaceCollection { get; set; }
        string SearchText { get; set; }
        string ReplaceText { get; set; }
        void Proceed();

    }

    class StringChangerUnit<ObjT, ItemT> : IStringChangerUnit<ObjT, ItemT> 
    {
        public TextGetter<ObjT, ItemT> textGetter { get ; set; }
        public TextSetter<ObjT> textSetter { get ; set ; }
        public TextReplacer textReplacer { get; set; }
        public Dictionary<ObjT, bool> ReplaceCollection { get ; set; }
        public string SearchText { get ; set; }
        public string ReplaceText { get ; set; }

        public void Proceed()
        {
            string input, output;
            bool success;
            foreach(var item in ReplaceCollection)
            {
                input = textGetter(item.Key);
                success = textReplacer(input, SearchText, ReplaceText, out output);
                if (success) 
                    textSetter(item.Key, output);
                ReplaceCollection[item.Key] = success;
            }
        }
    }

    public static class GetterColl
    {
        public static TextGetter<PropModifier> PropertyGetter = (PropModifier modifier) =>
        {
            modifier.
        }
    }
}
