namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// This action can be used to set the IsRunning bool of the NPC, thereby changing the movement speed
    /// of the owning entity.
    /// </summary>
    public class SetMovementSpeed : Leaf
    {
        private readonly bool _shouldRun;

        /// <summary>
        /// The shouldRun parameter is used to set this node up as a node which either sets the NPC to walk or to run.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="shouldRun">If true, IsRunning should be true, else it will walk</param>
        public SetMovementSpeed(DataModel dataModel, bool shouldRun) : base(dataModel)
        {
            _shouldRun = shouldRun;
        }

        protected override Status Update()
        {
            if (DataModel.Npc == null)
            {
                return Status.Invalid;
            }

            DataModel.Npc.IsRunning = _shouldRun;
            return Status.Success;
        }
    }
}
