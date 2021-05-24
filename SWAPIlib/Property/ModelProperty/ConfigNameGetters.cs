using SolidWorks.Interop.sldworks;
using System;
namespace SWAPIlib.Property.ModelProperty
{


    /// <summary>
    /// Имя активной конфигурации
    /// </summary>
    public class ActiveConfigNameGetter : ModelPropertyGetterBase
    {
        public ActiveConfigNameGetter()
        {
            Name = ModelPropertyNames.ActiveConfigName.ToString();
            Info = "Имя активной конфигурации";
            TargetType = TargetType.Model | TargetType.Component;
            ModelEntityType = ModelEntities.ConfigName;
        }
        public override GetterType GetterType => GetterType.IsReadable
                | GetterType.IsWritable
                | GetterType.WriteParamRequired;
        public override string Name { get; }
        public override string Info { get; }
        public override TargetType TargetType { get; }
        public override ModelEntities ModelEntityType { get; }


        public override bool CheckTarget(ITarget target, IPropertySettings settings)
        {
            bool typeCompare;
            typeCompare = CheckTargetType(target);
            Logger.Trace("ActiveConfigGetter CheckTarget = {state}", typeCompare);
            return typeCompare;
        }
        public override IComplexValue GetValue(ITarget target, IPropertySettings settings)
        {
            string result = null;

            switch (target)
            {
                case ITarget<ModelDoc2> model:
                    result = SWAPIlib.ComConn.Proxy.ModelConfigProxy.GetActiveConfName(model.Target);
                    break;
                case ITarget<Component2> comp:
                    result = SWAPIlib.ComConn.Proxy.ComponentProxy.RefConfigName(comp.Target);
                    break;
                default:
                    break;
            }

            var ret = CreateResultObj();
            ret.BaseValue = result;
            return ret;
        }
        public override bool SetValue(ITarget target, IComplexValue newValue, IPropertySettings settings)
        {
            bool ret = false;
            //Если свойство активной конфигурации присутствует
            string propName = ModelEntities.ConfigName.ToString();
            string newConfName = newValue[propName] ?? newValue[null];

            if (string.IsNullOrEmpty(newConfName))
            {
                Logger.Error("ActiveConfigNameGetter {target} SetValue: Отсутствует свойство с именем {propName}",target, propName);
                return ret;
            }
            if ((target.TargetType | TargetType.Model) == TargetType.Model)
            {
                ret = SWAPIlib.ComConn.Proxy.ModelConfigProxy.SetActiveConf((SolidWorks.Interop.sldworks.ModelDoc2)target.GetTarget(), newConfName);
                Logger.Info("ActiveConfigNameGetter {target}  SetValue to {newProp} is {propName}", target, newConfName, ret);
                return ret;
            }
            else if (target.TargetType == TargetType.Component)
            {
                ret = SWAPIlib.ComConn.Proxy.ComponentProxy.SetRefConfig((SolidWorks.Interop.sldworks.Component2)target.GetTarget(), newConfName);
                Logger.Info("ActiveConfigNameGetter {target} SetValue to {newProp} is {propName}", target, newConfName, ret);
                return ret;
            }
            Logger.Error("ActiveConfigNameGetter {target} SetValue Некорректный тип", target);
            throw new ArgumentException($"ActiveConfigNameGetter не совместим с {target?.TargetName}");
        }
    }

    /// <summary>
    /// Список имён конфигураций
    /// </summary>
    public class ConfigListGetter : ModelPropertyGetterBase
    {

        public override GetterType GetterType => GetterType.IsReadable;
        public override string Name => ModelPropertyNames.ConfigNamesList.ToString();
        public override string Info => "Список конфигураций";
        public override TargetType TargetType => TargetType.Model | TargetType.Component;
        public override ModelEntities ModelEntityType => ModelEntities.ConfigName;

        public override bool CheckTarget(ITarget target, IPropertySettings settings)
        {
            bool typeCompare;
            typeCompare = CheckTargetType(target);

            //Проверка для компонента
            if (target.TargetType.HasFlag(TargetType.Component))
            {
                var comp = (SolidWorks.Interop.sldworks.IComponent2) target.GetTarget();
                bool isSuppressed = comp.IsSuppressed() == false;
                typeCompare &= isSuppressed;
            }

            Logger.Trace("ConfigListGetter CheckTarget = {state}", typeCompare);
            return typeCompare;
        }

        public override IComplexValue GetValue(ITarget target, IPropertySettings settings)
        {
            string[] result = null;
            if ((target.TargetType | TargetType.Model) == TargetType.Model)
            {
                result = SWAPIlib.ComConn.Proxy.ModelConfigProxy.GetConfigList((SolidWorks.Interop.sldworks.ModelDoc2)target.GetTarget());
            }

            else if (target.TargetType == TargetType.Component && target.GetTarget() is SolidWorks.Interop.sldworks.IComponent2 comp)
            {
                Logger.Trace("ConfigListGetter: Try get confNames from component {name}", target.TargetName);
                SolidWorks.Interop.sldworks.ModelDoc2 swModel = comp.GetModelDoc2();
                if(swModel != null)
                {
                    result = SWAPIlib.ComConn.Proxy.ModelConfigProxy.GetConfigList(swModel);
                }
            }
            
            //Если количество конфигураций больше нуля
            if(result.Length > 0)
            {
                var resultObj = new ComplexValue(Name);
                for(int i = 0; i < result.Length; i++)
                {
                    resultObj[EntityType + " " + i.ToString()] = result[i];
                    //resultObj.ValuePairs.Add(ModelPropertyNames.ConfigName.ToString() + i, result[i]);
                }
                Logger.Info("ConfigListGetter: {name} return {num} configs", target.TargetName, result.Length);

                return resultObj;
            }
            else
            {
                Logger.Info("ConfigListGetter: {name} отсутствуют конфигурации", target.TargetName);
                return null;
            }
        }
        /// <summary>
        /// Запись невозможна
        /// </summary>
        /// <param name="target"></param>
        /// <param name="newValue"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override bool SetValue(ITarget target, IComplexValue newValue, IPropertySettings settings)
        {
            Logger.Error("ConfigListGetter {target} SetValue Операция недоступна", target);
            throw new InvalidOperationException($"ConfigListGetter Запись значения невозможна");
        }
    }
}
