using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.NPCs;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Points
{
    /// <inheritdoc />
    /// <summary>
    /// Finds and keeps track of all the points in the game. 
    /// </summary>
    public class PointList : MonoBehaviour
    {
        public static List<GameObject> EnemySpawn { get; private set; }
        public static List<GameObject> Interest { get; private set; }
        public static List<GameObject> Idle { get; private set; }
        public static List<GameObject> Despawn { get; private set; }

        private void Start()
        {
            Interest = GameObject.FindGameObjectsWithTag("PointOfInterest").ToList();
            Despawn = GameObject.FindGameObjectsWithTag("PointForDespawn").ToList();
            EnemySpawn = GameObject.FindGameObjectsWithTag("PointForEnemySpawn").ToList();
            Idle = GameObject.FindGameObjectsWithTag("PointForIdle").ToList();
        }

        /// <summary>
        /// Gets a random point from the specified list type. PointListType.Idle does NOT check for occupancy.
        /// Contains a position parameter to be compatible with the SelectPoint delegate.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static GameObject GetRandomPoint(PointType type, Vector3 position)
        {
            List<GameObject> points = GetListFromEnum(type);

            if (points.Count == 0) return null;

            return points[RNG.Next(0, points.Count - 1)];
        }

        /// <summary>
        /// Gets the point that is closest to the given position. 
        /// The point belongs to the list type specified. PointListType.Idle also checks for occupancy.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static GameObject GetClosestPoint(PointType type, Vector3 position)
        {
            List<GameObject> points = GetListFromEnum(type);

            if (points.Count == 0) return null;

            GameObject closest = null;
            float closestDistance = float.MaxValue;
            foreach (GameObject point in points)
            {
                if (type == PointType.Idle && point.GetComponent<PointForIdle>().Occupied)
                {
                    continue;
                }

                float distance = (point.transform.position - position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = point;
                }
            }

            return closest;
        }

        /// <summary>
        ///  Returns the safest point at which the distance to the nearest hostile NPC is the furthest.
        ///  If there are no hostiles, it returns the closest point of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static GameObject GetSafestPoint(PointType type, Vector3 position)
        {
            GameObject safestPoint = null;
            float maxDistance = float.MinValue;
            foreach (GameObject point in GetListFromEnum(type))
            {
                // Calculate midpoint of line between player and despawnPoint
                Vector3 midPoint = position +
                                   (point.transform.position - position) / 2;

                // Measure distance between midpoint and closest hostile NPC to that midpoint.
                Vector3 hostilePos;
                if (!GetClosestHostilePosToPos(midPoint, out hostilePos))
                {
                    // Since there are no hostiles, simply return the closest despawn point.
                    return GetClosestPoint(type, position);
                }

                float distance = (hostilePos - midPoint).sqrMagnitude;

                // Safest point is the point at which the distance to the nearest enemy NPC is the furthest.
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    safestPoint = point;
                }
            }

            return safestPoint;
        }

        private static bool GetClosestHostilePosToPos(Vector3 position, out Vector3 hostilePos)
        {
            hostilePos = Vector3.zero;
            float closestDistance = float.MaxValue;

            foreach (NPC hostileNpc in NPC.HostileNpcs)
            {
                if (hostileNpc.transform == null) continue;

                float distance = (position - hostileNpc.transform.position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    hostilePos = hostileNpc.transform.position;
                }
            }

            return hostilePos == Vector3.zero;
        }

        private static List<GameObject> GetListFromEnum(PointType type)
        {
            switch (type)
            {
                case PointType.EnemySpawn:
                    return EnemySpawn;
                case PointType.Interest:
                    return Interest;
                case PointType.Despawn:
                    return Despawn;
                case PointType.Idle:
                    return Idle;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
    }
}