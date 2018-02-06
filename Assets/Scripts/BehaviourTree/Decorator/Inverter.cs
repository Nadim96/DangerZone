namespace Assets.Scripts.BehaviourTree.Decorator
{
    /// <inheritdoc />
    /// <summary>
    /// Inverts the output of a child node. A success becomes a fail, a fail becomes a success.
    /// </summary>
    public class Inverter : Decorator
    {
        public Inverter(BTNode child) : base(child)
        {
        }

        protected override Status Update()
        {
            Status status = base.Update();

            switch (status)
            {
                case Status.Failure:
                    return Status.Success;
                case Status.Success:
                    return Status.Failure;
                default:
                    return status;
            }
        }
    }
}