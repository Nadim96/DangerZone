using Assets.Scripts.Items;

namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    /// <inheritdoc />
    /// <summary>
    /// Checks if owner's ranged weapon has ammo.
    /// Fails if there is no NPC, item, if item isn't a ranged weapon or if there is no ammo.
    /// </summary>
    public class HasAmmo : Condition
    {
        public HasAmmo(DataModel dataModel, bool negate = false, Mode mode = Mode.InstantCheck) :
            base(dataModel, negate, mode)
        {
        }

        protected override bool CheckCondition()
        {
            if (DataModel.Npc != null && DataModel.Npc.Item != null)
            {
                var weapon = DataModel.Npc.Item as RangedWeapon;
                if (weapon != null)
                {
                    return weapon.Ammo > 0;
                }
            }
            return false;
        }
    }
}