using Assets.Scripts.Items;

namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    /// <inheritdoc />
    /// <summary>
    /// Checks if the item the NPC is holding (if any) is a ranged weapon.
    /// Succeeds when item is a ranged weapon. Fails if there is no item or when the item isn't a ranged weapon.
    /// </summary>
    public class IsRangedWeapon : Condition
    {
        public IsRangedWeapon(DataModel dataModel, bool negate = false, Mode mode = Mode.InstantCheck) :
            base(dataModel, negate, mode)
        {
        }

        protected override bool CheckCondition()
        {
            if (DataModel.Npc != null && DataModel.Npc.Item != null)
            {
                return DataModel.Npc.Item is RangedWeapon;
            }
            return false;
        }
    }
}