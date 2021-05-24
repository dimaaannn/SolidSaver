using SWAPIlib.Property.PropertyBase;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property.PropertyBase
{
    public abstract class PropertyGetterBase : IPropertyGetter2
    {

        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private HashSet<string> optionsRequirement = new HashSet<string>();

        /// <summary>
        /// Набор требуемых настроек
        /// </summary>
        public virtual HashSet<string> OptionsRequirement { get => optionsRequirement; }
        /// <summary>
        /// Проверка совместимости флагов типов
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool CheckTargetType(ITarget target)
        {

            if (TargetType.HasFlag(target?.TargetType))
                return true;

            //if (this.TargetType == (target?.TargetType | this.TargetType))
            //    return true;
            else 
                return false;
        }
        /// <summary>
        /// Создать объект результата
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected IComplexValue CreateResultObj()
        {
            var ret = new ComplexValue(Name);
            return ret;
        }


        /// <summary>
        /// Тип сущности возвращаемая свойством (имя конфигурации, имя файла и т.п.)
        /// </summary>
        public abstract string EntityType { get; }
        /// <summary>
        /// Флаги настроек и требований
        /// </summary>
        public abstract GetterType GetterType { get; }
        /// <summary>
        /// Описание свойства
        /// </summary>
        public abstract string Info { get; }
        /// <summary>
        /// Уникальное имя свойства
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Требуемый тип цели
        /// </summary>
        public abstract TargetType TargetType { get; }

        public abstract bool CheckTarget(ITarget target, IPropertySettings settings);
        public abstract IComplexValue GetValue(ITarget target, IPropertySettings settings);
        public abstract bool SetValue(ITarget target, IComplexValue newValue, IPropertySettings settings);

        /// <summary>
        /// Провести проверку на наличие требуемых параметров в настройках
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        protected bool CheckPropertySettingRequirements(IPropertySettings settings)
        {
            bool ret = true;
            foreach (string paramName in OptionsRequirement)
            {
                ret &= settings[paramName] != null;
            }
            return ret;
        }
    }
}
namespace SWAPIlib.Property.ModelProperty
{

}
