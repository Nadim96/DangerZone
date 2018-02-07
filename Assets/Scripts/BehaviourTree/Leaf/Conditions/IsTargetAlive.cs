using Assets.Scripts.NPCs;

namespace Assets.Scripts.BehaviourTree.Leaf.Conditions
{
    /// <inheritdoc />
    /// <summary>
    /// Determines if the current target gameobject is alive. Always succeeds when target is the player. 
    /// Invalid if target is not an NPC or player. Returns success if NPC is alive, else fail.
    /// </summary>
    public class IsTargetAlive : Condition
    {
        public IsTargetAlive(DataModel dataModel, bool negate = false, Mode mode = Mode.InstantCheck) :
            base(dataModel, negate, mode)
        {
        }

        protected override bool CheckCondition()
        {
            if (DataModel.Target != null)
            {
                var npc = DataModel.Target.GetComponent<NPC>();
                if (npc != null)
                {
                    return npc.IsAlive;
                }

                var player = DataModel.Target.GetComponent<Player.Player>();
                if (player != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}