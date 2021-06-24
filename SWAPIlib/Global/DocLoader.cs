using SolidWorks.Interop.sldworks;
using SWAPIlib.BaseTypes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SWAPIlib.Global
{
    public class DocLoader : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool isBusy;

        public bool IsBusy { get => isBusy; protected set { isBusy = value; OnPropertyChanged(); } }

        public Action<IModelWrapper> ModelAction { get; set; }

        public async Task GetOpenedDocumentsAsync(CancellationToken ct) =>
            await GetOpenedDocumentsAsync(ct, ModelAction);

        public async Task GetOpenedDocumentsAsync(CancellationToken ct, Action<IModelWrapper> modelAction )
        {
            IsBusy = true;
            Action<ModelDoc2> visibleModelToWrapper = model =>
            {
                if (model.Visible)
                {
                    modelAction(BuildModelWrapper(model));
                }
            };


            ct.Register(() => IsBusy = false);
            await SWAPIlib.ComConn.SwAppControl.GetOpenedModels(visibleModelToWrapper, ct);
            IsBusy = false;
        }

        private static IModelWrapper BuildModelWrapper(ModelDoc2 model)
        {
            return new ModelWrapper(model) as IModelWrapper;
        }


    }
}
