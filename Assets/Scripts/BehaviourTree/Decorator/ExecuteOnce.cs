namespace Assets.Scripts.BehaviourTree.Decorator
{
    /// <summary>
    /// This decorator executes its child only once. 
    /// It does this by automatically returning success when 
    /// its child has returned success or failure sometime earlier.
    /// </summary>
    public class ExecuteOnce : Decorator
    {
        private bool _executed;

        public ExecuteOnce(BTNode child) : base(child)
        {
        }

        protected override Status Update()
        {
            if (_executed) return Status.Success;

            Status status = base.Update();

            if (status == Status.Success ||
                status == Status.Failure)
            {
                _executed = true;
            }

            return status;
        }
    }
}
