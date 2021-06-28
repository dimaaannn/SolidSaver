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

        public async Task GetOpenedDocumentsAsync(CancellationToken ct, Action<IModelWrapper> modelAction)
        {
            if (IsBusy == false)
            {
                IsBusy = true;
                ct.Register(() => IsBusy = false);
                Action<ModelDoc2> visibleModelToWrapper = model =>
                {
                    if (model.Visible)
                    {
                        modelAction(BuildModelWrapper(model));
                    }
                };

                ct.Register(() => IsBusy = false);
                await SWAPIlib.ComConn.SwAppControl.GetOpenedModelsAsync(visibleModelToWrapper, ct);
                IsBusy = false;
            }
            else
                throw new InvalidAsynchronousStateException("DocLoader: Can't execute when IsBusy state enabled");
        }

        private static IModelWrapper BuildModelWrapper(ModelDoc2 model)
        {
            return new ModelWrapper(model) as IModelWrapper;
        }

        public async Task GetActiveDoc(CancellationToken ct, Action<IModelWrapper> modelAction)
        {
            bool prevBusyState = IsBusy;
            IsBusy = true;
            ct.Register(() => IsBusy = prevBusyState);

            ModelDoc2 model = await SWAPIlib.ComConn.SwAppControl.GetActiveModelAsync(ct);
            IModelWrapper wrapper = await Task<IModelWrapper>.Run(() => 
                BuildModelWrapper(model));
            modelAction(wrapper);

            IsBusy = prevBusyState;
        }

        public async Task GetActiveDoc(CancellationToken ct) =>
            await GetActiveDoc(ct, ModelAction);

    }
}
