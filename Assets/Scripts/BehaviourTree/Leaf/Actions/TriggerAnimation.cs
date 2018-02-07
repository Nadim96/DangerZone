namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// This node triggers the animation trigger specified in the constructor. 
    /// Unfortunately, this node has no way of knowing if the animator 
    /// successfully accepted the trigger. Therefore, this node always succeeds.
    /// </summary>
    public class TriggerAnimation : Leaf
    {
        protected string TriggerName;

        public TriggerAnimation(DataModel dataModel, string triggerName = "") : base(dataModel)
        {
            TriggerName = triggerName;
        }

        protected override Status Update()
        {
            DataModel.Npc.Animator.SetTrigger(TriggerName);
            return Status.Success;
        }
    }
}