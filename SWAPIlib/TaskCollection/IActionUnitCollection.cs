using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAPIlib.TaskCollection
{
    public interface IActionUnitCollection
    {
        bool MoveNext();
        IActionUnit Current { get; }
        IActionUnit Next { get; }
    }

    public class ActionUnitSwitcher : IActionUnitCollection
    {
        private int currentActionIndex = 0;

        public ActionUnitSwitcher(List<IActionUnit> actions)
        {
            Actions = actions ?? throw new ArgumentNullException(nameof(actions));
        }

        public List<IActionUnit> Actions { get; set; }
        public IActionUnit Current => GetActionListByIndex(currentActionIndex);
        public IActionUnit Next => GetActionListByIndex(currentActionIndex + 1);
        public void Reset() => currentActionIndex = 0;

        public bool MoveNext()
        {
            return Actions.Count() > ++currentActionIndex;
        }

        private IActionUnit GetActionListByIndex(int index)
        {
            if (Actions?.Count() > index)
            {
                return Actions[index];
            }
            else 
                return null;
        }
    }


}
