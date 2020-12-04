using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.VisualInterface
{
    public interface IMainModel
    {
        AppModel RootModel { get; set; }
        AppDocType DocType { get; }
        string Title { get; }
        string Path { get; }
        IList<ISwProperty> PropList { get; }
        IFileModelProp GlobalModelProp { get; }

        IList<ISwComponent> SubComponents { get; }

        bool GetMainModel(string pathToModel);
        bool GetSubComponents();

        event EventHandler<SwEventArgs> CloseRootModel;

    }
}
