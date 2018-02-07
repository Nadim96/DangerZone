namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    /// <inheritdoc />
    /// <summary>
    /// Determines if the character is hostile. Fails when there is no NPC or when NPC isn't hostile.
    /// </summary>
    public class IsIdle : Condition
    {
        public IsIdle(DataModel dataModel, bool negate = false, Mode mode = Mode.InstantCheck) :
            base(dataModel, negate, mode)
        {
        }

        protected override bool CheckCondition()
        {
            return DataModel.Npc.IsIdle;
        }
    }
}