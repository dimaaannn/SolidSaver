using SWAPIlib.Property.ModelProperty;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SWAPIlib.Property
{
    public interface IPropertyView
    {
        IProperty Name { get; set; }
        IProperty Info { get; set; }
        IProperty Main { get; set; }

        bool Write();
        bool Update();

        //List<IProperty> PropertyList { get; }
    }



    /// <summary>
    /// Объект строки свойств с доп. данными
    /// </summary>
    public class PropertyView : IPropertyView
    {
        public PropertyView()
        {
            
            PropertyList = new List<IProperty>();
        }
        public IProperty Name { get; set; }
        public IProperty Info { get; set; }
        public IProperty Main
        {
            get => PropertyList.Count > 0 ? PropertyList.First() : null;
            set
            {
                PropertyList.Insert(0, value);
                if (PropertyList.Count > 1)
                    PropertyList.RemoveAt(1);
            }
        }

        /// <summary>
        /// Записать значения
        /// </summary>
        /// <returns></returns>
        public bool Write() => 
            PropertyList
                .Select(prop => WriteValueIfChanged(prop))
                .Aggregate((x, y) => x & y);
        /// <summary>
        /// Обновить значения
        /// </summary>
        /// <returns></returns>
        public bool Update() => 
            PropertyList
                .Select(prop => prop.Update())
                .Aggregate((x, y) => x & y);

        public List<IProperty> PropertyList { get; protected set; }
        public void Add(IProperty property) => PropertyList.Add(property);

        //public override string ToString() => $"{Name?.Value}: {Main?.Value}";

        /// <summary>
        /// Запись производится только в случае изменения значения
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected bool WriteValueIfChanged(IProperty property)
        {
            bool ret = false;
            if (property?.TempValue != null)
            {
                ret = property.WriteValue();
            }
            else
                ret = true;
            return ret;
        }
    }


    /// <summary>
    /// Создаёт строку с предустановленными свойствами имени и информации
    /// </summary>
    public class PropertyViewBuilder
    {
        private ITarget infoFieldTarget;

        public PropertyViewBuilder()
        {
            Name = new PropertyBuilder2() { Getter = new TitleGetter() };
            Info = new PropertyBuilder2() { Getter = new FileNameGetter() };
        }

        public PropertyViewBuilder(IPropertyGetter2 getter) : this()
        {
            Main = new PropertyBuilder2() { Getter = getter };
        }


        public IPropertyBuilder Name { get; set; }
        public IPropertyBuilder Info { get; set; }
        public IPropertyBuilder Main { get; set; }
        public ITarget Target { get; set; }


        private IProperty nameCashe;
        private IProperty infoCashe;

        protected IProperty NameCashe
        {
            get => nameCashe ?? (nameCashe = Name.IsPropertyValid ? Name.Build() : null);
            set => nameCashe = value;
        }
        protected IProperty InfoCashe 
        {
            get => infoCashe ?? (infoCashe = Info.IsPropertyValid ? Info.Build() : null); 
            set => infoCashe = value; 
        }

        /// <summary>
        /// Объект привязки для создания свойств Name и Info
        /// </summary>
        protected ITarget InfoFieldTarget
        {
            get => infoFieldTarget;
            set
            {
                infoFieldTarget = value;
                //Очистить кэшированные свойства
                NameCashe = null;
                InfoCashe = null;
            }
        }

        /// <summary>
        /// Создать на основе объекта
        /// </summary>
        /// <param name="target">Объект привязки</param>
        /// <returns></returns>
        public IPropertyView BuildPropertyView(ITarget target)
        {
            IPropertyView ret = new PropertyView();
            Target = target;

            if (InfoFieldTarget != Target)
                BuildInfoFields(Target);

            Main.Target = target;
            ret.Name = NameCashe;
            ret.Info = InfoCashe;
            ret.Main = Main.IsPropertyValid ? Main.Build() : null;

            //Clear main target
            Main.Target = null;

            return ret;
        }

        public IPropertyView BuildPropertyView(ITarget target, IPropertySettings settings)
        {
            IPropertyView ret = new PropertyView();
            Target = target;
            Main.Settings = settings;

            if (InfoFieldTarget != Target)
                BuildInfoFields(Target);

            Main.Target = target;
            ret.Name = NameCashe;
            ret.Info = InfoCashe;
            ret.Main = Main.IsPropertyValid ? Main.Build() : null;

            //Clear main target
            Main.Target = null;

            return ret;
        }

        /// <summary>
        /// Создать на основе геттера
        /// </summary>
        /// <param name="getter"></param>
        /// <returns></returns>
        public IPropertyView BuildPropertyView(IPropertyGetter2 getter)
        {
            if (Target == null)
                throw new MissingFieldException ("PropertyViewBuilder:BuildPropertyView Отсутствует объект привязки");

            IPropertyView ret = null;
            if (getter != null)
            {
                Main.Getter = getter;
                ret = new PropertyView
                {
                    Name = NameCashe,
                    Info = InfoCashe,
                    Main = Main.IsPropertyValid ? Main.Build() : null
                };
            }
            return ret;
        }


        /// <summary>
        /// Обновить привязку информационных полей
        /// </summary>
        /// <param name="target"></param>
        protected void BuildInfoFields(ITarget target)
        {
            InfoFieldTarget = target;

            Name.Target = InfoFieldTarget;
            Info.Target = InfoFieldTarget;
            NameCashe = null;
            InfoCashe = null;
        }
    }
}
