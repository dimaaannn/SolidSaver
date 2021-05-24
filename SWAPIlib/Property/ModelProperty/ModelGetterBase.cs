namespace SWAPIlib.Property.ModelProperty
{
    /// <summary>
    /// Абстрактный класс для свойства моделей
    /// </summary>
    public abstract class ModelPropertyGetterBase : PropertyBase.PropertyGetterBase
    {
        public override string EntityType => ModelEntityType.ToString();

        /// <summary>
        /// Привязка к набору сущностей модели
        /// </summary>
        public abstract ModelEntities ModelEntityType { get; }

        /// <summary>
        /// Проверить совместимость типов
        /// </summary>
        /// <param name="target"></param>
        /// <param name="settings"></param>
        /// <param name="logClassName"></param>
        /// <returns></returns>
        protected bool CheckTargetType(ITarget target, IPropertySettings settings, string logClassName)
        {
            bool typeCompare;
            typeCompare = CheckTargetType(target);
            Logger.Trace("{className} CheckTargetType = {state}", logClassName, typeCompare);
            return typeCompare;
        }
    }


    /// <summary>
    /// Имена конкретных свойств
    /// </summary>
    public enum ModelPropertyNames
    {
        None,
        ActiveConfigName,
        ConfigNamesList,
        FileName,
        FilePath,
        Title,
        UserProperty
    }

    /// <summary>
    /// Имена сущностей для использования в параметрах
    /// </summary>
    public enum ModelEntities
    {
        None,
        ConfigName,
        FileName,
        UserSelection,
        Title,
        UserProperty,
        UserPropertyName
    }
}
