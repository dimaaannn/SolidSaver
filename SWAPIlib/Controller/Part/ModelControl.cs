using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SWAPIlib.Controller
{

    public interface IModelControl<out T, out T1> :INotifyPropertyChanged 
        where T : IAppModel where T1 : IModelSelector
    {
        bool IsSelected { get; set; }
        /// <summary>
        /// Модель детали
        /// </summary>
        T Appmodel { get; }
        /// <summary>
        /// Объект выделения
        /// </summary>
        T1 Modelselector { get; }
        /// <summary>
        /// Тип детали
        /// </summary>
        AppDocType PartType { get; }
        /// <summary>
        /// Имя детали
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Активная группа выделения
        /// </summary>
        int SelectionGroup { get; set; }
    }

    /// <summary>
    /// Контроллер моделей
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelControl<T, T1> : IModelControl<T, T1> 
        where T : IAppModel where T1 : IModelSelector
    {
        /// <summary>
        /// PartControl constructor
        /// </summary>
        /// <param name="part"></param>
        public ModelControl(T part)
        {
            Appmodel = part;
        }

        public ModelControl() { }
        public virtual bool IsSelected
        {
            get => Modelselector.IsSelected;
            set
            {
                Modelselector.IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        public virtual int SelectionGroup
        {
            get => Modelselector.GroupNumber;
            set
            {
                Modelselector.GroupNumber = value;
                OnPropertyChanged("SelectionGroup");
            }
        }
        public virtual T Appmodel { get; protected set; }
        protected IModelSelector _modelSelector;
        public virtual T1 Modelselector
        {
            get
            {
                if(_modelSelector == null)
                {
                    _modelSelector = new ModelSelector(Appmodel);
                }
                return (T1)_modelSelector;
            }
        }
        public AppDocType PartType => Appmodel.DocType;
        public virtual string Title => Appmodel.Title;
        public override string ToString()
        {
            return $"{Appmodel.FileName}:{PartType}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
