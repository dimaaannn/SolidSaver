using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.MProperty
{
    /// <summary>
    /// Объект с информацией о привязке
    /// </summary>
    public interface IDataEntity
    {
        /// <summary>
        /// Получить объект привязки
        /// </summary>
        /// <returns></returns>
        object GetEntity();
    }
    public interface IDataEntity<out T> : IDataEntity
    {
        /// <summary>
        /// Типизированный объект привязки
        /// </summary>
        T TargetWrapper { get; }
    }

    /// <summary>
    /// Интерфейс для полей модели
    /// </summary>
    public interface IModelEntity : IDataEntity<IAppModel>
    {
        string Title { get; }
        string FileName { get; }
        string[] ConfigNames { get; }
    }

    /// <summary>
    /// Базовые поля для модели
    /// </summary>
    public class ModelEntity : IModelEntity
    {
        public ModelEntity(IAppModel targetModel)
        {
            this.targetModel = targetModel;
            title = targetModel.Title;
            fileName = targetModel.FileName;
            modelConfigNames = targetModel.ConfigList.ToArray();
        }

        private readonly IAppModel targetModel;
        private readonly string fileName;
        private readonly string[] modelConfigNames;
        private readonly string title;

        public IAppModel TargetWrapper => targetModel;
        public string FileName => fileName;
        public string[] ConfigNames => modelConfigNames;
        public string Title => title;

        public object GetEntity()
        {
            return TargetWrapper;
        }

    }
}
