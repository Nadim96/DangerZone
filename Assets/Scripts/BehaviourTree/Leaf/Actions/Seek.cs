using System;
using UnityEngine;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Move to a position (vec3). This can either be the position of a gameobject (dynamic) or just a position (static).
    /// If the position is dynamic (i.e. it belongs to a gameobject.transform) it will periodically update the destination.
    /// </summary>
    public class Seek : SeekBase
    {
        protected override Vector3 TargetPosition
        {
            get
            {
                return _isDynamic ? _dynamicDestination(DataModel).transform.position : _staticDestination(DataModel);
            }
        }

        private readonly bool _isDynamic;
        private readonly Func<DataModel, Vector3> _staticDestination;
        private readonly Func<DataModel, GameObject> _dynamicDestination;

        public Seek(DataModel dataModel, Func<DataModel, Vector3> staticDestination) : base(dataModel)
        {
            _isDynamic = false;
            _staticDestination = staticDestination;
        }


        public Seek(DataModel dataModel, Func<DataModel, GameObject> dynamicDestination, float stoppingDistance = 0f) :
            base(dataModel, stoppingDistance)
        {
            _isDynamic = true;
            _dynamicDestination = dynamicDestination;
        }

        protected override void Initialize()
        {
            if (_isDynamic && _dynamicDestination(DataModel) == null)
                return;

            base.Initialize();
        }

        protected override Status PeriodicUpdate()
        {
            if (_isDynamic)
            {
                if (_dynamicDestination(DataModel) != null)
                {
                    // Update the target location every few frames
                    DataModel.Npc.NavMeshAgent.SetDestination(TargetPosition);
                }
                else
                {
                    return Status.Failure; // Target disappeared
                }
            }
            else
            {
                // Fail seek if _staticDestination is set to zero (not existing)
                if (_staticDestination(DataModel) == Vector3.zero)
                {
                    return Status.Failure;
                }
            }
            return base.PeriodicUpdate();
        }
    }
}