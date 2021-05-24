using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property.ModelProperty
{
    public enum ModelPropertySet
    {
        None,
        UserParam
    }

    public class PreDefinedModelList
    {

        public PreDefinedModelList()
        {
            ViewBuilders = new Dictionary<ModelPropertySet, Lazy<PropertyViewBuilder>>()
            {
                {ModelPropertySet.UserParam, new Lazy<PropertyViewBuilder>(

                    ) }
            };
        }

        public Dictionary<ModelPropertySet, Lazy<PropertyViewBuilder>> ViewBuilders { get; }

        public Dictionary<string, PropertySetBuilder> SettingsBuilders { get; }

        public PropertyViewBuilder GetPropBuilder(ModelPropertySet setName) 
        {
            throw new NotImplementedException();
        }


        private PropertySetBuilder UserPropertyNameProp(string userPropertyName, string refConfigName = null)
        {
            PropertySetBuilder settingsBuilder = new PropertySetBuilder();

            string configParamName = ModelEntities.ConfigName.ToString();
            if (refConfigName == null)
                settingsBuilder.Add(configParamName, new ActiveConfigNameGetter());
            else
                settingsBuilder.Add(configParamName, refConfigName);

            string userPropParamName = ModelEntities.UserPropertyName.ToString();
            settingsBuilder.Add(userPropParamName, userPropertyName);

            return settingsBuilder;
        }

    }



}
