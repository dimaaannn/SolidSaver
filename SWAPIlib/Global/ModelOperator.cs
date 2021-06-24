using SolidWorks.Interop.sldworks;
using SWAPIlib.BaseTypes;
using SWAPIlib.ComConn;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public static List<ISwModelWrapper> GetVisibleAssembly()
        {
            var ret = new List<ISwModelWrapper>();
            foreach (var model in DocEnumerator)
            {
                if (model?.Visible == true)
                {
                    var wrapModel = new SwModelWrapper(model);
                    if(wrapModel.DocType == AppDocType.swASM)
                    {
                        ret.Add(wrapModel);
                    }
                }
            }
            return ret;
        }

        public static ModelDoc2 GetNextOpenedDoc(EnumDocuments2 enumerator)
        {
            int fetched = 0;
            ModelDoc2 model;
            enumerator.Next(1, out model, ref fetched);
            return model;
        }

        public static async Task<ModelDoc2> GetNextOpenedDocAsync (EnumDocuments2 enumerator)
        {
            var ret = Task<ModelDoc2>.Run(
                () => GetNextOpenedDoc(enumerator)
                );
            return await ret;
        }



        public static async System.Threading.Tasks.Task AddOpenedDocsAsync(Action<object> action, CancellationToken cancellationToken)
        {
            //var enumDocuments = SwAppControl.swApp.EnumDocuments2();

            //var model = await GetNextOpenedDocAsync(enumDocuments);
            //while(model != null 
            //    && cancellationToken.IsCancellationRequested == false)
            //{
            //    action(model.GetTitle());
            //    model = await GetNextOpenedDocAsync(enumDocuments);
            //}

            Action<ModelDoc2> modelAction = (model) => action(model.GetTitle());

            await SWAPIlib.ComConn.SwAppControl.GetOpenedModels(modelAction, cancellationToken);
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

            enumDocuments?.Next(1, out _Current, ref fetched);
            return Current is null ? false : true;
        }

        public void Reset() => enumDocuments.Reset();
        public void Dispose() => enumDocuments = null;

        public IEnumerator<ModelDoc2> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}
