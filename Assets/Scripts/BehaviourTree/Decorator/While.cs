namespace Assets.Scripts.BehaviourTree.Decorator
{
    /// <inheritdoc />
    /// <summary>
    /// As long as the specified condition node results in a success, 
    /// the execution of its child node continues. Returns success when it ends.
    /// Must be used carefully as it can result in an infinite loop when condition is always true.
    /// </summary>
    public class While : Decorator
    {
        private readonly BTNode _condition;

        public While(BTNode condition, BTNode child) : base(child)
        {
            _condition = condition;
        }

        protected override Status Update()
        {
            if (_condition.Tick() != Status.Success)
            {
                Child.Abort();
                return Status.Success;
            }

            Child.Tick();

            return Status.Running;
        }
    }
}
