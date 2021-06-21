using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.Table
{
    public interface ICell<out T> : ICell
    {
        T Value { get; }
    }

    public class Cell<T> : BaseCell, ICell<T>
    {
        public Cell(T val)
        {
            Value = val;
        }

        private T _value;

        public T Value {get => _value; protected set { this._value = value; OnPropertyChanged(); } }

        public override bool Update()
        {
            return true;
        }
    }

}
