namespace Assets.Scripts.BehaviourTree.Decorator
{
    /// <inheritdoc />
    /// <summary>
    /// Repeats a child behaviour n times, or optionally an infinite number of times. Fails when child fails.
    /// </summary>
    public class Repeater : Decorator
    {
        private int _counter;
        private readonly int _limit;
        private readonly bool _limitless;
        private readonly bool _repeatUntilFail;

        /// <summary>
        /// Repeats a child behaviour n times. Fails when child fails.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="limit">How many times the child behaviour should be repeated</param>
        public Repeater(BTNode child, int limit) : base(child)
        {
            _limit = limit;
            _limitless = false;
            _repeatUntilFail = true;
        }

        /// <summary>
        /// Repeats infinitely. Optionally stops repeating when child fails.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="repeatUntilFail"></param>
        public Repeater(BTNode child, bool repeatUntilFail) : base(child)
        {
            _limit = 0;
            _limitless = true;
            _repeatUntilFail = repeatUntilFail;
        }

        protected override Status Update()
        {
            while (true)
            {
                Child.Tick();

                if (Child.Status == Status.Running || 
                    !_repeatUntilFail && Child.Status == Status.Failure) break;

                if (Child.Status == Status.Failure) return Status.Failure;
                
                if (!_limitless && ++_counter == _limit) return Status.Success;

                Child.Reset();
            }
            return Status;
        }
    }
}