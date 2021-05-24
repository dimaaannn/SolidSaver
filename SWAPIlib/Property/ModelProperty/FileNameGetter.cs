using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Property.ModelProperty
{
    public class FileNameGetter : ModelPropertyGetterBase
    {
        public override ModelEntities ModelEntityType => ModelEntities.FileName;

        public override GetterType GetterType => GetterType.IsReadable;

        public override string Info => "Путь файла";

        public override string Name => ModelPropertyNames.FileName.ToString();

        public override TargetType TargetType =>
            TargetType.Model | TargetType.Component;

        public override bool CheckTarget(ITarget target, IPropertySettings settings)
        {
            return CheckTargetType(target, settings, "FileNameGetter");
        }

        public override IComplexValue GetValue(ITarget target, IPropertySettings settings)
        {
            IComplexValue ret = null;
            switch (target)
            {
                case ITarget<IComponent2> comp:
                    ret = GetComponentFilePath(comp.Target);
                    break;
                case ITarget<ModelDoc2> model:
                    ret = GetModelFilePath(model.Target);
                    break;
                default:
                    break;
            }
            return ret;
        }
        protected IComplexValue GetComponentFilePath(IComponent2 comp)
        {
            ModelDoc2 model = comp.GetModelDoc2();
            if (model != null)
                return GetModelFilePath(model);
            else
                return null;
        }

        IComplexValue GetModelFilePath(ModelDoc2 model)
        {
            IComplexValue ret = new ComplexValue(Name);
            string path = SWAPIlib.ComConn.Proxy.ModelProxy.GetPathName(model);
            ret.BaseValue = System.IO.Path.GetFileName(path);
            ret[ModelPropertyNames.FilePath.ToString()] = path;
            return ret;
        }

        public override bool SetValue(ITarget target, IComplexValue newValue, IPropertySettings settings)
        {
            throw new InvalidOperationException("FilePathGetter недоступен для записи");
        }
    }

    public class TitleGetter : ModelPropertyGetterBase
    {
        public override ModelEntities ModelEntityType => ModelEntities.Title;

        public override GetterType GetterType => GetterType.IsReadable;

        public override string Info => "Название";

        public override string Name => ModelEntities.Title.ToString();

        public override TargetType TargetType => TargetType.Component | TargetType.Model;

        public override bool CheckTarget(ITarget target, IPropertySettings settings)
        {
            return CheckTargetType(target, settings, "TitleGetter");
        }

        public override IComplexValue GetValue(ITarget target, IPropertySettings settings)
        {
            IComplexValue ret = null;
            switch (target)
            {
                case ITarget<ModelDoc2> model:
                    ret = GetModelTitle(model.Target);
                    break;
                case ITarget<Component2> comp:
                    ret = GetComponentTitle(comp.Target);
                    break;
                default:
                    break;
            }
            return ret;
        }

        public override bool SetValue(ITarget target, IComplexValue newValue, IPropertySettings settings)
        {
            throw new InvalidOperationException("TitleGetter недоступен для записи");
        }

        /// <summary>
        /// Получить название модели
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IComplexValue GetModelTitle(ModelDoc2 model)
        {
            IComplexValue ret = null;
            if (model != null)
            {
                ret = new ComplexValue(ModelEntities.Title.ToString());
                ret.BaseValue = model.GetTitle();
            }
            return ret;
        }
        /// <summary>
        /// Получить название компонента
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static IComplexValue GetComponentTitle(Component2 comp)
        {
            IComplexValue ret = null;
            if(comp != null)
            {
                ret = new ComplexValue(ModelEntities.Title.ToString());
                ret.BaseValue = comp.Name2;
            }
            return ret;
        }

        /// <summary>
        /// Задать название модели
        /// </summary>
        /// <param name="model"></param>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public static bool SetModelTitle(ModelDoc2 model, IComplexValue keyValuePairs)
        {
            bool ret = false;
            if(keyValuePairs != null)
            {
                string newData = keyValuePairs[ModelPropertyNames.Title.ToString()];
                string baseVal = keyValuePairs.BaseValue;

                ret = model.SetTitle2( newData ?? baseVal);
            }
            return ret;
        }
        /// <summary>
        /// Задать название компонента
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public static bool SetComponentTitle(Component2 comp, IComplexValue keyValuePairs)
        {
            bool ret = false; ;
            if(keyValuePairs != null)
            {
                string newData = keyValuePairs[ModelPropertyNames.Title.ToString()];
                string baseVal = keyValuePairs.BaseValue;

                comp.Name2 = newData ?? baseVal;
                ret = comp.Name2 == (newData ?? baseVal);
            }
            return ret;
        }

    }


}
