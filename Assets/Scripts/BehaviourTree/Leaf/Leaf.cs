namespace Assets.Scripts.BehaviourTree.Leaf
{
    /// <inheritdoc />
    /// <summary>
    /// A leaf contains a DataModel for storing and setting information about the entity it controls (the owner npc).
    /// It is recommended to only extend from Leaf if you actually need a DataModel.
    /// </summary>
    public abstract class Leaf : BTNode
    {
        protected readonly DataModel DataModel;

        protected Leaf(DataModel dataModel)
        {
            DataModel = dataModel;
        }
    }
}