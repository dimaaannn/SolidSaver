using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SWAPIlib.Controller
{

    public interface IModelControl<out T> :INotifyPropertyChanged where T : IAppModel
    {
        bool IsSelected { get; set; }
        /// <summary>
        /// Модель детали
        /// </summary>
        T Appmodel { get; }
        /// <summary>
        /// Объект выделения
        /// </summary>
        IModelSelector Modelselector { get; }
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
    public class ModelControl<T> : IModelControl<T> where T : IAppModel
    {
        /// <summary>
        /// PartControl constructor
        /// </summary>
        /// <param name="part"></param>
        public ModelControl(T part)
        {
            Appmodel = part;
            Modelselector = new ModelSelector(part);
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
        public IModelSelector Modelselector { get; protected set; }
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
