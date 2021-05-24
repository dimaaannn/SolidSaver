using SWAPIlib.Property.ModelProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property
{
    /// <summary>
    /// Создаёт недостающие дефолтные свойтства
    /// </summary>
    internal class DefaultPropertyBuilder
    {
        public PropertySet FillDefaultProperty(
            IPropertyBuilder builder
            , IPropertySettings settings = null)
        {
            PropertySet ret = new PropertySet(builder.Target);
            IProperty newProperty;

            var reqSet = new HashSet<string>(builder.GetterRequirement);
            reqSet.ExceptWith(settings.Select(x => x.Key));

            foreach (var requirementName in reqSet)
            {
                newProperty = GetSimpleProperty(builder.Target, requirementName);
                if (newProperty != null)
                    ret.Properties.Add(requirementName, newProperty);
            }

            return ret;
        }

        public IComplexProperty GetSimpleProperty(ITarget target, string entityName)
        {
            IComplexProperty ret = null;
            ModelEntities entity;
            IPropertyGetter2 getter;

            if (Enum.TryParse<ModelEntities>(entityName, out entity))
            {
                getter = DefaultGetter.Get(entity);
                ret = new ComplexProperty(target, getter);
            }
            return ret;
        }

    }

    internal static class DefaultGetter
    {
        public static IPropertyGetter2 Get(ModelEntities entity)
        {
            IPropertyGetter2 ret = null;
            switch (entity)
            {
                case ModelEntities.ConfigName:
                    ret = new ActiveConfigNameGetter();
                    break;
                case ModelEntities.FileName:
                    ret = new FileNameGetter();
                    break;
                case ModelEntities.UserSelection:
                    break;
                case ModelEntities.Title:
                    ret = new TitleGetter();
                    break;
                case ModelEntities.UserProperty:
                    break;
                case ModelEntities.UserPropertyName:
                    break;
                default:
                    ret = null;
                    break;
            }
            return ret;
        }
    }
}
