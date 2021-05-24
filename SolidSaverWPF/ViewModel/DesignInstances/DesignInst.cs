using SWAPIlib.Property;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidSaverWPF.ViewModel.DesignInstances
{
    public class PropertySetMock : IPropertySet
    {
        public IProperty this[string param] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Name => "MockName";

        public string MainPropertyKey { get => "MainPropertyKeyMock"; set => throw new NotImplementedException(); }

        public IProperty Main => throw new NotImplementedException();

        public Dictionary<string, IProperty> Properties => throw new NotImplementedException();

        public ITarget Target => throw new NotImplementedException();

        public void AddTextProperty(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, IProperty>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void UnionPreserve(IPropertySettings settings)
        {
            throw new NotImplementedException();
        }

        public void UnionReplace(IPropertySettings settings)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
