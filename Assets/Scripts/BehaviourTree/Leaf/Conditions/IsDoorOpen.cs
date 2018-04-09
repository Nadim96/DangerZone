using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Scenario;

namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    class IsDoorOpen : Condition
    {
        public IsDoorOpen(DataModel dataModel, bool negate = false, Mode mode = Mode.InstantCheck) : base(dataModel, negate, mode)
        {
        }

        protected override bool CheckCondition()
        {
            return DoorScenario.isOpen;
        }
    }
}
