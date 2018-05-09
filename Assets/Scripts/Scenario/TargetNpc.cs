using System;
using Assets.Scripts.Items;
using Assets.Scripts.NPCs;
using Assets.Scripts.Points;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Scenario
{
    /// <summary>
    /// Wrapper class for NPC, with functionality for the editor.
    /// </summary>
    public class TargetNpc : Target
    {
        public NPC NPC { get; private set; }

        public override bool IsAlive
        {
            get { return NPC == null || NPC.IsAlive; }
        }

        public override Transform Spawn(Transform prefab)
        {
            Transform npcTransform = Object.Instantiate(prefab, Position, Quaternion.identity);
            NPC = npcTransform.GetComponent<NPC>();
            if (ItemType != ItemType.None)
            {
                NPC.Item = ItemFactory.Instance.CreateItem(ItemType, NPC);
            }

            if (Waypoints != null)
            {
                NPC.CurrentWaypoint = 0;
                NPC.Waypoints = Waypoints;
            }

            NPC.Difficulty = Difficulty;

            NPC.IsHostile = IsHostile;

            NPC.transform.position = SpawnEnemyAtRandomSpawnPoint(NPC.transform.position);
            return npcTransform;
        }

        public override void Destroy()
        {
            if (NPC != null)
            {
                Object.Destroy(NPC.gameObject);
            }
            foreach (Waypoint waypoint in Waypoints)
            {
                Object.Destroy(waypoint.gameObject);
            }
        }

        public Vector3 SpawnEnemyAtRandomSpawnPoint(Vector3 current)
        {
            if (PointList.EnemySpawn == null)
            {
                return current;
            }
           

            int maxValue = Math.Max(0, PointList.EnemySpawn.Count-1);

            if (maxValue == 0)
            {
                return current;
            }

            System.Random r = new System.Random();

            return PointList.EnemySpawn[r.Next(0, maxValue)].transform.position;
        }
    }
}