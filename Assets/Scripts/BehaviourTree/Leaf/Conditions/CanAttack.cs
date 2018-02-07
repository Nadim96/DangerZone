using Assets.Scripts.Scenario;

namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    /// <inheritdoc />
    /// <summary>
    /// The NPC may attack if it has been triggered by the scenario.
    /// </summary>
    public class CanAttack : Condition
    {
        public CanAttack(DataModel dataModel, bool negate = false, Mode mode = Mode.InstantCheck) : base(dataModel, negate, mode)
        {
        }

        protected override bool CheckCondition()
        {
            return ScenarioBase.Instance.AttackTriggered;
        }
    }
}