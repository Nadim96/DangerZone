using UnityEngine;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    public class EquipWeapon : Leaf
    {
        public EquipWeapon(DataModel dataModel) : base(dataModel)
        {
        }

        protected override Status Update()
        {
            if (DataModel.Npc == null)
            {
                return Status.Invalid; // No owner npc, unable to get item
            }

            if (DataModel.Npc.Item == null)
            {
                Debug.LogWarning("Cannot equip item, NPC has no item to equip.");
                return Status.Invalid; // No weapon equipped
            }

            DataModel.Npc.Item.Equip();

            return Status.Success;
        }
    }
}