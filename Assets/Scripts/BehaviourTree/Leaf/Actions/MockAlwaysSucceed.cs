namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// This Action always succeeds. When RequireOneTick is true, it will return running one tick, then it will complete.
    /// </summary>
    public class MockAlwaysSucceed : BTNode
    {
        private bool _ticked;

        public bool RequireOneTick { get; set; }

        protected override Status Update()
        {
            if (RequireOneTick)
            {
                if (_ticked) return Status.Success;

                _ticked = true;

                return Status.Running;
            }
            
            return Status.Success;
        }

        protected override void Terminate(Status status)
        {
            _ticked = false;
        }
    }
}
