using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.PropertyObj
{
    public delegate string CompPropGetter(IAppComponent<AppComponent> appComponent);
    public delegate bool CompPropSetter(IAppComponent<AppComponent> appComponent, string val);

    public class ComponentBaseInfo : ComponentProperty
    {

        public ComponentBaseInfo(IAppComponent<AppComponent> appComponent)
        {
            AppComponent = appComponent;
            Validator = CompValidator.IsAppComp;
        }

        public CompPropGetter PropGetter;
        public CompPropSetter PropSetter;

        public override string ReadValue()
        {
            if (IsReadable)
                return PropGetter(AppComponent);
            else
                return null;
        }
        public override PropValidator Validator { get; set; }

        public override void Update()
        {
            if(Validator(AppComponent as ISwModel))
            {
                _propertyValue = ReadValue();
            }
            _tempPropertyValue = null;
        }

        public override bool WriteValue()
        {
            bool ret = false;
            if(IsWritable && _tempPropertyValue != null)
            {
                if(PropSetter.Invoke(AppComponent, _tempPropertyValue))
                {
                    _tempPropertyValue = null;
                    ret = true;
                }
            }
            return ret;
        }
    }

    public static class CompValidator
    {
        /// <summary>
        /// Является компонентом
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        public static bool IsAppComp(ISwModel swModel) => swModel is AppComponent;
        
    }
    public static class CompPropertyFactory
    {
        public static List<ComponentBaseInfo> ComponentMainProp(
            IAppComponent<AppComponent> appComponent)
        {
            var ret = new List<ComponentBaseInfo>();
            ret.Add(Title(appComponent));
            ret.Add(FileName(appComponent));
            ret.Add(RefConfig(appComponent));

            return ret;
        }

        public static ComponentBaseInfo Title(IAppComponent<AppComponent> appComponent)
        {
            return new ComponentBaseInfo(appComponent)
            {
                IsReadable = true,
                Validator = CompValidator.IsAppComp,
                UserName = "Имя компонента",
                PropGetter = CompGetter.GetTitle,
                IsWritable = false
                
            };
        }

        public static ComponentBaseInfo FileName(IAppComponent<AppComponent> appComponent)
        {
            return new ComponentBaseInfo(appComponent)
            {
                IsReadable = true,
                Validator = CompValidator.IsAppComp,
                UserName = "Имя компонента",
                PropGetter = CompGetter.GetFileName,
                IsWritable = false
            };
        }

        public static ComponentBaseInfo RefConfig(IAppComponent<AppComponent> appComponent)
        {
            return new ComponentBaseInfo(appComponent)
            {
                IsReadable = true,
                Validator = CompValidator.IsAppComp,
                UserName = "Имя компонента",
                PropGetter = CompGetter.GetRefConfig,
                IsWritable = false
            };
        }
    }

    public static class CompGetter
    {
        /// <summary>
        /// Title
        /// </summary>
        /// <param name="swModel"></param>
        /// <returns></returns>
        public static string GetTitle(IAppComponent<AppComponent> swModel)
            => (swModel as AppComponent).Title;

        public static string GetFileName(IAppComponent<AppComponent> swModel)
            => (swModel as AppComponent).FileName;

        public static string GetRefConfig(IAppComponent<AppComponent> swModel)
            => (swModel as AppComponent).RefConfigName;
    }

}
