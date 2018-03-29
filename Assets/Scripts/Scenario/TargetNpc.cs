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
        private NPC _npc;

        public override bool IsAlive
        {
            get { return _npc == null || _npc.IsAlive; }
        }

        public override Transform Spawn(Transform prefab)
        {
            Transform npcTransform = Object.Instantiate(prefab, Position, Quaternion.identity);
            _npc = npcTransform.GetComponent<NPC>();
            if (ItemType != ItemType.None)
            {
                _npc.Item = ItemFactory.Instance.CreateItem(ItemType, _npc);
            }

            if (Waypoints != null)
            {
                _npc.CurrentWaypoint = 0;
                _npc.Waypoints = Waypoints;
            }

            _npc.Difficulty = Difficulty;

            _npc.IsHostile = IsHostile;

            _npc.transform.position = SpawnEnemyAtRandomSpawnPoint(_npc.transform.position);
            return npcTransform;
        }

        public override void Destroy()
        {
            if (_npc != null)
            {
                Object.Destroy(_npc.gameObject);
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
            int maxValue = PointList.EnemySpawn.Count;
            System.Random r = new System.Random();

            return PointList.EnemySpawn[r.Next(0, maxValue)].transform.position;
        }
    }
}