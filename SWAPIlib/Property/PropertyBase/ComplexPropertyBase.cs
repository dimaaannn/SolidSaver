using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property.PropertyBase
{
    /// <summary>
    /// Базовый абстрактный класс составного свойства
    /// </summary>
    public abstract class ComplexPropertyBase : PropertyBase, IComplexProperty
    {
        #region Fields
        private IComplexValue resultValue;
        private readonly ITarget target;
        private readonly IPropertyGetter2 propertyGetter;
        protected readonly string loggerClassname = "ComplexPropertyBase";
        protected static readonly string loggerFormat = " name: {name} with target {target} ";

        protected IComplexValue tempComplexValue;
        private bool isValid = false;
        #endregion

        public static readonly Dictionary<object, List<WeakReference<IComplexProperty>>> propertyList = new Dictionary<object, List<WeakReference<IComplexProperty>>>();

        public static void AddToPropertyList(object target, IComplexProperty property)
        {
            if (!propertyList.ContainsKey(target))
                propertyList.Add(target, new List<WeakReference<IComplexProperty>>());

            propertyList[target].Add(new WeakReference<IComplexProperty>(property));
        }
        public static List<WeakReference<IComplexProperty>> GetFromPropertyList(object target)
        {
            List<WeakReference<IComplexProperty>> ret;
            bool isValueExist = propertyList.TryGetValue(target, out ret);
            if (isValueExist)
                return ret;
            else 
                return null;
        }

        #region Constructors
        /// <summary>
        /// Базовый конструктор
        /// </summary>
        private ComplexPropertyBase() : base()
        {
            Logger.Trace("ComplexPropertyBase launch constructor");
        }
        /// <summary>
        /// Основной конструктор
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyGetter"></param>
        /// <param name="propertySettings"></param>
        protected ComplexPropertyBase(
            ITarget target
            , IPropertyGetter2 propertyGetter
            , IPropertySettings propertySettings = null
            ) : this()
        {
            this.target = target;
            this.propertyGetter = propertyGetter;

            switch (propertySettings)
            {
                case PropertySet complexSettings:
                    PropertySettings = complexSettings;
                    break;
                case IPropertySettings propSettings:
                    PropertySettings = new PropertySet(target);
                    foreach (var property in propSettings)
                    {
                        PropertySettings.AddTextProperty(property.Key, property.Value);
                    }
                    break;
                default:
                    break;
            }

            AddToPropertyList(target.GetTarget(), this);
            
        }

        #endregion


        #region Class Property
        /// <summary>
        /// Возможность использования свойства
        /// </summary>
        public bool IsValid {
            get
            {
                if (isValid == false)
                {
                    isValid = CheckValidation(Target, PropertyGetter, PropertySettings);
                }
                return isValid;
            }
        }
        /// <summary>
        /// Доступен для записи
        /// </summary>
        public override bool IsReadOnly => !PropertyGetter?.GetterType.HasFlag(GetterType.IsWritable) ?? true;
        /// <summary>
        /// Описание свойства
        /// </summary>
        public override string Info => PropertyGetter.Info;

        /// <summary>
        /// Обработчик свойств
        /// </summary>
        public IPropertyGetter2 PropertyGetter => propertyGetter;
        /// <summary>
        /// Цель обработки
        /// </summary>
        public ITarget Target => target;
        /// <summary>
        /// Настройки параметров
        /// </summary>
        public PropertySet PropertySettings { get; protected set; }


        #endregion



        #region Value operations
        /// <summary>
        /// Полученный объект
        /// </summary>
        public override string Value
        {
            get
            {
                if (IsValid)
                {
                    //Обновить значение в случае отсутствия кэша
                    if (ResultValue == null || ResultValue.BaseValue == null)
                    {
                        Logger.Info(loggerClassname + loggerFormat + "invoke Auto update", Name, Target.TargetName);
                        Update();
                    }
                    //Возвратить базовое значение кэша
                    if (ResultValue != null)
                    {
                        Logger.Trace(loggerClassname + loggerFormat + "get cashed value {value}", Name, Target.TargetName, ResultValue.BaseValue);
                        return ResultValue.BaseValue;
                    }
                    //Если возвращён null - выбросить исключение
                    else
                    {
                        Logger.Warn(loggerClassname + loggerFormat + "Return null value", Name, Target.TargetName);
                        throw new InvalidOperationException($"ComplexPropertyBase: Свойство {Name} возвращено с нулевым значением, и валидно");

                        return null;
                    }
                }
                else
                {
                    Logger.Info(loggerClassname + loggerFormat + "Get value from not valid property", Name, Target.TargetName);
                    return null;
                }
            }
        }
        /// <summary>
        /// Результат обработки
        /// </summary>
        public IComplexValue ResultValue { get => resultValue; protected set => resultValue = value; }

        /// <summary>
        /// Обновить значение
        /// </summary>
        /// <returns></returns>
        public override bool Update()
        {
            if (IsValid != true) //Валидировать если статус не определён
            {
                Logger.Info(loggerClassname + loggerFormat + "CheckTarget is {status}", Name, Target, IsValid);
            }
            if (IsValid) //Обновить кэш
            {
                ResultValue = PropertyGetter.GetValue(Target, PropertySettings);
                bool result = ResultValue != null ? true : false;
                if (result)      //очистить сохранённые значения в случае успеха
                {
                    TempComplexValue = null;
                }
                Logger.Info(loggerClassname + loggerFormat + "Property updated with {result}", Name, Target, result);

                OnPropertyChanged("Value");
                OnPropertyChanged("TempValue");

                return result;
            }
            else
            {
                Logger.Warn(loggerClassname + loggerFormat + "Property invalid", Name, Target);
                ResultValue = null;
                return false;
            }
        }
        /// <summary>
        /// Задать новое значение
        /// </summary>
        /// <returns></returns>
        public override bool WriteValue()
        {
            Logger.Trace(loggerClassname + loggerFormat + "WriteValue invoked", Name, Target);

            if (IsValid == true && TempComplexValue != null)
            {
                bool ret = PropertyGetter.SetValue(Target, TempComplexValue, PropertySettings);
                Logger.Trace(loggerClassname + loggerFormat + "Property {prop} writed with{result}", Name, Target, TempComplexValue, ret);

                if (ret)
                {
                    //Очистить временное значение
                    TempComplexValue = null;
                    //Очистить сохранённое значение
                    ResultValue = null;
                }

                OnPropertyChanged("Value");
                OnPropertyChanged("ResultValue");
                OnPropertyChanged("TempValue");
                OnPropertyChanged("TempComplexValue");
                return ret;
            }
            else
            {
                Logger.Warn(loggerClassname + loggerFormat + "WriteValue: property not valid", Name, Target);
                throw new InvalidOperationException("ComplexPropertyBase:Write error. Property is not valid");
                return false;
            }
        }
        #endregion


        #region TempValue operations

        /// <summary>
        /// Временное значение свойства
        /// </summary>
        public override string TempValue
        {
            get
            {
                if (TempComplexValue != null)   //Возвратить значение по умолчанию
                {
                    Logger.Trace(loggerClassname + loggerFormat + "get temp val {value}", Name, Target.TargetName, ResultValue.BaseValue);
                    return TempComplexValue.BaseValue;
                }

                else                            //Вывести ошибку в логи
                {
                    Logger.Info(loggerClassname + loggerFormat + "get temp value without TempComplexVal object", Name, Target.TargetName);
                    return null;
                }
            }
            set
            {
                Logger.Trace("ComplexPropertyBase try set value: {value}", value);

                if (value == null)              //Удалить объект, если передан нуль
                {
                    Logger.Info("ComplexPropertyBase clear temp value");
                    TempComplexValue = null;
                    return;
                }

                if (TempComplexValue == null)   //Создать временный объект если отсутствует
                {
                    TempComplexValue = new ComplexValue(PropertyGetter.Name);
                }

                if (TempComplexValue != null)   //Поместить значение в временный объект
                {
                    TempComplexValue.BaseValue = value;
                    Logger.Info("ComplexPropertyBase user value set to: {value}", value);

                    //Обновить параметры
                    OnPropertyChanged("Value");
                    OnPropertyChanged("TempValue");
                }
                else                            //Выбросить ошибку
                {
                    Logger.Warn(loggerClassname + loggerFormat + "set temp value:{value} without Result object", Name, Target.TargetName, value);
                    throw new AggregateException("ComplexPropertyBase: Попытка записи в временное значение без объекта Result");
                }
            }
        }

        /// <summary>
        /// Объект для записи
        /// </summary>
        public IComplexValue TempComplexValue
        {
            get => tempComplexValue;
            set
            {
                Logger.Info(loggerClassname + loggerFormat + "tempCompexValue set to", Name, Target.TargetName, value);
                tempComplexValue = value;
            }
        }

        public Dictionary<object, List<WeakReference<IComplexProperty>>> PropertyList => propertyList;


        #endregion


        /// <summary>
        /// Проверка совместимости компонентов свойства
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyGetter"></param>
        /// <param name="propertySettings"></param>
        /// <returns></returns>
        public virtual bool CheckValidation(ITarget target, IPropertyGetter2 propertyGetter, IPropertySettings propertySettings)
        {
            bool ret = false;
            Logger.Trace(loggerClassname + loggerFormat + "invoke CheckValidation", Name, Target);
            ret = PropertyGetter.CheckTarget(Target, PropertySettings);
            Logger.Info(loggerClassname + loggerFormat + "CheckValidation {result}", Name, Target, ret);
            return ret;
        }

        public bool IsSimple()
        {
            bool ret = true;
            ret &= IsReadOnly;
            ret &= !PropertyGetter.GetterType.HasFlag(GetterType.IsWritable); //Not writable
            return ret;
        }

        public override IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ResultValue.GetEnumerator();
        }
    }
}


namespace SWAPIlib.Property
{
    
    /// <summary>
    /// Составное свойство
    /// </summary>
    public class ComplexProperty : SWAPIlib.Property.PropertyBase.ComplexPropertyBase
    {
        public ComplexProperty(ITarget target, IPropertyGetter2 getter, IPropertySettings settings = null) : base(target, getter, settings)
        {
        }

        /// <summary>
        /// Имя свойства
        /// </summary>
        public override string Name => PropertyGetter.Name;

    }
}
