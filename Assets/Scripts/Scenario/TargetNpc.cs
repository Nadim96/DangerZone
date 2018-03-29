using Assets.Scripts.Items;
using Assets.Scripts.NPCs;
using UnityEngine;
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
    }
}