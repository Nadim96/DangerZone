using Assets.Scripts.Items;

namespace Assets.Scripts.BehaviourTree.Leaf.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Tries to use the item of the owner. Invalid if there is no owner or item, 
    /// fail when not equipped, success when equipped.
    /// </summary>
    public class UseItem : Leaf
    {
        public UseItem(DataModel dataModel) : base(dataModel)
        {
        }

        protected override Status Update()
        {
            if (DataModel.Npc != null && DataModel.Npc.Item != null && DataModel.Npc.Item.IsEquipped)
            {
                if (DataModel.Npc.Item is Weapon)
                {
                    if (DataModel.Target != null && DataModel.Npc.IsAlive)
                    {
                        DataModel.Npc.Item.Use(DataModel.Target);
                        return Status.Success;
                    }
                }
                else
                {
                    DataModel.Npc.Item.Use(DataModel.Target);
                    return Status.Success;
                }
            }
            return Status.Failure;
        }
    }
}