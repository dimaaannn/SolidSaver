using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SolidSaverWPF.MessagesType;
using SWAPIlib.Property;
using SWAPIlib.Property.ModelProperty;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SolidSaverWPF.ViewModel.Property2
{


    public class PropertyLoaderViewModel : ViewModelBase
    {
        private IEnumerable<ITarget> targetSource;
        private string selectedTemplate;
        private IEnumerable<IPropertySet> output;

        public PropertyLoaderViewModel()
        {

            //Создать словарь выбора свойств пользователем
            PropertyTemplate[] templateArray = (PropertyTemplate[])Enum.GetValues(typeof(PropertyTemplate));
            SetTemplates = templateArray.ToDictionary(x => PropertySetTemplates.GetName(x), x => x);

        }

        public string SelectedTemplate { get => selectedTemplate; set => Set(ref selectedTemplate, value); }
        public Dictionary<string, PropertyTemplate> SetTemplates { get; }

        public IEnumerable<ITarget> TargetSource
        {
            get
            {
                if (targetSource == null)
                {
                    //Выбранные пользователем компоненты
                    targetSource = SWAPIlib.Global.MainModel.SelectionList?.Select(comp => comp.Appmodel.TargetObject);
                    //targetSource = from target in SWAPIlib.Global.MainModel.SelectionList
                    //       select target?.Appmodel.TargetObject;
                }
                return targetSource;
            }
            set => Set(ref targetSource, value);
        }
        public IEnumerable<IPropertySet> Output { get => output; private set => Set(ref output, value); }

        /// <summary>
        /// Создать список свойств и отправить сообщение
        /// </summary>
        public void LoadPropertyList()
        {
            List<IPropertySet> result = null;

            if (CheckLoadRequirements())
            {
                var builder = CreateSetBuilder(SelectedTemplate);
                result = BuildPropertySetList(builder, TargetSource);
            }

            Output = result;

            if (result?.Count() > 0)
            {
                SendMessage(result);
            }
        }

        /// <summary>
        /// Очистить список свойств
        /// </summary>
        public void ClearList()
        {
            SendMessage(null);
        }

        #region BuildMethods

        private bool CheckLoadRequirements()
        {
            bool ret = true;

            ret &= !string.IsNullOrWhiteSpace(SelectedTemplate);
            ret &= TargetSource != null;
            ret &= TargetSource?.Count() > 0;

            return ret;
        }

        private PropertySetBuilder2 CreateSetBuilder(string templateName)
        {
            PropertySetBuilder2 settingsBuilder = PropertySetTemplates.Get(SetTemplates[templateName]);
            return settingsBuilder;
        }
        private IPropertySet BuildProperty(
            PropertySetBuilder2 builder, ITarget target) =>
                builder.Build(target);

        private List<IPropertySet> BuildPropertySetList(
            PropertySetBuilder2 builder, IEnumerable<ITarget> targetList)
        {
            List<IPropertySet> ret = new List<IPropertySet>();
            IPropertySet tempSet;
            foreach (var target in targetList)
            {
                tempSet = BuildProperty(builder, target);
                if (tempSet != null)
                    ret.Add(tempSet);
            }
            return ret;
        }

        private void SendMessage(IEnumerable<IPropertySet> propSet)
        {
            MessengerInstance.Send<PropertyMessage>(new PropertyMessage() { Action = PropertyMessage.PropAction.Show, PropertySet = propSet });
        }
        #endregion




        private ICommand loadSelectedCommand;
        /// <summary>
        /// Выделена хотя бы одна деталь
        /// </summary>
        /// <returns></returns>
        private bool LoadSelectedCanExecute() => !string.IsNullOrWhiteSpace(SelectedTemplate);
        public ICommand LoadSelectedCommand => loadSelectedCommand ?? (loadSelectedCommand = new RelayCommand(LoadPropertyList, LoadSelectedCanExecute));

        private ICommand clearListCommand;

        private bool ClearListCommandCanExecute() => true;
        public ICommand ClearListCommand => clearListCommand ?? (clearListCommand = new RelayCommand(ClearList, ClearListCommandCanExecute));

    }


    /// <summary>
    /// Отображение списка представлений строк свойств - устарело
    /// </summary>
    public class PropertyListViewModel : ViewModelBase, IEnumerable<IPropertyView>
    {
        public PropertyListViewModel()
        {
            Properties = new ObservableCollection<IPropertyView>();
        }

        /// <summary>
        /// Основной список свойств
        /// </summary>
        public ObservableCollection<IPropertyView> Properties { get; protected set; }

        /// <summary>
        /// Обновить все значения свойств
        /// </summary>
        public void Update() => this.Select(propView => propView.Update());
        public bool WriteValues() => this.
            Select(propView => propView.Write()).
            Aggregate((x, y) => x & y);


        

        #region Enumerator
        public IEnumerator<IPropertyView> GetEnumerator() => Properties.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator(); 
        #endregion
    }
}
