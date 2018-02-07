using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for all seek actions.
    /// </summary>
    public abstract class SeekBase : Leaf
    {
        private const float MAX_TIME_STUCK = 4f;

        /// <summary>
        /// To increase performance, the destination is only updated every (UpdateInterval) seconds.
        /// This is only relevant for non-static targets (gameobjects), where the destination might change.
        /// </summary>
        protected const float UpdateInterval = 1f;

        protected float TimeSinceLastUpdate;

        protected abstract Vector3 TargetPosition { get; }

        private float _timeStuck;
        private float _stoppingDistance;

        protected SeekBase(DataModel dataModel, float stoppingDistance = 0f) : base(dataModel)
        {
            _stoppingDistance = stoppingDistance;
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (Math.Abs(_stoppingDistance) < 0.01f)
            {
                _stoppingDistance = DataModel.Npc.NavMeshAgent.stoppingDistance;
            }
            if (DataModel.Npc.NavMeshAgent.isOnNavMesh && DataModel.Npc.NavMeshAgent.isActiveAndEnabled)
                DataModel.Npc.NavMeshAgent.SetDestination(TargetPosition);
        }

        /// <inheritdoc />
        /// <summary>
        /// Update target location if target is not static. 
        /// Optimized to set destination only every few frames.
        /// </summary>
        /// <returns></returns>
        protected override Status Update()
        {
            if (Status == Status.Invalid) return Status;

            Status status = Status;

            if (Time.time >= TimeSinceLastUpdate + UpdateInterval)
            {
                status = PeriodicUpdate();
                TimeSinceLastUpdate = Time.time;
            }

            return status;
        }

        /// <summary>
        /// Contains the actual logic that should be executed every few ticks.
        /// Updates the target position if necessary and checks if stuck or destination is reached.
        /// </summary>
        protected virtual Status PeriodicUpdate()
        {
            if (IsDestinationReached())
            {
                return Status.Success;
            }

            if (IsStuck())
            {
                return Status.Failure;
            }

            return Status.Running;
        }

        /// <summary>
        /// Confirm NPC reached destination
        /// </summary>
        protected virtual bool IsDestinationReached()
        {
            return Vector3.Distance(DataModel.Npc.transform.position, TargetPosition) <= _stoppingDistance;
        }

        /// <summary>
        /// Returns true if the agent has been stuck for MAX_TIME_STUCK. 
        /// </summary>
        /// <returns></returns>
        protected bool IsStuck()
        {
            NavMeshAgent agent = DataModel.Npc.NavMeshAgent;

            bool currentlyStuck = Vector3.Distance(DataModel.Npc.transform.position, TargetPosition) >=
                                  _stoppingDistance && Math.Abs(agent.velocity.sqrMagnitude) < 0.01f;

            if (currentlyStuck)
            {
                _timeStuck += UpdateInterval;
            }
            else
            {
                _timeStuck = 0;
            }

            return _timeStuck > MAX_TIME_STUCK;
        }

        protected override void Terminate(Status status)
        {
            base.Terminate(status);

            if(DataModel.Npc.NavMeshAgent.isOnNavMesh && DataModel.Npc.NavMeshAgent.isActiveAndEnabled)
                DataModel.Npc.NavMeshAgent.ResetPath();
        }
    }
}
