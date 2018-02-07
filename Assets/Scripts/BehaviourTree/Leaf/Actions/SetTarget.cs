using System;
using Assets.Scripts.NPCs;
using Assets.Scripts.Points;
using Assets.Scripts.Scenario;
using UnityEngine;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Sets the target to a randomly chosen other NPC, or the player,
    /// depending on the settings.
    /// </summary>
    public class SetTarget : Leaf
    {
        private readonly bool _targetPlayer;
        private readonly bool _targetPoint;
        private readonly PointType _pointType;
        private readonly Func<PointType, Vector3, GameObject> _selectPointAlgorithm;

        /// <summary>
        /// Sets the target to a randomly chosen other NPC, or the player,
        /// depending on the settings.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="targetPlayer">Should the player be targeted instead of NPCs?</param>
        public SetTarget(DataModel dataModel, bool targetPlayer = false) : base(dataModel)
        {
            _targetPlayer = targetPlayer;
        }

        /// <summary>
        /// Set a point as a target.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="type">Which datamodel property should be changed</param>
        /// <param name="pointType"></param>
        /// <param name="selectPointAlgorithm">The algorithm to use for choosing a suitable point</param>
        public SetTarget(DataModel dataModel, PointType pointType, Func<PointType, Vector3, GameObject> selectPointAlgorithm) : base(dataModel)
        {
            _targetPoint = true;
            _pointType = pointType;
            _selectPointAlgorithm = selectPointAlgorithm;
        }

        public SetTarget(DataModel dataModel, Vector3 targetPosition) : base(dataModel)
        {
            dataModel.MovePosition = targetPosition;
        }

        protected override Status Update()
        {
            if (_targetPoint)
            {
                return SetTargetToPoint();
            }

            if (_targetPlayer)
            {
                return SetTargetToPlayer();
            }
            
            return SetTargetToClosestOtherNpc();
        }

        /// <summary>
        /// Select a point using a SelectPoint delegate.
        /// </summary>
        /// <returns></returns>
        private Status SetTargetToPoint()
        {
            if (_selectPointAlgorithm == null)
            {
                return Status.Invalid;
            }

            GameObject target = _selectPointAlgorithm(_pointType, DataModel.Npc.transform.position);
            if (target != null)
            {
                DataModel.Target = target;
                return Status.Success;
            }

            return Status.Failure;
        }

        private Status SetTargetToPlayer()
        {
            var player = ScenarioBase.Instance.PlayerCameraEye;

            if (player == null)
            {
                throw new NullReferenceException("Unable to set target to player. No player reference found.");
            }

            DataModel.Target = player;

            return Status.Success;
        }

        private Status SetTargetToClosestOtherNpc()
        {
            if (NPC.Npcs.Count == 0)
            {
                return Status.Failure;
            }

            NPC closestNpc = null;
            float closestDistance = float.MaxValue;
            foreach (NPC npc in NPC.Npcs)
            {
                if (DataModel.Npc == npc || !npc.IsAlive) continue;
                
                float distance = (DataModel.Npc.transform.position - npc.transform.position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNpc = npc;
                }
            }

            if (closestNpc != null)
            {
                DataModel.Target = closestNpc.gameObject;
                return Status.Success;
            }

            return Status.Failure;
        }
    }
}
