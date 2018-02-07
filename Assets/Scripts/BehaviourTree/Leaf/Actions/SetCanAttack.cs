using Assets.Scripts.Scenario;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    public class SetCanAttack : Leaf
    {
        private readonly bool _value;

        public SetCanAttack(DataModel dataModel, bool value) : base(dataModel)
        {
            _value = value;
        }

        protected override Status Update()
        {
            ScenarioBase.Instance.AttackTriggered = _value;
            return Status.Success;
        }
    }
}
