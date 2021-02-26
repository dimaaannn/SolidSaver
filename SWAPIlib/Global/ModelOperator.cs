using SolidWorks.Interop.sldworks;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Global
{
    public static class OpenedDocs
    {
        static OpenedDocs()
        {
            DocEnumerator = new OpenedDocsEnumerator();
        }
        static OpenedDocsEnumerator DocEnumerator;

        public static List<ISwModelWrapper> GetAllDocs()
        {
            var ret = new List<ISwModelWrapper>();
            foreach (var model in DocEnumerator)
            {
                var wrapModel = new SwModelWrapper(model);
                ret.Add(wrapModel);
            }
            return ret;
        }

        public static List<ISwModelWrapper> GetVisibleDocs()
        {
            var ret = new List<ISwModelWrapper>();
            foreach (var model in DocEnumerator)
            {
                if(model?.Visible == true)
                {
                    var wrapModel = new SwModelWrapper(model);
                    ret.Add(wrapModel);
                }
            }
            return ret;
        }
    }

    /// <summary>
    /// Перебор открытых в SW документов (всех загруженных)
    /// </summary>
    public class OpenedDocsEnumerator : IEnumerator<ModelDoc2>, IEnumerable<ModelDoc2>
    {
        private EnumDocuments2 enumDocuments;
        private int fetched;
        private ModelDoc2 _Current;
        public ModelDoc2 Current => _Current;

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if(enumDocuments is null)
                enumDocuments = SwAppControl.swApp.EnumDocuments2();

            enumDocuments.Next(1, out _Current, ref fetched);
            return Current is null ? false : true;
        }

        public void Reset() => enumDocuments.Reset();
        public void Dispose() => enumDocuments = null;

        public IEnumerator<ModelDoc2> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}
