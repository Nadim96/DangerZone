using Assets.Scripts.NPCs;
using UnityEngine;

namespace Assets.Scripts.BehaviourTree
{
    /// <summary>
    /// The DataModel is used to store critical information about an NPC's behaviour tree.
    /// It is required for the correct execution of a behaviour tree.
    /// </summary>
    public class DataModel
    {
        public NPC Npc { get; set; }
        public GameObject Target { get; set; }
        public Vector3 MovePosition {get;set;}

        public DataModel(NPC npc)
        {
            Npc = npc;
        }
    }
}
