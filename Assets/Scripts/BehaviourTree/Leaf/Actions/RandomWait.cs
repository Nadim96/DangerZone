using System;
using Assets.Scripts.Utility;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Node used to wait a random amount of time somewhere between
    /// the minimum and maximum amount of wait time.
    /// </summary>
    public class RandomWait : Wait
    {
        private readonly float _minWaitTime;
        private readonly float _maxWaitTime;

        public RandomWait(float minWaitTime, float maxWaitTime) : base(0.0f)
        {
            if (minWaitTime < 0)
                throw new ArgumentOutOfRangeException("minWaitTime", "Time duration cannot be negative.");

            if (maxWaitTime < 0)
                throw new ArgumentOutOfRangeException("maxWaitTime", "Time duration cannot be negative.");

            if (minWaitTime > maxWaitTime)
                throw new ArgumentException("The minimum wait time may not exceed the maximum wait time.");

            _minWaitTime = minWaitTime;
            _maxWaitTime = maxWaitTime;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Duration = RNG.NextFloat(_minWaitTime, _maxWaitTime);
        }
    }
}
