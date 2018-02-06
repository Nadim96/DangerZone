using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Audio;
using Assets.Scripts.NPCs;
using UnityEngine;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// This action triggers a panic for all friendly NPC's in range of the owner.
    /// It always succeeds.
    /// </summary>
    public class CausePanic : Leaf
    {
        private const float DEFAULT_CAUSE_PANIC_RANGE = 100f;

        public static bool _isTriggered { get; set; }

        private readonly float _range;

        public CausePanic(DataModel dataModel, float range = DEFAULT_CAUSE_PANIC_RANGE) : base(dataModel)
        {
            _range = range;
        }

        protected override Status Update()
        {
            CausePanicForAllInRange();
            return Status.Success;
        }

        /// <summary>
        /// Sets all NPC's to panic. As an optimization, panic can only be triggered once until
        /// it resets.
        /// </summary>
        private void CausePanicForAllInRange()
        {
            if (_isTriggered) return;
            _isTriggered = true;
            foreach (NPC npc in GetNpcsInRange(_range))
            {
                if (!npc.IsHostile)
                {
                    npc.IsPanicking = true;
                    AudioController.ScreamAudio(npc);
                }
            }
        }

        private List<NPC> GetNpcsInRange(float distance)
        {
            float distSquared = distance * distance;
            Vector3 position = DataModel.Npc.gameObject.transform.position;

            return NPC.Npcs.Where(x => (position - x.transform.position).sqrMagnitude <= distSquared).ToList();
        }
    }
}
