namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Despawns the NPC.
    /// </summary>
    public class Despawn : Leaf
    {
        public Despawn(DataModel dataModel) : base(dataModel)
        {
        }

        protected override Status Update()
        {
            if (DataModel.Npc == null)
            {
                return Status.Invalid;
            }
            if (!DataModel.Npc.IsHostile)
            {
                DataModel.Npc.Despawn();
            }
            return Status.Success;
        }
    }
}
