using SWAPIlib.Property.ModelProperty;
using SWAPIlib.TaskUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAPIlib.TaskCollection
{
    public class TestAction
    {
        public List<IActionList> GetActions()
        {
            List<IActionList> ret = new List<IActionList>();

            
            

            return ret;
        }

        public IActionList GlobalModelOptions()
        {
            IActionList ret = new ActionList();

            //ret.Add(
            //    CellFactoryBuilder.Create(ModelPropertyNames.FileName.ToString)

            return ret;
        }
    }
}
