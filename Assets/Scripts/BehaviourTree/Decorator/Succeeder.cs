namespace Assets.Scripts.BehaviourTree.Decorator
{
    /// <inheritdoc />
    /// <summary>
    /// A succeeder decorator will keep running until its child behavior finishes. 
    /// Regardless of the outcome of the child, it will return Status.Success.
    /// </summary>
    public class Succeeder : Decorator
    {
        public Succeeder(BTNode child) : base(child)
        {
        }

        protected override Status Update()
        {
            Status status = base.Update();

            if (status == Status.Failure)
            {
                return Status.Success;
            }

            return status;
        }
    }
}