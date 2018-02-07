using Assets.Scripts.Items;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Gets a random weapon from the ItemFactory and equips it on the owning NPC.
    /// Destroys currently equipped item.
    /// </summary>
    public class EquipRandomWeapon : Leaf
    {
        public EquipRandomWeapon(DataModel dataModel) : base(dataModel)
        {
        }

        protected override Status Update()
        {
            if (DataModel.Npc == null)
            {
                return Status.Invalid; // No owner npc, unable to get item
            }

            if (DataModel.Npc.Item != null)
            {
                DataModel.Npc.Item.Dispose();
            }

            Item item = ItemFactory.Instance.GetRandomWeapon(DataModel.Npc);
            DataModel.Npc.Item = item;
            item.Equip();

            return Status.Success;
        }
    }
}
