using SWAPIlib;
using SWAPIlib.Controller;
using SWAPIlib.MProperty;
using SWAPIlib.PropertyObj;
using SWAPIlib.PropReplacer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SolidSaverWPF.PropertyView
{

    public interface IPropConfigurator
    {
        /// <summary>
        /// Загрузить модели из источника
        /// </summary>
        void LoadPartList();
        void ClearPartList();

        /// <summary>
        /// Начать поиск
        /// </summary>
        void BeginSearch();
        /// <summary>
        /// Отменить изменения
        /// </summary>
        void DropChanges();
        /// <summary>
        /// Записать новые значения
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Словарь доступных привязок
        /// </summary>
        Dictionary<string, IProperty> PropertySelector { get; }
        /// <summary>
        /// Список объектов свойств
        /// </summary>
        ICollectionView PropertyViewList { get; }

        /// <summary>
        /// Искомое значение
        /// </summary>
        string SearchValue { get; set; }
        /// <summary>
        /// Новое значение
        /// </summary>
        string NewValue { get; set; }
        /// <summary>
        /// Регистрозависимый поиск
        /// </summary>
        bool CaseSensitive { get; set; }

        /// <summary>
        /// Отобразить все конфигурации компонента
        /// </summary>
        bool ForEachConfig { get; set; }

    }
    /// <summary>
    /// Типизированный интерфейс
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public interface IPropConfigurator<TTarget> : IPropConfigurator
    {
        /// <summary>
        /// Задать источник
        /// </summary>
        void SetSource(IEnumerable<TTarget> items);
    }



    public class PropConfigurator : DependencyObject,
        IPropConfigurator<IComponentControl>
    {
        public PropConfigurator()
        {
            //this.propertySelector =
            TextReplacer = new TextReplacer();
            TextReplacer.UseRegExp = true;
            ModelPropMaker = new ModelPropMaker();
        }

        private Dictionary<string, IProperty> propertySelector;
        private IEnumerable<IComponentControl> SourceList;
        /// <summary>
        /// Промежуточный список свойств
        /// </summary>
        public List<IPropertyModel> PropList;


        /// <summary>
        /// Список выбора свойств
        /// </summary>
        public Dictionary<string, IProperty> PropertySelector => propertySelector;

        /// <summary>
        /// Представление списка свойств
        /// </summary>
        public ICollectionView PropertyViewList
        {
            get { return (ICollectionView)GetValue(PropertyViewListProperty); }
            set { SetValue(PropertyViewListProperty, value); }
        }
        public static readonly DependencyProperty PropertyViewListProperty =
            DependencyProperty.Register("PropertyViewList", typeof(ICollectionView), typeof(PropConfigurator), new PropertyMetadata(null));
        /// <summary>
        /// Генерировать свойства для всех конфигураций
        /// </summary>
        public bool ForEachConfig { get => ModelPropMaker.ForEachConfig; set => ModelPropMaker.ForEachConfig = value; }

        /// <summary>
        /// Строка поиска
        /// </summary>
        public string SearchValue
        {
            get { return (string)GetValue(SearchValueProperty); }
            set { SetValue(SearchValueProperty, value); }
        }
        public static readonly DependencyProperty SearchValueProperty =
            DependencyProperty.Register("SearchValue", typeof(string), typeof(PropConfigurator), new PropertyMetadata(null));
        /// <summary>
        /// Новое значение для замены
        /// </summary>
        public string NewValue { get; set; }
        /// <summary>
        /// РегистроЗависимый поиск
        /// </summary>
        public bool CaseSensitive { get => TextReplacer.RegisterSensitive; set => TextReplacer.RegisterSensitive = value; }

        /// <summary>
        /// Обработчик текста
        /// </summary>
        public ITextReplacer TextReplacer { get; private set; }
        /// <summary>
        /// Генератор свойств для модели
        /// </summary>
        public IModelPropMaker ModelPropMaker { get; private set; }

        /// <summary>
        /// Задать источник компонентов
        /// </summary>
        /// <param name="items"></param>
        public void SetSource(IEnumerable<IComponentControl> items) => SourceList = items;

        /// <summary>
        /// Сгенирировать список свойств для компонентов
        /// </summary>
        public void LoadPartList()
        {
            //Продумать очистку
            //ModelPropMaker.ClearEntityList();
            //ModelPropMaker.ClearGettersList();
            var entityRequest = from compcontrol in SourceList
                                let entity = compcontrol.Appmodel.ModelEntity
                                where entity != null
                                select entity;

            if (SourceList != null && ModelPropMaker.Getters.Count > 0)
            {
                ModelPropMaker.DataEntities.AddRange(entityRequest);

                PropList = ModelPropMaker.GetModelProperties();

                PropertyViewList = CollectionViewSource.GetDefaultView(
                    PropList.GroupBy(
                        modelprop => modelprop.TargetName));
            }
        }

        public void ClearPartList()
        {
            PropList = null;
            PropertyViewList.Refresh();

            ModelPropMaker.ClearGettersList();
            ModelPropMaker.ClearEntityList();
        }

        public void BeginSearch() => PropList.Select(prop => TextReplacer.Replace(prop.Value));
        public void DropChanges() => PropList.Select(prop => prop.Value = null);
        public void SaveChanges() => PropList.Select(prop => prop.WriteValue());


    }






    /// <summary>
    /// TestView
    /// </summary>
    class GroupedPropView : DependencyObject
    {
        public GroupedPropView(KeyValuePair<string, IEnumerable<IProperty>>propGroup)
        {
            this.groupName = propGroup.Key;
            PropViews = CollectionViewSource.GetDefaultView(propGroup.Value);
        }

        private readonly string groupName;

        /// <summary>
        /// Имя группы свойств
        /// </summary>
        public string GroupName => groupName;
        /// <summary>
        /// Коллекция свойств
        /// </summary>
        public ICollectionView PropViews
        {
            get { return (ICollectionView)GetValue(PropViewsProperty); }
            set { SetValue(PropViewsProperty, value); }
        }

        public static readonly DependencyProperty PropViewsProperty =
            DependencyProperty.Register("PropViews", typeof(ICollectionView), typeof(GroupedPropView), new PropertyMetadata(null));

    }
}
