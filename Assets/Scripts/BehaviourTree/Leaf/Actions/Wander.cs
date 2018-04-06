using System;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Moves to random positions in the area. 
    /// </summary>
    public class Wander : SeekBase
    {
        private const float MIN_WANDER_DISTANCE = 2f;
        private const float MAX_WANDER_DISTANCE = 12f;
        private const int NAVMESH_HIT_ATTEMPT_TIMEOUT = 5;

        private Vector3 _destination;

        protected override Vector3 TargetPosition
        {
            get { return _destination; }
        }

        protected override void Initialize()
        {
            _destination = GetRandomNavMeshPosition(
                DataModel.Npc.transform.position,
                MIN_WANDER_DISTANCE,
                MAX_WANDER_DISTANCE);

            base.Initialize();
        }

        public Wander(DataModel dataModel) : base(dataModel)
        {
        }

        /// <summary>
        /// Gets a random nav mesh position which is at least minDist away and closer than maxDist.
        /// </summary>
        /// <param name="currentPos"></param>
        /// <param name="minDist"></param>
        /// <param name="maxDist"></param>
        /// <returns></returns>
        private static Vector3 GetRandomNavMeshPosition(Vector3 currentPos, float minDist, float maxDist)
        {
            float distance = RNG.NextFloat(minDist, maxDist);

            Vector2 randomCirclePoint = Random.insideUnitCircle.normalized * distance;

            Vector3 randomPosition = new Vector3(
                currentPos.x + randomCirclePoint.x,
                currentPos.y,
                currentPos.z + randomCirclePoint.y);

            // Ensure randomPosition is located on the navmesh by taking closest valid point.
            NavMeshHit hit;
            bool isHit;
            int attempts = 0;
            do
            {
                isHit = NavMesh.SamplePosition(randomPosition, out hit, maxDist, 1);
                attempts++;
            } while (!isHit && attempts < NAVMESH_HIT_ATTEMPT_TIMEOUT);

            if (!isHit)
            {
                Debug.Log("Failed to get random point to wander too:    " + currentPos);
                //throw new Exception("Unable to get random point on NavMesh.");
                hit.position = currentPos;
            }

            return hit.position;
        }
    }
}