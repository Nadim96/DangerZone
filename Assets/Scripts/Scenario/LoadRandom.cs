using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.BehaviourTree;
using Assets.Scripts.Items;
using Assets.Scripts.Settings;
using Assets.Scripts.Utility;
using UnityEngine.AI;

namespace Assets.Scripts.Scenario
{
    /// <summary>
    /// Load a random amount of npcs at a random positions.
    /// </summary>
    public class LoadRandom : ILoad
    {
        private static readonly List<string> Names;

        public Difficulty difficulty;
        public bool Plein { get; set; }
        /// <summary>
        /// generate random name for the scene
        /// </summary>
        static LoadRandom()
        {
            Names = new List<string>
            {
                "Gevaars beheersing",
                "Dangerzone",
                "random",
                "Highway to the danger zone",
                "Ride into the danger zone",
                "Topgun"
            };
        }

        /// <summary>
        /// load random amount of NPCS selected in scenariosettings
        /// </summary>
        public void Load()
        {
            ScenarioBase scenario = ScenarioBase.Instance;
            scenario.TargetType = ScenarioSettings.TargetType;
            scenario.GoalType = ScenarioSettings.GoalType;
            scenario.name = GetName();                  
        }

        public void Create()
        {
           int enemies = RNG.Next(ScenarioSettings.MinEnemies, ScenarioSettings.MaxEnemies + 1);
            int friendlies = RNG.Next(ScenarioSettings.MinFriendlies, ScenarioSettings.MaxFriendlies + 1);
            for (int i = 0; i < enemies; i++)
                CreateRandomTarget(true);

            for (int i = 0; i < friendlies; i++)
                CreateRandomTarget(false);
        }

        /// <summary>
        /// Returns a random generated name
        /// </summary>
        private string GetName()
        {
            int random = RNG.Next(0, Names.Count);
            return Names[random];
        }

        /// <summary>
        /// Places a target at a random position with settigns selected in ScenarioSettings
        /// </summary>
        /// <param name="isHostile"></param>
        private void CreateRandomTarget(bool isHostile)
        {
            Target target = GetTarget();

            target.ItemType = isHostile ? GetItem() : ItemType.None;
            target.Difficulty = difficulty;
            target.IsHostile = isHostile;

       

            List<Vector3> waypoints = GetWaypoints();

            //add target to scenario
            ScenarioBase scenario = ScenarioBase.Instance;
            scenario.SpawnWaypointsForTarget(target, waypoints);
            scenario.Targets.Add(target);
        }

        /// <summary>
        /// Get the correct targetType
        /// </summary>
        private static Target GetTarget()
        {
            Target target;
            switch (ScenarioBase.Instance.TargetType)
            {
                case TargetType.Paper:
                    throw new NotImplementedException("I don't know how to make paper targets!");
                case TargetType.Dummy:
                    target = new TargetNpc();
                    break;
                case TargetType.Person:
                    target = new TargetNpc();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return target;
        }

        /// <summary>
        /// Get random position on the navmesh
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomTargetPoint()
        {
            Vector3 minBoundsPoint = Vector3.zero;
            Vector3 maxBoundsPoint = Vector3.zero;

            float boundsSize = float.NegativeInfinity;
            if (boundsSize < 0)
            {
                minBoundsPoint = Vector3.one * float.PositiveInfinity;
                maxBoundsPoint = -minBoundsPoint;
                var vertices = NavMesh.CalculateTriangulation().vertices;
                foreach (var point in vertices)
                {
                    if (minBoundsPoint.x > point.x)
                        minBoundsPoint = new Vector3(point.x, minBoundsPoint.y, minBoundsPoint.z);
                    if (minBoundsPoint.y > point.y)
                        minBoundsPoint = new Vector3(minBoundsPoint.x, point.y, minBoundsPoint.z);
                    if (minBoundsPoint.z > point.z)
                        minBoundsPoint = new Vector3(minBoundsPoint.x, minBoundsPoint.y, point.z);
                    if (maxBoundsPoint.x < point.x)
                        maxBoundsPoint = new Vector3(point.x, maxBoundsPoint.y, maxBoundsPoint.z);
                    if (maxBoundsPoint.y < point.y)
                        maxBoundsPoint = new Vector3(maxBoundsPoint.x, point.y, maxBoundsPoint.z);
                    if (maxBoundsPoint.z < point.z)
                        maxBoundsPoint = new Vector3(maxBoundsPoint.x, maxBoundsPoint.y, point.z);
                }
                boundsSize = Vector3.Distance(minBoundsPoint, maxBoundsPoint);
            }
            var randomPoint = new Vector3(
                UnityEngine.Random.Range(minBoundsPoint.x, maxBoundsPoint.x),
                UnityEngine.Random.Range(minBoundsPoint.y, maxBoundsPoint.y),
                UnityEngine.Random.Range(minBoundsPoint.z, maxBoundsPoint.z)
            );
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPoint, out hit, boundsSize, NavMesh.AllAreas);
            return hit.position;
        }

        /// <summary>
        /// Tries to find a valid NavMesh point near the player.
        /// </summary>
        /// <param name="playerPos"></param>
        /// <param name="minSpawnDist">Minimum distance from player</param>
        /// <param name="maxSpawnDist">Maximum distance from player</param>
        /// <returns></returns>
        private static Vector3 GetRandomNavMeshPosition(Vector3 playerPos, float minSpawnDist, float maxSpawnDist)
        {
            float distance = RNG.NextFloat(minSpawnDist, maxSpawnDist) + minSpawnDist;

            Vector3 randomPosition = playerPos + UnityEngine.Random.insideUnitSphere * distance;
            randomPosition.y = 0;

            // Ensure randomPosition is located on the navmesh by taking closest valid point.
            NavMeshHit hit;
            bool isHit;
            int attempts = 0;
            do
            {
                isHit = NavMesh.SamplePosition(randomPosition, out hit, maxSpawnDist, 1);
                attempts++;
            } while (!isHit && attempts < 5);

            if (!isHit)
            {
                throw new Exception("Unable to place NPC on NavMesh.");
            }

            return hit.position;
        }
        /// <summary>
        /// returns weapontype
        /// This only works for hostiles at the moment
        /// </summary>
        /// <returns></returns>
        private static ItemType GetItem()
        {
            switch (ScenarioSettings.WeaponSize)
            {
                case WeaponSize.Small:
                    return ItemType.P99;
                case WeaponSize.Big:
                    return ItemType.Ak;
                case WeaponSize.Random:
                    int random = RNG.Next(0, 2);
                    return random == 0 ? ItemType.P99 : ItemType.Ak;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns 2 waypoints a character can walk between
        /// </summary>
        /// <returns></returns>
        private List<Vector3> GetWaypoints()
        {
            List<UnityEngine.Vector3> waypoints;
            if (!Plein)
            {
                waypoints = new List<Vector3>
                {
                    GetRandomTargetPoint(),
                    GetRandomTargetPoint()
                };
            }
            else
            { //temporary hack to spawn NPCs in plein level
                waypoints = new List<Vector3>
                {
                    GetRandomNavMeshPosition(ScenarioBase.Instance.PlayerCameraEye.transform.position, 5f, 20f),
                    GetRandomNavMeshPosition(ScenarioBase.Instance.PlayerCameraEye.transform.position, 5f, 20f)
                };
            }

            return waypoints;
        }

        /// <summary>
        /// sets difficulty of target
        /// </summary>
        /// <param name="difficulty"></param>
        public void SetDifficulty(Difficulty difficulty)
        {
         
            this.difficulty = difficulty;
        }
    }
}