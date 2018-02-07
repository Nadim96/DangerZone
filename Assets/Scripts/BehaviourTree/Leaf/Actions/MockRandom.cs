using Assets.Scripts.Utility;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// This Action either succeeds or fails, it is random. When RequireOneTick is true, it will return running one tick, then it will complete.
    /// </summary>
    public class MockRandom : BTNode
    {
        private bool _ticked;
        public bool RequireOneTick { get; set; }

        protected override Status Update()
        {
            if (RequireOneTick)
            {
                if (_ticked) return GetRandomStatus();

                _ticked = true;

                return Status.Running;
            }

            return GetRandomStatus();
        }

        protected override void Terminate(Status status)
        {
            _ticked = false;
        }

        private Status GetRandomStatus()
        {
            int random = RNG.Next(0, 2);

            if (random == 0)
            {
                return Status.Failure;
            }
            return Status.Success;
        }
    }
}
