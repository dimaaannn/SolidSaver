using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property.ModelProperty
{

    /// <summary>
    /// Пользовательское поле в свойствах детали
    /// </summary>
    public class GetUserParam : ModelPropertyGetterBase
    {
        public override ModelEntities ModelEntityType => ModelEntities.UserProperty;

        public override GetterType GetterType => GetterType.IsReadable | 
            GetterType.IsWritable | 
            GetterType.ReadParamRequired | 
            GetterType.WriteParamRequired;

        public override HashSet<string> OptionsRequirement => new HashSet<string>() { 
            ModelEntities.ConfigName.ToString(),
            ModelEntities.UserPropertyName.ToString()
        };

        public override string Info => "Пользовательское свойство";

        public override string Name => ModelPropertyNames.UserProperty.ToString();

        public override TargetType TargetType => TargetType.Model | TargetType.Component;

        public override bool CheckTarget(ITarget target, IPropertySettings settings)
        {
            bool ret = false;
            ret = CheckTargetType(target);
            ret &= CheckPropertySettingRequirements(settings);
            return ret;
        }

        public override string EntityType => ModelEntities.ConfigName.ToString();

        /// <summary>
        /// Получить пользовательское свойство
        /// </summary>
        /// <param name="target"></param>
        /// <param name="settings">Требуется указать имя конфигурации и имя свойства</param>
        /// <returns></returns>
        public override IComplexValue GetValue(ITarget target, IPropertySettings settings)
        {
            IComplexValue ret = null;
            switch (target)
            {
                case ITarget<ModelDoc2> model:
                    ret = GetModelUserProperty(model.Target, settings);
                    break;
                case ITarget<Component2> component:
                    ret = GetComponentUserProperty(component.Target, settings);
                    break;
                default:
                    break;
            }

            if(ret != null)
                ret["Name"] = target.TargetName;

            return ret;
        }

        public override bool SetValue(ITarget target, IComplexValue newValue, IPropertySettings settings)
        {
            bool ret = false;
            Logger.Info("GetUserParam: value set {value} ", newValue);
            switch (target)
            {
                case ITarget<ModelDoc2> modelTarget:
                    ret = SetModelUserProperty(modelTarget.Target, settings, newValue);
                    break;
                case ITarget<Component2> componentTarget:
                    ret = SetComponentUserProperty(componentTarget.Target, settings, newValue);
                    break;
                default:
                    break;
            }
            return ret;
        }


        /// <summary>
        /// Получить пользовательское свойство
        /// </summary>
        /// <param name="model"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public IComplexValue GetModelUserProperty(ModelDoc2 model, IPropertySettings settings)
        {
            IComplexValue ret = null;
            string config = settings[ModelEntities.ConfigName.ToString()];
            string userKey = settings[ModelEntities.UserPropertyName.ToString()];

            string value = SWAPIlib.ComConn.Proxy.ModelConfigProxy.GetConfParamValue(
                swModel: model,
                configName: config,
                fieldName: userKey);
            if(value != null)
            {
                ret = CreateResultObj();
                ret.BaseValue = value;
                ret[ModelEntities.ConfigName.ToString()] = config;
                ret[ModelEntities.UserPropertyName.ToString()] = userKey;
            }

            return ret;
        }

        public IComplexValue GetComponentUserProperty(Component2 comp, IPropertySettings settings)
        {
            IComplexValue ret = null;
            ModelDoc2 model = comp.GetModelDoc2();
            if(model != null)
            {
                ret = GetModelUserProperty(model, settings);
            }
            return ret;
        }

        public bool SetModelUserProperty(ModelDoc2 model, IPropertySettings settings, IComplexValue value)
        {
            bool ret = false;
            string config = settings[ModelEntities.ConfigName.ToString()];
            string userKey = settings[ModelEntities.UserPropertyName.ToString()];

            string valueString = value[ModelPropertyNames.UserProperty.ToString()] ?? value.BaseValue;

            if(valueString != null)
            {
                ret = SWAPIlib.ComConn.Proxy.ModelConfigProxy.SetConfParam(
                    swModel: model
                    ,configName: config
                    ,fieldName: userKey
                    ,fieldVal: valueString);
            }
            return ret;
        }

        public bool SetComponentUserProperty(Component2 component2, IPropertySettings settings, IComplexValue value)
        {
            
            ModelDoc2 model = component2.GetModelDoc2();
            if (model != null)
                return SetModelUserProperty(model, settings, value);
            else
                return false;
        }
    }

}
