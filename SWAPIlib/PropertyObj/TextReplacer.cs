using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SWAPIlib.PropertyObj
{

    public delegate bool TextReplacerDel(string input, string searchString, string replaceString, out string output);


    public interface ITextReplacer
    {

        string SearchText { get; set; }
        string ReplaceText { get; set; }
        bool Replace(string input);
        bool IsReplaced { get; }
        string ReplaceResult { get; }
        bool UseRegExp { get; set; }
        bool RegisterSensitive { get; set; }
    }

    public class TextReplacer : ITextReplacer
    {
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                //Remove regex pattern
                regex = null;
            }
        }
        string _searchText;
        public string ReplaceText { get; set; }
        TextReplacerDel textReplacer { get; set; } = ReplaceString;
        public bool IsReplaced { get; set; } = false;

        public string ReplaceResult { get => _result; }
        string _result;
        Regex regex;

        public bool UseRegExp
        {
            get => _useRegExp;
            set
            {
                if (value)
                    textReplacer = ReplaceRegExp;
                else
                    textReplacer = ReplaceString;
                _useRegExp = value;
            }
        }
        private bool _useRegExp;

        public bool RegisterSensitive { get; set; } = false;

        public bool Replace(string input)
        {
            IsReplaced = false;
            _result = null;
            if (!String.IsNullOrEmpty(input))
            {
                IsReplaced = textReplacer(input, SearchText, ReplaceText, out _result);
            }
            return IsReplaced;
        }

        public static bool ReplaceString(
            string input, string searchStr,
            string replaceStr, out string output)
        {
            bool ret = false;
            output = null;
            if (input.Contains(searchStr))
            {
                output = input.Replace(searchStr, replaceStr);
                ret = true;
            }
            return ret;
        }

        public bool ReplaceRegExp(
            string input, string searchStr,
            string replaceStr, out string output)
        {
            bool ret = false;
            //Create pattern if not exist
            if(regex == null)
                regex = new Regex(searchStr, RegexOptions.Compiled);

            ret = regex.IsMatch(input);

            if (ret)
                output = regex.Replace(input, replaceStr);
            else output = null;

            return ret;
        }
    }
}
