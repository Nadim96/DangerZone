using System;
using UnityEngine;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// This leaf node succeeds after the specified duration of time since Initialization.
    /// </summary>
    public class Wait : BTNode
    {
        protected float Duration;

        private float _timestampStartedWaiting;

        /// <summary>
        /// This leaf node succeeds after the specified duration of time since Initialization.
        /// </summary>
        /// <param name="duration">The amount of time to wait in seconds.</param>
        public Wait(float duration)
        {
            if (duration < 0)
                throw new ArgumentOutOfRangeException("duration", "Time duration cannot be negative.");

            Duration = duration;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _timestampStartedWaiting = Time.time;
        }

        protected override Status Update()
        {
            return _timestampStartedWaiting + Duration <= Time.time ? Status.Success : Status.Running;
        }
    }
}