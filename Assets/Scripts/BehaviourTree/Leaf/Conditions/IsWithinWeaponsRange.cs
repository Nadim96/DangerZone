using Assets.Scripts.Items;
using Assets.Scripts.Settings;
using UnityEngine;

namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    /// <inheritdoc />
    /// <summary>
    /// Checks if targetNpc is within range of the owner. Fails if there is no NPC, no target, no item or out of range.
    /// </summary>
    /// 
    public class IsWithinWeaponsRange : Condition
    {

        public IsWithinWeaponsRange(DataModel dataModel,  bool negate = false, Mode mode = Mode.InstantCheck) :
            base(dataModel, negate, mode)
        {
        }

        protected override bool CheckCondition()
        {
            if (DataModel.Npc == null || DataModel.Target == null ||
                DataModel.Npc.Item == null)
            {
                return false; // Unable to determine range
            }

            var weapon = DataModel.Npc.Item as Weapon;
            if (weapon != null)
            {
                float distance = Vector3.Distance(
                    DataModel.Npc.transform.position,
                    DataModel.Target.transform.position);

               Debug.Log("Range: " + (distance <= ScenarioSettings.EngagementDistance)); 

                return distance <= ScenarioSettings.EngagementDistance;
            }


            return false; // Unable to cast: NPC is not holding a weapon
        }
    }
}