using Assets.Scripts.Items;

namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    /// <inheritdoc />
    /// <summary>
    /// Checks if weapon is equipped. Fails when there is no NPC, NPC doesn't have an item or when the item isn't a weapon.
    /// </summary>
    public class IsWeaponEquipped : Condition
    {
        public IsWeaponEquipped(DataModel dataModel, bool negate = false, Mode mode = Mode.InstantCheck) :
            base(dataModel, negate, mode)
        {
        }

        protected override bool CheckCondition()
        {
            return DataModel.Npc != null && DataModel.Npc.Item is Weapon && DataModel.Npc.Item.IsEquipped;
        }
    }
}