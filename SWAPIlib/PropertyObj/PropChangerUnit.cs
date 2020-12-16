using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.PropertyObj
{
    public static class Test
    {
        public static void TestFunc()
        {
            var testColl = new ObservableCollection<string>();

            testColl.CollectionChanged += AddToColl;

            testColl.Add("test1");
            testColl.Add("test2");
            testColl.RemoveAt(0);

        }

        private static void AddToColl(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                Console.WriteLine($"AddValue {e.NewItems[0] as string}");
            }

            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                Console.WriteLine($"Remove {e.OldItems[0] as string}");
            }
        }
    }

    public class PropModifier : IPropModifier, IEnumerable
    {
        /// <summary>
        /// ExtendedConstructor
        /// </summary>
        /// <param name="appModel"></param>
        /// <param name="propConstructor"></param>
        public PropModifier(AppModel appModel, PropConstructor propConstructor) : this()
        {
            if (appModel.DocType == AppDocType.swDRAWING || appModel.DocType == AppDocType.swNONE)
                throw new ArgumentException($"PropModifier - wrong docType {appModel.DocType}");
            Model = appModel;
            this.propConstructor = propConstructor;
        }
        /// <summary>
        /// VoidConstructor
        /// </summary>
        public PropModifier()
        {
            SwPropList = new Dictionary<string, ISwProperty>();
            ConfigNames = new ObservableCollection<string>();
            OldValues = new Dictionary<string, string>();
            ConfigNames.CollectionChanged += AddRemoveConfig;
        }


        public string PartName => Model.FileName;
        public AppModel Model { get; set; }
        public ObservableCollection<string> ConfigNames { get; set; }
        public Dictionary<string, string> OldValues { get; set; }
        public Dictionary<string, ISwProperty> SwPropList { get; set; }
        public PropConstructor propConstructor { get; set; }

        /// <summary>
        /// Включить все конфигурации
        /// </summary>
        public bool AllConfiguration 
        { 
            get => _AllConfiguration;
            set
            {
                if(value == true)
                {
                    Debug.WriteLine($"Add all config in {PartName}");
                    _SelectAllstorage.AddRange(
                        Model.ConfigList.Except(ConfigNames)
                        );
                    foreach (var confName in _SelectAllstorage)
                    {
                        ConfigNames.Add(confName);
                    }
                    _AllConfiguration = true;
                }
                else
                {
                    Debug.WriteLine($"remove all config in {PartName}");
                    //var removeList = Model.ConfigList.Except(_SelectAllstorage).ToArray();
                    foreach (var delConfName in _SelectAllstorage)
                    {
                        ConfigNames.Remove(delConfName);
                    }
                    _AllConfiguration = false;
                    _SelectAllstorage.Clear();
                }
            }
        }
        bool _AllConfiguration = false;
        /// <summary>
        /// Список для удаления конфигураций при откл. свойства "все конфигурации"
        /// </summary>
        List<string> _SelectAllstorage = new List<string>();

        public void RestoreValues()
        {
            foreach (var prop in SwPropList.Values.Where(prop => prop.IsModifyed))
            {
                prop.Update();
            }
        }
        public void WriteValues()
        {
            foreach (var prop in SwPropList.Values.Where(prop => prop.IsModifyed))
            {
                prop.WriteValue();
            }
        }

        /// <summary>
        /// Add - remove event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRemoveConfig(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var confName = e.NewItems[0] as string;
                if (SwPropList.ContainsKey(confName))
                    return;
                ISwProperty newProp = propConstructor(Model, confName);
                SwPropList.Add(confName, newProp);

                OldValues.Add(confName, newProp.PropertyValue);

                Debug.WriteLine($"Add conf {confName} to PropModifier {PartName}");
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                Debug.WriteLine($"remove conf {e.OldItems[0] as string} from PropModifier {PartName}");
                SwPropList.Remove(e.OldItems[0] as string);
                OldValues.Remove(e.OldItems[0] as string);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return SwPropList.Values.GetEnumerator();
        }
    }

    public static class PropFactory
    {
        /// <summary>
        /// Свойство Обозначение
        /// </summary>
        /// <param name="appModel"></param>
        /// <param name="confname"></param>
        /// <returns></returns>
        public static AppModelPropGetter DeNomination(AppModel appModel, string confname = null)
        {
            string propName = "Обозначение";
            var ret =
                new AppModelPropGetter(appModel)
                {
                    ConfigName = confname,
                    IsReadable = true,
                    IsWritable = true,
                    Validator = PropValidatorTemplate.IsPartOrAsmOrComp,
                    UserName = propName,
                    PropertyName = propName
                };
            return ret;
        }

        /// <summary>
        /// Свойство наименование
        /// </summary>
        /// <param name="appModel"></param>
        /// <param name="confname"></param>
        /// <returns></returns>
        public static AppModelPropGetter Nomination(AppModel appModel, string confname = null)
        {
            string propName = "Наименование";
            var ret =
                new AppModelPropGetter(appModel)
                {
                    ConfigName = confname,
                    IsReadable = true,
                    IsWritable = true,
                    Validator = PropValidatorTemplate.IsPartOrAsmOrComp,
                    UserName = propName,
                    PropertyName = propName
                };
            return ret;
        }
    }
}
