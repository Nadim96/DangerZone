namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    /// <inheritdoc />
    /// <summary>
    /// Determines if the character is nervous by checking its property.
    /// Fails when there is no NPC or when NPC isn't nervous.
    /// </summary>
    public class IsPanicking : Condition
    {
        public IsPanicking(DataModel dataModel, bool negate = false, Mode mode = Mode.InstantCheck) :
            base(dataModel, negate, mode)
        {
        }

        protected override bool CheckCondition()
        {
            return DataModel.Npc != null && DataModel.Npc.IsPanicking;
        }
    }
}