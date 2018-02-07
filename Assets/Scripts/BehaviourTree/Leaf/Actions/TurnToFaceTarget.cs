using UnityEngine;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Rotates the owner entity to face the Target.
    /// Fails if there is no owner entity or Target.
    /// </summary>
    public class TurnToFaceTarget : Leaf
    {
        /// <summary>
        /// Default maximum rotation speed in degrees per second.
        /// </summary>
        private const float DEFAULT_ROTATION_SPEED = 90;

        /// <summary>
        /// Target should be within x degrees to 'face the target' by default.
        /// </summary>
        private const float DEFAULT_IS_FACING_TARGET_THRESHOLD = 5;

        private readonly float _rotationSpeed;
        private readonly float _isFacingTargetTreshold;

        /// <summary>
        /// Give reference to datamodel and set RotationSpeed in degrees.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="rotationSpeed"></param>
        public TurnToFaceTarget(DataModel dataModel, float rotationSpeed = DEFAULT_ROTATION_SPEED,
            float isFacingTargetTreshold = DEFAULT_IS_FACING_TARGET_THRESHOLD) : base(dataModel)
        {
            _rotationSpeed = rotationSpeed;
            _isFacingTargetTreshold = isFacingTargetTreshold;
        }

        protected override Status Update()
        {
            if (DataModel.Npc == null || DataModel.Target == null)
            {
                return Status.Failure; // No entity to turn with or target to turn to
            }

            bool isFacingTarget = RotateToFaceObject(DataModel.Npc.transform, DataModel.Target.transform);

            return isFacingTarget ? Status.Success : Status.Running;
        }

        /// <summary>
        /// Rotates this NPC to face the target. Note that this method requires that the NavMeshAgent
        /// is either disabled or stops updating the rotation. This usually requires multiple calls 
        /// from update to face a rotation.
        /// </summary>
        /// <param name="thisNpc">The object to face</param>
        /// <param name="target"></param>
        /// <param name="onlyUseYaw">Should the relative position be flattened to avoid changing the pitch and roll?</param>
        /// <returns>Is facing object</returns>
        private bool RotateToFaceObject(Transform thisNpc, Transform target, bool onlyUseYaw = true)
        {
            Vector3 relativePos = target.transform.position - thisNpc.position;

            // Flatten relativePos to avoid unrealistic tilting of this NPC.
            if (onlyUseYaw)
            {
                relativePos.Scale(new Vector3(1, 0, 1));
            }

            Quaternion targetRotation = Quaternion.LookRotation(relativePos);
            thisNpc.rotation =
                Quaternion.RotateTowards(thisNpc.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            bool isFacing = Quaternion.Angle(thisNpc.rotation, targetRotation) < _isFacingTargetTreshold;

            return isFacing;
        }
    }
}