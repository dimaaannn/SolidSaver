using SWAPIlib.Property.ModelProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property
{
    public enum PropertyTemplate
    {
        UserParamDenomination,
        UserParamName
    }

    public class PropertySetTemplates
    {
        public static PropertySetBuilder2 Get(PropertyTemplate templateName)
        {
            PropertySetBuilder2 ret = null;
            switch (templateName)
            {
                case PropertyTemplate.UserParamDenomination:
                    ret = GetUserParam("Обозначение");
                    break;
                case PropertyTemplate.UserParamName:
                    ret = GetUserParam("Наименование");
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static string GetName(PropertyTemplate templateName)
        {
            string ret;
            switch (templateName)
            {
                case PropertyTemplate.UserParamDenomination:
                    ret = "Обозначение";
                    break;
                case PropertyTemplate.UserParamName:
                    ret = "Наименование";
                    break;
                default:
                    ret = "По умолчанию";
                    break;
            }
            return ret;
        }

        public static PropertySetBuilder2 GetUserParam(string paramName, IPropertyGetter2 configNameGetter = null)
        {
            PropertySetBuilder2 settingsBuilder = new PropertySetBuilder2();

            if (configNameGetter != null)
                settingsBuilder.Add(ModelEntities.ConfigName.ToString(), configNameGetter);
            else
                settingsBuilder.Add(ModelEntities.ConfigName.ToString(), new ActiveConfigNameGetter());

            settingsBuilder.Add(ModelEntities.UserPropertyName.ToString(), paramName);
            settingsBuilder.Add(ModelEntities.UserProperty.ToString(), new GetUserParam());
            return settingsBuilder;
        }
    }
}
